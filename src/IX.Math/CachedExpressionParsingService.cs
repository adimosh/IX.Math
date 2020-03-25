// <copyright file="CachedExpressionParsingService.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Threading;
using IX.StandardExtensions.Efficiency;
using JetBrains.Annotations;
using DiagCA = System.Diagnostics.CodeAnalysis;

namespace IX.Math
{
    /// <summary>
    ///     A service that is able to parse strings containing mathematical expressions and solve them in a cached way. This
    ///     class cannot be inherited.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This service also caches expressions so that they can be garbage-collected after a specific time period with
    ///         no use.
    ///     </para>
    /// </remarks>
    /// <seealso cref="ExpressionParsingServiceBase" />
    [PublicAPI]
    public sealed class CachedExpressionParsingService : ExpressionParsingServiceBase
    {
        private ConcurrentDictionary<string, ComputedExpression> cachedComputedExpressions;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CachedExpressionParsingService" /> class.
        /// </summary>
        public CachedExpressionParsingService()
            : base(MathDefinition.Default)
        {
            this.cachedComputedExpressions = new ConcurrentDictionary<string, ComputedExpression>();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="CachedExpressionParsingService" /> class.
        /// </summary>
        /// <param name="definition">The math definition to use.</param>
        public CachedExpressionParsingService([NotNull] MathDefinition definition)
            : base(definition)
        {
            this.cachedComputedExpressions = new ConcurrentDictionary<string, ComputedExpression>();
        }

        /// <summary>
        ///     Interprets the mathematical expression and returns a container that can be invoked for solving using specific
        ///     mathematical types.
        /// </summary>
        /// <param name="expression">The expression to interpret.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A <see cref="ComputedExpression" /> that represents the interpreted expression.</returns>
        /// <remarks>
        ///     <para>
        ///         Due to the specifics of executing multiple expressions, the returned object will always be a clone of the
        ///         originally-interpreted object, unless any of the following conditions are met:
        ///     </para>
        ///     <list type="bullet">
        ///         <item>The expression is constant</item>
        ///         <item>The expression has no parameters</item>
        ///         <item>The expression has not been recognized correctly.</item>
        ///     </list>
        ///     <para>
        ///         This way, a computed expression that has parameters which depend on outside influence will not be subject to
        ///         reinterpretation, but will execute without having to force undefined parameters into specific types.
        ///     </para>
        /// </remarks>
        public override ComputedExpression Interpret(
            string expression,
            CancellationToken cancellationToken = default)
        {
            ComputedExpression expr = this.cachedComputedExpressions.GetOrAdd(
                expression,
                (
                    ex,
                    st) => st.Item1.Interpret(
                    ex,
                    st.Item2),
                new Tuple<ExpressionParsingServiceBase, CancellationToken>(
                    this,
                    cancellationToken));

            if (!expr.RecognizedCorrectly || expr.IsConstant)
            {
                return expr;
            }

            return expr.DeepClone();
        }

        /// <summary>
        ///     Disposes in the managed context.
        /// </summary>
        protected override void DisposeManagedContext()
        {
            this.cachedComputedExpressions.Clear();

            base.DisposeManagedContext();
        }

        /// <summary>
        ///     Disposes in the general (managed and unmanaged) context.
        /// </summary>
        [DiagCA.SuppressMessageAttribute(
            "IDisposableAnalyzers.Correctness",
            "IDISP003:Dispose previous before re-assigning.",
            Justification = "It is disposed.")]
        protected override void DisposeGeneralContext()
        {
            base.DisposeGeneralContext();

            Interlocked.Exchange(
                ref this.cachedComputedExpressions,
                null);
        }
    }
}