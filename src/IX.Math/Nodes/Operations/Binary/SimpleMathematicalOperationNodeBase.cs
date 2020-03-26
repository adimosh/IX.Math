// <copyright file="SimpleMathematicalOperationNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;

namespace IX.Math.Nodes.Operations.Binary
{
    /// <summary>
    /// A node base for simple mathematical operations.
    /// </summary>
    /// <seealso cref="IX.Math.Nodes.Operations.Binary.BinaryOperatorNodeBase" />
    internal abstract class SimpleMathematicalOperationNodeBase : BinaryOperatorNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleMathematicalOperationNodeBase"/> class.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        protected private SimpleMathematicalOperationNodeBase(NodeBase left, NodeBase right)
            : base(left, right)
        {
        }

        /// <summary>
        /// Gets the return type of this node.
        /// </summary>
        /// <value>
        /// The node return type.
        /// </value>
        public override SupportedValueType ReturnType => SupportedValueType.Numeric;

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

            if (left.ReturnType != SupportedValueType.Numeric || right.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        protected void EnsureCompatibleNumericExpressions(
            ref Expression left,
            ref Expression right)
        {
            if (left.Type == right.Type)
            {
                return;
            }

            if (left.Type == typeof(double))
            {
                right = Expression.Convert(
                    right,
                    typeof(double));
                return;
            }

            if (right.Type == typeof(double))
            {
                left = Expression.Convert(
                    left,
                    typeof(double));
                return;
            }

            if (left.Type != typeof(long))
            {
                right = Expression.Convert(
                    right,
                    typeof(long));
            }

            if (right.Type != typeof(long))
            {
                left = Expression.Convert(
                    left,
                    typeof(long));
            }
        }
    }
}