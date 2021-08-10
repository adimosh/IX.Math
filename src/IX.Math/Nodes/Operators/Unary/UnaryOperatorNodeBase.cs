// <copyright file="UnaryOperatorNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using IX.Math.Values;
using IX.StandardExtensions.Contracts;

namespace IX.Math.Nodes.Operators.Unary
{
    /// <summary>
    /// A base class for an unary operator.
    /// </summary>
    internal abstract class UnaryOperatorNodeBase : OperatorNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnaryOperatorNodeBase"/> class.
        /// </summary>
        /// <param name="operand">The operand.</param>
        protected private UnaryOperatorNodeBase(NodeBase operand)
        {
            this.Operand = Requires.NotNull(
                    operand,
                    nameof(operand))
                .Simplify();
        }

        /// <summary>
        /// Gets the operand for this unary operator.
        /// </summary>
        private protected NodeBase Operand { get; }

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>A simplified node, or this instance.</returns>
        public sealed override NodeBase Simplify() =>
            this.Operand is not ConstantNode constant ? this : this.SimplifyOnConvertibleValue(constant.Value);

        /// <summary>
        ///     Simplifies this node, if possible, based on a constant operand value.
        /// </summary>
        /// <returns>A simplified node, or this instance.</returns>
        private protected abstract NodeBase SimplifyOnConvertibleValue(ConvertibleValue value);
    }
}