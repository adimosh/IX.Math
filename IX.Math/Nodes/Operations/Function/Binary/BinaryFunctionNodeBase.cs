// <copyright file="BinaryFunctionNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.PlatformMitigation;

namespace IX.Math.Nodes.Operations.Function.Binary
{
    internal abstract class BinaryFunctionNodeBase : FunctionNodeBase
    {
        protected BinaryFunctionNodeBase(NodeBase firstParameter, NodeBase secondParameter)
        {
            this.FirstParameter = firstParameter ?? throw new ArgumentNullException(nameof(firstParameter));
            this.SecondParameter = secondParameter ?? throw new ArgumentNullException(nameof(secondParameter));
        }

        public NodeBase FirstParameter { get; protected set; }

        public NodeBase SecondParameter { get; protected set; }

        public override NodeBase RefreshParametersRecursive()
        {
            this.FirstParameter = this.FirstParameter.RefreshParametersRecursive();
            this.SecondParameter = this.SecondParameter.RefreshParametersRecursive();

            return this;
        }

        protected Expression GenerateStaticBinaryFunctionCall<T>(string functionName) => this.GenerateStaticBinaryFunctionCall(typeof(T), functionName);

        protected Expression GenerateStaticBinaryFunctionCall(Type t, string functionName)
        {
            Type firstParameterType = ParameterTypeFromParameter(this.FirstParameter);
            Type secondParameterType = ParameterTypeFromParameter(this.SecondParameter);

            MethodInfo mi = t.GetTypeMethod(functionName, firstParameterType, secondParameterType);

            if (mi == null)
            {
                if ((firstParameterType == typeof(long) && secondParameterType == typeof(double)) ||
                    (firstParameterType == typeof(double) && secondParameterType == typeof(long)))
                {
                    firstParameterType = typeof(double);
                    secondParameterType = typeof(double);

                    mi = t.GetTypeMethod(functionName, firstParameterType, secondParameterType);

                    if (mi == null)
                    {
                        firstParameterType = typeof(long);
                        secondParameterType = typeof(long);

                        mi = t.GetTypeMethod(functionName, firstParameterType, secondParameterType);

                        if (mi == null)
                        {
                            firstParameterType = typeof(int);
                            secondParameterType = typeof(int);

                            mi = t.GetTypeMethod(functionName, firstParameterType, secondParameterType);

                            if (mi == null)
                            {
                                throw new ArgumentException(Resources.FunctionCouldNotBeFound);
                            }
                        }
                    }
                }
                else
                {
                    throw new ArgumentException(Resources.FunctionCouldNotBeFound);
                }
            }

            var e1 = this.FirstParameter.GenerateExpression();
            var e2 = this.SecondParameter.GenerateExpression();

            if (e1.Type != firstParameterType)
            {
                e1 = Expression.Convert(e1, firstParameterType);
            }

            if (e2.Type != secondParameterType)
            {
                e2 = Expression.Convert(e2, secondParameterType);
            }

            return Expression.Call(mi, e1, e2);
        }
    }
}