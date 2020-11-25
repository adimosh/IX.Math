// <copyright file="NodeCloningContext.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Collections.Generic;
using IX.Math.Nodes.Parameters;
using JetBrains.Annotations;

namespace IX.Math.Nodes
{
    /// <summary>
    ///     A context for cloning nodes.
    /// </summary>
    [PublicAPI]
    public readonly struct NodeCloningContext
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NodeCloningContext" /> struct.
        /// </summary>
        /// <param name="parameterRegistry">The parameter registry.</param>
        public NodeCloningContext(IDictionary<string, ExternalParameterNode> parameterRegistry)
        {
            this.ParameterRegistry = parameterRegistry;
        }

        /// <summary>
        ///     Gets the parameter registry.
        /// </summary>
        /// <value>The parameter registry.</value>
        public IDictionary<string, ExternalParameterNode> ParameterRegistry
        {
            get;
        }
    }
}