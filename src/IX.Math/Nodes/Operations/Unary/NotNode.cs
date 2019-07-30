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
            if (operand is ParameterNode op)
            {
                op.LimitPossibleType(SupportableValueType.Boolean | SupportableValueType.Numeric);
                if (op.ReturnType == SupportedValueType.Numeric || op.ReturnType == SupportedValueType.Unknown)
                {
                    // If this is or can be a number, it has to be an integer number, as we cannot binary-negate a floating point expression
                    op.DetermineInteger();
                }
                else if (operand.ReturnType != SupportedValueType.Boolean)
                {
                    throw new ExpressionNotValidLogicallyException();
                }
            }
            else
            {
                if (operand.ReturnType != SupportedValueType.Numeric &&
                    operand.ReturnType != SupportedValueType.Boolean)
                {
                    throw new ExpressionNotValidLogicallyException();
                }
            }
        }

        public override SupportedValueType ReturnType => this.Operand.ReturnType;

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

        protected override Expression GenerateExpressionInternal() => Expression.Not(this.Operand.GenerateExpression());
    }
}