#if !NETSTANDARD10 && !NETSTANDARD11
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace IX.Math
{
    internal static class ExpressionReductionHelperService
    {
        internal static Expression ReduceIfConstantOperation(this BinaryExpression operationExpression)
        {
            if (!(operationExpression.Left is ConstantExpression) || !(operationExpression.Right is ConstantExpression))
                return operationExpression;

            var leftConstant = (ConstantExpression)operationExpression.Left;
            var rightConstant = (ConstantExpression)operationExpression.Right;

            if (leftConstant.Type != rightConstant.Type)
                return operationExpression;

            object result;

            MethodInfo mi = GetProperOperator(leftConstant.Type, operationExpression.NodeType);
            if (mi != null)
            {
                if (mi.IsStatic)
                {
                    result = mi.Invoke(null, new[] { leftConstant.Value, rightConstant.Value });
                }
                else
                {
                    result = mi.Invoke(leftConstant.Value, new[] { rightConstant.Value });
                }
                return Expression.Constant(result, result.GetType());
            }
            else
            {
                return operationExpression;
            }
        }

        private static MethodInfo GetProperOperator(Type type, ExpressionType eType)
        {
            MethodInfo mi;
            Type[] typeParams = new Type[] { type, type };

#if !NETSTANDARD12
            mi = typeof(ValueGetters).GetMethods(BindingFlags.Static | BindingFlags.Public)
                .SingleOrDefault(p => p.Name == Enum.GetName(typeof(ExpressionType), eType) && p.GetParameters().Select(q => q.ParameterType).SequenceEqual(typeParams));
#else
            mi = type.GetRuntimeMethods()
                .SingleOrDefault(p => p.IsStatic && p.Name == Enum.GetName(typeof(ExpressionType), eType) && p.GetParameters().Select(q => q.ParameterType).SequenceEqual(typeParams));
#endif

            if (mi == null)
            {
                string opName;
                switch (eType)
                {
                    case ExpressionType.Add:
                        opName = "op_Addition";
                        break;
                    case ExpressionType.Subtract:
                        opName = "op_Subtraction";
                        break;
                    case ExpressionType.Multiply:
                        opName = "op_Multiply";
                        break;
                    case ExpressionType.Divide:
                        opName = "op_Division";
                        break;
                    case ExpressionType.Equal:
                        opName = "op_Equality";
                        break;
                    case ExpressionType.NotEqual:
                        opName = "op_Inequality";
                        break;
                    default:
                        opName = null;
                        break;
                }

                if (opName != null)
                {
#if !NETSTANDARD12
                    mi = type.GetMethods(BindingFlags.Static | BindingFlags.Public)
                        .SingleOrDefault(p => p.Name == opName && p.GetParameters().Select(q => q.ParameterType).SequenceEqual(typeParams));
#else
                    mi = type.GetRuntimeMethods()
                        .SingleOrDefault(p => p.IsStatic && p.Name == opName && p.GetParameters().Select(q => q.ParameterType).SequenceEqual(typeParams));
#endif
                }
                else
                    mi = null;
            }

            return mi;
        }
    }
}
#endif