// <copyright file="BoolParameterNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.PlatformMitigation;

namespace IX.Math.Nodes.Parameters
{
    /// <summary>
    /// A boolean parameter node. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="IX.Math.Nodes.Parameters.ParameterNodeBase" />
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
        /// Generates the expression that will be compiled into code as a string expression.
        /// </summary>
        /// <returns>The string expression.</returns>
        public override Expression GenerateStringExpression() => Expression.Call(this.GenerateExpression(), typeof(object).GetTypeMethod(nameof(object.ToString)));

        /// <summary>
        /// Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>The expression.</returns>
        protected override Expression GenerateExpressionInternal() => Expression.Parameter(typeof(bool), this.ParameterName);
    }
}