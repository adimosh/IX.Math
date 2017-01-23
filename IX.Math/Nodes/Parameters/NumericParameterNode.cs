// <copyright file="NumericParameterNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;

namespace IX.Math.Nodes.Parameters
{
    internal sealed class NumericParameterNode : ParameterNodeBase
    {
        public NumericParameterNode(string parameterName)
            : base(parameterName)
        {
        }

        public bool RequireFloat { get; set; }

        protected override Expression GenerateExpressionInternal()
        {
            return this.RequireFloat ?
                Expression.Parameter(typeof(double), this.ParameterName) :
                Expression.Parameter(typeof(long), this.ParameterName);
        }
    }
}