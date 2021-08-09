// <copyright file="ConstantNode.Obsolete.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;

// ReSharper disable once CheckNamespace
namespace IX.Math.Nodes
{
#pragma warning disable SA1601 // Partial elements should be documented
    public partial class ConstantNode
#pragma warning restore SA1601 // Partial elements should be documented
    {
        /// <summary>
        ///     Gets a value indicating whether or not this node is actually a constant.
        /// </summary>
        /// <value><see langword="true" /> if the node is a constant, <see langword="false" /> otherwise.</value>
        [Obsolete("This is not going to be used anymore.")]
        public override bool IsConstant => true;

        /// <summary>
        ///     Gets a value indicating whether this node supports tolerance.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is tolerant; otherwise, <c>false</c>.
        /// </value>
        [Obsolete("This is not going to be used anymore.")]
        public override bool IsTolerant => false;

        /// <summary>
        ///     Gets the return type of this node.
        /// </summary>
        /// <value>The node return type.</value>
        [Obsolete("This is not to be used anymore in determining the possible uses for a node.")]
        public override SupportedValueType ReturnType => SupportedValueType.Unknown;

        /// <summary>
        ///     Generates the expression that will be compiled into code as a string expression.
        /// </summary>
        /// <returns>The generated <see cref="Expression" /> that gives the values as a string.</returns>
        [Obsolete("This is not going to be used anymore.")]
        public override Expression GenerateStringExpression() =>
            this.stringExpression ?? throw new ExpressionNotValidLogicallyException();
    }
}