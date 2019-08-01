// <copyright file="ComparisonOperationNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

namespace IX.Math.Nodes.Operations.Binary
{
    internal abstract class ComparisonOperationNodeBase : BinaryOperationNodeBase
    {
        protected ComparisonOperationNodeBase(
            NodeBase left,
            NodeBase right)
            : base(
                left,
                right)
        {
        }

        public override SupportedValueType ReturnType => SupportedValueType.Boolean;

        private static void DetermineChildren(
            NodeBase parameter,
            NodeBase other)
        {
            if (other.ReturnType == SupportedValueType.Unknown)
            {
                return;
            }

            parameter.DetermineStrongly(other.ReturnType);
        }

        /// <summary>
        ///     Strongly determines the node's type, if possible.
        /// </summary>
        /// <param name="type">The type to determine to.</param>
        public override void DetermineStrongly(SupportedValueType type)
        {
            this.Left.DetermineStrongly(type);
            this.Right.DetermineStrongly(type);

            this.EnsureCompatibleOperands(
                this.Left,
                this.Right);
        }

        /// <summary>
        ///     Weakly determines the node's type, if possible, and, optionally, strongly determines if there is only one possible
        ///     type left.
        /// </summary>
        /// <param name="type">The type or types to determine to.</param>
        public override void DetermineWeakly(SupportableValueType type)
        {
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

            if (left.ReturnType != right.ReturnType)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }
    }
}