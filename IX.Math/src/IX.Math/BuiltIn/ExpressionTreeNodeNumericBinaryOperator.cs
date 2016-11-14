﻿using IX.Math.PlatformMitigation;
using IX.Math.SimplificationAide;
using System;
using System.Linq.Expressions;

namespace IX.Math.BuiltIn
{
    internal sealed class ExpressionTreeNodeNumericBinaryOperator : ExpressionTreeNodeBase
    {
        private readonly ExpressionType type;

        public ExpressionTreeNodeNumericBinaryOperator(ExpressionType type)
            : base(WorkingConstants.defaultNumericType)
        {
            this.type = type;
        }

        public ExpressionTreeNodeNumericBinaryOperator()
            : base(typeof(double))
        {
            type = ExpressionType.Power;
        }

        public override SupportedValueType[] OperandTypes
        {
            get
            {
                return new SupportedValueType[2] { SupportedValueType.Numeric, SupportedValueType.Numeric };
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

                Type numericType = NumericTypeAide.InverseNumericTypesConversionDictionary[numericTypeValue];

                var mi = typeof(MathematicalBinaryOperationsAide).GetTypeMethod(Enum.GetName(typeof(ExpressionType), type), new Type[2] { numericType, numericType });

                if (mi != null)
                {
                    var result = mi.Invoke(null, new[] { leftConverted.Value, rightConverted.Value });

                    return Expression.Constant(result, numericType);
                }
            }

            return Expression.MakeBinary(type, leftExpression, rightExpression);
        }
    }
}