﻿// <copyright file="GreaterThanOrEqualNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Binary
{
    internal sealed class GreaterThanOrEqualNode : BinaryOperationNodeBase
    {
        public GreaterThanOrEqualNode(NumericNode left, NumericNode right)
            : base(left, right)
        {
        }

        public GreaterThanOrEqualNode(NumericNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public GreaterThanOrEqualNode(NumericParameterNode left, NumericNode right)
            : base(left, right)
        {
        }

        public GreaterThanOrEqualNode(NumericParameterNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public GreaterThanOrEqualNode(NumericNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public GreaterThanOrEqualNode(OperationNodeBase left, NumericNode right)
            : base(left?.Simplify(), right)
        {
        }

        public GreaterThanOrEqualNode(OperationNodeBase left, OperationNodeBase right)
            : base(left?.Simplify(), right?.Simplify())
        {
        }

        public GreaterThanOrEqualNode(NumericParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public GreaterThanOrEqualNode(OperationNodeBase left, NumericParameterNode right)
            : base(left?.Simplify(), right)
        {
        }

        public override NodeBase Simplify()
        {
            if (this.Left is NumericNode && this.Right is NumericNode)
            {
                var value = NumericNode.ExtractFloats((NumericNode)this.Left, (NumericNode)this.Right);

                return new BoolNode(value.Item1 >= value.Item2);
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal()
        {
            return Expression.GreaterThanOrEqual(this.Left.GenerateExpression(), this.Right.GenerateExpression());
        }
    }
}