using IX.Math.PlatformMitigation;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace IX.Math
{
    internal class SupportedFunctionsLocator
    {
        internal static Expression LoadUnaryFunction(string functionName, Expression body)
        {
            MethodInfo method;
            switch (functionName)
            {
                case "abs":
                    method = LoadMathMethod(nameof(System.Math.Abs));
                    break;
                case "acos":
                    method = LoadMathMethod(nameof(System.Math.Acos));
                    break;
                case "asin":
                    method = LoadMathMethod(nameof(System.Math.Asin));
                    break;
                case "atan":
                    method = LoadMathMethod(nameof(System.Math.Atan));
                    break;
                case "ceiling":
                    method = LoadMathMethod(nameof(System.Math.Ceiling));
                    break;
                case "cos":
                    method = LoadMathMethod(nameof(System.Math.Cos));
                    break;
                case "cosh":
                    method = LoadMathMethod(nameof(System.Math.Cosh));
                    break;
                case "exp":
                    method = LoadMathMethod(nameof(System.Math.Exp));
                    break;
                case "floor":
                    method = LoadMathMethod(nameof(System.Math.Floor));
                    break;
                case "ln":
                    method = LoadMathMethod(nameof(System.Math.Log));
                    break;
                case "lg":
                    method = LoadMathMethod(nameof(System.Math.Log10));
                    break;
                case "round":
                    method = LoadMathMethod(nameof(System.Math.Round));
                    break;
                case "sin":
                    method = LoadMathMethod(nameof(System.Math.Sin));
                    break;
                case "sinh":
                    method = LoadMathMethod(nameof(System.Math.Sinh));
                    break;
                case "sqrt":
                    method = LoadMathMethod(nameof(System.Math.Sqrt));
                    break;
                case "tan":
                    method = LoadMathMethod(nameof(System.Math.Tan));
                    break;
                case "tanh":
                    method = LoadMathMethod(nameof(System.Math.Tanh));
                    break;
                case "trun":
                    method = LoadMathMethod(nameof(System.Math.Truncate));
                    break;
                default:
                    return null;
            }

            if (method == null)
                return null;
            return Expression.Call(method, body);
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
