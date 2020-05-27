// <copyright file="ComparisonNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

namespace IX.Math.Nodes.Operations.Binary.Comparison
{
    /// <summary>
    ///     A base node for comparison operations.
    /// </summary>
    /// <seealso cref="BinaryOperatorNodeBase" />
    internal abstract class ComparisonNodeBase : BinaryOperatorNodeBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ComparisonNodeBase" /> class.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        protected private ComparisonNodeBase(
            NodeBase left,
            NodeBase right)
            : base(
                left,
                right)
        {
        }

        /// <summary>
        ///     Gets the return type of this node.
        /// </summary>
        /// <value>The node return type.</value>
        public sealed override SupportedValueType ReturnType => SupportedValueType.Boolean;

        /// <summary>
        ///     Gets a value indicating whether this node supports tolerance.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is tolerant; otherwise, <c>false</c>.
        /// </value>
        public sealed override bool IsTolerant => true;

        /// <summary>
        ///     Strongly determines the node's type, if possible.
        /// </summary>
        /// <param name="type">The type to determine to.</param>
        public sealed override void DetermineStrongly(SupportedValueType type)
        {
            if (type != SupportedValueType.Boolean)
            {
                throw new Exceptions.ExpressionNotValidLogicallyException();
            }
        }

        /// <summary>
        ///     Weakly determines the node's type, if possible, and, optionally, strongly determines if there is only one possible
        ///     type left.
        /// </summary>
        /// <param name="type">The type or types to determine to.</param>
        public sealed override void DetermineWeakly(SupportableValueType type)
        {
            if ((type & SupportableValueType.Boolean) == 0)
            {
                throw new Exceptions.ExpressionNotValidLogicallyException();
            }
        }
    }
}