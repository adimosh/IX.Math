using IX.Math.PlatformMitigation;
using IX.Math.SimplificationAide;
using System;
using System.Linq.Expressions;
using System.Reflection;

namespace IX.Math.BuiltIn
{
    internal class ExpressionTreeNodeMathematicUnarySupportedFunction : ExpressionTreeNodeBase
    {
        private static readonly Type mathUnaryFunctionType = typeof(double);

        private readonly string name;

        internal ExpressionTreeNodeMathematicUnarySupportedFunction(string name)
            : base(mathUnaryFunctionType)
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
                return new SupportedValueType[1] { SupportedValueType.Numeric };
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
            MethodInfo mi = typeof(System.Math).GetTypeMethod(name, mathUnaryFunctionType, new[] { mathUnaryFunctionType });
            if (mi == null)
            {
                throw new InvalidOperationException();
            }

            var operand = operandExpressions[0];
            var operandExpression = operand.GenerateExpression(NumericTypeAide.NumericTypesConversionDictionary[mathUnaryFunctionType]);

            if (operandExpression is ConstantExpression)
            {
                var value = mi.Invoke(null, new[] { ((ConstantExpression)operandExpression).Value });
                return Expression.Constant(value, mathUnaryFunctionType);
            }

            return Expression.Call(mi, operandExpression);
        }
    }
}