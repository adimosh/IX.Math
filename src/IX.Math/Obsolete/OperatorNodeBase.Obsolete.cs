// <copyright file="OperatorNodeBase.Obsolete.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;

// ReSharper disable once CheckNamespace
namespace IX.Math.Nodes.Operators
{
#pragma warning disable SA1601 // Partial elements should be documented
    internal partial class OperatorNodeBase
#pragma warning restore SA1601 // Partial elements should be documented
    {
        /// <summary>
        ///     Gets the return type of this node.
        /// </summary>
        /// <value>The node return type.</value>
        [Obsolete("This is not to be used anymore in determining the possible uses for a node.")]
        public SupportedValueType ReturnType => SupportedValueType.Unknown;

        /// <summary>
        ///     Generates the expression that will be compiled into code as a string expression.
        /// </summary>
        /// <returns>The generated <see cref="Expression" /> that gives the values as a string.</returns>
        [Obsolete("This is not going to be used anymore.")]
        public Expression GenerateStringExpression() => this.GenerateExpression();
    }
}