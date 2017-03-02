// <copyright file="OperationNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using IX.Math.PlatformMitigation;

namespace IX.Math.Nodes.Operations
{
    internal abstract class OperationNodeBase : NodeBase
    {
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

        public sealed override Expression GenerateStringExpression() => Expression.Call(this.GenerateExpression(), typeof(object).GetTypeMethod(nameof(object.ToString)));

        protected abstract Expression GenerateExpressionInternal();
    }
}