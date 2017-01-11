// <copyright file="ExpressionTreeNodeNumericConstant.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using IX.Math.SimplificationAide;

namespace IX.Math.BuiltIn.Constants
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

        internal object GetValueSpecific(int numericTypeValue)
        {
            var numericType = NumericTypeAide.InverseNumericTypesConversionDictionary[numericTypeValue];

            return Convert.ChangeType(this.Value, numericType);
        }

        protected override Expression GenerateExpressionWithOperands(ExpressionTreeNodeBase[] operandExpressions, int numericTypeValue)
        {
            var numericType = NumericTypeAide.InverseNumericTypesConversionDictionary[numericTypeValue];

            return Expression.Constant(Convert.ChangeType(this.Value, numericType), numericType);
        }
    }
}