// <copyright file="RightShiftNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Binary
{
    internal sealed class RightShiftNode : BinaryOperationNodeBase
    {
        public RightShiftNode(NumericNode left, NumericNode right)
            : base(left, right)
        {
        }

        public RightShiftNode(NumericNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public RightShiftNode(NumericParameterNode left, NumericNode right)
            : base(left, right)
        {
        }

        public RightShiftNode(NumericParameterNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public RightShiftNode(NumericNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public RightShiftNode(OperationNodeBase left, NumericNode right)
            : base(left?.Simplify(), right)
        {
        }

        public RightShiftNode(NumericParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public RightShiftNode(OperationNodeBase left, NumericParameterNode right)
            : base(left?.Simplify(), right)
        {
        }

        public RightShiftNode(OperationNodeBase left, OperationNodeBase right)
            : base(left?.Simplify(), right?.Simplify())
        {
        }

        public override NodeBase Simplify()
        {
            if (this.Right is NumericNode && this.Right is NumericNode)
            {
                return NumericNode.RightShift((NumericNode)this.Right, (NumericNode)this.Right);
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal()
        {
            return Expression.RightShift(this.Right.GenerateExpression(), this.Right.GenerateExpression());
        }
    }
}