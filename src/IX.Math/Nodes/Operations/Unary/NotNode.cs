// <copyright file="NotNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Operations.Unary
{
    [DebuggerDisplay("!{" + nameof(Operand) + "}")]
    internal sealed class NotNode : UnaryOperatorNodeBase
    {
        public NotNode([NotNull] NodeBase operand)
            : base(operand.Simplify())
        {
            operand.DetermineWeakly(SupportableValueType.Boolean | SupportableValueType.Numeric);

            if (operand is ParameterNode op)
            {
                // If this is or can be a number, it has to be an integer number, as we cannot binary-negate a floating point expression
                op.DetermineInteger();
            }

            EnsureCompatibleOperand(operand);
        }

        public override SupportedValueType ReturnType => this.Operand.ReturnType;

        private static void EnsureCompatibleOperand(NodeBase operand)
        {
            if (operand.ReturnType != SupportedValueType.Numeric &&
                operand.ReturnType != SupportedValueType.Boolean &&
                operand.ReturnType != SupportedValueType.Unknown)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public override NodeBase Simplify()
        {
            switch (this.Operand)
            {
                case NumericNode numericNode:
                    return new NumericNode(~numericNode.ExtractInteger());
                case BoolNode boolNode:
                    return new BoolNode(!boolNode.Value);
                default:
                    return this;
            }
        }

        /// <summary>
        /// Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) => new NotNode(this.Operand.DeepClone(context));

        /// <summary>
        /// Strongly determines the node's type, if possible.
        /// </summary>
        /// <param name="type">The type to determine to.</param>
        public override void DetermineStrongly(SupportedValueType type)
        {
            if (type != SupportedValueType.Boolean && type != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }

            this.Operand.DetermineStrongly(type);

            EnsureCompatibleOperand(this.Operand);
        }

        /// <summary>
        /// Weakly determines the node's type, if possible, and, optionally, strongly determines if there is only one possible type left.
        /// </summary>
        /// <param name="type">The type or types to determine to.</param>
        public override void DetermineWeakly(SupportableValueType type)
        {
            if ((type & SupportableValueType.Boolean) == 0 && (type & SupportableValueType.Numeric) == 0)
            {
                throw new ExpressionNotValidLogicallyException();
            }

            this.Operand.DetermineWeakly(type);

            EnsureCompatibleOperand(this.Operand);
        }

        protected override Expression GenerateExpressionInternal() => Expression.Not(this.Operand.GenerateExpression());
    }
}