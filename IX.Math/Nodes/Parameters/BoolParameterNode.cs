// <copyright file="BoolParameterNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;

namespace IX.Math.Nodes.Parameters
{
    internal sealed class BoolParameterNode : ParameterNodeBase
    {
        public BoolParameterNode(string parameterName)
            : base(parameterName)
        {
        }

        protected override Expression GenerateExpressionInternal()
        {
            return Expression.Parameter(typeof(bool), this.ParameterName);
        }
    }
}