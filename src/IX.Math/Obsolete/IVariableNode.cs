// <copyright file="IVariableNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;

// ReSharper disable once CheckNamespace
namespace IX.Math.Nodes
{
    // ReSharper disable once UnusedType.Global

    /// <summary>
    /// A service contract for a parameter node that is, in fact, a variable.
    /// </summary>
    [Obsolete("This type is not going to be used anymore.")]
    public interface IVariableNode
    {
        /// <summary>
        /// Gets the reference node for this variable.
        /// </summary>
        /// <value>The reference node.</value>
        NodeBase ReferenceNode { get; }

        /// <summary>
        /// Generates the expression that represents the variable itself.
        /// </summary>
        /// <returns>A <see cref="ParameterExpression"/> representing the variable.</returns>
        ParameterExpression GenerateVariableExpression();
    }
}