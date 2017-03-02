// <copyright file="ParameterNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;

namespace IX.Math.Nodes.Parameters
{
    internal abstract class ParameterNodeBase : NodeBase
    {
        private readonly string parameterName;

        private Expression cachedExpression;

        public ParameterNodeBase(string parameterName)
        {
            this.parameterName = parameterName;
        }

        public string ParameterName => this.parameterName;

        public sealed override Expression GenerateExpression()
        {
            if (this.cachedExpression == null)
            {
                this.cachedExpression = this.GenerateExpressionInternal();
            }

            return this.cachedExpression;
        }

        public override NodeBase RefreshParametersRecursive()
        {
            return this;
        }

        protected abstract Expression GenerateExpressionInternal();
    }
}