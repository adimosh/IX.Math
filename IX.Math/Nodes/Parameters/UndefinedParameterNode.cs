// <copyright file="UndefinedParameterNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Generators;

namespace IX.Math.Nodes.Parameters
{
    /// <summary>
    /// An undefined parameter. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="IX.Math.Nodes.Parameters.ParameterNodeBase" />
    [DebuggerDisplay("{ParameterName} (undefined)")]
    public sealed class UndefinedParameterNode : ParameterNodeBase
    {
        /// <summary>
        /// The parameters table.
        /// </summary>
        private IDictionary<string, ParameterNodeBase> parametersTable;

        /// <summary>
        /// The determine float.
        /// </summary>
        private bool? determineFloat;

        /// <summary>
        /// Initializes a new instance of the <see cref="UndefinedParameterNode"/> class.
        /// </summary>
        /// <param name="parameterName">Name of the parameter.</param>
        /// <param name="parametersTable">The parameters table.</param>
        internal UndefinedParameterNode(string parameterName, IDictionary<string, ParameterNodeBase> parametersTable)
            : base(parameterName)
        {
            this.parametersTable = parametersTable;
        }

        /// <summary>
        /// Gets the return type of this node.
        /// </summary>
        /// <value>The node return type.</value>
        public override SupportedValueType ReturnType => SupportedValueType.Unknown;

        /// <summary>
        /// Transforms this undefined parameter into a numeric parameter.
        /// </summary>
        /// <returns>A numeric parameter node.</returns>
        public NumericParameterNode DetermineNumeric() => ParametersGenerator.DetermineNumeric(this.parametersTable, this.ParameterName, this.determineFloat);

        /// <summary>
        /// If the parameter is later determined to be numeric, also determine it to be floating-point.
        /// </summary>
        /// <returns>Reflexive return.</returns>
        public UndefinedParameterNode IfDeterminedNumericAlsoDetermineInteger()
        {
            this.determineFloat = false;
            return this;
        }

        /// <summary>
        /// If the parameter is later determined to be numeric, also determine it to be integer.
        /// </summary>
        /// <returns>Reflexive return.</returns>
        public UndefinedParameterNode IfDeterminedNumericAlsoDetermineFloat()
        {
            this.determineFloat = true;
            return this;
        }

        /// <summary>
        /// Transforms this undefined parameter into a boolean parameter.
        /// </summary>
        /// <returns>A boolean parameter node.</returns>
        public BoolParameterNode DetermineBool() => ParametersGenerator.DetermineBool(this.parametersTable, this.ParameterName);

        /// <summary>
        /// Transforms this undefined parameter into a string parameter.
        /// </summary>
        /// <returns>A string parameter node.</returns>
        public StringParameterNode DetermineString() => ParametersGenerator.DetermineString(this.parametersTable, this.ParameterName);

        /// <summary>
        /// Generates the expression that will be compiled into code as a string expression.
        /// </summary>
        /// <returns>Nothing, as this method always throws an exception.</returns>
        /// <exception cref="System.InvalidOperationException">This node cannot be compiled, as it's supposed to be determined beforehand.</exception>
        public override Expression GenerateStringExpression() => throw new InvalidOperationException();

        /// <summary>
        /// Refreshes all the parameters recursively.
        /// </summary>
        /// <returns>A reference to the determined parameter.</returns>
        public override NodeBase RefreshParametersRecursive() => this.parametersTable[this.ParameterName];

        /// <summary>
        /// Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>Nothing, as this method always throws an exception.</returns>
        /// <exception cref="System.InvalidOperationException">This node cannot be compiled, as it's supposed to be determined beforehand.</exception>
        protected override Expression GenerateExpressionInternal() => throw new InvalidOperationException();
    }
}