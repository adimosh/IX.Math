using IX.Math.PlatformMitigation;
using IX.Math.SimplificationAide;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace IX.Math
{
    internal static class ExpressionReductionHelperService
    {
        internal static Expression ReduceIfConstantOperation(this Expression operationExpression)
        {
            if (operationExpression is BinaryExpression)
            {
                var opExp = operationExpression as BinaryExpression;

                if (!(opExp.Left is ConstantExpression) || !(opExp.Right is ConstantExpression))
                    return opExp;

                var leftConstant = (ConstantExpression)opExp.Left;
                var rightConstant = (ConstantExpression)opExp.Right;

                if (leftConstant.Type != rightConstant.Type)
                    return opExp;

                object result;

                MethodInfo mi;
                mi = TryCalculateDirect(leftConstant.Type, opExp.NodeType);

                if (mi == null)
                {
                    mi = GetProperOperator(leftConstant.Type, opExp.NodeType);
                }

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
                    return opExp;
                }
            }
            else if (operationExpression is UnaryExpression)
            {
                return operationExpression;
            }
            else
            {
                return operationExpression;
            }
        }

        private static MethodInfo TryCalculateDirect(Type type, ExpressionType nodeType)
        {
            return typeof(MathematicalOperationsAide)
                .GetTypeMethods()
                .SingleOrDefault(p =>
                {
                    var pars = p.GetParameters();

                    if (pars.Length != 2)
                        return false;

                    return p.Name == Enum.GetName(typeof(ExpressionType), nodeType) &&
                        pars[0].ParameterType == type &&
                        pars[1].ParameterType == type;
                });
        }

        private static MethodInfo GetProperOperator(Type type, ExpressionType eType)
        {
            MethodInfo mi;
            Type[] typeParams = new Type[] { type, type };

            mi = type.GetTypeMethods().SingleOrDefault(p =>
                    p.IsStatic &&
                    p.IsPublic &&
                    p.Name == Enum.GetName(typeof(ExpressionType), eType) &&
                    p.GetParameters().Select(q => q.ParameterType).SequenceEqual(typeParams));

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
                    case ExpressionType.LessThan:
                        opName = "op_LessThan";
                        break;
                    case ExpressionType.LessThanOrEqual:
                        opName = "op_LessThanOrEqual";
                        break;
                    case ExpressionType.GreaterThan:
                        opName = "op_GreaterThan";
                        break;
                    case ExpressionType.GreaterThanOrEqual:
                        opName = "op_GreaterThanOrEqual";
                        break;
                    default:
                        opName = null;
                        break;
                }

                if (opName != null)
                    mi = type.GetTypeMethods().SingleOrDefault(p =>
                            p.IsStatic &&
                            p.IsPublic &&
                            p.Name == opName &&
                            p.GetParameters().Select(q => q.ParameterType).SequenceEqual(typeParams));
                else
                    mi = null;
            }

            return mi;
        }
    }
}