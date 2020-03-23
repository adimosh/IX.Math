// <copyright file="NodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;
using IX.StandardExtensions;
using JetBrains.Annotations;

namespace IX.Math.Nodes
{
    /// <summary>
    ///     A base class for mathematics nodes.
    /// </summary>
    /// <seealso cref="IDeepCloneable{T}" />
    [PublicAPI]
    public abstract class NodeBase : IContextAwareDeepCloneable<NodeCloningContext, NodeBase>
    {
        /// <summary>
        ///     Gets a value indicating whether or not this node is actually a constant.
        /// </summary>
        /// <value><see langword="true" /> if the node is a constant, <see langword="false" /> otherwise.</value>
        public abstract bool IsConstant { get; }

        /// <summary>
        ///     Gets the return type of this node.
        /// </summary>
        /// <value>The node return type.</value>
        public abstract SupportedValueType ReturnType { get; }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public abstract NodeBase DeepClone(NodeCloningContext context);

        /// <summary>
        ///     Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>The generated <see cref="Expression" />.</returns>
        [NotNull]
        public abstract Expression GenerateExpression();

        /// <summary>
        ///     Generates the expression that will be compiled into code.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>
        ///     The generated <see cref="Expression" />.
        /// </returns>
        [NotNull]
        public virtual Expression GenerateExpression(Tolerance tolerance) => this.GenerateExpression();

        /// <summary>
        ///     Generates the expression that will be compiled into code as a string expression.
        /// </summary>
        /// <returns>The generated <see cref="Expression" /> that gives the values as a string.</returns>
        [NotNull]
        public abstract Expression GenerateStringExpression();

        /// <summary>
        ///     Generates the expression that will be compiled into code as a string expression.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The generated <see cref="Expression" /> that gives the values as a string.</returns>
        [NotNull]
        public virtual Expression GenerateStringExpression(Tolerance tolerance) => this.GenerateStringExpression();

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>A simplified node, or this instance.</returns>
        [NotNull]
        public abstract NodeBase Simplify();

        /// <summary>
        ///     Strongly determines the node's type, if possible.
        /// </summary>
        /// <param name="type">The type to determine to.</param>
        public abstract void DetermineStrongly(SupportedValueType type);

        /// <summary>
        ///     Weakly determines the node's type, if possible, and, optionally, strongly determines if there is only one possible
        ///     type left.
        /// </summary>
        /// <param name="type">The type or types to determine to.</param>
        public abstract void DetermineWeakly(SupportableValueType type);
    }
}