// <copyright file="OperationNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;

namespace IX.Math.Nodes.Operations
{
    internal abstract class OperationNodeBase : NodeBase
    {
        public abstract NodeBase Simplify();

        public sealed override Expression GenerateExpression()
        {
            NodeBase simplifiedExpression = this.Simplify();

            if (simplifiedExpression != this)
            {
                return simplifiedExpression.GenerateExpression();
            }
            else
            {
                return this.GenerateExpressionInternal();
            }
        }

        protected abstract Expression GenerateExpressionInternal();
    }
}