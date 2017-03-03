// <copyright file="UnaryFunctionNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.PlatformMitigation;

namespace IX.Math.Nodes.Operations.Function.Unary
{
    internal abstract class UnaryFunctionNodeBase : FunctionNodeBase
    {
        protected UnaryFunctionNodeBase(NodeBase parameter)
        {
            this.Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        }

        public NodeBase Parameter { get; private set; }

        public override NodeBase RefreshParametersRecursive()
        {
            this.Parameter = this.Parameter.RefreshParametersRecursive();

            return this;
        }

        protected Expression GenerateStaticUnaryFunctionCall<T>(string functionName) =>
            this.GenerateStaticUnaryFunctionCall(typeof(T), functionName);

        protected Expression GenerateStaticUnaryFunctionCall(Type t, string functionName)
        {
            Type parameterType = ParameterTypeFromParameter(this.Parameter);

            MethodInfo mi = t.GetTypeMethod(functionName, parameterType);

            if (mi == null)
            {
                parameterType = typeof(double);

                mi = t.GetTypeMethod(functionName, parameterType);

                if (mi == null)
                {
                    parameterType = typeof(long);

                    mi = t.GetTypeMethod(functionName, parameterType);

                    if (mi == null)
                    {
                        parameterType = typeof(int);

                        mi = t.GetTypeMethod(functionName, parameterType);

                        if (mi == null)
                        {
                            throw new ArgumentException(string.Format(Resources.FunctionCouldNotBeFound, functionName), nameof(functionName));
                        }
                    }
                }
            }

            Expression e = this.Parameter.GenerateExpression();

            if (e.Type != parameterType)
            {
                e = Expression.Convert(e, parameterType);
            }

            return Expression.Call(mi, e);
        }

        protected Expression GenerateStaticUnaryPropertyCall<T>(string parameterName)
        {
            PropertyInfo pi = typeof(T).GetTypeProperty(parameterName);

            if (pi == null)
            {
                throw new ArgumentException(string.Format(Resources.FunctionCouldNotBeFound, parameterName), nameof(parameterName));
            }

            return Expression.Property(this.Parameter.GenerateExpression(), pi);
        }
    }
}