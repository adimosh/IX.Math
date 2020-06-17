// <copyright file="MathematicPortfolio.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IX.Math.Extensibility;
using IX.Math.Extraction;
using IX.StandardExtensions.ComponentModel;
using IX.StandardExtensions.Contracts;
using IX.StandardExtensions.Efficiency;
using IX.StandardExtensions.Extensions;
using JetBrains.Annotations;

namespace IX.Math
{
    /// <summary>
    /// A holder for a portfolio of computable and computed mathematics expressions.
    /// </summary>
    /// <seealso cref="IX.StandardExtensions.ComponentModel.DisposableBase" />
    [PublicAPI]
    public class MathematicPortfolio : DisposableBase
    {
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP006:Implement IDisposable.",
            Justification = "It is correctly implemented, but the analyzer can't tell.")]
        private readonly ExpressionParsingService parsingService;

        private readonly ConcurrentDictionary<ExpressionTypedKey, CompiledExpressionResult> compiledDelegate =
            new ConcurrentDictionary<ExpressionTypedKey, CompiledExpressionResult>();

        private readonly ConcurrentDictionary<string, ComputedExpression> computedExpressions =
            new ConcurrentDictionary<string, ComputedExpression>();

        /// <summary>
        /// Initializes a new instance of the <see cref="MathematicPortfolio"/> class.
        /// </summary>
        /// <param name="functionAssemblies">The function assemblies.</param>
        public MathematicPortfolio(params Assembly[] functionAssemblies)
            : this(
                null,
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
                null,
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
            this.parsingService = mathDefinition == null
                ? new ExpressionParsingService()
                : new ExpressionParsingService(mathDefinition);

            functionAssemblies.ForEach(this.parsingService.RegisterFunctionsAssembly);

            if (stringFormatter != null)
            {
                this.parsingService.RegisterTypeFormatter(stringFormatter);
            }
        }

        /// <summary>
        /// Loads the expressions into context.
        /// </summary>
        /// <param name="expressions">The expressions to load.</param>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance",
            "HAA0603:Delegate allocation from a method group",
            Justification = "We're using PLINQ, this is unavoidable.")]
        public void LoadIntoContext(IEnumerable<string> expressions)
        {
            Requires.NotNull(
                expressions,
                nameof(expressions));

            expressions.ParallelForEach(ContextLoadingAction);

            #region Local functions
            void ContextLoadingAction(string p)
            {
                var computedExpression = this.computedExpressions.GetOrAdd(
                    p,
                    ValueFactory);

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

                ComputedExpression ValueFactory(string key) => this.parsingService.Interpret(key);
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
                this.InnerValueFactory);

            if (!computedExpression.RecognizedCorrectly)
            {
                return null;
            }

            return computedExpression.GetParameterNames();
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
                this.InnerValueFactory);

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
            ExpressionTypedKey etk = new ExpressionTypedKey(expression, tolerance, pars.Select(p => p.GetType()).ToArray());

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
            catch
            {
                return expression;
            }

            CompiledExpressionResult ValueFactory(ExpressionTypedKey key)
            {
                var computedExpression = this.computedExpressions.GetOrAdd(
                    key.Expression,
                    this.InnerValueFactory);

                if (!computedExpression.RecognizedCorrectly)
                {
                    return CompiledExpressionResult.UncomputableResult;
                }

                var localTolerance = key.Tolerance;

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
            this.parsingService.Dispose();
            base.DisposeManagedContext();
        }

        private ComputedExpression InnerValueFactory(string key) => this.parsingService.Interpret(key);
    }
}