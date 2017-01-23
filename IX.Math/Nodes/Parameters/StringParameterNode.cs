// <copyright file="StringParameterNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;

namespace IX.Math.Nodes.Parameters
{
    internal sealed class StringParameterNode : ParameterNodeBase
    {
        public StringParameterNode(string parameterName)
            : base(parameterName)
        {
        }

        protected override Expression GenerateExpressionInternal()
        {
            return Expression.Parameter(typeof(string), this.ParameterName);
        }
    }
}