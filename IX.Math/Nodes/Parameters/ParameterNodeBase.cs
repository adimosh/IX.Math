// <copyright file="ParameterNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;

namespace IX.Math.Nodes.Parameters
{
    /// <summary>
    /// A base class for a parameter node.
    /// </summary>
    /// <seealso cref="IX.Math.Nodes.NodeBase" />
    public abstract class ParameterNodeBase : NodeBase
    {
        private readonly string parameterName;

        private Expression cachedExpression;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParameterNodeBase"/> class.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        internal ParameterNodeBase(string parameterName)
        {
            this.parameterName = parameterName;
        }

        /// <summary>
        /// Gets the name of the parameter.
        /// </summary>
        /// <value>The name of the parameter.</value>
        public string ParameterName => this.parameterName;

        /// <summary>
        /// Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>The expression.</returns>
        public sealed override Expression GenerateExpression()
        {
            if (this.cachedExpression == null)
            {
                this.cachedExpression = this.GenerateExpressionInternal();
            }

            return this.cachedExpression;
        }

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

        /// <summary>
        /// Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>The expression.</returns>
        protected abstract Expression GenerateExpressionInternal();
    }
}