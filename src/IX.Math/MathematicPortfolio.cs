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
        private readonly ExpressionParsingService ceps;

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
            this.ceps = mathDefinition == null
                ? new ExpressionParsingService()
                : new ExpressionParsingService(mathDefinition);

            functionAssemblies.ForEach(this.ceps.RegisterFunctionsAssembly);

            if (stringFormatter != null)
            {
                this.ceps.RegisterTypeFormatter(stringFormatter);
            }
        }

        /// <summary>
        /// Loads the expressions into context.
        /// </summary>
        /// <param name="expressions">The expressions to load.</param>
        public void LoadIntoContext(IEnumerable<string> expressions)
        {
            Requires.NotNull(
                expressions,
                nameof(expressions));

            expressions.ParallelForEach(p =>
            {
                var computedExpression = this.computedExpressions.GetOrAdd(
                    p,
                    (key) => this.ceps.Interpret(key));

                if (!computedExpression.RecognizedCorrectly)
                {
                    this.compiledDelegate[new ExpressionTypedKey(p)] = CompiledExpressionResult.UncomputableResult;
                    return;
                }

                if (!computedExpression.IsConstant)
                {
                    return;
                }

                this.compiledDelegate[new ExpressionTypedKey(p)] = new CompiledExpressionResult(computedExpression.Compute());
            });
        }

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
                return result.CompiledExpression.DynamicInvoke(parameters);
            }
            catch
            {
                return expression;
            }

            CompiledExpressionResult ValueFactory(ExpressionTypedKey key)
            {
                var compiledExpression = this.computedExpressions.GetOrAdd(
                    key.Expression,
                    this.InnerValueFactory);

                if (!compiledExpression.RecognizedCorrectly)
                {
                    return CompiledExpressionResult.UncomputableResult;
                }

                var tolerance = key.Tolerance;
                var (success, isConstant, @delegate, constantValue) = compiledExpression.CompileDelegate(
                    in tolerance,
                    etk.ParameterTypes);

                return success
                    ? isConstant ? new CompiledExpressionResult(constantValue) : new CompiledExpressionResult(@delegate)
                    : CompiledExpressionResult.UncomputableResult;
            }
        }

        private ComputedExpression InnerValueFactory(string key) => this.ceps.Interpret(key);
    }
}