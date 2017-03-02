// <copyright file="AndNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Binary
{
    [DebuggerDisplay("{Left} & {Right}")]
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
            if (right?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public AndNode(OperationNodeBase left, NumericNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public AndNode(NumericParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public AndNode(OperationNodeBase left, NumericParameterNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public AndNode(OperationNodeBase left, OperationNodeBase right)
            : base(left?.Simplify(), right?.Simplify())
        {
            if (left?.ReturnType != SupportedValueType.Numeric && right?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
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
            if (right?.ReturnType != SupportedValueType.Boolean)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public AndNode(OperationNodeBase left, BoolNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.Boolean)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public AndNode(BoolParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Boolean)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public AndNode(OperationNodeBase left, BoolParameterNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.Boolean)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public AndNode(NumericNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineNumeric())
        {
        }

        public AndNode(UndefinedParameterNode left, NumericNode right)
            : base(left?.DetermineNumeric(), right)
        {
        }

        public AndNode(BoolNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineBool())
        {
        }

        public AndNode(UndefinedParameterNode left, BoolNode right)
            : base(left?.DetermineBool(), right)
        {
        }

        public AndNode(NumericParameterNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineNumeric())
        {
        }

        public AndNode(UndefinedParameterNode left, NumericParameterNode right)
            : base(left?.DetermineNumeric(), right)
        {
        }

        public AndNode(BoolParameterNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineBool())
        {
        }

        public AndNode(UndefinedParameterNode left, BoolParameterNode right)
            : base(left?.DetermineBool(), right)
        {
        }

        public override SupportedValueType ReturnType
        {
            get
            {
                if (this.Left.ReturnType == SupportedValueType.String || this.Right.ReturnType == SupportedValueType.String)
                {
                    return SupportedValueType.String;
                }

                return SupportedValueType.Numeric;
            }
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

        protected override Expression GenerateExpressionInternal()
        {
            return Expression.And(this.Left.GenerateExpression(), this.Right.GenerateExpression());
        }
    }
}