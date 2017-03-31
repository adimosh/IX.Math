// <copyright file="NumericParameterNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.PlatformMitigation;

namespace IX.Math.Nodes.Parameters
{
    /// <summary>
    /// A numeric parameter node. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="IX.Math.Nodes.Parameters.ParameterNodeBase" />
    [DebuggerDisplay("{ParameterName} (numeric)")]
    public sealed class NumericParameterNode : ParameterNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NumericParameterNode" /> class.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        internal NumericParameterNode(string parameterName)
                    : base(parameterName)
        {
        }

        /// <summary>
        /// Gets or sets whether this parameter is required to be float.
        /// </summary>
        /// <value>
        /// <c>true</c> if this parameter has to be a floating-point number,
        /// <c>false</c> if this parameter must not be a floating-point number and
        /// <c>null</c>, if it doesn't matter what type of numeric parameter this is.
        /// </value>
        public bool? RequireFloat { get; set; }

        /// <summary>
        /// Gets the return type.
        /// </summary>
        /// <value><see cref="SupportedValueType.Numeric"/>.</value>
        public override SupportedValueType ReturnType => SupportedValueType.Numeric;

        /// <summary>
        /// Generates the expression that will be compiled into code as a string expression.
        /// </summary>
        /// <returns>The string expression.</returns>
        public override Expression GenerateStringExpression() => Expression.Call(this.GenerateExpression(), typeof(object).GetTypeMethod(nameof(object.ToString)));

        /// <summary>
        /// Sets this parameter as an obligatory floating-point parameter.
        /// </summary>
        /// <returns>Reflexive return.</returns>
        public NumericParameterNode ParameterMustBeFloat()
        {
            if (this.RequireFloat != null)
            {
                if (!this.RequireFloat.Value)
                {
                    throw new InvalidOperationException(string.Format(Resources.ParameterMustBeFloatButAlreadyRequiredToBeInteger, this.ParameterName));
                }
            }
            else
            {
                this.RequireFloat = true;
            }

            return this;
        }

        /// <summary>
        /// Sets this parameter as an obligatory integer parameter.
        /// </summary>
        /// <returns>Reflexive return.</returns>
        public NumericParameterNode ParameterMustBeInteger()
        {
            if (this.RequireFloat != null)
            {
                if (this.RequireFloat.Value)
                {
                    throw new InvalidOperationException(string.Format(Resources.ParameterMustBeIntegerButAlreadyRequiredToBeFloat, this.ParameterName));
                }
            }
            else
            {
                this.RequireFloat = false;
            }

            return this;
        }

        /// <summary>
        /// Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>The expression.</returns>
        protected override Expression GenerateExpressionInternal() => (this.RequireFloat ?? true) ?
                        Expression.Parameter(typeof(double), this.ParameterName) :
                        Expression.Parameter(typeof(long), this.ParameterName);
    }
}