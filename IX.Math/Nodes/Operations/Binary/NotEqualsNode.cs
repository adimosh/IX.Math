// <copyright file="NotEqualsNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;
using IX.Math.PlatformMitigation;
using IX.StandardExtensions;

namespace IX.Math.Nodes.Operations.Binary
{
    [DebuggerDisplay("{Left} != {Right}")]
    internal sealed class NotEqualsNode : BinaryOperationNodeBase
    {
        public NotEqualsNode(NumericNode left, NumericNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(NumericNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(NumericParameterNode left, NumericNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(NumericParameterNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(StringNode left, StringNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(StringNode left, StringParameterNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(StringParameterNode left, StringNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(StringParameterNode left, StringParameterNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(BoolNode left, BoolNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(BoolNode left, BoolParameterNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(BoolParameterNode left, BoolNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(BoolParameterNode left, BoolParameterNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(ByteArrayNode left, ByteArrayNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(ByteArrayNode left, ByteArrayParameterNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(ByteArrayParameterNode left, ByteArrayNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(ByteArrayParameterNode left, ByteArrayParameterNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(NumericNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public NotEqualsNode(OperationNodeBase left, NumericNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public NotEqualsNode(OperationNodeBase left, OperationNodeBase right)
            : base(left?.Simplify(), right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Numeric && left?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public NotEqualsNode(NumericParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public NotEqualsNode(OperationNodeBase left, NumericParameterNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public NotEqualsNode(StringNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public NotEqualsNode(OperationNodeBase left, StringNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public NotEqualsNode(StringParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public NotEqualsNode(OperationNodeBase left, StringParameterNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public NotEqualsNode(BoolNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Boolean)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public NotEqualsNode(OperationNodeBase left, BoolNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.Boolean)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public NotEqualsNode(BoolParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Boolean)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public NotEqualsNode(OperationNodeBase left, BoolParameterNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.Boolean)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public NotEqualsNode(ByteArrayNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.ByteArray)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public NotEqualsNode(OperationNodeBase left, ByteArrayNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.ByteArray)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public NotEqualsNode(ByteArrayParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.ByteArray)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public NotEqualsNode(OperationNodeBase left, ByteArrayParameterNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.ByteArray)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public NotEqualsNode(UndefinedParameterNode left, UndefinedParameterNode right)
            : base(left, right)
        {
        }

        public NotEqualsNode(UndefinedParameterNode left, NodeBase right)
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
            else if (this.Right.ReturnType == SupportedValueType.ByteArray)
            {
                this.Left = left.DetermineByteArray();
            }
            else
            {
                this.Left = left.DetermineString();
            }
        }

        public NotEqualsNode(NodeBase left, UndefinedParameterNode right)
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
            else if (this.Left.ReturnType == SupportedValueType.ByteArray)
            {
                this.Right = right.DetermineByteArray();
            }
            else
            {
                this.Right = right.DetermineString();
            }
        }

        public override SupportedValueType ReturnType => SupportedValueType.Boolean;

        public override NodeBase Simplify()
        {
            if (this.Left is NumericNode && this.Right is NumericNode)
            {
                var l = Convert.ToDouble(((NumericNode)this.Left).Value);
                var r = Convert.ToDouble(((NumericNode)this.Right).Value);
                return new BoolNode(l != r);
            }

            if (this.Left is StringNode && this.Right is StringNode)
            {
                var l = Convert.ToString(((StringNode)this.Left).Value);
                var r = ((StringNode)this.Right).Value;
                return new BoolNode(l != r);
            }

            if (this.Left is BoolNode && this.Right is BoolNode)
            {
                var l = ((BoolNode)this.Left).Value;
                var r = ((BoolNode)this.Right).Value;
                return new BoolNode(l != r);
            }

            if (this.Left is ByteArrayNode && this.Right is ByteArrayNode)
            {
                byte[] l = ((ByteArrayNode)this.Left).Value;
                byte[] r = ((ByteArrayNode)this.Right).Value;
                return new BoolNode(!l.SequenceEqualsWithMsb(r));
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal()
        {
            Tuple<Expression, Expression> pars = this.GetExpressionsOfSameTypeFromOperands();

            if (this.Left.ReturnType == SupportedValueType.ByteArray || this.Right.ReturnType == SupportedValueType.ByteArray)
            {
                return Expression.Equal(
                    Expression.Call(
                        typeof(ArraySequenceEqualsWithMsbExtensions).GetTypeMethod(nameof(ArraySequenceEqualsWithMsbExtensions.SequenceEqualsWithMsb), typeof(byte[]), typeof(byte[])),
                        pars.Item1,
                        pars.Item2),
                    Expression.Constant(false, typeof(bool)));
            }
            else
            {
                return Expression.NotEqual(pars.Item1, pars.Item2);
            }
        }
    }
}