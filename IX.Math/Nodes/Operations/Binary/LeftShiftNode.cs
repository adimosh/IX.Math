// <copyright file="LeftShiftNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Binary
{
    [DebuggerDisplay("{Left} << {Right}")]
    internal sealed class LeftShiftNode : BinaryOperationNodeBase
    {
        public LeftShiftNode(NumericNode left, NumericNode right)
            : base(left, right)
        {
        }

        public LeftShiftNode(NumericNode left, NumericParameterNode right)
            : base(left, right)
        {
            OperationsHelper.ParameterMustBeInteger(right);
        }

        public LeftShiftNode(NumericParameterNode left, NumericNode right)
            : base(left, right)
        {
        }

        public LeftShiftNode(NumericParameterNode left, NumericParameterNode right)
            : base(left, right)
        {
            OperationsHelper.ParameterMustBeInteger(right);
        }

        public LeftShiftNode(NumericNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public LeftShiftNode(OperationNodeBase left, NumericNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public LeftShiftNode(NumericParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public LeftShiftNode(OperationNodeBase left, NumericParameterNode right)
            : base(left?.Simplify(), right)
        {
            OperationsHelper.ParameterMustBeInteger(right);
            if (left?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public LeftShiftNode(OperationNodeBase left, OperationNodeBase right)
            : base(left?.Simplify(), right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Numeric && left?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public LeftShiftNode(NumericNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineNumeric())
        {
            OperationsHelper.ParameterMustBeInteger(this.Right as NumericParameterNode);
        }

        public LeftShiftNode(UndefinedParameterNode left, NumericNode right)
            : base(left?.DetermineNumeric(), right)
        {
        }

        public LeftShiftNode(NumericParameterNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineNumeric())
        {
            OperationsHelper.ParameterMustBeInteger(this.Right as NumericParameterNode);
        }

        public LeftShiftNode(UndefinedParameterNode left, NumericParameterNode right)
            : base(left?.DetermineNumeric(), right)
        {
            OperationsHelper.ParameterMustBeInteger(right);
        }

        public override SupportedValueType ReturnType => SupportedValueType.Numeric;

        public override NodeBase Simplify()
        {
            if (this.Left is NumericNode && this.Right is NumericNode)
            {
                return NumericNode.LeftShift((NumericNode)this.Left, (NumericNode)this.Right);
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal()
        {
            return Expression.LeftShift(this.Left.GenerateExpression(), this.Right.GenerateExpression());
        }
    }
}