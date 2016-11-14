﻿using IX.Math.PlatformMitigation;
using IX.Math.SimplificationAide;
using System;
using System.Linq.Expressions;

namespace IX.Math.BuiltIn
{
    internal sealed class ExpressionTreeNodeNumericUnaryOperator : ExpressionTreeNodeBase
    {
        private readonly ExpressionType type;

        public ExpressionTreeNodeNumericUnaryOperator(ExpressionType type)
            : base(WorkingConstants.defaultNumericType)
        {
            this.type = type;
        }

        public override SupportedValueType[] OperandTypes
        {
            get
            {
                return new SupportedValueType[1] { SupportedValueType.Numeric };
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
            var operand = operandExpressions[0];
            var operandExpression = operand.GenerateExpression(numericTypeValue);

            if (operandExpression is ConstantExpression)
            {
                var convertedOperand = (ConstantExpression)operandExpression;

                var numericType = NumericTypeAide.InverseNumericTypesConversionDictionary[numericTypeValue];

                var mi = typeof(MathematicalUnaryOperationsAide).GetTypeMethod(Enum.GetName(typeof(ExpressionType), type), new Type[1] { numericType });

                if (mi != null)
                {
                    var result = mi.Invoke(null, new[] { convertedOperand.Value });

                    return Expression.Constant(result, numericType);
                }
            }

            return Expression.MakeUnary(type, operandExpression, null);
        }
    }
}