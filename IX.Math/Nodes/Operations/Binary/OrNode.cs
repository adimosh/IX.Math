// <copyright file="OrNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Binary
{
    [DebuggerDisplay("{Left} | {Right}")]
    internal sealed class OrNode : BinaryOperationNodeBase
    {
        public OrNode(NumericNode left, NumericNode right)
            : base(left, right)
        {
        }

        public OrNode(NumericNode left, NumericParameterNode right)
            : base(left, right?.ParameterMustBeInteger())
        {
        }

        public OrNode(NumericParameterNode left, NumericNode right)
            : base(left?.ParameterMustBeInteger(), right)
        {
        }

        public OrNode(NumericParameterNode left, NumericParameterNode right)
            : base(left?.ParameterMustBeInteger(), right?.ParameterMustBeInteger())
        {
        }

        public OrNode(NumericNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public OrNode(OperationNodeBase left, NumericNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public OrNode(NumericParameterNode left, OperationNodeBase right)
            : base(left?.ParameterMustBeInteger(), right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public OrNode(OperationNodeBase left, NumericParameterNode right)
            : base(left?.Simplify(), right?.ParameterMustBeInteger())
        {
            if (left?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public OrNode(OperationNodeBase left, OperationNodeBase right)
            : base(left?.Simplify(), right?.Simplify())
        {
            if (right?.ReturnType != left?.ReturnType)
            {
                throw new ExpressionNotValidLogicallyException();
            }

            if (right?.ReturnType != SupportedValueType.Numeric && right?.ReturnType != SupportedValueType.Boolean)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public OrNode(BoolNode left, BoolNode right)
            : base(left, right)
        {
        }

        public OrNode(BoolNode left, BoolParameterNode right)
            : base(left, right)
        {
        }

        public OrNode(BoolParameterNode left, BoolNode right)
            : base(left, right)
        {
        }

        public OrNode(BoolParameterNode left, BoolParameterNode right)
            : base(left, right)
        {
        }

        public OrNode(BoolNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Boolean)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public OrNode(OperationNodeBase left, BoolNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.Boolean)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public OrNode(BoolParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Boolean)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public OrNode(OperationNodeBase left, BoolParameterNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.Boolean)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public OrNode(UndefinedParameterNode left, UndefinedParameterNode right)
            : base(left?.DetermineBool(), right?.DetermineBool())
        {
        }

        public OrNode(UndefinedParameterNode left, NodeBase right)
            : base(left, right?.Simplify())
        {
            if (this.Right.ReturnType == SupportedValueType.Numeric)
            {
                this.Left = left.DetermineNumeric().ParameterMustBeInteger();
            }
            else
            {
                this.Left = left.DetermineBool();
            }
        }

        public OrNode(NodeBase left, UndefinedParameterNode right)
            : base(left?.Simplify(), right)
        {
            if (this.Left.ReturnType == SupportedValueType.Numeric)
            {
                this.Right = right.DetermineNumeric().ParameterMustBeInteger();
            }
            else
            {
                this.Right = right.DetermineBool();
            }
        }

        public override SupportedValueType ReturnType => this.Left?.ReturnType ?? this.Right?.ReturnType ?? SupportedValueType.Unknown;

        public override NodeBase Simplify()
        {
            if (this.Left is NumericNode && this.Right is NumericNode)
            {
                var left = (this.Left as NumericNode).ExtractInteger();
                var right = (this.Right as NumericNode).ExtractInteger();

                return new NumericNode(left | right);
            }

            if (this.Left is BoolNode && this.Right is BoolNode)
            {
                return new BoolNode(((BoolNode)this.Left).Value | ((BoolNode)this.Right).Value);
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal()
        {
            return Expression.Or(this.Left.GenerateExpression(), this.Right.GenerateExpression());
        }
    }
}