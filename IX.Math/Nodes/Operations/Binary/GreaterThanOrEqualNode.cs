﻿// <copyright file="GreaterThanOrEqualNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Binary
{
    [DebuggerDisplay("{Left} >= {Right}")]
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
            if (right?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public GreaterThanOrEqualNode(OperationNodeBase left, NumericNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public GreaterThanOrEqualNode(OperationNodeBase left, OperationNodeBase right)
            : base(left?.Simplify(), right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Numeric && left?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public GreaterThanOrEqualNode(NumericParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public GreaterThanOrEqualNode(OperationNodeBase left, NumericParameterNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public GreaterThanOrEqualNode(UndefinedParameterNode left, UndefinedParameterNode right)
            : base(left?.DetermineNumeric(), right?.DetermineNumeric())
        {
        }

        public GreaterThanOrEqualNode(UndefinedParameterNode left, NodeBase right)
            : base(left, right?.Simplify())
        {
            if (this.Right.ReturnType == SupportedValueType.Numeric)
            {
                this.Left = left.DetermineNumeric();
            }
            else
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public GreaterThanOrEqualNode(NodeBase left, UndefinedParameterNode right)
            : base(left?.Simplify(), right)
        {
            if (this.Left.ReturnType == SupportedValueType.Numeric)
            {
                this.Right = right.DetermineNumeric();
            }
            else
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public override SupportedValueType ReturnType => SupportedValueType.Numeric;

        public override NodeBase Simplify()
        {
            if (this.Left is NumericNode && this.Right is NumericNode)
            {
                System.Tuple<double, double> value = NumericNode.ExtractFloats((NumericNode)this.Left, (NumericNode)this.Right);

                return new BoolNode(value.Item1 >= value.Item2);
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal()
        {
            System.Tuple<Expression, Expression> pars = this.GetExpressionsOfSameTypeFromOperands();
            return Expression.GreaterThanOrEqual(pars.Item1, pars.Item2);
        }
    }
}