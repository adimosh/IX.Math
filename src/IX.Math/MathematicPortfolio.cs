// <copyright file="MathematicPortfolio.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using IX.Math.Computation;
using IX.Math.Exceptions;
using IX.Math.Extensibility;
using IX.Math.Extraction;
using IX.Math.Generators;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;
using IX.Math.Obsolete;
using IX.Math.WorkingSet;
using IX.StandardExtensions.ComponentModel;
using IX.StandardExtensions.Contracts;
using IX.StandardExtensions.Efficiency;
using IX.StandardExtensions.Extensions;
using IX.System.Collections.Generic;
using JetBrains.Annotations;

namespace IX.Math
{
    /// <summary>
    /// A holder for a portfolio of computable and computed mathematics expressions.
    /// </summary>
    /// <seealso cref="IX.StandardExtensions.ComponentModel.DisposableBase" />
    [PublicAPI]
    public class MathematicPortfolio : DisposableBase, IMathematicPortfolio
    {
        private static readonly ConcurrentDictionary<string, ExternalParameterNode> EmptyParameterRegistry = new ConcurrentDictionary<string, ExternalParameterNode>();

        #if NET452
        /// <summary>
        /// Gets the current context of the mathematic portfolio.
        /// </summary>
        /// <value>
        /// The current context.
        /// </value>
        [field: ThreadStatic]
        public static ThreadStatic<MathematicPortfolio> CurrentContext { get; private set; }
        #else
        /// <summary>
        /// Gets the current context of the mathematic portfolio.
        /// </summary>
        /// <value>
        /// The current context.
        /// </value>
        public static AsyncLocal<MathematicPortfolio> CurrentContext { get; } = new AsyncLocal<MathematicPortfolio>();
        #endif

        private readonly ConcurrentDictionary<ExpressionTypedKey, CompiledExpressionResult> compiledDelegate =
            new ConcurrentDictionary<ExpressionTypedKey, CompiledExpressionResult>();

        private readonly ConcurrentDictionary<string, ComputedExpression> computedExpressions =
            new ConcurrentDictionary<string, ComputedExpression>();

        private readonly List<Assembly> assembliesToRegister;

        private readonly MathDefinition workingDefinition;

        private readonly List<IStringFormatter> stringFormatters;

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP006:Implement IDisposable.",
            Justification = "We have it correctly implemented, but the analyzer can't tell.")]
        private readonly LevelDictionary<Type, IConstantsExtractor> constantExtractors;
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP006:Implement IDisposable.",
            Justification = "We have it correctly implemented, but the analyzer can't tell.")]
        private readonly LevelDictionary<Type, IConstantInterpreter> constantInterpreters;
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP006:Implement IDisposable.",
            Justification = "We have it correctly implemented, but the analyzer can't tell.")]
        private readonly LevelDictionary<Type, IConstantPassThroughExtractor> constantPassThroughExtractors;

        private readonly Dictionary<string, Type> nonaryFunctions;
        private readonly Dictionary<string, Type> unaryFunctions;
        private readonly Dictionary<string, Type> binaryFunctions;
        private readonly Dictionary<string, Type> ternaryFunctions;

        /// <summary>
        /// Initializes a new instance of the <see cref="MathematicPortfolio"/> class.
        /// </summary>
        /// <param name="functionAssemblies">The function assemblies.</param>
        public MathematicPortfolio(params Assembly[] functionAssemblies)
            : this(
                MathDefinition.Default,
                null,
                functionAssemblies)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MathematicPortfolio"/> class.
        /// </summary>
        /// <param name="stringFormatter">The string formatter.</param>
        /// <param name="functionAssemblies">The function assemblies.</param>
        public MathematicPortfolio(
            IStringFormatter stringFormatter,
            params Assembly[] functionAssemblies)
            : this(
                MathDefinition.Default,
                stringFormatter,
                functionAssemblies)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MathematicPortfolio"/> class.
        /// </summary>
        /// <param name="mathDefinition">The math definition.</param>
        /// <param name="functionAssemblies">The function assemblies.</param>
        public MathematicPortfolio(
            MathDefinition mathDefinition,
            params Assembly[] functionAssemblies)
            : this(
                mathDefinition,
                null,
                functionAssemblies)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MathematicPortfolio"/> class.
        /// </summary>
        /// <param name="mathDefinition">The math definition.</param>
        /// <param name="stringFormatter">The string formatter.</param>
        /// <param name="functionAssemblies">The function assemblies.</param>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance",
            "HAA0603:Delegate allocation from a method group",
            Justification = "It does not matter at this point.")]
        public MathematicPortfolio(
            MathDefinition mathDefinition,
            IStringFormatter stringFormatter,
            params Assembly[] functionAssemblies)
        {
            Requires.NotNull(ref this.workingDefinition, mathDefinition, nameof(mathDefinition));

            // Register function assemblies
            this.assembliesToRegister = new List<Assembly>(functionAssemblies.Length + 1)
            {
                typeof(MathematicPortfolio).Assembly
            };

            foreach (Assembly assembly in functionAssemblies)
            {
                if (assembly == null || this.assembliesToRegister.Contains(assembly))
                {
                    return;
                }

                this.assembliesToRegister.Add(assembly);
            }

            // Load functions
            this.nonaryFunctions =
                FunctionsDictionaryGenerator.GenerateInternalNonaryFunctionsDictionary(this.assembliesToRegister);
            this.unaryFunctions =
                FunctionsDictionaryGenerator.GenerateInternalUnaryFunctionsDictionary(this.assembliesToRegister);
            this.binaryFunctions =
                FunctionsDictionaryGenerator.GenerateInternalBinaryFunctionsDictionary(this.assembliesToRegister);
            this.ternaryFunctions =
                FunctionsDictionaryGenerator.GenerateInternalTernaryFunctionsDictionary(this.assembliesToRegister);

            // String formatter
            this.stringFormatters = new List<IStringFormatter>();

            if (stringFormatter != null)
            {
                this.stringFormatters.Add(stringFormatter);
            }

            // Constant pass-through extractors
            this.constantPassThroughExtractors = new LevelDictionary<Type, IConstantPassThroughExtractor>();

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
                        MathematicPortfolio thisL1) =>
                    {
                        thisL1.constantPassThroughExtractors.Add(
                            p,
                            (IConstantPassThroughExtractor)p.Instantiate(),
                            p.GetAttributeDataByTypeWithoutVersionBinding<ConstantsPassThroughExtractorAttribute, int>(
                                out var explicitLevel)
                                ? explicitLevel
                                : i++);
                    },
                    ref incrementer,
                    this);

            // Constant extractors
            this.constantExtractors = new LevelDictionary<Type, IConstantsExtractor>
            {
                { typeof(StringExtractor), new StringExtractor(), 1000 },
                { typeof(ScientificFormatNumberExtractor), new ScientificFormatNumberExtractor(), 2000 }
            };

            incrementer = 2001;
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
                        MathematicPortfolio thisL1) =>
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

            // Constant interpreters
            this.constantInterpreters = new LevelDictionary<Type, IConstantInterpreter>();

            incrementer = 2001;
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
                        MathematicPortfolio thisL1) =>
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

        /// <summary>
        /// Gets the string formatters.
        /// </summary>
        /// <value>
        /// The string formatters.
        /// </value>
        public IReadOnlyList<IStringFormatter> StringFormatters => this.stringFormatters;

        /// <summary>
        /// Gets the constant interpreters.
        /// </summary>
        /// <value>
        /// The constant interpreters.
        /// </value>
        public IDictionary<Type, IConstantInterpreter> ConstantInterpreters => this.constantInterpreters;

        /// <summary>
        ///     Returns the prototypes of all registered functions.
        /// </summary>
        /// <returns>All function names, with all possible combinations of input and output data.</returns>
        public string[] GetRegisteredFunctions()
        {
            this.ThrowIfCurrentObjectDisposed();

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
        /// Loads the expressions into context.
        /// </summary>
        /// <param name="expressions">The expressions to load.</param>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance",
            "HAA0603:Delegate allocation from a method group",
            Justification = "We're using PLINQ, this is unavoidable.")]
        public void LoadIntoContext(params string[] expressions)
        {
            this.ThrowIfCurrentObjectDisposed();

            expressions.ParallelForEach(ContextLoadingAction);

            #region Local functions
            void ContextLoadingAction(string p)
            {
                var computedExpression = this.computedExpressions.GetOrAdd(
                    p,
                    this.ValueFactory);

                if (!computedExpression.RecognizedCorrectly)
                {
                    this.compiledDelegate[new ExpressionTypedKey(p)] = CompiledExpressionResult.UncomputableResult;
                    return;
                }

                using var clonedComputedExpression = computedExpression.DeepClone();
                var (success, isConstant, @delegate, constantValue) = clonedComputedExpression.CompileDelegate(in ComparisonTolerance.Empty);
                if (success)
                {
                    if (isConstant)
                    {
                        this.compiledDelegate[new ExpressionTypedKey(p)] = new CompiledExpressionResult(constantValue);
                    }
                    else
                    {
                        this.compiledDelegate[new ExpressionTypedKey(p)] = new CompiledExpressionResult(@delegate);
                    }
                }
                else
                {
                    this.compiledDelegate[new ExpressionTypedKey(p)] = CompiledExpressionResult.UncomputableResult;
                }
            }
            #endregion
        }

        /// <summary>
        /// Gets the required parameters of an expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>An array with the required parameter names and order.</returns>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance",
            "HAA0603:Delegate allocation from a method group",
            Justification = "The methods that we're dealing with use delegate instances, and it is unavoidable.")]
        public string[] GetRequiredParameters(string expression)
        {
            Requires.NotNullOrWhiteSpace(
                expression,
                nameof(expression));

            var computedExpression = this.computedExpressions.GetOrAdd(
                expression,
                this.ValueFactory);

            return !computedExpression.RecognizedCorrectly ? null : computedExpression.GetParameterNames();
        }

        /// <summary>
        /// Solves the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="dataFinder">The data finder.</param>
        /// <returns>A computed object.</returns>
        public object Solve(
            string expression,
            IDataFinder dataFinder) =>
            this.Solve(
                expression,
                in ComparisonTolerance.Empty,
                dataFinder);

        /// <summary>
        /// Solves the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <param name="dataFinder">The data finder.</param>
        /// <returns>A computed object.</returns>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance",
            "HAA0603:Delegate allocation from a method group",
            Justification = "The methods that we're dealing with use delegate instances, and it is unavoidable.")]
        public object Solve(
            string expression,
            in ComparisonTolerance tolerance,
            IDataFinder dataFinder)
        {
            Requires.NotNullOrWhiteSpace(
                expression,
                nameof(expression));
            Requires.NotNull(
                dataFinder,
                nameof(dataFinder));

            var computedExpression = this.computedExpressions.GetOrAdd(
                expression,
                this.ValueFactory);

            if (!computedExpression.RecognizedCorrectly)
            {
                return expression;
            }

            var names = computedExpression.GetParameterNames();

            object[] parameterValues = new object[names.Length];

            for (int i = 0; i < names.Length; i++)
            {
                if (!dataFinder.TryGetData(
                    names[i],
                    out var val))
                {
                    return expression;
                }

                parameterValues[i] = val;
            }

            return this.Solve(
                expression,
                in tolerance,
                parameterValues);
        }

        /// <summary>
        /// Solves the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A computed object.</returns>
        public object Solve(
            string expression,
            params object[] parameters) =>
            this.Solve(
                expression,
                in ComparisonTolerance.Empty,
                parameters);

        /// <summary>
        /// Solves the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A computed object.</returns>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance",
            "HAA0603:Delegate allocation from a method group",
            Justification = "The methods that we're dealing with use delegate instances, and it is unavoidable.")]
        public object Solve(
            string expression,
            in ComparisonTolerance tolerance,
            params object[] parameters)
        {
            Requires.NotNullOrWhiteSpace(
                expression,
                nameof(expression));

            var pars = parameters.ComputeToStandard();
            var etk = new ExpressionTypedKey(expression, tolerance, pars.Select(p => p.GetType()).ToArray());

            var result = this.compiledDelegate.GetOrAdd(
                etk,
                ValueFactory);

            if (result.Uncomputable)
            {
                return expression;
            }

            if (result.IsConstant)
            {
                return result.ConstantValue;
            }

            try
            {
                return result.CompiledExpression.DynamicInvoke(pars);
            }
            catch (MathematicsEngineException)
            {
                throw;
            }
            catch (Exception)
            {
                return expression;
            }

            CompiledExpressionResult ValueFactory(ExpressionTypedKey key)
            {
                var computedExpression = this.computedExpressions.GetOrAdd(
                    key.Expression,
                    this.ValueFactory);

                if (!computedExpression.RecognizedCorrectly)
                {
                    return CompiledExpressionResult.UncomputableResult;
                }

                var localTolerance = key.Tolerance;

#if NET452
                if (CurrentContext?.Value != this)
                {
                    CurrentContext = new ThreadStatic<MathematicPortfolio>(this);
                }
#else
                CurrentContext.Value = this;
#endif

                using var clonedComputedExpression = computedExpression.DeepClone();
                var (success, isConstant, @delegate, constantValue) = clonedComputedExpression.CompileDelegate(
                    in localTolerance,
                    etk.ParameterTypes);

                return success
                    ? isConstant ? new CompiledExpressionResult(constantValue) : new CompiledExpressionResult(@delegate)
                    : CompiledExpressionResult.UncomputableResult;
            }
        }

        /// <summary>Disposes in the managed context.</summary>
        protected override void DisposeManagedContext()
        {
            this.assembliesToRegister.Clear();

            this.stringFormatters.Clear();

            this.nonaryFunctions.Clear();
            this.unaryFunctions.Clear();
            this.binaryFunctions.Clear();
            this.ternaryFunctions.Clear();

            this.constantPassThroughExtractors.Dispose();
            this.constantExtractors.Dispose();
            this.constantInterpreters.Dispose();

            base.DisposeManagedContext();
        }

        private ComputedExpression ValueFactory(string expression)
        {
            #if NET452
            if (CurrentContext?.Value != this)
            {
                CurrentContext = new ThreadStatic<MathematicPortfolio>(this);
            }
            #else
            CurrentContext.Value = this;
            #endif

            if (this.constantPassThroughExtractors.KeysByLevel.SelectMany(p => p.Value)
                .Any(
                    (
                        cpteKey,
                        innerExpression,
                        innerThis) => innerThis.constantPassThroughExtractors[cpteKey]
                        .Evaluate(
                            innerExpression,
                            innerThis.workingDefinition),
                    expression,
                    this))
            {
                return new ComputedExpression(
                    expression,
                    new StringNode(
                        expression),
                    true,
                    EmptyParameterRegistry,
                    this.stringFormatters);
            }

            using var workingSet = new WorkingExpressionSet(
                expression,
                this.workingDefinition.DeepClone(),
                this.nonaryFunctions,
                this.unaryFunctions,
                this.binaryFunctions,
                this.ternaryFunctions,
                this.constantExtractors);
            ComputationBody cb = workingSet.CreateBody();

            ComputedExpression result = !workingSet.Success
                ? new ComputedExpression(
                    expression,
                    null,
                    false,
                    null,
                    this.stringFormatters)
                : new ComputedExpression(
                    expression,
                    cb.BodyNode,
                    true,
                    cb.ParameterRegistry,
                    this.stringFormatters);

            Interlocked.MemoryBarrier();

            return result;
        }
    }
}