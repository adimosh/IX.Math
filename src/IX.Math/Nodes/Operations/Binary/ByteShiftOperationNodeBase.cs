// <copyright file="ByteShiftOperationNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

namespace IX.Math.Nodes.Operations.Binary
{
    /// <summary>
    /// A node base for byte shift operations.
    /// </summary>
    /// <seealso cref="IX.Math.Nodes.Operations.Binary.BinaryOperatorNodeBase" />
    internal abstract class ByteShiftOperationNodeBase : BinaryOperatorNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ByteShiftOperationNodeBase"/> class.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        protected private ByteShiftOperationNodeBase(NodeBase left, NodeBase right)
            : base(left, right)
        {
        }

        /// <summary>
        /// Gets the return type of this node.
        /// </summary>
        /// <value>
        /// The node return type.
        /// </value>
        public override SupportedValueType ReturnType => this.Left.ReturnType;

        /// <summary>
        /// Strongly determines the node's type, if possible.
        /// </summary>
        /// <param name="type">The type to determine to.</param>
        public override void DetermineStrongly(SupportedValueType type)
        {
            if (type != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        /// <summary>
        /// Weakly determines the node's type, if possible, and, optionally, strongly determines if there is only one possible type left.
        /// </summary>
        /// <param name="type">The type or types to determine to.</param>
        public override void DetermineWeakly(SupportableValueType type)
        {
            if ((type & SupportableValueType.Numeric) == 0)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        protected override void EnsureCompatibleOperands(NodeBase left, NodeBase right)
        {
            left.DetermineStrongly(SupportedValueType.Numeric);
            right.DetermineStrongly(SupportedValueType.Numeric);

            if (left is ParameterNode uLeft)
            {
                uLeft.DetermineInteger();
            }

            if (right is ParameterNode uRight)
            {
                uRight.DetermineInteger();
            }

            if (left.ReturnType != SupportedValueType.Numeric || right.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }
    }
}