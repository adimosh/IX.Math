// <copyright file="ExpressionTreeNodeBooleanBinaryOperator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using IX.Math.PlatformMitigation;
using IX.Math.SimplificationAide;

namespace IX.Math.BuiltIn
{
    internal sealed class ExpressionTreeNodeBooleanBinaryOperator : ExpressionTreeNodeBase
    {
        private readonly ExpressionType type;

        public ExpressionTreeNodeBooleanBinaryOperator(ExpressionType type)
            : base(WorkingConstants.DefaultNumericType)
        {
            this.type = type;
        }

        public override SupportedValueType[] OperandTypes
        {
            get
            {
                return new SupportedValueType[2] { SupportedValueType.Boolean, SupportedValueType.Boolean };
            }
        }

        public override SupportedValueType ReturnType
        {
            get
            {
                return SupportedValueType.Numeric;
            }
        }

        protected override Expression GenerateExpressionWithOperands(ExpressionTreeNodeBase[] operandExpressions, int numericTypeValue)
        {
            var left = operandExpressions[0];
            var right = operandExpressions[1];

            var leftExpression = left.GenerateExpression(numericTypeValue);
            var rightExpression = right.GenerateExpression(numericTypeValue);

            if (leftExpression is ConstantExpression && rightExpression is ConstantExpression)
            {
                var leftConverted = (ConstantExpression)leftExpression;
                var rightConverted = (ConstantExpression)rightExpression;

                var mi = typeof(MathematicalBinaryOperationsAide).GetTypeMethod(Enum.GetName(typeof(ExpressionType), this.type), new Type[2] { typeof(bool), typeof(bool) });

                var result = mi.Invoke(null, new[] { leftConverted.Value, rightConverted.Value });

                return Expression.Constant(result, typeof(bool));
            }

            return Expression.MakeBinary(this.type, leftExpression, rightExpression);
        }
    }
}