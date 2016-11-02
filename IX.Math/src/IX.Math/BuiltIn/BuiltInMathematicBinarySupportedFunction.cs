using IX.Math.PlatformMitigation;
using IX.Math.SimplificationAide;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace IX.Math.BuiltIn
{
    internal sealed class BuiltInMathematicBinarySupportedFunction : ExpressionTreeNodeBase
    {
        private readonly string name;

        internal BuiltInMathematicBinarySupportedFunction(string name)
            : base(typeof(double))
        {
            this.name = name;
        }

        public string Name
        {
            get
            {
                return name;
            }
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
                return SupportedValueType.Numeric;
            }
        }

        protected override Expression GenerateExpressionWithOperands(ExpressionTreeNodeBase[] operandExpressions, int numericTypeValue)
        {
            MethodInfo mi = typeof(System.Math).GetTypeMethod(name, typeof(double), new[] { typeof(double), typeof(double) });
            if (mi == null)
            {
                throw new InvalidOperationException();
            }

            var operand1 = operandExpressions[0];
            var operand2 = operandExpressions[1];

            if (operand1 is ExpressionTreeNodeNumericConstant && operand2 is ExpressionTreeNodeNumericConstant)
            {
                var value = mi.Invoke(null, new[]
                {
                    ((ExpressionTreeNodeNumericConstant)operand1).GetValueSpecific(NumericTypeAide.NumericTypesConversionDictionary[typeof(double)]),
                    ((ExpressionTreeNodeNumericConstant)operand2).GetValueSpecific(NumericTypeAide.NumericTypesConversionDictionary[typeof(double)])
                });
                return Expression.Constant(value, typeof(double));
            }

            return Expression.Call(mi, operand1.GenerateExpression(), operand2.GenerateExpression());
        }
    }
}