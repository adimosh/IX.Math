// <copyright file="NotEqualsNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

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

        public NotEqualsNode(NumericNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public NotEqualsNode(OperationNodeBase left, NumericNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public NotEqualsNode(OperationNodeBase left, OperationNodeBase right)
            : base(left?.Simplify(), right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Numeric && left?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public NotEqualsNode(NumericParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public NotEqualsNode(OperationNodeBase left, NumericParameterNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public NotEqualsNode(StringNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public NotEqualsNode(OperationNodeBase left, StringNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public NotEqualsNode(StringParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public NotEqualsNode(OperationNodeBase left, StringParameterNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public NotEqualsNode(BoolNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Boolean)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public NotEqualsNode(OperationNodeBase left, BoolNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.Boolean)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public NotEqualsNode(BoolParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Boolean)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public NotEqualsNode(OperationNodeBase left, BoolParameterNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.Boolean)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public NotEqualsNode(NumericNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineNumeric())
        {
        }

        public NotEqualsNode(UndefinedParameterNode left, NumericNode right)
            : base(left?.DetermineNumeric(), right)
        {
        }

        public NotEqualsNode(NumericParameterNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineNumeric())
        {
        }

        public NotEqualsNode(UndefinedParameterNode left, NumericParameterNode right)
            : base(left?.DetermineNumeric(), right)
        {
        }

        public NotEqualsNode(BoolNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineBool())
        {
        }

        public NotEqualsNode(UndefinedParameterNode left, BoolNode right)
            : base(left?.DetermineBool(), right)
        {
        }

        public NotEqualsNode(BoolParameterNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineBool())
        {
        }

        public NotEqualsNode(UndefinedParameterNode left, BoolParameterNode right)
            : base(left?.DetermineBool(), right)
        {
        }

        public NotEqualsNode(StringNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineString())
        {
        }

        public NotEqualsNode(UndefinedParameterNode left, StringNode right)
            : base(left?.DetermineString(), right)
        {
        }

        public NotEqualsNode(StringParameterNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineString())
        {
        }

        public NotEqualsNode(UndefinedParameterNode left, StringParameterNode right)
            : base(left?.DetermineString(), right)
        {
        }

        public override SupportedValueType ReturnType => this.Left?.ReturnType ?? this.Right?.ReturnType ?? SupportedValueType.Unknown;

        public override NodeBase Simplify()
        {
            if (this.Left is NumericNode && this.Right is NumericNode)
            {
                return new BoolNode(((NumericNode)this.Left).Value != ((NumericNode)this.Right).Value);
            }

            if (this.Left is StringNode && this.Right is StringNode)
            {
                return new BoolNode(((StringNode)this.Left).Value != ((StringNode)this.Right).Value);
            }

            if (this.Left is BoolNode && this.Right is BoolNode)
            {
                return new BoolNode(((BoolNode)this.Left).Value != ((BoolNode)this.Right).Value);
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal()
        {
            var pars = this.GetExpressionsOfSameTypeFromOperands();
            return Expression.NotEqual(pars.Item1, pars.Item2);
        }
    }
}