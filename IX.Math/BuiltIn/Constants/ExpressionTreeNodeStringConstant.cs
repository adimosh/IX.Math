using System.Linq.Expressions;

namespace IX.Math.BuiltIn.Constants
{
    internal sealed class ExpressionTreeNodeStringConstant : ExpressionTreeNodeConstant
    {
        public ExpressionTreeNodeStringConstant(string value)
            : base(WorkingConstants.defaultNumericType, value)
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
            return Expression.Constant(Value, typeof(string));
        }
    }
}