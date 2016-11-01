using IX.Math.PlatformMitigation;
using IX.Math.SimplificationAide;
using System;
using System.Linq.Expressions;

namespace IX.Math.BuiltIn
{
    internal sealed class ExpressionTreeNodeBooleanBinaryOperator : ExpressionTreeNodeBase
    {
        private readonly ExpressionType type;

        public ExpressionTreeNodeBooleanBinaryOperator(ExpressionType type)
            : base(WorkingConstants.defaultNumericType)
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

            if (left is ExpressionTreeNodeBooleanConstant && right is ExpressionTreeNodeBooleanConstant)
            {
                var leftConverted = (ExpressionTreeNodeBooleanConstant)left;
                var rightConverted = (ExpressionTreeNodeBooleanConstant)right;

                var mi = typeof(MathematicalBinaryOperationsAide).GetTypeMethod(Enum.GetName(typeof(ExpressionType), type), new Type[2] { typeof(bool), typeof(bool) });

                var result = mi.Invoke(null, new[] { leftConverted.Value, rightConverted.Value });

                return Expression.Constant(result, typeof(bool));
            }

            return Expression.MakeBinary(type, left.GenerateExpression(numericTypeValue), right.GenerateExpression(numericTypeValue));
        }
    }
}