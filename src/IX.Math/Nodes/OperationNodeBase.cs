// <copyright file="OperationNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;
using IX.Math.Formatters;
using JetBrains.Annotations;

namespace IX.Math.Nodes
{
    /// <summary>
    /// A base class for a node representing an operation.
    /// </summary>
    /// <seealso cref="NodeBase" />
    [PublicAPI]
    public abstract class OperationNodeBase : CachedExpressionNodeBase
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="OperationNodeBase"/> class from being created.
        /// </summary>
        protected private OperationNodeBase()
        {
        }

        /// <summary>
        /// Gets a value indicating whether or not this node is actually a constant.
        /// </summary>
        /// <value><see langword="true"/> if the node is a constant, <see langword="false"/> otherwise.</value>
        public override bool IsConstant => false;

        /// <summary>
        /// Generates an expression that will be cached before being compiled.
        /// </summary>
        /// <returns>The generated <see cref="Expression" /> to be cached.</returns>
        /// <remarks>
        /// <para>This method works by first attempting to simplify this node.</para>
        /// <para>If the node can be simplified, <see cref="CachedExpressionNodeBase.GenerateExpression()"/> is called on the new node and returned in lieu of this expression.</para>
        /// <para>If this node cannot be simplified, or its simplification method returns reflexively, <see cref="GenerateExpressionInternal()"/> is called.</para>
        /// </remarks>
        public sealed override Expression GenerateCachedExpression()
        {
            NodeBase simplifiedExpression = this.Simplify();

            return simplifiedExpression != this ? simplifiedExpression.GenerateExpression() : this.GenerateExpressionInternal();
        }

        /// <summary>
        /// Generates an expression with tolerance that will be cached before being compiled.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>
        /// The generated <see cref="Expression" /> to be cached.
        /// </returns>
        /// <remarks>
        /// <para>This method works by first attempting to simplify this node.</para>
        /// <para>If the node can be simplified, <see cref="CachedExpressionNodeBase.GenerateExpression()" /> is called on the new node and returned in lieu of this expression.</para>
        /// <para>If this node cannot be simplified, or its simplification method returns reflexively, <see cref="GenerateExpressionInternal(Tolerance)" /> is called.</para>
        /// </remarks>
        public sealed override Expression GenerateCachedExpression(Tolerance tolerance)
        {
            NodeBase simplifiedExpression = this.Simplify();

            return simplifiedExpression != this ? simplifiedExpression.GenerateExpression(tolerance) : this.GenerateExpressionInternal(tolerance);
        }

        /// <summary>
        /// Generates the cached string expression.
        /// </summary>
        /// <returns>System.Linq.Expressions.Expression.</returns>
        /// <remarks>Since it is not possible for this node to be a constant node, the function <see cref="object.ToString"/> is called in whatever the node outputs.</remarks>
        public sealed override Expression GenerateCachedStringExpression()
        {
            var expression = this.GenerateExpression();

            if (expression.Type == typeof(string))
            {
                return expression;
            }

            return StringFormatter.CreateStringConversionExpression(expression);
        }

        /// <summary>
        ///     Generates a string expression that will be cached before being compiled.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The generated <see cref="Expression" /> to be cached.</returns>
        public sealed override Expression GenerateCachedStringExpression(Tolerance tolerance)
        {
            var expression = this.GenerateExpression(tolerance);

            if (expression.Type == typeof(string))
            {
                return expression;
            }

            return StringFormatter.CreateStringConversionExpression(expression);
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public sealed override NodeBase DeepClone(NodeCloningContext context)
        {
            var node = this.DeepCloneNode(context);
            return node;
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        protected abstract OperationNodeBase DeepCloneNode(NodeCloningContext context);

        /// <summary>
        /// Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>The expression.</returns>
        protected abstract Expression GenerateExpressionInternal();

        /// <summary>
        /// Generates the expression with tolerance that will be compiled into code.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The expression.</returns>
        protected virtual Expression GenerateExpressionInternal(Tolerance tolerance) => this.GenerateExpressionInternal();
    }
}