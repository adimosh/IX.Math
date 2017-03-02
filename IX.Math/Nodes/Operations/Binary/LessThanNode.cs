﻿// <copyright file="LessThanNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Binary
{
    [DebuggerDisplay("{Left} < {Right}")]
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
            if (right?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public LessThanNode(OperationNodeBase left, NumericNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public LessThanNode(OperationNodeBase left, OperationNodeBase right)
            : base(left?.Simplify(), right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Numeric && left?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public LessThanNode(NumericParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public LessThanNode(OperationNodeBase left, NumericParameterNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public LessThanNode(NumericNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineNumeric())
        {
        }

        public LessThanNode(UndefinedParameterNode left, NumericNode right)
            : base(left?.DetermineNumeric(), right)
        {
        }

        public LessThanNode(NumericParameterNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineNumeric())
        {
        }

        public LessThanNode(UndefinedParameterNode left, NumericParameterNode right)
            : base(left?.DetermineNumeric(), right)
        {
        }

        public override SupportedValueType ReturnType => SupportedValueType.Numeric;

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
            var pars = this.GetExpressionsOfSameTypeFromOperands();
            return Expression.LessThan(pars.Item1, pars.Item2);
        }
    }
}