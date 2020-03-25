// <copyright file="ExpressionParsingService.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Threading;
using JetBrains.Annotations;

namespace IX.Math
{
    /// <summary>
    ///     A service that is able to parse strings containing mathematical expressions and solve them. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="ExpressionParsingServiceBase"/>
    [PublicAPI]
    public sealed class ExpressionParsingService : ExpressionParsingServiceBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ExpressionParsingService" /> class with a standard math definition
        ///     object.
        /// </summary>
        public ExpressionParsingService()
            : base(MathDefinition.Default)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExpressionParsingService" /> class with a specified math definition
        ///     object.
        /// </summary>
        /// <param name="definition">The math definition to use.</param>
        public ExpressionParsingService(MathDefinition definition)
            : base(definition)
        {
        }

        /// <summary>
        ///     Interprets the mathematical expression and returns a container that can be invoked for solving using specific
        ///     mathematical types.
        /// </summary>
        /// <param name="expression">The expression to interpret.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A <see cref="ComputedExpression" /> that represents the interpreted expression.</returns>
        public override ComputedExpression Interpret(
            string expression,
            CancellationToken cancellationToken = default) =>
            this.InterpretInternal(expression, cancellationToken);
    }
}