// <copyright file="ExpressionParsingService.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

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
    }
}