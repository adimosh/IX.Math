// <copyright file="SubtractNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Binary
{
    internal sealed class SubtractNode : BinaryOperationNodeBase
    {
        public SubtractNode(NumericNode left, NumericNode right)
            : base(left, right)
        {
        }

        public SubtractNode(NumericNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public SubtractNode(NumericParameterNode left, NumericNode right)
            : base(left, right)
        {
        }

        public SubtractNode(NumericParameterNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public SubtractNode(NumericNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public SubtractNode(OperationNodeBase left, NumericNode right)
            : base(left?.Simplify(), right)
        {
        }

        public SubtractNode(NumericParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public SubtractNode(OperationNodeBase left, NumericParameterNode right)
            : base(left?.Simplify(), right)
        {
        }

        public SubtractNode(OperationNodeBase left, OperationNodeBase right)
            : base(left?.Simplify(), right?.Simplify())
        {
        }

        public override NodeBase Simplify()
        {
            if (this.Left is NumericNode && this.Right is NumericNode)
            {
                return NumericNode.Subtract((NumericNode)this.Left, (NumericNode)this.Right);
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal()
        {
            return Expression.Subtract(this.Left.GenerateExpression(), this.Right.GenerateExpression());
        }
    }
}