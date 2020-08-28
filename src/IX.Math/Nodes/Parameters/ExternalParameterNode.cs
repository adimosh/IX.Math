// <copyright file="ExternalParameterNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using IX.Math.Exceptions;
using IX.StandardExtensions.Contracts;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Parameters
{
    /// <summary>
    ///     A class representing an external parameter node.
    /// </summary>
    /// <seealso cref="IX.Math.Nodes.NodeBase" />
    [PublicAPI]
    [DebuggerDisplay("param:{" + nameof(Name) + "}")]
    public sealed class ExternalParameterNode : NodeBase
    {
        private ParameterExpression compiledParameterExpression;
        private Expression compiledParameterValueExpression;
        private Type parameterType;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExternalParameterNode" /> class.
        /// </summary>
        /// <param name="name">The name.</param>
        internal ExternalParameterNode(string name)
        {
            this.Name = Requires.NotNullOrWhiteSpace(
                name,
                nameof(name));

            this.CalculatedCosts.Add(
                SupportedValueType.Numeric,
                (0, SupportedValueType.Numeric));
            this.CalculatedCosts.Add(
                SupportedValueType.Integer,
                (0, SupportedValueType.Integer));
            this.CalculatedCosts.Add(
                SupportedValueType.ByteArray,
                (0, SupportedValueType.ByteArray));
            this.CalculatedCosts.Add(
                SupportedValueType.Boolean,
                (0, SupportedValueType.Boolean));
            this.CalculatedCosts.Add(
                SupportedValueType.String,
                (0, SupportedValueType.String));

            this.IsFunction = true;
            this.ParameterType = SupportedValueType.Unknown;
        }

        /// <summary>
        ///     Gets a value indicating whether or not this node is actually a constant.
        /// </summary>
        /// <value><see langword="true" /> if the node is a constant, <see langword="false" /> otherwise.</value>
        public override bool IsConstant => false;

        /// <summary>
        ///     Gets a value indicating whether this node supports tolerance.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the node is tolerant, <see langword="false" /> otherwise.
        /// </value>
        public override bool IsTolerant => false;

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name { get; }

        /// <summary>
        ///     Gets the parameter definition expression.
        /// </summary>
        /// <value>
        ///     The parameter definition expression.
        /// </value>
        public ParameterExpression ParameterDefinitionExpression
        {
            get
            {
                this.Verify();

                return this.compiledParameterExpression;
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this node requires preservation of the original expression.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the node requires original expression preservation, or <see langword="false" />
        ///     if it can be polynomially-reduced.
        /// </value>
        public override bool RequiresPreservedExpression => this.IsFunction;

        /// <summary>
        ///     Gets a value indicating whether this parameter node is a function.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this parameter node is a function; otherwise, <c>false</c>.
        /// </value>
        public bool IsFunction { get; private set; }

        /// <summary>
        ///     Gets or sets the order.
        /// </summary>
        /// <value>
        ///     The order.
        /// </value>
        public int Order { get; set; }

        /// <summary>
        ///     Gets the type of the parameter.
        /// </summary>
        /// <value>
        ///     The type of the parameter.
        /// </value>
        public SupportedValueType ParameterType { get; private set; }

        /// <summary>
        ///     Determines the type of the parameter.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <exception cref="ExpressionNotValidLogicallyException">Expression is not valid.</exception>
        public void DetermineParameterType(Type type)
        {
            if (this.ParameterType != SupportedValueType.Unknown)
            {
                throw new MathematicsEngineException();
            }

            SupportedValueType svt;
            if (type == typeof(long))
            {
                svt = SupportedValueType.Integer;
                this.IsFunction = false;
            }
            else if (type == typeof(double))
            {
                svt = SupportedValueType.Numeric;
                this.IsFunction = false;
            }
            else if (type == typeof(byte[]))
            {
                svt = SupportedValueType.ByteArray;
                this.IsFunction = false;
            }
            else if (type == typeof(bool))
            {
                svt = SupportedValueType.Boolean;
                this.IsFunction = false;
            }
            else if (type == typeof(string))
            {
                svt = SupportedValueType.String;
                this.IsFunction = false;
            }
            else if (type == typeof(Func<long>))
            {
                svt = SupportedValueType.Integer;
                this.IsFunction = true;
            }
            else if (type == typeof(Func<double>))
            {
                svt = SupportedValueType.Numeric;
                this.IsFunction = true;
            }
            else if (type == typeof(Func<byte[]>))
            {
                svt = SupportedValueType.ByteArray;
                this.IsFunction = true;
            }
            else if (type == typeof(Func<bool>))
            {
                svt = SupportedValueType.Boolean;
                this.IsFunction = true;
            }
            else if (type == typeof(Func<string>))
            {
                svt = SupportedValueType.String;
                this.IsFunction = true;
            }
            else
            {
                throw new ExpressionNotValidLogicallyException();
            }

            this.ParameterType = svt;
            this.parameterType = type;
            this.PossibleReturnType = GetSupportableConversions(in svt);

            this.CalculatedCosts.Clear();
            foreach (SupportedValueType possibleType in GetSupportedTypeOptions(this.PossibleReturnType))
            {
                this.CalculatedCosts[possibleType] = (GetStandardConversionStrategyCost(
                    in svt,
                    in possibleType), svt);
            }
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context)
        {
            if (!context.ParameterRegistry.TryGetValue(
                this.Name,
                out ExternalParameterNode result))
            {
                // We might have a indexed variable with a discovered indexer
                ExternalParameterNode par = context.ParameterRegistry.FirstOrDefault(p => p.Value.Name == this.Name)
                    .Value;
                if (par == null)
                {
                    throw new MathematicsEngineException();
                }

                return par;
            }

            return result;
        }

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>A simplified node, or this instance.</returns>
        public override NodeBase Simplify() => this;

        /// <summary>
        ///     Verifies this node and all nodes above it for logical validity.
        /// </summary>
        /// <remarks>
        ///     <para>This method is expected to be overridden, and is a good place to do type restriction verification.</para>
        /// </remarks>
        public override void Verify()
        {
            if (this.ParameterType == SupportedValueType.Unknown)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        /// <summary>
        ///     Generates the expression that this node represents.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="comparisonTolerance">The comparison tolerance.</param>
        /// <returns>A compiled expression, if one is possible.</returns>
        protected override Expression GenerateExpressionInternal(
            in SupportedValueType valueType,
            in ComparisonTolerance comparisonTolerance)
        {
            // We get from cache first
            if (this.compiledParameterValueExpression != null)
            {
                return this.compiledParameterValueExpression;
            }

            // We verify next
            this.Verify();

            // Then we create the parameter expression and return it
            this.compiledParameterExpression = Expression.Parameter(
                this.parameterType,
                this.Name);

            if (this.IsFunction)
            {
                return this.compiledParameterValueExpression = Expression.Invoke(this.compiledParameterExpression);
            }

            return this.compiledParameterValueExpression = this.compiledParameterExpression;
        }
    }
}