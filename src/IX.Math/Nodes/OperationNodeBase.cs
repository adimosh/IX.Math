// <copyright file="OperationNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using IX.Math.Extensibility;
using IX.Math.Formatters;
using JetBrains.Annotations;

namespace IX.Math.Nodes
{
    /// <summary>
    /// A base class for a node representing an operation.
    /// </summary>
    /// <seealso cref="NodeBase" />
    [PublicAPI]
    public abstract class OperationNodeBase : CachedExpressionNodeBase, ISpecialRequestNode
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
        /// Gets the special object request function.
        /// </summary>
        /// <value>
        /// The special object request function.
        /// </value>
        protected Func<Type, object> SpecialObjectRequestFunction { get; private set; }

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

            var stringFormatters = this.SpecialObjectRequestFunction?.Invoke(typeof(IStringFormatter)) as List<IStringFormatter>;

            return StringFormatter.CreateStringConversionExpression(
                expression,
                stringFormatters);
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

            var stringFormatters = this.SpecialObjectRequestFunction?.Invoke(typeof(IStringFormatter)) as List<IStringFormatter>;

            return StringFormatter.CreateStringConversionExpression(
                expression,
                stringFormatters);
        }

        /// <summary>
        /// Sets the request special object function.
        /// </summary>
        /// <param name="func">The function to set.</param>
        void ISpecialRequestNode.SetRequestSpecialObjectFunction(Func<Type, object> func)
        {
            this.SpecialObjectRequestFunction = func;
            this.SetSpecialObjectRequestFunctionForSubObjects(func);
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public sealed override NodeBase DeepClone(NodeCloningContext context)
        {
            var node = this.DeepCloneNode(context);
            node.SpecialObjectRequestFunction = context.SpecialRequestFunction;
            return node;
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        protected abstract OperationNodeBase DeepCloneNode(NodeCloningContext context);

        /// <summary>
        /// Sets the special object request function for sub objects.
        /// </summary>
        /// <param name="func">The function.</param>
        protected abstract void SetSpecialObjectRequestFunctionForSubObjects(Func<Type, object> func);

        /// <summary>
        /// Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>The expression.</returns>
        [NotNull]
        protected abstract Expression GenerateExpressionInternal();

        /// <summary>
        /// Generates the expression with tolerance that will be compiled into code.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The expression.</returns>
        [NotNull]
        protected virtual Expression GenerateExpressionInternal(Tolerance tolerance) => this.GenerateExpressionInternal();
    }
}