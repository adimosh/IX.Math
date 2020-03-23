// <copyright file="CachedExpressionNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;
using JetBrains.Annotations;

namespace IX.Math.Nodes
{
    /// <summary>
    ///     A node base that caches its generated expression.
    /// </summary>
    /// <seealso cref="NodeBase" />
    [PublicAPI]
    public abstract class CachedExpressionNodeBase : NodeBase
    {
        private Expression generatedExpression;
        private Expression generatedStringExpression;

        /// <summary>
        ///     Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>The generated <see cref="Expression" />.</returns>
        public sealed override Expression GenerateExpression() =>
            this.generatedExpression ??= this.GenerateCachedExpression();

        /// <summary>
        ///     Generates the expression that will be compiled into code.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>
        ///     The generated <see cref="Expression" />.
        /// </returns>
        public sealed override Expression GenerateExpression(Tolerance tolerance) =>
            this.generatedExpression ??= this.GenerateCachedExpression(tolerance);

        /// <summary>
        ///     Generates the expression that will be compiled into code as a string expression.
        /// </summary>
        /// <returns>The generated <see cref="Expression" /> that gives the values as a string.</returns>
        public sealed override Expression GenerateStringExpression() =>
            this.generatedStringExpression ??= this.GenerateCachedStringExpression();

        /// <summary>
        ///     Generates the expression that will be compiled into code as a string expression.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The generated <see cref="Expression" /> that gives the values as a string.</returns>
        public override Expression GenerateStringExpression(Tolerance tolerance) =>
            this.generatedStringExpression ??= this.GenerateCachedStringExpression(tolerance);

        /// <summary>
        ///     Generates an expression that will be cached before being compiled.
        /// </summary>
        /// <returns>The generated <see cref="Expression" /> to be cached.</returns>
        [NotNull]
        public abstract Expression GenerateCachedExpression();

        /// <summary>
        ///     Generates an expression with tolerance that will be cached before being compiled.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>
        ///     The generated <see cref="Expression" /> to be cached.
        /// </returns>
        [NotNull]
        public virtual Expression GenerateCachedExpression(Tolerance tolerance) => this.GenerateCachedExpression();

        /// <summary>
        ///     Generates a string expression that will be cached before being compiled.
        /// </summary>
        /// <returns>The generated <see cref="Expression" /> to be cached.</returns>
        [NotNull]
        public abstract Expression GenerateCachedStringExpression();

        /// <summary>
        ///     Generates a string expression that will be cached before being compiled.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The generated <see cref="Expression" /> to be cached.</returns>
        [NotNull]
        public virtual Expression GenerateCachedStringExpression(Tolerance tolerance) =>
            this.GenerateCachedStringExpression();
    }
}