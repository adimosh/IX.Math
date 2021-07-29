// <copyright file="PluginCollection.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
using IX.Abstractions.Logging;
using IX.Math.Extensibility;
using IX.Math.Interpretation;
using IX.Math.Nodes;
using IX.StandardExtensions.Contracts;
using IX.StandardExtensions.Extensions;
using IX.StandardExtensions.Threading;
using IX.System.Collections.Generic;
using JetBrains.Annotations;

namespace IX.Math
{
    /// <summary>
    ///     A collection of functions for the parsing services.
    /// </summary>
    [PublicAPI]
    public class PluginCollection : ReaderWriterSynchronizedBase
    {
#region Internal state

        private readonly List<Assembly> assembliesToRegister;
        private readonly Dictionary<string, Type> binaryFunctions;
        private readonly LevelDictionary<Type, IConstantsExtractor> constantExtractors;
        private readonly LevelDictionary<Type, IConstantInterpreter> constantInterpreters;
        private readonly LevelDictionary<Type, IConstantPassThroughExtractor> constantPassThroughExtractors;
        private readonly List<(TypeInfo Type, bool TolerateMissingAttribute)> customTypesToRegister;
        private readonly Dictionary<string, Type> nonaryFunctions;
        private readonly LevelDictionary<Type, IStringFormatter> stringFormatters;
        private readonly Dictionary<string, Type> ternaryFunctions;
        private readonly Dictionary<string, Type> unaryFunctions;

        private int constantExtractorsIndex;
        private int constantInterpretersIndex;
        private int constantPassThroughExtractorsIndex;

        private bool initialized;
        private int stringFormattersIndex;

#endregion

#region Constructors and destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="PluginCollection" /> class.
        /// </summary>
        private PluginCollection()
        {
            this.assembliesToRegister = new List<Assembly>();
            this.assembliesToRegister.Add(Assembly.GetExecutingAssembly());
            this.customTypesToRegister = new List<(TypeInfo Type, bool TolerateMissingAttribute)>();

            this.nonaryFunctions = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            this.unaryFunctions = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            this.binaryFunctions = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);
            this.ternaryFunctions = new Dictionary<string, Type>(StringComparer.OrdinalIgnoreCase);

            this.constantExtractors = new LevelDictionary<Type, IConstantsExtractor>();
            this.constantInterpreters = new LevelDictionary<Type, IConstantInterpreter>();
            this.constantPassThroughExtractors = new LevelDictionary<Type, IConstantPassThroughExtractor>();
            this.stringFormatters = new LevelDictionary<Type, IStringFormatter>();
        }

#endregion

#region Properties and indexers

        /// <summary>
        ///     Gets the current plugin collection.
        /// </summary>
        public static PluginCollection Current { get; } = new();

#endregion

#region Methods

        /// <summary>
        ///     Interprets an expression based on constant interpreters.
        /// </summary>
        /// <param name="expression">The expression to interpret.</param>
        /// <returns>A node, if the expression is recognized, or <c>null</c> (<c>Nothing</c> in Visual Basic) otherwise.</returns>
        [SuppressMessage(
            "Performance",
            "HAA0401:Possible allocation of reference type enumerator",
            Justification = "We're OK with this enumeration.")]
        [SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "Currently string interpolation does this.")]
        public ConstantNodeBase? InterpretExpression(string expression)
        {
            this.ThrowIfCurrentObjectDisposed();

            using ReadOnlySynchronizationLocker locker = this.EnsureFunctionsInitialized();

            // TODO: Remove the suppression after string interpolation is optimized
            Log.Debug(
                $"Currently interpreting expression \"{expression}\" for constants with {this.constantInterpreters.Count} interpreters.");

            // TODO: Remove the suppression after EnumerateValuesOnLevelKeys gets a struct enumerator
            foreach (var interpreter in this.constantInterpreters.KeysByLevel.SelectMany(p => p.Value))
            {
                var (success, result) = this.constantInterpreters[interpreter]
                    .EvaluateIsConstant(expression);
                if (!success)
                {
                    continue;
                }

                Log.Debug($"Interpretation of constant complete, with result \"{result}\".");

                return result;
            }

            return null;
        }

        /// <summary>
        ///     Checks whether or not the expression is recognized by a pass-through extractor.
        /// </summary>
        /// <param name="expression">The expression to check.</param>
        /// <returns><c>true</c> if the expression is a constant, <c>false</c> otherwise.</returns>
        [SuppressMessage(
            "Performance",
            "HAA0603:Delegate allocation from a method group",
            Justification = "We don't care.")]
        [SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "Currently string interpolation does this.")]
        public bool CheckExpressionPassThroughConstant(string expression)
        {
            this.ThrowIfCurrentObjectDisposed();

            using ReadOnlySynchronizationLocker locker = this.EnsureFunctionsInitialized();

            // TODO: Remove the suppression after string interpolation is optimized
            Log.Debug(
                $"Checking pass-through expression \"{expression}\" with {this.constantPassThroughExtractors.Count} extractors.");
            Log.Debug(
                $"Current plugins: {this.constantExtractors.Count} CE, {this.constantInterpreters.Count} CI, {this.constantPassThroughExtractors.Count} CPTE, {this.stringFormatters.Count} SF, {this.nonaryFunctions.Count} NF, {this.unaryFunctions.Count} UF, {this.binaryFunctions.Count} BF, {this.ternaryFunctions.Count} TF.");

            return this.constantPassThroughExtractors.EnumerateValuesOnLevelKeys()
                .Any(
                    ConstantPassThroughExtractorPredicate,
                    expression);

            static bool ConstantPassThroughExtractorPredicate(
                IConstantPassThroughExtractor extractor,
                string innerExpression)
            {
                return extractor.Evaluate(innerExpression);
            }
        }

        /// <summary>
        ///     Registers the assembly calling this method as an assembly to extract compatible functions from.
        /// </summary>
        public void RegisterCurrentAssembly() => this.RegisterFunctionsAssembly(Assembly.GetCallingAssembly());

        /// <summary>
        ///     Registers an assembly to extract compatible functions from.
        /// </summary>
        /// <param name="assembly">The assembly to register.</param>
        public void RegisterFunctionsAssembly(Assembly assembly)
        {
            Requires.NotNull(
                assembly,
                nameof(assembly));

            this.ThrowIfCurrentObjectDisposed();

            Log.Debug($"Currently registering assembly \"{assembly}\".");

            using ReadWriteSynchronizationLocker locker = this.ReadWriteLock();

            if (this.assembliesToRegister.Contains(assembly))
            {
                Log.Debug($"Assembly \"{assembly}\" had previously been registered.");
                return;
            }

            locker.Upgrade();

            this.assembliesToRegister.Add(assembly);

            Log.Debug($"The assembly \"{assembly}\" has been successfully registered.");

            this.ClearData();
        }

        /// <summary>
        ///     Registers an assembly to extract compatible functions from.
        /// </summary>
        /// <param name="assemblies">The assembly to register.</param>
        public void RegisterFunctionsAssemblies(params Assembly[] assemblies)
        {
            Requires.NotNull(
                assemblies,
                nameof(assemblies));

            this.ThrowIfCurrentObjectDisposed();

            using ReadWriteSynchronizationLocker locker = this.ReadWriteLock();

            Assembly[] unregisteredAssemblies = assemblies.Except(this.assembliesToRegister)
                .ToArray();
            if (unregisteredAssemblies.Length == 0)
            {
                return;
            }

            locker.Upgrade();

            this.assembliesToRegister.AddRange(unregisteredAssemblies);

            this.ClearData();
        }

        /// <summary>
        ///     Registers a specific class as a plugin.
        /// </summary>
        /// <typeparam name="TPlugin">The type of the plugin to register.</typeparam>
        /// <param name="tolerateMissingAttribute">Whether or not to tolerate the missing attribute when registering.</param>
        public void RegisterSpecificPlugin<TPlugin>(bool tolerateMissingAttribute = false)
            where TPlugin : class =>
            this.RegisterSpecificPlugin(
                typeof(TPlugin).GetTypeInfo(),
                tolerateMissingAttribute);

        /// <summary>
        ///     Registers a specific class as a plugin.
        /// </summary>
        /// <param name="pluginType">The type of the plugin to register.</param>
        /// <param name="tolerateMissingAttribute">Whether or not to tolerate the missing attribute when registering.</param>
        public void RegisterSpecificPlugin(
            Type pluginType,
            bool tolerateMissingAttribute = false) =>
            this.RegisterSpecificPlugin(
                Requires.NotNull(
                        pluginType,
                        nameof(pluginType))
                    .GetTypeInfo(),
                tolerateMissingAttribute);

        /// <summary>
        ///     Registers a specific class as a plugin.
        /// </summary>
        /// <param name="pluginType">The type of the plugin to register.</param>
        /// <param name="tolerateMissingAttribute">Whether or not to tolerate the missing attribute when registering.</param>
        [SuppressMessage(
            "Performance",
            "HAA0301:Closure Allocation Source",
            Justification = "LINQ works like this")]
        [SuppressMessage(
            "Performance",
            "HAA0302:Display class allocation to capture closure",
            Justification = "LINQ works like this")]
        public void RegisterSpecificPlugin(
            TypeInfo pluginType,
            bool tolerateMissingAttribute = false)
        {
            Requires.NotNull(
                pluginType,
                nameof(pluginType));

            this.ThrowIfCurrentObjectDisposed();

            using ReadWriteSynchronizationLocker locker = this.ReadWriteLock();

            if (this.customTypesToRegister.Any(p => p.Type == pluginType))
            {
                return;
            }

            locker.Upgrade();

            if (!this.initialized)
            {
                this.LoadData();
            }

            this.CheckType(
                pluginType,
                tolerateMissingAttribute);

            this.customTypesToRegister.Add((pluginType, tolerateMissingAttribute));
        }

        /// <summary>
        ///     Resets the plugin collection entirely.
        /// </summary>
        public void Reset()
        {
            Log.Debug("Resetting plugins.");

            try
            {
                using (this.WriteLock())
                {
                    this.assembliesToRegister.Clear();
                    this.assembliesToRegister.Add(Assembly.GetExecutingAssembly());
                    this.customTypesToRegister.Clear();

                    this.ClearData();
                }

                Log.Debug("Plugins reset and went out of lock.");
            }
            catch (Exception e)
            {
                Log.Error(
                    e,
                    "Exception during resetting the plugin collection.");
            }
        }

        /// <summary>
        ///     Returns the prototypes of all registered functions.
        /// </summary>
        /// <returns>All function names, with all possible combinations of input and output data.</returns>
        public string[] GetRegisteredFunctions()
        {
            this.ThrowIfCurrentObjectDisposed();

            using ReadOnlySynchronizationLocker locker = this.EnsureFunctionsInitialized();

            // Capacity is sum of all, times 3; the "3" number was chosen as a good-enough average of how many overloads are defined, on average
            var builder = new List<string>(
                (this.nonaryFunctions.Count +
                 this.unaryFunctions.Count +
                 this.binaryFunctions.Count +
                 this.ternaryFunctions.Count) *
                3);

            builder.AddRange(this.nonaryFunctions.Select(function => $"{function.Key}()"));

            (
                from KeyValuePair<string, Type> function in this.unaryFunctions
                from ConstructorInfo constructor in function.Value.GetTypeInfo()
                    .DeclaredConstructors
                let parameters = constructor.GetParameters()
                where parameters.Length == 1
                let parameterName = parameters[0]
                    .Name
                where parameterName != null
                let functionName = function.Key
                select (functionName, parameterName)).ForEach(
                (
                    parameter,
                    builderInternal) => builderInternal.Add($"{parameter.functionName}({parameter.parameterName})"),
                builder);

            (
                from KeyValuePair<string, Type> function in this.binaryFunctions
                from ConstructorInfo constructor in function.Value.GetTypeInfo()
                    .DeclaredConstructors
                let parameters = constructor.GetParameters()
                where parameters.Length == 2
                let parameterNameLeft = parameters[0]
                    .Name
                let parameterNameRight = parameters[1]
                    .Name
                where parameterNameLeft != null && parameterNameRight != null
                let functionName = function.Key
                select (functionName, parameterNameLeft, parameterNameRight)).ForEach(
                (
                    parameter,
                    builderInternal) => builderInternal.Add(
                    $"{parameter.functionName}({parameter.parameterNameLeft}, {parameter.parameterNameRight})"),
                builder);

            (
                from KeyValuePair<string, Type> function in this.ternaryFunctions
                from ConstructorInfo constructor in function.Value.GetTypeInfo()
                    .DeclaredConstructors
                let parameters = constructor.GetParameters()
                where parameters.Length == 3
                let parameterNameLeft = parameters[0]
                    .Name
                let parameterNameMiddle = parameters[1]
                    .Name
                let parameterNameRight = parameters[2]
                    .Name
                where parameterNameLeft != null && parameterNameMiddle != null && parameterNameRight != null
                let functionName = function.Key
                select (functionName, parameterNameLeft, parameterNameMiddle, parameterNameRight)).ForEach(
                (
                    parameter,
                    builderInternal) => builderInternal.Add(
                    $"{parameter.functionName}({parameter.parameterNameLeft}, {parameter.parameterNameMiddle}, {parameter.parameterNameRight})"),
                builder);

            return builder.ToArray();
        }

        /// <summary>
        ///     Interprets an object as a string.
        /// </summary>
        /// <typeparam name="T">The type of object to interpret.</typeparam>
        /// <param name="value">The value to interpret as string.</param>
        /// <returns>A success status, and the resulting string.</returns>
        [SuppressMessage(
            "Performance",
            "HAA0401:Possible allocation of reference type enumerator",
            Justification = "We're OK with this enumeration.")]
        public (bool Success, string? ResultingString) InterpretAsString<T>(T value)
        {
            this.ThrowIfCurrentObjectDisposed();

            using ReadOnlySynchronizationLocker locker = this.EnsureFunctionsInitialized();

            Log.Debug($"Interpreting \"{value}\" as string.");

            // TODO: Remove the suppression after EnumerateValuesOnLevelKeys gets a struct enumerator
            foreach (var interpreter in this.stringFormatters.EnumerateValuesOnLevelKeys())
            {
                var (success, result) = interpreter.ParseIntoString(value);

                if (!success)
                {
                    continue;
                }

                Log.Debug($"String interpretation of value \"{value}\" successful with result \"{result}\".");

                return (true, result);
            }

            return (false, default);
        }

        /// <summary>
        ///     Tries to find an nonary function by a specific name.
        /// </summary>
        /// <param name="name">The function name.</param>
        /// <param name="value">The value, if one exists.</param>
        /// <returns><c>true</c> if the function is found, <c>false</c> otherwise.</returns>
        public bool TryFindNonary(
            string name,
            out Type value)
        {
            this.ThrowIfCurrentObjectDisposed();

            using ReadOnlySynchronizationLocker locker = this.EnsureFunctionsInitialized();

            return this.nonaryFunctions.TryGetValue(
                name,
                out value);
        }

        /// <summary>
        ///     Tries to find an unary function by a specific name.
        /// </summary>
        /// <param name="name">The function name.</param>
        /// <param name="value">The value, if one exists.</param>
        /// <returns><c>true</c> if the function is found, <c>false</c> otherwise.</returns>
        public bool TryFindUnary(
            string name,
            out Type value)
        {
            this.ThrowIfCurrentObjectDisposed();

            using ReadOnlySynchronizationLocker locker = this.EnsureFunctionsInitialized();

            return this.unaryFunctions.TryGetValue(
                name,
                out value);
        }

        /// <summary>
        ///     Tries to find an binary function by a specific name.
        /// </summary>
        /// <param name="name">The function name.</param>
        /// <param name="value">The value, if one exists.</param>
        /// <returns><c>true</c> if the function is found, <c>false</c> otherwise.</returns>
        public bool TryFindBinary(
            string name,
            out Type value)
        {
            this.ThrowIfCurrentObjectDisposed();

            using ReadOnlySynchronizationLocker locker = this.EnsureFunctionsInitialized();

            return this.binaryFunctions.TryGetValue(
                name,
                out value);
        }

        /// <summary>
        ///     Tries to find an ternary function by a specific name.
        /// </summary>
        /// <param name="name">The function name.</param>
        /// <param name="value">The value, if one exists.</param>
        /// <returns><c>true</c> if the function is found, <c>false</c> otherwise.</returns>
        public bool TryFindTernary(
            string name,
            out Type value)
        {
            this.ThrowIfCurrentObjectDisposed();

            using ReadOnlySynchronizationLocker locker = this.EnsureFunctionsInitialized();

            return this.ternaryFunctions.TryGetValue(
                name,
                out value);
        }

        /// <summary>
        ///     Extracts constants out of an expression.
        /// </summary>
        /// <param name="expression">The expression to extract constants from.</param>
        /// <returns>The expression, with constants removed, or the original expression if no constants were found.</returns>
        [SuppressMessage(
            "Performance",
            "HAA0401:Possible allocation of reference type enumerator",
            Justification = "We're OK with this enumeration.")]
        [SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "Currently string interpolation does this.")]
        internal string ExtractConstants(string expression)
        {
            this.ThrowIfCurrentObjectDisposed();

            using ReadOnlySynchronizationLocker locker = this.EnsureFunctionsInitialized();

            // TODO: Remove the suppression after string interpolation is optimized
            Log.Debug(
                $"Currently extracting constants from \"{expression}\" with {this.constantExtractors.Count} extractors.");

            InterpretationContext localContext = InterpretationContext.Current;

            // TODO: Remove the suppression after EnumerateValuesOnLevelKeys gets a struct enumerator
            foreach (var extractor in this.constantExtractors.EnumerateValuesOnLevelKeys())
            {
                if (localContext.CancellationToken.IsCancellationRequested)
                {
                    return expression;
                }

                string localExpression;
                try
                {
                    localExpression = extractor.ExtractAllConstants(
                        expression,
                        localContext.ConstantsTable,
                        localContext.ReverseConstantsTable,
                        localContext.Definition);
                }
                catch (Exception ex)
                {
                    Log.Error(
                        ex,
                        "Constant extractor threw exception.");

                    continue;
                }

                if (!string.IsNullOrEmpty(localExpression))
                {
                    Log.Debug($"Extracted constants, resulting in \"{expression}\".");

                    expression = localExpression;
                }
            }

            Log.Debug($"Final expression after constants extraction is \"{expression}\".");

            return expression;
        }

#region Disposable

        /// <summary>Disposes in the managed context.</summary>
        [SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP007:Don't dispose injected.",
            Justification = "These are not injected, the analyzer is misidentifying things.")]
        protected override void DisposeManagedContext()
        {
            this.constantExtractors.Dispose();
            this.constantInterpreters.Dispose();
            this.constantPassThroughExtractors.Dispose();
            this.stringFormatters.Dispose();

            base.DisposeManagedContext();
        }

#endregion

        [SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP011:Don't return disposed instance.",
            Justification = "This is acceptable in this case.")]
        private ReadOnlySynchronizationLocker EnsureFunctionsInitialized()
        {
            using (ReadOnlySynchronizationLocker readLocker = this.ReadLock())
            {
                if (this.initialized)
                {
                    return readLocker;
                }
            }

            using (ReadWriteSynchronizationLocker locker = this.ReadWriteLock())
            {
                if (this.initialized)
                {
                    // We'll se if anyone else, by any chance, also started writing
                    locker.Dispose();

                    return this.ReadLock();
                }

                locker.Upgrade();

                this.LoadData();
            }

            return this.ReadLock();
        }

        private void RegisterCustomTypeAction((TypeInfo Type, bool TolerateMissingAttribute) p) =>
            this.CheckType(
                p.Type,
                p.TolerateMissingAttribute);

        [SuppressMessage(
            "Performance",
            "HAA0603:Delegate allocation from a method group",
            Justification = "Acceptable for parallel foreach.")]
        private void InitializeAssembly(Assembly assembly) =>
            assembly.DefinedTypes.Where(
                    p => p.IsClass &&
                         !p.IsAbstract &&
                         !p.IsGenericTypeDefinition &&
                         typeof(IMathematicsPlugin).IsAssignableFrom(p))
                .ParallelForEach(this.RegisterAssemblyAction);

        private void RegisterAssemblyAction(TypeInfo p) => this.CheckType(p);

        private void ClearData()
        {
            this.initialized = false;

            this.nonaryFunctions.Clear();
            this.unaryFunctions.Clear();
            this.binaryFunctions.Clear();
            this.ternaryFunctions.Clear();

            this.constantExtractors.Clear();
            this.constantInterpreters.Clear();
            this.constantPassThroughExtractors.Clear();
            this.stringFormatters.Clear();
        }

        [SuppressMessage(
            "Performance",
            "HAA0603:Delegate allocation from a method group",
            Justification = "Acceptable for parallel foreach.")]
        [SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "Currently string interpolation does this.")]
        private void LoadData()
        {
            this.initialized = false;

            Log.Debug("Started refreshing plugins list.");

            this.constantExtractorsIndex = 2001;
            this.constantInterpretersIndex = 2001;
            this.constantPassThroughExtractorsIndex = 2001;
            this.stringFormattersIndex = 2001;

            this.assembliesToRegister.ParallelForEach(this.InitializeAssembly);
            this.customTypesToRegister.ForEach(this.RegisterCustomTypeAction);

            // TODO: Remove the suppression after string interpolation is optimized
            Log.Debug(
                $"Plugins: {this.constantExtractors.Count} CE, {this.constantInterpreters.Count} CI, {this.constantPassThroughExtractors.Count} CPTE, {this.stringFormatters.Count} SF, {this.nonaryFunctions.Count} NF, {this.unaryFunctions.Count} UF, {this.binaryFunctions.Count} BF, {this.ternaryFunctions.Count} TF.");

            this.initialized = true;
        }

        private void CheckType(
            TypeInfo p,
            bool tolerateMissingAttribute = false)
        {
            Log.Debug($"Started analyzing type {p.FullName}.");
            if (typeof(NonaryFunctionNodeBase).IsAssignableFrom(p) && p.HasPublicParameterlessConstructor())
            {
                Log.Debug($"Type {p.FullName} analyzed as NF.");
                AddToTypeDictionary(
                    p,
                    this.nonaryFunctions,
                    tolerateMissingAttribute);
            }
            else if (typeof(UnaryFunctionNodeBase).IsAssignableFrom(p))
            {
                Log.Debug($"Type {p.FullName} analyzed as UF.");
                AddToTypeDictionary(
                    p,
                    this.unaryFunctions,
                    tolerateMissingAttribute);
            }
            else if (typeof(BinaryFunctionNodeBase).IsAssignableFrom(p))
            {
                Log.Debug($"Type {p.FullName} analyzed as BF.");
                AddToTypeDictionary(
                    p,
                    this.binaryFunctions,
                    tolerateMissingAttribute);
            }
            else if (typeof(TernaryFunctionNodeBase).IsAssignableFrom(p))
            {
                Log.Debug($"Type {p.FullName} analyzed as TF.");
                AddToTypeDictionary(
                    p,
                    this.ternaryFunctions,
                    tolerateMissingAttribute);
            }

            if (!p.HasPublicParameterlessConstructor())
            {
                // All of the below require a public parameter-less constructor to work, so let's quit if we don't have one
                return;
            }

            if (typeof(IConstantsExtractor).IsAssignableFrom(p))
            {
                Log.Debug($"Type {p.FullName} analyzed as CE.");
                AddToPluginDictionary<IConstantsExtractor, ConstantsExtractorAttribute>(
                    this.constantExtractors,
                    p,
                    tolerateMissingAttribute,
                    ref this.constantExtractorsIndex);
            }

            if (typeof(IConstantPassThroughExtractor).IsAssignableFrom(p))
            {
                Log.Debug($"Type {p.FullName} analyzed as CPTE.");
                AddToPluginDictionary<IConstantPassThroughExtractor, ConstantsPassThroughExtractorAttribute>(
                    this.constantPassThroughExtractors,
                    p,
                    tolerateMissingAttribute,
                    ref this.constantPassThroughExtractorsIndex);
            }

            if (typeof(IConstantInterpreter).IsAssignableFrom(p))
            {
                Log.Debug($"Type {p.FullName} analyzed as CI.");
                AddToPluginDictionary<IConstantInterpreter, ConstantsInterpreterAttribute>(
                    this.constantInterpreters,
                    p,
                    tolerateMissingAttribute,
                    ref this.constantInterpretersIndex);
            }

            if (typeof(IStringFormatter).IsAssignableFrom(p))
            {
                Log.Debug($"Type {p.FullName} analyzed as SF.");
                AddToPluginDictionary<IStringFormatter, StringFormatterAttribute>(
                    this.stringFormatters,
                    p,
                    tolerateMissingAttribute,
                    ref this.stringFormattersIndex);
            }

            static void AddToPluginDictionary<T, TAttribute>(
                LevelDictionary<Type, T> levelDictionary,
                TypeInfo p,
                bool tolerateMissingAttribute,
                ref int cci)
                where TAttribute : Attribute, ILevelAttribute
            {
                int explicitLevel;
                var attr = p.GetCustomAttribute<TAttribute>(true);

                if (attr == null)
                {
                    if (tolerateMissingAttribute)
                    {
                        explicitLevel = Interlocked.Increment(ref cci);
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    explicitLevel = attr.Level == default ? Interlocked.Increment(ref cci) : attr.Level;
                }

                lock (levelDictionary)
                {
                    levelDictionary.Add(
                        p,
                        (T)p.Instantiate(),
                        explicitLevel);
                }
            }

            static void AddToTypeDictionary(
                TypeInfo p,
                IDictionary<string, Type> td,
                bool tolerateMissingAttribute)
            {
                string[] names;
                var attr = p.GetCustomAttribute<CallableMathematicsFunctionAttribute>(true);

                if (attr == null)
                {
                    if (tolerateMissingAttribute)
                    {
                        names = new[]
                        {
                            p.Name
                        };
                    }
                    else
                    {
                        return;
                    }
                }
                else
                {
                    names = attr.Names;
                }

                lock (td)
                {
                    foreach (var q in names)
                    {
                        if (td.ContainsKey(q))
                        {
                            continue;
                        }

                        td.Add(
                            q,
                            p.AsType());
                    }
                }
            }
        }

#endregion
    }
}