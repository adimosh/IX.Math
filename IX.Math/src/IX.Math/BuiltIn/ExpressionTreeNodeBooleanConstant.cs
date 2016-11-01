using System;
using System.Linq.Expressions;

namespace IX.Math.BuiltIn
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
            return Expression.Constant(Value, typeof(bool));
        }
    }
}