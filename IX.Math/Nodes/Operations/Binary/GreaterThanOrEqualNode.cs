﻿// <copyright file="GreaterThanOrEqualNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;
using IX.Math.PlatformMitigation;

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
            if (this.Right.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public GreaterThanOrEqualNode(OperationNodeBase left, NumericNode right)
            : base(left?.Simplify(), right)
        {
            if (this.Left.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public GreaterThanOrEqualNode(OperationNodeBase left, OperationNodeBase right)
            : base(left?.Simplify(), right?.Simplify())
        {
            if (this.Left.ReturnType == SupportedValueType.Numeric)
            {
                if (this.Right.ReturnType != SupportedValueType.Numeric)
                {
                    throw new ExpressionNotValidLogicallyException();
                }
            }
            else if (this.Left.ReturnType == SupportedValueType.String)
            {
                if (this.Right.ReturnType != SupportedValueType.String)
                {
                    throw new ExpressionNotValidLogicallyException();
                }
            }
            else
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public GreaterThanOrEqualNode(NumericParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (this.Right.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public GreaterThanOrEqualNode(OperationNodeBase left, NumericParameterNode right)
            : base(left?.Simplify(), right)
        {
            if (this.Left.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public GreaterThanOrEqualNode(StringNode left, StringNode right)
            : base(left, right)
        {
        }

        public GreaterThanOrEqualNode(StringParameterNode left, StringNode right)
            : base(left, right)
        {
        }

        public GreaterThanOrEqualNode(OperationNodeBase left, StringNode right)
            : base(left?.Simplify(), right)
        {
            if (this.Left.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public GreaterThanOrEqualNode(StringNode left, StringParameterNode right)
            : base(left, right)
        {
        }

        public GreaterThanOrEqualNode(StringParameterNode left, StringParameterNode right)
            : base(left, right)
        {
        }

        public GreaterThanOrEqualNode(OperationNodeBase left, StringParameterNode right)
            : base(left?.Simplify(), right)
        {
            if (this.Left.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public GreaterThanOrEqualNode(StringNode left, OperationNodeBase right)
            : base(left, right)
        {
            if (this.Right.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public GreaterThanOrEqualNode(StringParameterNode left, OperationNodeBase right)
            : base(left, right)
        {
            if (this.Right.ReturnType != SupportedValueType.String)
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
            else if (this.Right.ReturnType == SupportedValueType.String)
            {
                this.Left = left.DetermineString();
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
            else if (this.Left.ReturnType == SupportedValueType.String)
            {
                this.Right = right.DetermineString();
            }
            else
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public override SupportedValueType ReturnType => SupportedValueType.Boolean;

        public override NodeBase Simplify()
        {
            if (this.Left is NumericNode && this.Right is NumericNode)
            {
                Tuple<double, double> value = NumericNode.ExtractFloats((NumericNode)this.Left, (NumericNode)this.Right);

                return new BoolNode(value.Item1 >= value.Item2);
            }
            else if (this.Left is StringNode left && this.Right is StringNode right)
            {
                return new BoolNode(left.Value.CompareTo(right.Value) >= 0);
            }
            else
            {
                return this;
            }
        }

        protected override Expression GenerateExpressionInternal()
        {
            Tuple<Expression, Expression> pars = this.GetExpressionsOfSameTypeFromOperands();
            if (pars.Item1.Type == typeof(string))
            {
                MethodInfo mi = typeof(string).GetTypeMethod(nameof(string.Compare), typeof(string), typeof(string));
                return Expression.GreaterThanOrEqual(
                    Expression.Call(mi, this.Left.GenerateStringExpression(), this.Right.GenerateStringExpression()),
                    Expression.Constant(0, typeof(int)));
            }
            else
            {
                return Expression.GreaterThanOrEqual(pars.Item1, pars.Item2);
            }
        }
    }
}