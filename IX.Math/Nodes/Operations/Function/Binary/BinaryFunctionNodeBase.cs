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

        protected Expression GenerateStaticBinaryFunctionCall<T>(string functionName)
        {
            Type firstParameterType = ParameterTypeFromParameter(this.FirstParameter);
            Type secondParameterType = ParameterTypeFromParameter(this.SecondParameter);

            MethodInfo mi = typeof(T).GetTypeMethod(functionName, firstParameterType, secondParameterType);

            if (mi == null)
            {
                throw new ArgumentException(Resources.FunctionCouldNotBeFound);
            }

            return Expression.Call(mi, this.FirstParameter.GenerateExpression(), this.SecondParameter.GenerateExpression());
        }

        protected Expression GenerateStaticBinaryFunctionCall(Type t, string functionName)
        {
            Type firstParameterType = ParameterTypeFromParameter(this.FirstParameter);
            Type secondParameterType = ParameterTypeFromParameter(this.SecondParameter);

            MethodInfo mi = t.GetTypeMethod(functionName, firstParameterType, secondParameterType);

            if (mi == null)
            {
                throw new ArgumentException(Resources.FunctionCouldNotBeFound);
            }

            return Expression.Call(mi, this.FirstParameter.GenerateExpression(), this.SecondParameter.GenerateExpression());
        }
    }
}