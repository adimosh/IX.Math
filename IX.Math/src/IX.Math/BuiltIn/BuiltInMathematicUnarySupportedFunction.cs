using IX.Math.PlatformMitigation;
using IX.Math.SimplificationAide;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace IX.Math.BuiltIn
{
    internal class BuiltInMathematicUnarySupportedFunction : ExpressionTreeNodeBase
    {
        private readonly string name;

        internal BuiltInMathematicUnarySupportedFunction(string name)
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
            MethodInfo mi = typeof(System.Math).GetTypeMethod(name, typeof(double), new[] { typeof(double) });
            if (mi == null)
            {
                throw new InvalidOperationException();
            }

            var operand = operandExpressions[0];

            if (operand is ExpressionTreeNodeNumericConstant)
            {
                var value = mi.Invoke(null, new[] { ((ExpressionTreeNodeNumericConstant)operand).GetValueSpecific(NumericTypeAide.NumericTypesConversionDictionary[typeof(double)]) });
                return Expression.Constant(value, typeof(double));
            }

            return Expression.Call(mi, operand.GenerateExpression());
        }
    }
}
