﻿// <copyright file="EqualsNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Binary
{
    [DebuggerDisplay("{Left} = {Right}")]
    internal sealed class EqualsNode : BinaryOperationNodeBase
    {
        public EqualsNode(NumericNode left, NumericNode right)
            : base(left, right)
        {
        }

        public EqualsNode(NumericNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public EqualsNode(NumericParameterNode left, NumericNode right)
            : base(left, right)
        {
        }

        public EqualsNode(NumericParameterNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public EqualsNode(StringNode left, StringNode right)
            : base(left, right)
        {
        }

        public EqualsNode(StringNode left, StringParameterNode right)
            : base(left, right)
        {
        }

        public EqualsNode(StringParameterNode left, StringNode right)
            : base(left, right)
        {
        }

        public EqualsNode(StringParameterNode left, StringParameterNode right)
            : base(left, right)
        {
        }

        public EqualsNode(BoolNode left, BoolNode right)
            : base(left, right)
        {
        }

        public EqualsNode(BoolNode left, BoolParameterNode right)
            : base(left, right)
        {
        }

        public EqualsNode(BoolParameterNode left, BoolNode right)
            : base(left, right)
        {
        }

        public EqualsNode(BoolParameterNode left, BoolParameterNode right)
            : base(left, right)
        {
        }

        public EqualsNode(NumericNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public EqualsNode(OperationNodeBase left, NumericNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public EqualsNode(OperationNodeBase left, OperationNodeBase right)
            : base(left?.Simplify(), right?.Simplify())
        {
            if (right?.ReturnType != left?.ReturnType)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public EqualsNode(NumericParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public EqualsNode(OperationNodeBase left, NumericParameterNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public EqualsNode(StringNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public EqualsNode(OperationNodeBase left, StringNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public EqualsNode(StringParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public EqualsNode(OperationNodeBase left, StringParameterNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public EqualsNode(BoolNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Boolean)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public EqualsNode(OperationNodeBase left, BoolNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.Boolean)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public EqualsNode(BoolParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Boolean)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public EqualsNode(OperationNodeBase left, BoolParameterNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.Boolean)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public EqualsNode(UndefinedParameterNode left, UndefinedParameterNode right)
            : base(left, right)
        {
        }

        public EqualsNode(UndefinedParameterNode left, NodeBase right)
            : base(left, right?.Simplify())
        {
            if (this.Right.ReturnType == SupportedValueType.Numeric)
            {
                this.Left = left.DetermineNumeric();
            }
            else if (this.Right.ReturnType == SupportedValueType.Boolean)
            {
                this.Left = left.DetermineBool();
            }
            else
            {
                this.Left = left.DetermineString();
            }
        }

        public EqualsNode(NodeBase left, UndefinedParameterNode right)
            : base(left?.Simplify(), right)
        {
            if (this.Left.ReturnType == SupportedValueType.Numeric)
            {
                this.Right = right.DetermineNumeric();
            }
            else if (this.Left.ReturnType == SupportedValueType.Boolean)
            {
                this.Right = right.DetermineBool();
            }
            else
            {
                this.Right = right.DetermineString();
            }
        }

        public override SupportedValueType ReturnType => this.Left?.ReturnType ?? this.Right?.ReturnType ?? SupportedValueType.Unknown;

        public override NodeBase Simplify()
        {
            if (this.Left is NumericNode && this.Right is NumericNode)
            {
                var l = Convert.ToDouble(((NumericNode)this.Left).Value);
                var r = Convert.ToDouble(((NumericNode)this.Right).Value);
                return new BoolNode(l == r);
            }

            if (this.Left is StringNode && this.Right is StringNode)
            {
                var l = Convert.ToString(((StringNode)this.Left).Value);
                var r = ((StringNode)this.Right).Value;
                return new BoolNode(l == r);
            }

            if (this.Left is BoolNode && this.Right is BoolNode)
            {
                var l = Convert.ToBoolean(((BoolNode)this.Left).Value);
                var r = Convert.ToBoolean(((BoolNode)this.Right).Value);
                return new BoolNode(l == r);
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal()
        {
            Tuple<Expression, Expression> pars = this.GetExpressionsOfSameTypeFromOperands();
            return Expression.Equal(pars.Item1, pars.Item2);
        }
    }
}