// <copyright file="ExpressionTreeNodeBooleanConstant.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;

namespace IX.Math.BuiltIn.Constants
{
    internal sealed class ExpressionTreeNodeBooleanConstant : ExpressionTreeNodeConstant
    {
        public ExpressionTreeNodeBooleanConstant(Type minimalRequiredNumericType, object value)
            : base(minimalRequiredNumericType, value)
        {
        }

        public override SupportedValueType ReturnType
        {
            get
            {
                return SupportedValueType.Boolean;
            }
        }

        protected override Expression GenerateExpressionWithOperands(ExpressionTreeNodeBase[] operandExpressions, int numericTypeValue)
        {
            return Expression.Constant(this.Value, typeof(bool));
        }
    }
}