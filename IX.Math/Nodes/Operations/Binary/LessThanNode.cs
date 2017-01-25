// <copyright file="LessThanNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Binary
{
    internal sealed class LessThanNode : BinaryOperationNodeBase
    {
        public LessThanNode(NumericNode left, NumericNode right)
            : base(left, right)
        {
        }

        public LessThanNode(NumericNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public LessThanNode(NumericParameterNode left, NumericNode right)
            : base(left, right)
        {
        }

        public LessThanNode(NumericParameterNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public LessThanNode(NumericNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public LessThanNode(OperationNodeBase left, NumericNode right)
            : base(left?.Simplify(), right)
        {
        }

        public LessThanNode(OperationNodeBase left, OperationNodeBase right)
            : base(left?.Simplify(), right?.Simplify())
        {
        }

        public LessThanNode(NumericParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public LessThanNode(OperationNodeBase left, NumericParameterNode right)
            : base(left?.Simplify(), right)
        {
        }

        public override NodeBase Simplify()
        {
            if (this.Left is NumericNode && this.Right is NumericNode)
            {
                var value = NumericNode.ExtractFloats((NumericNode)this.Left, (NumericNode)this.Right);

                return new BoolNode(value.Item1 < value.Item2);
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal()
        {
            return Expression.LessThan(this.Left.GenerateExpression(), this.Right.GenerateExpression());
        }
    }
}