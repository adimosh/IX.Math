using IX.Math.PlatformMitigation;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace IX.Math.SupportedFunctions
{
    internal class BuiltInMathematicUnarySupportedFunction : SupportedFunction
    {
        private readonly string name;

        internal BuiltInMathematicUnarySupportedFunction(string name)
        {
            this.name = name;
        }

        public override Type ActualMinimalNumericTypeRequired
        {
            get
            {
                return typeof(double);
            }
        }

        public override string Name
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

        protected override Expression GenerateExpressionWithOperands(Expression[] operandExpressions)
        {
            if (operandExpressions == null)
            {
                throw new ArgumentNullException(nameof(operandExpressions));
            }

            if (operandExpressions.Length != 1)
            {
                throw new ArgumentException(Resources.OperandMismatchInFunctionCall, nameof(operandExpressions));
            }

            MethodInfo mi = LoadMathMethod(name);
            if (mi == null)
            {
                throw new InvalidOperationException();
            }

            return Expression.Call(mi, operandExpressions[0]);
        }

        private static MethodInfo LoadMathMethod(string methodName)
        {
            return typeof(System.Math).GetTypeMethods().SingleOrDefault(p =>
            {
                if (p.Name != methodName)
                    return false;

                var pars = p.GetParameters();

                if (pars == null || pars.Length != 1)
                    return false;

                return p.ReturnType == typeof(double) && pars[0].ParameterType == typeof(double);
            });
        }
    }
}
