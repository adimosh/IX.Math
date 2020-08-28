// <copyright file="FunctionNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using JetBrains.Annotations;

namespace IX.Math.Nodes.Functions
{
    /// <summary>
    /// A base class for a function node.
    /// </summary>
    /// <seealso cref="NodeBase" />
    [PublicAPI]
    public abstract class FunctionNodeBase : NodeBase
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="FunctionNodeBase" /> class from being created.
        /// </summary>
        protected private FunctionNodeBase()
        {
        }

        /// <summary>
        ///     Gets a value indicating whether or not this node is actually a constant. Since functions
        ///     can never be considered constant, this always returns false.
        /// </summary>
        /// <value><see langword="true" /> if the node is a constant, <see langword="false" /> otherwise.</value>
        public sealed override bool IsConstant => false;
    }
}