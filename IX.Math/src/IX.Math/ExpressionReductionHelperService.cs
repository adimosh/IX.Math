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
        internal static Expression ReduceIfConstantOperation(this Expression operationExpression, Type numericType)
        {
            if (operationExpression is BinaryExpression)
            {
                return ReduceBinaryExpression((BinaryExpression)operationExpression, numericType);
            }
            else if (operationExpression is UnaryExpression)
            {
                return ReduceUnaryExpression((UnaryExpression)operationExpression);
            }
            else
            {
                return operationExpression;
            }
        }

        internal static Expression ReduceIfConstantOperation(this Expression operationExpression)
        {
            if (operationExpression is BinaryExpression)
            {
                return ReduceBinaryExpression((BinaryExpression)operationExpression);
            }
            else if (operationExpression is UnaryExpression)
            {
                return ReduceUnaryExpression((UnaryExpression)operationExpression);
            }
            else
            {
                return operationExpression;
            }
        }

        private static Expression ReduceBinaryExpression(BinaryExpression operationExpression, Type numericType = null)
        {
            var expLeft = operationExpression.Left;

            if (!(expLeft is ConstantExpression))
            {
                expLeft = ReduceIfConstantOperation(expLeft);
            }

            var expRight = operationExpression.Right;

            if (!(expRight is ConstantExpression))
            {
                expRight = ReduceIfConstantOperation(expRight);
            }

            if (!(expLeft is ConstantExpression) || !(expRight is ConstantExpression))
            {
                return operationExpression;
            }

            var leftConstant = (ConstantExpression)expLeft;
            var rightConstant = (ConstantExpression)expRight;

            if (numericType == null)
            {
                numericType = leftConstant.Type;
            }

            object result;

            MethodInfo mi = TryCalculateBinaryDirect(numericType, operationExpression.NodeType);

            if (mi == null)
            {
                mi = GetProperOperator(numericType, operationExpression.NodeType);
            }

            if (mi != null)
            {
                if (mi.IsStatic)
                {
                    result = mi.Invoke(null, new[] { Convert.ChangeType(leftConstant.Value, numericType), Convert.ChangeType(rightConstant.Value, numericType) });
                }
                else
                {
                    result = mi.Invoke(Convert.ChangeType(leftConstant.Value, numericType), new[] { Convert.ChangeType(rightConstant.Value, numericType) });
                }

                return Expression.Constant(result, result.GetType());
            }
            else
            {
                return operationExpression;
            }
        }

        private static Expression ReduceUnaryExpression(UnaryExpression operationExpression)
        {
            var exp = operationExpression.Operand;

            if (!(exp is ConstantExpression))
            {
                exp = ReduceIfConstantOperation(exp);
            }

            if (!(exp is ConstantExpression))
            {
                return operationExpression;
            }

            var constant = (ConstantExpression)exp;

            MethodInfo mi = TryCalculateUnaryDirect(constant.Type, operationExpression.NodeType);

            if (mi != null)
            {
                var result = mi.Invoke(null, new[] { constant.Value });

                return Expression.Constant(result, result.GetType());
            }
            else
            {
                return operationExpression;
            }
        }

        private static MethodInfo TryCalculateUnaryDirect(Type type, ExpressionType nodeType)
        {
            return typeof(MathematicalUnaryOperationsAide)
                .GetTypeMethods()
                .SingleOrDefault(p =>
                {
                    var pars = p.GetParameters();

                    if (pars.Length != 1)
                    {
                        return false;
                    }

                    return p.Name == Enum.GetName(typeof(ExpressionType), nodeType) &&
                        pars[0].ParameterType == type;
                });
        }

        private static MethodInfo TryCalculateBinaryDirect(Type type, ExpressionType nodeType)
        {
            return typeof(MathematicalBinaryOperationsAide)
                .GetTypeMethods()
                .SingleOrDefault(p =>
                {
                    var pars = p.GetParameters();

                    if (pars.Length != 2)
                    {
                        return false;
                    }

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
                {
                    mi = type.GetTypeMethods().SingleOrDefault(p =>
                            p.IsStatic &&
                            p.IsPublic &&
                            p.Name == opName &&
                            p.GetParameters().Select(q => q.ParameterType).SequenceEqual(typeParams));
                }
                else
                {
                    mi = null;
                }
            }

            return mi;
        }
    }
}