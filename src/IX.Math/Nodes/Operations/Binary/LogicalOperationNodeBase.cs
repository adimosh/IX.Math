// <copyright file="LogicalOperationNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

namespace IX.Math.Nodes.Operations.Binary
{
    internal abstract class LogicalOperationNodeBase : BinaryOperatorNodeBase
    {
        protected LogicalOperationNodeBase(NodeBase left, NodeBase right)
            : base(left, right)
        {
        }

        public override SupportedValueType ReturnType => this.Left.ReturnType;

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
                    throw new ExpressionNotValidLogicallyException();
            }
        }

        /// <summary>
        /// Strongly determines the node's type, if possible.
        /// </summary>
        /// <param name="type">The type to determine to.</param>
        public override void DetermineStrongly(SupportedValueType type)
        {
            if (type == SupportedValueType.Boolean || type == SupportedValueType.Numeric)
            {
                this.Left.DetermineStrongly(type);
                this.Right.DetermineStrongly(type);

                this.EnsureCompatibleOperands(this.Left, this.Right);
            }
            else
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
            if ((type & SupportableValueType.Numeric) == 0 && (type & SupportableValueType.Boolean) == 0)
            {
                throw new ExpressionNotValidLogicallyException();
            }

            this.Left.DetermineWeakly(type);
            this.Right.DetermineWeakly(type);

            this.EnsureCompatibleOperands(this.Left, this.Right);
        }

        protected override void EnsureCompatibleOperands(NodeBase left, NodeBase right)
        {
            left.DetermineWeakly(SupportableValueType.Boolean | SupportableValueType.Numeric);
            right.DetermineWeakly(SupportableValueType.Boolean | SupportableValueType.Numeric);

            DetermineChildren(left, right);
            DetermineChildren(right, left);
            DetermineChildren(left, right);
            DetermineChildren(right, left);
        }
    }
}