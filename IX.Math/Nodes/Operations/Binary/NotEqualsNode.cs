// <copyright file="NotEqualsNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Binary
{
    internal sealed class NotEqualsNode : BinaryOperationNodeBase
    {
        public NotEqualsNode(NumericNode left, NumericNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(NumericNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(NumericParameterNode left, NumericNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(NumericParameterNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(StringNode left, StringNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(StringNode left, StringParameterNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(StringParameterNode left, StringNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(StringParameterNode left, StringParameterNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(BoolNode left, BoolNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(BoolNode left, BoolParameterNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(BoolParameterNode left, BoolNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(BoolParameterNode left, BoolParameterNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(NumericNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public NotEqualsNode(OperationNodeBase left, NumericNode right)
            : base(left?.Simplify(), right)
        {
        }

        public NotEqualsNode(OperationNodeBase left, OperationNodeBase right)
            : base(left?.Simplify(), right?.Simplify())
        {
        }

        public NotEqualsNode(NumericParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public NotEqualsNode(OperationNodeBase left, NumericParameterNode right)
            : base(left?.Simplify(), right)
        {
        }

        public NotEqualsNode(StringNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public NotEqualsNode(OperationNodeBase left, StringNode right)
            : base(left?.Simplify(), right)
        {
        }

        public NotEqualsNode(StringParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public NotEqualsNode(OperationNodeBase left, StringParameterNode right)
            : base(left?.Simplify(), right)
        {
        }

        public NotEqualsNode(BoolNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public NotEqualsNode(OperationNodeBase left, BoolNode right)
            : base(left?.Simplify(), right)
        {
        }

        public NotEqualsNode(BoolParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public NotEqualsNode(OperationNodeBase left, BoolParameterNode right)
            : base(left?.Simplify(), right)
        {
        }

        public override NodeBase Simplify()
        {
            if (this.Left is NumericNode && this.Right is NumericNode)
            {
                return new BoolNode(((NumericNode)this.Left).Value != ((NumericNode)this.Right).Value);
            }

            if (this.Left is StringNode && this.Right is StringNode)
            {
                return new BoolNode(((StringNode)this.Left).Value != ((StringNode)this.Right).Value);
            }

            if (this.Left is BoolNode && this.Right is BoolNode)
            {
                return new BoolNode(((BoolNode)this.Left).Value != ((BoolNode)this.Right).Value);
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal()
        {
            return Expression.NotEqual(this.Left.GenerateExpression(), this.Right.GenerateExpression());
        }
    }
}