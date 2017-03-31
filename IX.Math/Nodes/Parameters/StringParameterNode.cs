// <copyright file="StringParameterNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;

namespace IX.Math.Nodes.Parameters
{
    /// <summary>
    /// A string parameter node. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="IX.Math.Nodes.Parameters.ParameterNodeBase" />
    [DebuggerDisplay("{ParameterName} (string)")]
    public sealed class StringParameterNode : ParameterNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringParameterNode"/> class.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        internal StringParameterNode(string parameterName)
            : base(parameterName)
        {
        }

        /// <summary>
        /// Gets the return type.
        /// </summary>
        /// <value><see cref="SupportedValueType.String"/>.</value>
        public override SupportedValueType ReturnType => SupportedValueType.String;

        /// <summary>
        /// Generates the expression that will be compiled into code as a string expression.
        /// </summary>
        /// <returns>The string expression.</returns>
        public override Expression GenerateStringExpression() => this.GenerateExpression();

        /// <summary>
        /// Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>The expression.</returns>
        protected override Expression GenerateExpressionInternal() => Expression.Parameter(typeof(string), this.ParameterName);
    }
}