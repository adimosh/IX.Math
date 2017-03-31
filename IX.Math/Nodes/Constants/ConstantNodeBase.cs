﻿// <copyright file="ConstantNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

namespace IX.Math.Nodes.Constants
{
    /// <summary>
    /// A base class for constants.
    /// </summary>
    /// <seealso cref="IX.Math.Nodes.NodeBase" />
    public abstract class ConstantNodeBase : NodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantNodeBase"/> class.
        /// </summary>
        internal ConstantNodeBase()
        {
        }

        /// <summary>
        /// Distills the value into a usable constant.
        /// </summary>
        /// <returns>A usable constant.</returns>
        public abstract object DistillValue();

        /// <summary>
        /// Refreshes all the parameters recursively.
        /// </summary>
        /// <returns>A reflexive return.</returns>
        public sealed override NodeBase RefreshParametersRecursive() => this;

        /// <summary>
        /// Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>A reflexive return.</returns>
        public sealed override NodeBase Simplify() => this;
    }
}