// <copyright file="ExpressionSymbol.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using JetBrains.Annotations;

namespace IX.Math.ExpressionState
{
    /// <summary>
    ///     An expression symbol.
    /// </summary>
    [DebuggerDisplay("Expression: {Name} -> {Expression}")]
    [PublicAPI]
    public class ExpressionSymbol
    {
        private string expression;

        private ExpressionSymbol()
        {
        }

        /// <summary>
        ///     Gets or sets the expression.
        /// </summary>
        /// <value>The name.</value>
        public string Expression
        {
            get => this.expression;
            set => this.expression = string.IsNullOrWhiteSpace(value) ? null : value.Trim();
        }

        /// <summary>
        ///     Gets a value indicating whether this symbol represents a function call.
        /// </summary>
        /// <value><see langword="true" /> if this symbol is a function call; otherwise, <see langword="false" />.</value>
        public bool IsFunctionCall { get; private set; }

        /// <summary>
        ///     Gets or sets the name of the expression symbol.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; set; }

        internal static ExpressionSymbol GenerateSymbol(
            string name,
            string expression) => new ExpressionSymbol
            {
                Name = name,
                Expression = expression
            };

        internal static ExpressionSymbol GenerateFunctionCall(
            string name,
            string expression)
        {
            ExpressionSymbol generatedExpression = GenerateSymbol(
                name,
                expression);
            generatedExpression.IsFunctionCall = true;
            return generatedExpression;
        }
    }
}