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
    public abstract class ExpressionParsingServiceBase : DisposableBase, IExpressionParsingService
    {
        private List<Assembly> assembliesToRegister;
        private Dictionary<string, Type> binaryFunctions;

        private LevelDictionary<Type, IConstantsExtractor> constantExtractors;
        private LevelDictionary<Type, IConstantInterpreter> constantInterpreters;
        private LevelDictionary<Type, IConstantPassThroughExtractor> constantPassThroughExtractors;

        private int interpretationDone;
        private Dictionary<string, Type> nonaryFunctions;
        private List<IStringFormatter> stringFormatters;
        private Dictionary<string, Type> ternaryFunctions;
        private Dictionary<string, Type> unaryFunctions;
        private MathDefinition workingDefinition;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExpressionParsingServiceBase" /> class with a specified math
        ///     definition
        ///     object.
        /// </summary>
        /// <param name="definition">The math definition to use.</param>
        protected private ExpressionParsingServiceBase(MathDefinition definition)
        {
            Contract.RequiresNotNull(ref this.workingDefinition, definition, nameof(definition));

            this.assembliesToRegister = new List<Assembly>
            {
                typeof(ExpressionParsingService).GetTypeInfo()
                    .Assembly
            };

            this.stringFormatters = new List<IStringFormatter>();
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
            "HAA0401:Possible allocation of reference type enumerator",
            Justification = "Unavoidable here.")]
        [DiagCA.SuppressMessage(
            "Performance",
            "HAA0603:Delegate allocation from a method group",
            Justification = "We're OK with this.")]
        protected ComputedExpression InterpretInternal(
            string expression,
            CancellationToken cancellationToken = default)
        {
            Contract.RequiresNotNullOrWhitespace(
                expression,
                nameof(expression));

            this.ThrowIfCurrentObjectDisposed();

            if (this.constantPassThroughExtractors == null)
            {
                this.InitializePassThroughExtractorsDictionary();
            }

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

            if (this.nonaryFunctions == null ||
                this.unaryFunctions == null ||
                this.binaryFunctions == null ||
                this.ternaryFunctions == null)
            {
                this.InitializeFunctionsDictionary();
            }

            if (this.constantExtractors == null)
            {
                this.InitializeExtractorsDictionary();
            }

            if (this.constantInterpreters == null)
            {
                this.InitializeInterpretersDictionary();
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

            if (this.nonaryFunctions == null ||
                this.unaryFunctions == null ||
                this.binaryFunctions == null ||
                this.ternaryFunctions == null)
            {
                this.InitializeFunctionsDictionary();
            }

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
            Contract.RequiresNotNull(
                in assembly,
                nameof(assembly));

            this.ThrowIfCurrentObjectDisposed();

            if (this.assembliesToRegister.Contains(assembly))
            {
                return;
            }

            this.assembliesToRegister.Add(assembly);

            this.nonaryFunctions?.Clear();
            this.unaryFunctions?.Clear();
            this.binaryFunctions?.Clear();
            this.ternaryFunctions?.Clear();
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
            Contract.RequiresNotNull(
                in formatter,
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
            this.nonaryFunctions?.Clear();
            this.unaryFunctions?.Clear();
            this.binaryFunctions?.Clear();
            this.ternaryFunctions?.Clear();
            this.assembliesToRegister?.Clear();
            this.stringFormatters?.Clear();

            base.DisposeManagedContext();
        }

        /// <summary>
        ///     Disposes in the general (managed and unmanaged) context.
        /// </summary>
        protected override void DisposeGeneralContext()
        {
            base.DisposeGeneralContext();

            Interlocked.Exchange(
                ref this.nonaryFunctions,
                null);
            Interlocked.Exchange(
                ref this.unaryFunctions,
                null);
            Interlocked.Exchange(
                ref this.binaryFunctions,
                null);
            Interlocked.Exchange(
                ref this.ternaryFunctions,
                null);
            Interlocked.Exchange(
                ref this.assembliesToRegister,
                null);
            Interlocked.Exchange(
                ref this.stringFormatters,
                null);

            Interlocked.Exchange(
                ref this.workingDefinition,
                null);
        }

        private void InitializeFunctionsDictionary()
        {
            Interlocked.Exchange(
                    ref this.nonaryFunctions,
                    FunctionsDictionaryGenerator.GenerateInternalNonaryFunctionsDictionary(this.assembliesToRegister))
                ?.Clear();

            Interlocked.Exchange(
                    ref this.unaryFunctions,
                    FunctionsDictionaryGenerator.GenerateInternalUnaryFunctionsDictionary(this.assembliesToRegister))
                ?.Clear();

            Interlocked.Exchange(
                    ref this.binaryFunctions,
                    FunctionsDictionaryGenerator.GenerateInternalBinaryFunctionsDictionary(this.assembliesToRegister))
                ?.Clear();

            Interlocked.Exchange(
                    ref this.ternaryFunctions,
                    FunctionsDictionaryGenerator.GenerateInternalTernaryFunctionsDictionary(this.assembliesToRegister))
                ?.Clear();
        }

        [DiagCA.SuppressMessage(
            "StyleCop.CSharp.ReadabilityRules",
            "SA1118:Parameter should not span multiple lines",
            Justification = "Parameters are very complex in this method.")]
        [DiagCA.SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP004:Don't ignore created IDisposable.",
            Justification = "Objects are correctly disposed, but the analyzer can't tell.")]
        private void InitializeExtractorsDictionary()
        {
            Interlocked.Exchange(
                    ref this.constantExtractors,
                    new LevelDictionary<Type, IConstantsExtractor>
                    {
                        { typeof(StringExtractor), new StringExtractor(), 1000 },
                        { typeof(ScientificFormatNumberExtractor), new ScientificFormatNumberExtractor(), 2000 }
                    })
                ?.Dispose();

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
                        Type p,
                        ref int i,
                        ExpressionParsingServiceBase thisL1) =>
                    {
                        thisL1.constantExtractors.Add(
                            p,
                            (IConstantsExtractor)p.Instantiate(),
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
        [DiagCA.SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP004:Don't ignore created IDisposable.",
            Justification = "We're not ignoring it, but the analysis can't tell.")]
        private void InitializePassThroughExtractorsDictionary()
        {
            Interlocked.Exchange(
                    ref this.constantPassThroughExtractors,
                    new LevelDictionary<Type, IConstantPassThroughExtractor>())
                ?.Dispose();

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
                        Type p,
                        ref int i,
                        ExpressionParsingServiceBase thisL1) =>
                    {
                        thisL1.constantPassThroughExtractors.Add(
                            p,
                            (IConstantPassThroughExtractor)p.Instantiate(),
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
        [DiagCA.SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP004:Don't ignore created IDisposable.",
            Justification = "Objects are correctly disposed, but the analyzer can't tell.")]
        private void InitializeInterpretersDictionary()
        {
            Interlocked.Exchange(
                    ref this.constantInterpreters,
                    new LevelDictionary<Type, IConstantInterpreter>())
                ?.Dispose();

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
                        Type p,
                        ref int i,
                        ExpressionParsingServiceBase thisL1) =>
                    {
                        thisL1.constantInterpreters.Add(
                            p,
                            (IConstantInterpreter)p.Instantiate(),
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