using IX.Math.PlatformMitigation;
using IX.Math.SimplificationAide;
using System;
using System.Linq.Expressions;

namespace IX.Math.BuiltIn
{
    internal sealed class ExpressionTreeNodeNumericLogicalBinaryOperator : ExpressionTreeNodeBase
    {
        private readonly ExpressionType type;

        public ExpressionTreeNodeNumericLogicalBinaryOperator(ExpressionType type)
            : base(WorkingConstants.defaultNumericType)
        {
            this.type = type;
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
                return SupportedValueType.Boolean;
            }
        }

        protected override Expression GenerateExpressionWithOperands(ExpressionTreeNodeBase[] operandExpressions, int numericTypeValue)
        {
            var left = operandExpressions[0];
            var right = operandExpressions[1];

            if (left is ExpressionTreeNodeNumericConstant && right is ExpressionTreeNodeNumericConstant)
            {
                var leftConverted = (ExpressionTreeNodeNumericConstant)left;
                var rightConverted = (ExpressionTreeNodeNumericConstant)right;

                Type numericType = NumericTypeAide.InverseNumericTypesConversionDictionary[numericTypeValue];

                var mi = typeof(MathematicalBinaryOperationsAide).GetTypeMethod(Enum.GetName(typeof(ExpressionType), type), new Type[2] { numericType, numericType });

                if (mi != null)
                {
                    var result = mi.Invoke(null, new[] { leftConverted.GetValueSpecific(numericTypeValue), rightConverted.GetValueSpecific(numericTypeValue) });

                    return Expression.Constant(result, typeof(bool));
                }
            }

            return Expression.MakeBinary(type, left.GenerateExpression(numericTypeValue), right.GenerateExpression(numericTypeValue));
        }
    }
}