// <copyright file="SubtractNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Operations.Unary
{
    [DebuggerDisplay("-{" + nameof(Operand) + "}")]
    internal sealed class SubtractNode : UnaryOperatorNodeBase
    {
        public SubtractNode([NotNull] NodeBase operand)
            : base(operand.Simplify())
        {
            operand.DetermineStrongly(SupportedValueType.Numeric);

            if (operand.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public override SupportedValueType ReturnType => SupportedValueType.Numeric;

        public override NodeBase Simplify()
        {
            switch (this.Operand)
            {
                case NumericNode numericNode:
                    return NumericNode.Subtract(new NumericNode(0), numericNode);
                default:
                    return this;
            }
        }

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

        /// <summary>
        /// Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) => new SubtractNode(this.Operand.DeepClone(context));

#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation - This is desired
        protected override Expression GenerateExpressionInternal() => Expression.Subtract(Expression.Constant(0L, typeof(long)), this.Operand.GenerateExpression());
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation
    }
}