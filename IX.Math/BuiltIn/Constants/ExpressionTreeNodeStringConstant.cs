// <copyright file="ExpressionTreeNodeStringConstant.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;

namespace IX.Math.BuiltIn.Constants
{
    internal sealed class ExpressionTreeNodeStringConstant : ExpressionTreeNodeConstant
    {
        public ExpressionTreeNodeStringConstant(string value)
            : base(WorkingConstants.DefaultNumericType, value)
        {
        }

        public override SupportedValueType ReturnType
        {
            get
            {
                return SupportedValueType.String;
            }
        }

        protected override Expression GenerateExpressionWithOperands(ExpressionTreeNodeBase[] operandExpressions, int numericTypeValue)
        {
            return Expression.Constant(this.Value, typeof(string));
        }
    }
}