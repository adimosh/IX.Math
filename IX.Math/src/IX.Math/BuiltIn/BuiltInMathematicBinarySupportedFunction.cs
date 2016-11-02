using IX.Math.PlatformMitigation;
using IX.Math.SimplificationAide;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace IX.Math.BuiltIn
{
    internal sealed class BuiltInMathematicBinarySupportedFunction : ExpressionTreeNodeBase
    {
        private static readonly Type mathBinaryFunctionType = typeof(double);

        private readonly string name;

        internal BuiltInMathematicBinarySupportedFunction(string name)
            : base(mathBinaryFunctionType)
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
            MethodInfo mi = typeof(System.Math).GetTypeMethod(name, mathBinaryFunctionType, new[] { mathBinaryFunctionType, mathBinaryFunctionType });
            if (mi == null)
            {
                throw new InvalidOperationException();
            }

            var operand1 = operandExpressions[0];
            var operandExpression1 = operand1.GenerateExpression(NumericTypeAide.NumericTypesConversionDictionary[mathBinaryFunctionType]);
            var operand2 = operandExpressions[1];
            var operandExpression2 = operand2.GenerateExpression(NumericTypeAide.NumericTypesConversionDictionary[mathBinaryFunctionType]);

            if (operandExpression1 is ConstantExpression && operandExpression2 is ConstantExpression)
            {
                var value = mi.Invoke(null, new[]
                {
                    ((ConstantExpression)operandExpression1).Value,
                    ((ConstantExpression)operandExpression2).Value,
                });
                return Expression.Constant(value, mathBinaryFunctionType);
            }

            return Expression.Call(mi, operandExpression1, operandExpression2);
        }
    }
}