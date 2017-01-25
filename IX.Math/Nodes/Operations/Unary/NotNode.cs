// <copyright file="NotNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Unary
{
    internal sealed class NotNode : UnaryOperatorNodeBase
    {
        public NotNode(NumericNode operand)
            : base(operand)
        {
        }

        public NotNode(BoolNode operand)
            : base(operand)
        {
        }

        public NotNode(NumericParameterNode operand)
            : base(operand)
        {
            OperationsHelper.ParameterMustBeInteger(operand);
        }

        public NotNode(BoolParameterNode operand)
            : base(operand)
        {
        }

        public NotNode(OperationNodeBase operand)
            : base(operand)
        {
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

        protected override Expression GenerateExpressionInternal()
        {
            return Expression.Not(this.Operand.GenerateExpression());
        }
    }
}