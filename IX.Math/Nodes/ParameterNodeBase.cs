// <copyright file="ParameterNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;

namespace IX.Math.Nodes
{
    /// <summary>
    /// A base class for a parameter node.
    /// </summary>
    /// <seealso cref="IX.Math.Nodes.CachedExpressionNodeBase" />
    public abstract class ParameterNodeBase : CachedExpressionNodeBase
    {
        private readonly string name;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterNodeBase"/> class.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        internal ParameterNodeBase(string parameterName)
        {
            if (string.IsNullOrWhiteSpace(parameterName))
            {
                throw new ArgumentNullException(nameof(parameterName));
            }

            this.name = parameterName;
        }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        /// <value>The name of the parameter.</value>
        public string Name => this.name;

        /// <summary>
        /// Refreshes all the parameters recursively.
        /// </summary>
        /// <returns>A reflexive reference.</returns>
        public override NodeBase RefreshParametersRecursive() => this;

        /// <summary>
        /// Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>A reflexive reference.</returns>
        public override NodeBase Simplify() => this;
    }
}