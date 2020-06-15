// <copyright file="NodeCloningContext.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Collections.Generic;
using IX.Math.Nodes.Parameters;
using JetBrains.Annotations;

namespace IX.Math.Nodes
{
    /// <summary>
    /// A context for cloning nodes.
    /// </summary>
    [PublicAPI]
    public readonly struct NodeCloningContext
    {
        /// <summary>
        /// Gets the parameter registry.
        /// </summary>
        /// <value>The parameter registry.</value>
        public IDictionary<string, ExternalParameterNode> ParameterRegistry { get; }
    }
}