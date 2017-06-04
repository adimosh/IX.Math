// <copyright file="BoolParameterNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.PlatformMitigation;

namespace IX.Math.Nodes.Parameters
{
    /// <summary>
    /// A boolean parameter node. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="ParameterNodeBase" />
    [DebuggerDisplay("{ParameterName} (bool)")]
    public sealed class BoolParameterNode : ParameterNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoolParameterNode" /> class.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        internal BoolParameterNode(string parameterName)
                    : base(parameterName)
        {
        }

        /// <summary>
        /// Gets the return type of this node.
        /// </summary>
        /// <value>Always <see cref="SupportedValueType.Boolean"/>.</value>
        public override SupportedValueType ReturnType => SupportedValueType.Boolean;

        /// <summary>
        /// Generates a string expression that will be cached before being compiled.
        /// </summary>
        /// <returns>The generated <see cref="T:System.Linq.Expressions.Expression" /> to be cached.</returns>
        public override Expression GenerateCachedStringExpression() => Expression.Call(this.GenerateExpression(), typeof(object).GetTypeMethod(nameof(object.ToString)));

        /// <summary>
        /// Generates an expression that will be cached before being compiled.
        /// </summary>
        /// <returns>The generated <see cref="T:System.Linq.Expressions.Expression" /> to be cached.</returns>
        public override Expression GenerateCachedExpression() => Expression.Parameter(typeof(bool), this.Name);
    }
}