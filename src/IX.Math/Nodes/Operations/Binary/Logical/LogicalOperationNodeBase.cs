// <copyright file="LogicalOperationNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

namespace IX.Math.Nodes.Operations.Binary.Logical
{
    /// <summary>
    ///     A node base for logical operations.
    /// </summary>
    /// <seealso cref="IX.Math.Nodes.Operations.Binary.BinaryOperatorNodeBase" />
    internal abstract class LogicalOperationNodeBase : BinaryOperatorNodeBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicalOperationNodeBase" /> class.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        protected private LogicalOperationNodeBase(
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
        /// <value>
        ///     The node return type.
        /// </value>
        public override SupportedValueType ReturnType => this.Left.ReturnType;

        /// <summary>
        ///     Determines the children.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="other">The other.</param>
        /// <exception cref="ExpressionNotValidLogicallyException">Undeterminable children.</exception>
        private static void DetermineChildren(
            NodeBase parameter,
            NodeBase other)
        {
            switch (other.ReturnType)
            {
                case SupportedValueType.Boolean:
                    parameter.DetermineStrongly(SupportedValueType.Boolean);
                    break;
                case SupportedValueType.Numeric:
                    parameter.DetermineStrongly(SupportedValueType.Numeric);
                    break;
                case SupportedValueType.Unknown:
                    break;
                default:
                    throw new Exceptions.ExpressionNotValidLogicallyException();
            }
        }

        /// <summary>
        ///     Strongly determines the node's type, if possible.
        /// </summary>
        /// <param name="type">The type to determine to.</param>
        public override void DetermineStrongly(SupportedValueType type)
        {
            if (type == SupportedValueType.Boolean || type == SupportedValueType.Numeric)
            {
                this.Left.DetermineStrongly(type);
                this.Right.DetermineStrongly(type);

                this.EnsureCompatibleOperands(
                    this.Left,
                    this.Right);
            }
            else
            {
                throw new Exceptions.ExpressionNotValidLogicallyException();
            }
        }

        /// <summary>
        ///     Weakly determines the node's type, if possible, and, optionally, strongly determines if there is only one possible
        ///     type left.
        /// </summary>
        /// <param name="type">The type or types to determine to.</param>
        public override void DetermineWeakly(SupportableValueType type)
        {
            if ((type & SupportableValueType.Numeric) == 0 && (type & SupportableValueType.Boolean) == 0)
            {
                throw new Exceptions.ExpressionNotValidLogicallyException();
            }

            this.Left.DetermineWeakly(type);
            this.Right.DetermineWeakly(type);

            this.EnsureCompatibleOperands(
                this.Left,
                this.Right);
        }

        protected override void EnsureCompatibleOperands(
            NodeBase left,
            NodeBase right)
        {
            left.DetermineWeakly(SupportableValueType.Boolean | SupportableValueType.Numeric);
            right.DetermineWeakly(SupportableValueType.Boolean | SupportableValueType.Numeric);

            DetermineChildren(
                left,
                right);
            DetermineChildren(
                right,
                left);
            DetermineChildren(
                left,
                right);
            DetermineChildren(
                right,
                left);
        }
    }
}