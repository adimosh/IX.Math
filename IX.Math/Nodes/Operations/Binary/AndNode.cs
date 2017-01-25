// <copyright file="AndNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Binary
{
    internal sealed class AndNode : BinaryOperationNodeBase
    {
        public AndNode(NumericNode left, NumericNode right)
            : base(left, right)
        {
        }

        public AndNode(NumericNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public AndNode(NumericParameterNode left, NumericNode right)
            : base(left, right)
        {
        }

        public AndNode(NumericParameterNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public AndNode(NumericNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public AndNode(OperationNodeBase left, NumericNode right)
            : base(left?.Simplify(), right)
        {
        }

        public AndNode(NumericParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public AndNode(OperationNodeBase left, NumericParameterNode right)
            : base(left?.Simplify(), right)
        {
        }

        public AndNode(OperationNodeBase left, OperationNodeBase right)
            : base(left?.Simplify(), right?.Simplify())
        {
        }

        public AndNode(BoolNode left, BoolNode right)
            : base(left, right)
        {
        }

        public AndNode(BoolNode left, BoolParameterNode right)
            : base(left, right)
        {
        }

        public AndNode(BoolParameterNode left, BoolNode right)
            : base(left, right)
        {
        }

        public AndNode(BoolParameterNode left, BoolParameterNode right)
            : base(left, right)
        {
        }

        public AndNode(BoolNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public AndNode(OperationNodeBase left, BoolNode right)
            : base(left?.Simplify(), right)
        {
        }

        public AndNode(BoolParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public AndNode(OperationNodeBase left, BoolParameterNode right)
            : base(left?.Simplify(), right)
        {
        }

        public override NodeBase Simplify()
        {
            if (this.Left is NumericNode && this.Right is NumericNode)
            {
                var left = (this.Left as NumericNode).ExtractInteger();
                var right = (this.Right as NumericNode).ExtractInteger();

                return new NumericNode(left & right);
            }

            if (this.Left is BoolNode && this.Right is BoolNode)
            {
                return new BoolNode(((BoolNode)this.Left).Value & ((BoolNode)this.Right).Value);
            }

            return this;
        }

        public override Expression GenerateExpression()
        {
            NodeBase simplifiedExpression = this.Simplify();

            if (simplifiedExpression != null)
            {
                return simplifiedExpression.GenerateExpression();
            }
            else
            {
                return Expression.And(this.Left.GenerateExpression(), this.Right.GenerateExpression());
            }
        }
    }
}