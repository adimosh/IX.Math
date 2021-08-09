// <copyright file="OperatorNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using JetBrains.Annotations;

namespace IX.Math.Nodes.Operators
{
    /// <summary>
    /// A base class for an operator.
    /// </summary>
    [PublicAPI]
    internal abstract partial class OperatorNodeBase : NodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OperatorNodeBase"/> class.
        /// </summary>
        protected private OperatorNodeBase()
        {
        }
    }
}