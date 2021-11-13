// <copyright file="ExpressionParsingServiceBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using IX.Math.Extensibility;
using IX.Math.Extraction;
using IX.Math.Generators;
using IX.Math.Nodes;
using IX.Math.Nodes.Constants;
using IX.Math.Registration;
using IX.StandardExtensions.ComponentModel;
using IX.StandardExtensions.Contracts;
using IX.StandardExtensions.Extensions;
using IX.StandardExtensions.Threading;
using IX.System.Collections.Generic;
using JetBrains.Annotations;
using DiagCA = System.Diagnostics.CodeAnalysis;
using IConstantPassThroughExtractor = IX.Math.Extensibility.IConstantPassThroughExtractor;
using IConstantsExtractor = IX.Math.Extensibility.IConstantsExtractor;

namespace IX.Math
{
    /// <summary>
    ///     A base class for an expression parsing service.
    /// </summary>
    /// <seealso cref="DisposableBase" />
    /// <seealso cref="IExpressionParsingService" />
    [PublicAPI]
    public abstract class ExpressionParsingServiceBase : ReaderWriterSynchronizedBase, IExpressionParsingService
    {
        private readonly List<Assembly> assembliesToRegister;

        private readonly LevelDictionary<Type, IConstantsExtractor> constantExtractors;
        private readonly LevelDictionary<Type, IConstantInterpreter> constantInterpreters;
        private readonly LevelDictionary<Type, IConstantPassThroughExtractor> constantPassThroughExtractors;
        private readonly List<IStringFormatter> stringFormatters;

        private readonly Dictionary<string, Type> nonaryFunctions;
        private readonly Dictionary<string, Type> unaryFunctions;
        private readonly Dictionary<string, Type> binaryFunctions;
        private readonly Dictionary<string, Type> ternaryFunctions;

        private readonly MathDefinition workingDefinition;

        private bool isInitialized = false;

        private int interpretationDone;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExpressionParsingServiceBase" /> class with a specified math
        ///     definition
        ///     object.
        /// </summary>
        /// <param name="definition">The math definition to use.</param>
        protected private ExpressionParsingServiceBase(MathDefinition definition)
        {
            // Preconditions
            Requires.NotNull(out this.workingDefinition, definition, nameof(definition));

            // Initialized internal state
            this.constantExtractors = new LevelDictionary<Type, IConstantsExtractor>();
            this.constantInterpreters = new LevelDictionary<Type, IConstantInterpreter>();
            this.constantPassThroughExtractors = new LevelDictionary<Type, IConstantPassThroughExtractor>();
            this.stringFormatters = new List<IStringFormatter>();

            this.nonaryFunctions = new Dictionary<string, Type>();
            this.unaryFunctions = new Dictionary<string, Type>();
            this.binaryFunctions = new Dictionary<string, Type>();
            this.ternaryFunctions = new Dictionary<string, Type>();

            this.assembliesToRegister = new List<Assembly>
            {
                typeof(ExpressionParsingService).GetTypeInfo()
                    .Assembly
            };
        }

        /// <summary>
        ///     Interprets the mathematical expression and returns a container that can be invoked for solving using specific
        ///     mathematical types.
        /// </summary>
        /// <param name="expression">The expression to interpret.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A <see cref="ComputedExpression" /> that represents the interpreted expression.</returns>
        public abstract ComputedExpression Interpret(
            string expression,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Interprets the mathematical expression and returns a container that can be invoked for solving using specific
        ///     mathematical types.
        /// </summary>
        /// <param name="expression">The expression to interpret.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>
        ///     A <see cref="ComputedExpression" /> that represents a compilable form of the original expression, if the
        ///     expression itself makes sense.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="expression" /> is either null, empty or whitespace-only.</exception>
        [DiagCA.SuppressMessage(
            "Performance",
            "HAA0603:Delegate allocation from a method group",
            Justification = "We're OK with this.")]
        protected ComputedExpression InterpretInternal(
            string expression,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNullOrWhiteSpace(
                expression,
                nameof(expression));

            this.ThrowIfCurrentObjectDisposed();

            using var innerLock = this.EnsureInitialization();

            // Check expression through pass-through extractors
            if (this.constantPassThroughExtractors.KeysByLevel.SelectMany(p => p.Value)
                .Any(
                    ConstantPassThroughExtractorPredicate,
                    expression,
                    this))
            {
                return new ComputedExpression(
                    expression,
                    new StringNode(expression),
                    true,
                    new StandardParameterRegistry(this.stringFormatters),
                    this.stringFormatters,
                    null);
            }

            static bool ConstantPassThroughExtractorPredicate(
                Type cpteKey,
                string innerExpression,
                ExpressionParsingServiceBase innerThis)
            {
                return innerThis.constantPassThroughExtractors[cpteKey]
                    .Evaluate(innerExpression);
            }

            ComputedExpression result;
            using (var workingSet = new WorkingExpressionSet(
                expression,
                this.workingDefinition.DeepClone(),
                this.nonaryFunctions,
                this.unaryFunctions,
                this.binaryFunctions,
                this.ternaryFunctions,
                this.constantExtractors,
                this.constantInterpreters,
                this.stringFormatters,
                cancellationToken))
            {
                (NodeBase node, IParameterRegistry parameterRegistry) = ExpressionGenerator.CreateBody(workingSet);

                result = !workingSet.Success
                    ? new ComputedExpression(
                        expression,
                        null,
                        false,
                        null,
                        this.stringFormatters,
                        null)
                    : new ComputedExpression(
                        expression,
                        node,
                        true,
                        parameterRegistry,
                        this.stringFormatters,
                        workingSet.OfferReservedType);

                Interlocked.MemoryBarrier();
            }

            Interlocked.Exchange(
                ref this.interpretationDone,
                1);
            return result;
        }

        /// <summary>
        ///     Returns the prototypes of all registered functions.
        /// </summary>
        /// <returns>All function names, with all possible combinations of input and output data.</returns>
        public string[] GetRegisteredFunctions()
        {
            this.ThrowIfCurrentObjectDisposed();

            using var innerLock = this.EnsureInitialization();

            // Capacity is sum of all, times 3; the "3" number was chosen as a good-enough average of how many overloads are defined, on average
            var bldr = new List<string>(
                (this.nonaryFunctions.Count +
                 this.unaryFunctions.Count +
                 this.binaryFunctions.Count +
                 this.ternaryFunctions.Count) *
                3);

            bldr.AddRange(this.nonaryFunctions.Select(function => $"{function.Key}()"));

            (from KeyValuePair<string, Type> function in this.unaryFunctions
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

            (from KeyValuePair<string, Type> function in this.binaryFunctions
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

            (from KeyValuePair<string, Type> function in this.ternaryFunctions
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
        ///     Registers an assembly to extract compatible functions from.
        /// </summary>
        /// <param name="assembly">The assembly to register.</param>
        public void RegisterFunctionsAssembly(Assembly assembly)
        {
            Requires.NotNull(
                assembly,
                nameof(assembly));

            this.ThrowIfCurrentObjectDisposed();

            using var innerLocker = this.ReadWriteLock();

            if (this.isInitialized)
            {
                throw new InvalidOperationException("Initialization has already completed, so you cannot register any more assemblies for this service.");
            }

            if (this.assembliesToRegister.Contains(assembly))
            {
                return;
            }

            innerLocker.Upgrade();

            this.assembliesToRegister.Add(assembly);
        }

        /// <summary>
        ///     Registers type formatters.
        /// </summary>
        /// <param name="formatter">The formatter to register.</param>
        /// <exception cref="InvalidOperationException">
        ///     This method was called after having called <see cref="Interpret" />
        ///     successfully for the first time.
        /// </exception>
        public void RegisterTypeFormatter(IStringFormatter formatter)
        {
            Requires.NotNull(
                formatter,
                nameof(formatter));

            if (this.interpretationDone != 0)
            {
                throw new InvalidOperationException(
                    Resources
                        .TheExpressionParsingServiceHasAlreadyDoneInterpretationAndCannotHaveAnyMoreFormattersRegistered);
            }

            this.stringFormatters.Add(formatter);
        }

        /// <summary>
        ///     Disposes in the managed context.
        /// </summary>
        protected override void DisposeManagedContext()
        {
            this.nonaryFunctions.Clear();
            this.unaryFunctions.Clear();
            this.binaryFunctions.Clear();
            this.ternaryFunctions.Clear();

            this.stringFormatters.Clear();
            this.constantExtractors.Clear();
            this.constantInterpreters.Clear();
            this.constantPassThroughExtractors.Clear();

            this.assembliesToRegister.Clear();

            base.DisposeManagedContext();
        }

        [DiagCA.SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP017:Prefer using.",
            Justification = "This ")]
        private SynchronizationLocker EnsureInitialization()
        {
            var innerLock = this.ReadLock();

            if (this.isInitialized)
            {
                return innerLock;
            }

            innerLock.Dispose();

            var innerWriteLock = this.ReadWriteLock();

            if (this.isInitialized)
            {
                return innerWriteLock;
            }

            try
            {
                innerWriteLock.Upgrade();

                // Initializing functions dictionaries
                this.assembliesToRegister.GenerateInternalNonaryFunctionsDictionary(this.nonaryFunctions);
                this.assembliesToRegister.GenerateInternalUnaryFunctionsDictionary(this.unaryFunctions);
                this.assembliesToRegister.GenerateInternalBinaryFunctionsDictionary(this.binaryFunctions);
                this.assembliesToRegister.GenerateInternalTernaryFunctionsDictionary(this.ternaryFunctions);

                // Extractors
                this.InitializePassThroughExtractorsDictionary();
                this.InitializeExtractorsDictionary();
                this.InitializeInterpretersDictionary();

                this.isInitialized = true;
            }
            finally
            {
                innerWriteLock.Dispose();
            }

            return this.ReadLock();
        }

        [DiagCA.SuppressMessage(
            "StyleCop.CSharp.ReadabilityRules",
            "SA1118:Parameter should not span multiple lines",
            Justification = "Parameters are very complex in this method.")]
        private void InitializeExtractorsDictionary()
        {
            this.constantExtractors.Add(typeof(StringExtractor), new StringExtractor(), 1000);
            this.constantExtractors.Add(typeof(ScientificFormatNumberExtractor), new ScientificFormatNumberExtractor(), 2000);

            var incrementer = 2001;
            this.assembliesToRegister.GetTypesAssignableFrom<IConstantsExtractor>()
                .Where(
                    p => p.IsClass &&
                         !p.IsAbstract &&
                         !p.IsGenericTypeDefinition &&
                         p.HasPublicParameterlessConstructor())
                .Select(p => p.AsType())
                .Where(
                    (
                        p,
                        thisL1) => !thisL1.constantExtractors.ContainsKey(p),
                    this)
                .ForEach(
                    (
                        in Type p,
                        ref int i,
                        ExpressionParsingServiceBase thisL1) =>
                    {
                        if (p.Instantiate() is not IConstantsExtractor extractor)
                        {
                            return;
                        }

                        thisL1.constantExtractors.Add(
                            p,
                            extractor,
                            p.GetAttributeDataByTypeWithoutVersionBinding<ConstantsExtractorAttribute, int>(
                                out var explicitLevel)
                                ? explicitLevel
                                : Interlocked.Increment(ref i));
                    },
                    ref incrementer,
                    this);
        }

        [DiagCA.SuppressMessage(
            "StyleCop.CSharp.ReadabilityRules",
            "SA1118:Parameter should not span multiple lines",
            Justification = "Parameters are very complex in this method.")]
        private void InitializePassThroughExtractorsDictionary()
        {
            var incrementer = 2001;
            this.assembliesToRegister.GetTypesAssignableFrom<IConstantPassThroughExtractor>()
                .Where(
                    p => p.IsClass &&
                         !p.IsAbstract &&
                         !p.IsGenericTypeDefinition &&
                         p.HasPublicParameterlessConstructor())
                .Select(p => p.AsType())
                .Where(
                    (
                        p,
                        thisL1) => !thisL1.constantPassThroughExtractors.ContainsKey(p),
                    this)
                .ForEach(
                    (
                        in Type p,
                        ref int i,
                        ExpressionParsingServiceBase thisL1) =>
                    {
                        if (p.Instantiate() is not IConstantPassThroughExtractor extractor)
                        {
                            return;
                        }

                        thisL1.constantPassThroughExtractors.Add(
                            p,
                            extractor,
                            p.GetAttributeDataByTypeWithoutVersionBinding<ConstantsPassThroughExtractorAttribute, int>(
                                out var explicitLevel)
                                ? explicitLevel
                                : Interlocked.Increment(ref i));
                    },
                    ref incrementer,
                    this);
        }

        [DiagCA.SuppressMessage(
            "StyleCop.CSharp.ReadabilityRules",
            "SA1118:Parameter should not span multiple lines",
            Justification = "Parameters are very complex in this method.")]
        private void InitializeInterpretersDictionary()
        {
            var incrementer = 2001;
            this.assembliesToRegister.GetTypesAssignableFrom<IConstantInterpreter>()
                .Where(
                    p => p.IsClass &&
                         !p.IsAbstract &&
                         !p.IsGenericTypeDefinition &&
                         p.HasPublicParameterlessConstructor())
                .Select(p => p.AsType())
                .Where(
                    (
                        p,
                        thisL1) => !thisL1.constantInterpreters.ContainsKey(p),
                    this)
                .ForEach(
                    (
                        in Type p,
                        ref int i,
                        ExpressionParsingServiceBase thisL1) =>
                    {
                        if (p.Instantiate() is not IConstantInterpreter interpreter)
                        {
                            return;
                        }

                        thisL1.constantInterpreters.Add(
                            p,
                            interpreter,
                            p.GetAttributeDataByTypeWithoutVersionBinding<ConstantsInterpreterAttribute, int>(
                                out var explicitLevel)
                                ? explicitLevel
                                : Interlocked.Increment(ref i));
                    },
                    ref incrementer,
                    this);
        }
    }
}