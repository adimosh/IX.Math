// <copyright file="BinaryOperatorNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using IX.Math.Values;
using IX.StandardExtensions.Contracts;

namespace IX.Math.Nodes.Operators.Binary
{
    /// <summary>
    /// A base class for a binary operator.
    /// </summary>
    internal abstract class BinaryOperatorNodeBase : OperatorNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryOperatorNodeBase"/> class.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        protected private BinaryOperatorNodeBase(
            NodeBase leftOperand,
            NodeBase rightOperand)
        {
            this.LeftOperand = Requires.NotNull(
                    leftOperand,
                    nameof(leftOperand))
                .Simplify();
            this.RightOperand = Requires.NotNull(
                    rightOperand,
                    nameof(rightOperand))
                .Simplify();
        }

        /// <summary>
        /// Gets the left operand for this unary operator.
        /// </summary>
        private protected NodeBase LeftOperand { get; }

        /// <summary>
        /// Gets the right operand for this unary operator.
        /// </summary>
        private protected NodeBase RightOperand { get; }

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>A simplified node, or this instance.</returns>
        public sealed override NodeBase Simplify()
        {
            if (this.LeftOperand is not ConstantNode leftConstant ||
                this.RightOperand is not ConstantNode rightConstant)
            {
                return this;
            }

            return this.SimplifyOnConvertibleValue(
                leftConstant.Value,
                rightConstant.Value);
        }

        /// <summary>
        ///     Simplifies this node, if possible, based on a constant operand value.
        /// </summary>
        /// <returns>A simplified node, or this instance.</returns>
        private protected abstract NodeBase SimplifyOnConvertibleValue(
            ConvertibleValue leftValue,
            ConvertibleValue rightValue);
    }
}