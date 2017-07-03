﻿// <copyright file="LessThanOrEqualNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;
using IX.Math.PlatformMitigation;
using IX.StandardExtensions;

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
            if (this.Right.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public LessThanOrEqualNode(OperationNodeBase left, NumericNode right)
            : base(left?.Simplify(), right)
        {
            if (this.Left.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public LessThanOrEqualNode(OperationNodeBase left, OperationNodeBase right)
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

        public LessThanOrEqualNode(NumericParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (this.Right.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public LessThanOrEqualNode(OperationNodeBase left, NumericParameterNode right)
            : base(left?.Simplify(), right)
        {
            if (this.Left.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public LessThanOrEqualNode(StringNode left, StringNode right)
            : base(left, right)
        {
        }

        public LessThanOrEqualNode(StringParameterNode left, StringNode right)
            : base(left, right)
        {
        }

        public LessThanOrEqualNode(OperationNodeBase left, StringNode right)
            : base(left?.Simplify(), right)
        {
            if (this.Left.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public LessThanOrEqualNode(StringNode left, StringParameterNode right)
            : base(left, right)
        {
        }

        public LessThanOrEqualNode(StringParameterNode left, StringParameterNode right)
            : base(left, right)
        {
        }

        public LessThanOrEqualNode(OperationNodeBase left, StringParameterNode right)
            : base(left?.Simplify(), right)
        {
            if (this.Left.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public LessThanOrEqualNode(StringNode left, OperationNodeBase right)
            : base(left, right)
        {
            if (this.Right.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public LessThanOrEqualNode(StringParameterNode left, OperationNodeBase right)
            : base(left, right)
        {
            if (this.Right.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public LessThanOrEqualNode(ByteArrayNode left, ByteArrayNode right)
            : base(left, right)
        {
        }

        public LessThanOrEqualNode(ByteArrayNode left, ByteArrayParameterNode right)
            : base(left, right)
        {
        }

        public LessThanOrEqualNode(ByteArrayParameterNode left, ByteArrayNode right)
            : base(left, right)
        {
        }

        public LessThanOrEqualNode(ByteArrayParameterNode left, ByteArrayParameterNode right)
            : base(left, right)
        {
        }

        public LessThanOrEqualNode(ByteArrayNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.ByteArray)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public LessThanOrEqualNode(OperationNodeBase left, ByteArrayNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.ByteArray)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public LessThanOrEqualNode(ByteArrayParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.ByteArray)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public LessThanOrEqualNode(OperationNodeBase left, ByteArrayParameterNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.ByteArray)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public LessThanOrEqualNode(UndefinedParameterNode left, UndefinedParameterNode right)
            : base(left?.DetermineNumeric(), right?.DetermineNumeric())
        {
        }

        public LessThanOrEqualNode(UndefinedParameterNode left, NodeBase right)
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
            else if (this.Right.ReturnType == SupportedValueType.ByteArray)
            {
                this.Left = left.DetermineByteArray();
            }
            else
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public LessThanOrEqualNode(NodeBase left, UndefinedParameterNode right)
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
            else if (this.Left.ReturnType == SupportedValueType.ByteArray)
            {
                this.Right = right.DetermineByteArray();
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

                return new BoolNode(value.Item1 <= value.Item2);
            }
            else if (this.Left is StringNode left && this.Right is StringNode right)
            {
                return new BoolNode(left.Value.CompareTo(right.Value) <= 0);
            }
            else if (this.Left is ByteArrayNode && this.Right is ByteArrayNode)
            {
                byte[] l = ((ByteArrayNode)this.Left).Value;
                byte[] r = ((ByteArrayNode)this.Right).Value;
                return new BoolNode(l.SequenceCompareWithMsb(r) <= 0);
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
                return Expression.LessThanOrEqual(
                    Expression.Call(mi, this.Left.GenerateStringExpression(), this.Right.GenerateStringExpression()),
                    Expression.Constant(0, typeof(int)));
            }
            else if (this.Left.ReturnType == SupportedValueType.ByteArray || this.Right.ReturnType == SupportedValueType.ByteArray)
            {
                return Expression.LessThanOrEqual(
                    Expression.Call(
                        typeof(ArraySequenceCompareWithMsbExtensions).GetTypeMethod(nameof(ArraySequenceCompareWithMsbExtensions.SequenceCompareWithMsb), typeof(byte[]), typeof(byte[])),
                        pars.Item1,
                        pars.Item2),
                    Expression.Constant(0, typeof(int)));
            }
            else
            {
                return Expression.LessThanOrEqual(pars.Item1, pars.Item2);
            }
        }
    }
}