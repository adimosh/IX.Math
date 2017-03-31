// <copyright file="OperationNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using IX.Math.PlatformMitigation;

namespace IX.Math.Nodes.Operations
{
    /// <summary>
    /// A base class for a node representing an operation.
    /// </summary>
    /// <seealso cref="IX.Math.Nodes.NodeBase" />
    public abstract class OperationNodeBase : NodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperationNodeBase"/> class.
        /// </summary>
        protected OperationNodeBase()
            : base()
        {
        }

        /// <summary>
        /// Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>The expression.</returns>
        /// <remarks>
        /// <para>This method works by first attempting to simplify this node.</para>
        /// <para>If the node can be simplified, <see cref="GenerateExpression"/> is called on the new node and returned in lieu of this expression.</para>
        /// <para>If this node cannot be simplified, or its simplification method returns reflexively, <see cref="GenerateExpressionInternal"/> is called.</para>
        /// </remarks>
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

        /// <summary>
        /// Generates the expression that will be compiled into code as a string expression.
        /// </summary>
        /// <returns>The string expression.</returns>
        /// <remarks>Since it is not possible for this node to be a constant node, the function <see cref="object.ToString"/> is called in whatever the node outputs.</remarks>
        public sealed override Expression GenerateStringExpression() => Expression.Call(this.GenerateExpression(), typeof(object).GetTypeMethod(nameof(object.ToString)));

        /// <summary>
        /// Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>The expression.</returns>
        protected abstract Expression GenerateExpressionInternal();
    }
}