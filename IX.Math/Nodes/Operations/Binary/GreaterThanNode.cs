// <copyright file="GreaterThanNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Binary
{
    [DebuggerDisplay("{Left} > {Right}")]
    internal sealed class GreaterThanNode : BinaryOperationNodeBase
    {
        public GreaterThanNode(NumericNode left, NumericNode right)
            : base(left, right)
        {
        }

        public GreaterThanNode(NumericNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public GreaterThanNode(NumericParameterNode left, NumericNode right)
            : base(left, right)
        {
        }

        public GreaterThanNode(NumericParameterNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public GreaterThanNode(NumericNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public GreaterThanNode(OperationNodeBase left, NumericNode right)
            : base(left?.Simplify(), right)
        {
        }

        public GreaterThanNode(OperationNodeBase left, OperationNodeBase right)
            : base(left?.Simplify(), right?.Simplify())
        {
        }

        public GreaterThanNode(NumericParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public GreaterThanNode(OperationNodeBase left, NumericParameterNode right)
            : base(left?.Simplify(), right)
        {
        }

        public GreaterThanNode(NumericNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineNumeric())
        {
        }

        public GreaterThanNode(UndefinedParameterNode left, NumericNode right)
            : base(left?.DetermineNumeric(), right)
        {
        }

        public GreaterThanNode(NumericParameterNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineNumeric())
        {
        }

        public GreaterThanNode(UndefinedParameterNode left, NumericParameterNode right)
            : base(left?.DetermineNumeric(), right)
        {
        }

        public override NodeBase Simplify()
        {
            if (this.Left is NumericNode && this.Right is NumericNode)
            {
                var value = NumericNode.ExtractFloats((NumericNode)this.Left, (NumericNode)this.Right);

                return new BoolNode(value.Item1 > value.Item2);
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal()
        {
            return Expression.GreaterThan(this.Left.GenerateExpression(), this.Right.GenerateExpression());
        }
    }
}