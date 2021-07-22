// <copyright file="PluginCollection.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Threading;
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

        #region Assemblies and types
        private readonly List<Assembly> assembliesToRegister;
        private readonly List<(TypeInfo Type, bool TolerateMissingAttribute)> customTypesToRegister;
        #endregion

        [SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP006:Implement IDisposable.",
            Justification = "IDisposable is correctly implemented.")]
        private readonly LevelDictionary<Type, IConstantsExtractor> constantExtractors;

        [SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP006:Implement IDisposable.",
            Justification = "IDisposable is correctly implemented.")]
        private readonly LevelDictionary<Type, IConstantInterpreter> constantInterpreters;

        [SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP006:Implement IDisposable.",
            Justification = "IDisposable is correctly implemented.")]
        private readonly LevelDictionary<Type, IConstantPassThroughExtractor> constantPassThroughExtractors;

        [SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP006:Implement IDisposable.",
            Justification = "IDisposable is correctly implemented.")]
        private readonly LevelDictionary<Type, IStringFormatter> stringFormatters;

        #region Function dictionaries

        private readonly Dictionary<string, Type> binaryFunctions;
        private readonly Dictionary<string, Type> ternaryFunctions;
        private readonly Dictionary<string, Type> unaryFunctions;
        private readonly Dictionary<string, Type> nonaryFunctions;

        #endregion

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
            this.assembliesToRegister = new();
            this.customTypesToRegister = new();

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

        /// <summary>
        /// The current plugin collection.
        /// </summary>
        public static PluginCollection Current { get; } = new();

        #region Methods

        /// <summary>
        /// Interprets an expression based on constant interpreters.
        /// </summary>
        /// <param name="expression">The expression to interpret.</param>
        /// <returns>A node, if the expression is recognized, or <c>null</c> (<c>Nothing</c> in Visual Basic) otherwise.</returns>
        public ConstantNodeBase? InterpretExpression(string expression)
        {
            this.ThrowIfCurrentObjectDisposed();

            this.EnsureFunctionsInitialized();

            using var locker = this.ReadLock();

            foreach (var interpreter in this.constantInterpreters.KeysByLevel.SelectMany(p => p.Value))
            {
                var (success, result) = this.constantInterpreters[interpreter].EvaluateIsConstant(expression);
                if (success)
                {
                    return result;
                }
            }

            return null;
        }

        /// <summary>
        /// Extracts constants out of an expression.
        /// </summary>
        /// <param name="expression">The expression to extract constants from.</param>
        /// <returns>The expression, with constants removed, or the original expression if no constants were found.</returns>
        internal string ExtractConstants(string expression)
        {
            this.ThrowIfCurrentObjectDisposed();

            this.EnsureFunctionsInitialized();

            using var locker = this.ReadLock();

            var localContext = InterpretationContext.Current;
            foreach (var extractor in this.constantExtractors.EnumerateValuesOnLevelKeys())
            {
                if (localContext.CancellationToken.IsCancellationRequested)
                {
                    return expression;
                }

                string localExpression;
                try
                {
                    localExpression = extractor
                        .ExtractAllConstants(
                            expression,
                            localContext.ConstantsTable,
                            localContext.ReverseConstantsTable,
                            localContext.Definition);
                }
                catch
                {
                    continue;
                }

                if (!string.IsNullOrEmpty(localExpression))
                {
                    expression = localExpression;
                }
            }

            return expression;
        }

        /// <summary>
        /// Checks whether or not the expression is recognized by a pass-through extractor.
        /// </summary>
        /// <param name="expression">The expression to check.</param>
        /// <returns><c>true</c> if the expression is a constant, <c>false</c> otherwise.</returns>
        [SuppressMessage(
            "Performance",
            "HAA0603:Delegate allocation from a method group",
            Justification = "We don't care.")]
        public bool CheckExpressionPassThroughConstant(string expression)
        {
            this.ThrowIfCurrentObjectDisposed();

            this.EnsureFunctionsInitialized();

            using var locker = this.ReadLock();

            return this.constantPassThroughExtractors.EnumerateValuesOnLevelKeys()
                .Any(
                    ConstantPassThroughExtractorPredicate,
                    expression);

            static bool ConstantPassThroughExtractorPredicate(
                IConstantPassThroughExtractor extractor,
                string innerExpression)
            {
                return extractor
                    .Evaluate(innerExpression);
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

            using ReadWriteSynchronizationLocker locker = this.ReadWriteLock();

            if (this.assembliesToRegister.Contains(assembly))
            {
                return;
            }

            locker.Upgrade();

            this.assembliesToRegister.Add(assembly);

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
        /// Registers a specific class as a plugin.
        /// </summary>
        /// <typeparam name="TPlugin">The type of the plugin to register.</typeparam>
        /// <param name="tolerateMissingAttribute">Whether or not to tolerate the missing attribute when registering.</param>
        public void RegisterSpecificPlugin<TPlugin>(bool tolerateMissingAttribute = false)
            where TPlugin : class =>
            this.RegisterSpecificPlugin(typeof(TPlugin).GetTypeInfo(), tolerateMissingAttribute);

        /// <summary>
        /// Registers a specific class as a plugin.
        /// </summary>
        /// <param name="pluginType">The type of the plugin to register.</param>
        /// <param name="tolerateMissingAttribute">Whether or not to tolerate the missing attribute when registering.</param>
        public void RegisterSpecificPlugin(Type pluginType, bool tolerateMissingAttribute = false) =>
            this.RegisterSpecificPlugin(Requires.NotNull(pluginType, nameof(pluginType)).GetTypeInfo(), tolerateMissingAttribute);

        /// <summary>
        /// Registers a specific class as a plugin.
        /// </summary>
        /// <param name="pluginType">The type of the plugin to register.</param>
        /// <param name="tolerateMissingAttribute">Whether or not to tolerate the missing attribute when registering.</param>
        [SuppressMessage("Performance", "HAA0301:Closure Allocation Source", Justification = "LINQ works like this")]
        public void RegisterSpecificPlugin(TypeInfo pluginType, bool tolerateMissingAttribute = false)
        {
            Requires.NotNull(pluginType, nameof(pluginType));

            this.ThrowIfCurrentObjectDisposed();

            this.EnsureFunctionsInitialized();

            using var locker = this.ReadWriteLock();

            if (this.customTypesToRegister.Any(p => p.Type == pluginType))
            {
                return;
            }

            locker.Upgrade();

            this.CheckType(pluginType, tolerateMissingAttribute);

            this.customTypesToRegister.Add((pluginType, tolerateMissingAttribute));
        }

        /// <summary>
        /// Resets the plugin collection entirely.
        /// </summary>
        public void Reset()
        {
            using (this.WriteLock())
            {
                this.assembliesToRegister.Clear();
                this.customTypesToRegister.Clear();

                this.ClearData();
            }
        }

        /// <summary>
        ///     Returns the prototypes of all registered functions.
        /// </summary>
        /// <returns>All function names, with all possible combinations of input and output data.</returns>
        public string[] GetRegisteredFunctions()
        {
            this.ThrowIfCurrentObjectDisposed();

            this.EnsureFunctionsInitialized();

            using var locker = this.ReadLock();

            // Capacity is sum of all, times 3; the "3" number was chosen as a good-enough average of how many overloads are defined, on average
            var bldr = new List<string>(
                (this.nonaryFunctions.Count +
                 this.unaryFunctions.Count +
                 this.binaryFunctions.Count +
                 this.ternaryFunctions.Count) *
                3);

            bldr.AddRange(this.nonaryFunctions.Select(function => $"{function.Key}()"));

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
                    bldrL1) => bldrL1.Add($"{parameter.functionName}({parameter.parameterName})"),
                bldr);

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
                    bldrL1) => bldrL1.Add(
                    $"{parameter.functionName}({parameter.parameterNameLeft}, {parameter.parameterNameRight})"),
                bldr);

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
                    bldrL1) => bldrL1.Add(
                    $"{parameter.functionName}({parameter.parameterNameLeft}, {parameter.parameterNameMiddle}, {parameter.parameterNameRight})"),
                bldr);

            return bldr.ToArray();
        }

        /// <summary>
        /// Interprets an object as a string.
        /// </summary>
        /// <typeparam name="T">The type of object to interpret.</typeparam>
        /// <param name="value">The value to interpret as string.</param>
        /// <returns>A success status, and the resulting string.</returns>
        public (bool Success, string? ResultingString) InterpretAsString<T>(T value)
        {
            this.ThrowIfCurrentObjectDisposed();

            this.EnsureFunctionsInitialized();

            using ReadOnlySynchronizationLocker locker = this.ReadLock();

            foreach (var interpreter in this.stringFormatters.EnumerateValuesOnLevelKeys())
            {
                var (success, result) = interpreter.ParseIntoString(value);
                if (success)
                {
                    return (true, result);
                }
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

            this.EnsureFunctionsInitialized();

            using ReadOnlySynchronizationLocker locker = this.ReadLock();

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

            this.EnsureFunctionsInitialized();

            using ReadOnlySynchronizationLocker locker = this.ReadLock();

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

            this.EnsureFunctionsInitialized();

            using ReadOnlySynchronizationLocker locker = this.ReadLock();

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

            this.EnsureFunctionsInitialized();

            using ReadOnlySynchronizationLocker locker = this.ReadLock();

            return this.ternaryFunctions.TryGetValue(
                name,
                out value);
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
            "Performance",
            "HAA0603:Delegate allocation from a method group",
            Justification = "Acceptable for parallel foreach.")]
        private void EnsureFunctionsInitialized()
        {
            using ReadWriteSynchronizationLocker locker = this.ReadWriteLock();

            if (this.initialized)
            {
                return;
            }

            locker.Upgrade();

            if (this.initialized)
            {
                return;
            }

            this.constantExtractorsIndex = 2001;
            this.constantInterpretersIndex = 2001;
            this.constantPassThroughExtractorsIndex = 2001;
            this.stringFormattersIndex = 2001;

            this.assembliesToRegister.ParallelForEach(this.InitializeAssembly);
            this.customTypesToRegister.ForEach(this.RegisterCustomTypeAction);

            this.initialized = true;
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
            this.nonaryFunctions.Clear();
            this.unaryFunctions.Clear();
            this.binaryFunctions.Clear();
            this.ternaryFunctions.Clear();

            this.constantExtractors.Clear();
            this.constantInterpreters.Clear();
            this.constantPassThroughExtractors.Clear();
            this.stringFormatters.Clear();

            this.initialized = false;
        }

        private void CheckType(
            TypeInfo p,
            bool tolerateMissingAttribute = false)
        {
            if (typeof(NonaryFunctionNodeBase).IsAssignableFrom(p) && p.HasPublicParameterlessConstructor())
            {
                AddToTypeDictionary(
                    p,
                    this.nonaryFunctions,
                    tolerateMissingAttribute);
            }
            else if (typeof(UnaryFunctionNodeBase).IsAssignableFrom(p))
            {
                AddToTypeDictionary(
                    p,
                    this.unaryFunctions,
                    tolerateMissingAttribute);
            }
            else if (typeof(BinaryFunctionNodeBase).IsAssignableFrom(p))
            {
                AddToTypeDictionary(
                    p,
                    this.binaryFunctions,
                    tolerateMissingAttribute);
            }
            else if (typeof(TernaryFunctionNodeBase).IsAssignableFrom(p))
            {
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
                AddToPluginDictionary<IConstantsExtractor, ConstantsExtractorAttribute>(
                    this.constantExtractors,
                    p,
                    tolerateMissingAttribute,
                    ref this.constantExtractorsIndex);
            }

            if (typeof(IConstantPassThroughExtractor).IsAssignableFrom(p))
            {
                AddToPluginDictionary<IConstantPassThroughExtractor, ConstantsPassThroughExtractorAttribute>(
                    this.constantPassThroughExtractors,
                    p,
                    tolerateMissingAttribute,
                    ref this.constantPassThroughExtractorsIndex);
            }

            if (typeof(IConstantInterpreter).IsAssignableFrom(p))
            {
                AddToPluginDictionary<IConstantInterpreter, ConstantsInterpreterAttribute>(
                    this.constantInterpreters,
                    p,
                    tolerateMissingAttribute,
                    ref this.constantInterpretersIndex);
            }

            if (typeof(IStringFormatter).IsAssignableFrom(p))
            {
                AddToPluginDictionary<IStringFormatter, StringFormatterAttribute>(
                    this.stringFormatters,
                    p,
                    tolerateMissingAttribute,
                    ref this.stringFormattersIndex);
            }

            static void AddToPluginDictionary<T, TAttribute>(LevelDictionary<Type, T> levelDictionary, TypeInfo p, bool tolerateMissingAttribute, ref int cci)
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