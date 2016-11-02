using IX.Math.SimplificationAide;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace IX.Math.BuiltIn
{
    internal sealed class ExpressionTreeNodeStringPropertySupportedFunction : ExpressionTreeNodeBase
    {
        public ExpressionTreeNodeStringPropertySupportedFunction(string name)
            : base(WorkingConstants.defaultNumericType)
        {
            Name = name;
        }

        public string Name { get; private set; }

        public override SupportedValueType[] OperandTypes
        {
            get
            {
                return new SupportedValueType[1] { SupportedValueType.String };
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
            var pi = typeof(string).GetTypeInfo().DeclaredProperties.SingleOrDefault(p => p.Name == Name);

            ExpressionTreeNodeBase op = operandExpressions[0];
            var opExpression = op.GenerateExpression(numericTypeValue);

            if (opExpression is ConstantExpression)
            {
                Type numType = NumericTypeAide.InverseNumericTypesConversionDictionary[numericTypeValue];
                return Expression.Constant(Convert.ChangeType(pi.GetValue(((ConstantExpression)opExpression).Value), numType), numType);
            }

            return Expression.Property(opExpression, pi);
        }
    }
}
