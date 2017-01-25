// <copyright file="LessThanOrEqualNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Binary
{
    [DebuggerDisplay("{Left} <= {Right}")]
    internal sealed class LessThanOrEqualNode : BinaryOperationNodeBase
    {
        public LessThanOrEqualNode(NumericNode left, NumericNode right)
            : base(left, right)
        {
        }

        public LessThanOrEqualNode(NumericNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public LessThanOrEqualNode(NumericParameterNode left, NumericNode right)
            : base(left, right)
        {
        }

        public LessThanOrEqualNode(NumericParameterNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public LessThanOrEqualNode(NumericNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public LessThanOrEqualNode(OperationNodeBase left, NumericNode right)
            : base(left?.Simplify(), right)
        {
        }

        public LessThanOrEqualNode(OperationNodeBase left, OperationNodeBase right)
            : base(left?.Simplify(), right?.Simplify())
        {
        }

        public LessThanOrEqualNode(NumericParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public LessThanOrEqualNode(OperationNodeBase left, NumericParameterNode right)
            : base(left?.Simplify(), right)
        {
        }

        public override NodeBase Simplify()
        {
            if (this.Left is NumericNode && this.Right is NumericNode)
            {
                var value = NumericNode.ExtractFloats((NumericNode)this.Left, (NumericNode)this.Right);

                return new BoolNode(value.Item1 <= value.Item2);
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal()
        {
            return Expression.LessThanOrEqual(this.Left.GenerateExpression(), this.Right.GenerateExpression());
        }
    }
}