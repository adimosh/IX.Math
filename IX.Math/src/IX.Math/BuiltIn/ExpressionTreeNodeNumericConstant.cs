﻿using IX.Math.SimplificationAide;
using System;
using System.Linq.Expressions;

namespace IX.Math.BuiltIn
{
    internal abstract class ExpressionTreeNodeNumericConstant : ExpressionTreeNodeConstant
    {
        internal ExpressionTreeNodeNumericConstant(Type minimalRequiredNumericType, object value)
            : base(minimalRequiredNumericType, value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
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
            var numericType = NumericTypeAide.InverseNumericTypesConversionDictionary[numericTypeValue];

            return Expression.Constant(Value, numericType);
        }

        internal object GetValueSpecific(int numericTypeValue)
        {
            var numericType = NumericTypeAide.InverseNumericTypesConversionDictionary[numericTypeValue];

            return Convert.ChangeType(Value, numericType);
        }
    }
}