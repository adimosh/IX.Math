// <copyright file="ConstantNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace IX.Math.Nodes
{
    /// <summary>
    /// A base class for constants.
    /// </summary>
    /// <seealso cref="NodeBase" />
    [PublicAPI]
    [Obsolete("This type of node is not going to be used anymore.")]
    public abstract class ConstantNodeBase : CachedExpressionNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantNodeBase"/> class.
        /// </summary>
        protected private ConstantNodeBase()
        {
        }

        /// <summary>
        /// Gets a value indicating whether or not this node is actually a constant.
        /// </summary>
        /// <value><see langword="true"/> if the node is a constant, <see langword="false"/> otherwise.</value>
        [Obsolete("This is not going to be used anymore.")]
        public bool IsConstant => true;

        /// <summary>
        ///     Gets a value indicating whether this node supports tolerance.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is tolerant; otherwise, <c>false</c>.
        /// </value>
        [Obsolete("This is not going to be used anymore.")]
        public bool IsTolerant => false;

        /// <summary>
        /// Distills the value into a usable constant.
        /// </summary>
        /// <returns>A usable constant.</returns>
        public abstract object DistillValue();

        /// <summary>
        /// Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>A reflexive return.</returns>
        public sealed override NodeBase Simplify() => this;
    }
}