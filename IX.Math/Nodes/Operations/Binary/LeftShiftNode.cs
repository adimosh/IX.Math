// <copyright file="LeftShiftNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Binary
{
    internal sealed class LeftShiftNode : BinaryOperationNodeBase
    {
        public LeftShiftNode(NumericNode left, NumericNode right)
            : base(left, right)
        {
        }

        public LeftShiftNode(NumericNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public LeftShiftNode(NumericParameterNode left, NumericNode right)
            : base(left, right)
        {
        }

        public LeftShiftNode(NumericParameterNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public LeftShiftNode(NumericNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public LeftShiftNode(OperationNodeBase left, NumericNode right)
            : base(left?.Simplify(), right)
        {
        }

        public LeftShiftNode(NumericParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public LeftShiftNode(OperationNodeBase left, NumericParameterNode right)
            : base(left?.Simplify(), right)
        {
        }

        public LeftShiftNode(OperationNodeBase left, OperationNodeBase right)
            : base(left?.Simplify(), right?.Simplify())
        {
        }

        public override NodeBase Simplify()
        {
            if (this.Left is NumericNode && this.Right is NumericNode)
            {
                return NumericNode.LeftShift((NumericNode)this.Left, (NumericNode)this.Right);
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal()
        {
            return Expression.LeftShift(this.Left.GenerateExpression(), this.Right.GenerateExpression());
        }
    }
}