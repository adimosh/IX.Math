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
        internal ExpressionSymbol(string name, string? expression, bool isFunctionCall)
        {
            this.Name = name;
            this.Expression = string.IsNullOrWhiteSpace(expression) ? null : expression?.Trim();
        }

        /// <summary>
        ///     Gets the expression.
        /// </summary>
        /// <value>The name.</value>
        public string? Expression { get; }

        /// <summary>
        ///     Gets a value indicating whether this symbol represents a function call.
        /// </summary>
        /// <value><see langword="true" /> if this symbol is a function call; otherwise, <see langword="false" />.</value>
        public bool IsFunctionCall { get; }

        /// <summary>
        ///     Gets the name of the expression symbol.
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; }

        internal static ExpressionSymbol GenerateSymbol(
            string name,
            string expression) =>
            new(
                name,
                expression,
                false);

        internal static ExpressionSymbol GenerateFunctionCall(
            string name,
            string expression) =>
            new(
                name,
                expression,
                true);
    }
}