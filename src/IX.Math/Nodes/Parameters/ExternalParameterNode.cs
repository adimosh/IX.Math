// <copyright file="ExternalParameterNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using IX.Math.Exceptions;
using IX.StandardExtensions.Contracts;
using IX.StandardExtensions.Efficiency;
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
#region Internal state

        private ParameterExpression? compiledParameterExpression;
        private Dictionary<SupportedValueType, Expression> compiledParameterValueExpressions;
        private Type? parameterType;

#endregion

#region Constructors

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
                SupportedValueType.Binary,
                (0, SupportedValueType.Binary));
            this.CalculatedCosts.Add(
                SupportedValueType.Boolean,
                (0, SupportedValueType.Boolean));
            this.CalculatedCosts.Add(
                SupportedValueType.String,
                (0, SupportedValueType.String));

            this.IsFunction = true;
            this.ParameterType = SupportedValueType.Unknown;
            this.compiledParameterValueExpressions = new Dictionary<SupportedValueType, Expression>(5);
        }

#endregion

#region Properties and indexers

        /// <summary>
        ///     Gets a value indicating whether or not this node is actually a constant.
        /// </summary>
        /// <value><see langword="true" /> if the node is a constant, <see langword="false" /> otherwise.</value>
        public override bool IsConstant =>
            false;

        /// <summary>
        ///     Gets a value indicating whether this node supports tolerance.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the node is tolerant, <see langword="false" /> otherwise.
        /// </value>
        public override bool IsTolerant =>
            false;

        /// <summary>
        ///     Gets the name.
        /// </summary>
        /// <value>
        ///     The name.
        /// </value>
        public string Name
        {
            get;
        }

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

                return this.compiledParameterExpression ?? throw new InvalidOperationException();
            }
        }

        /// <summary>
        ///     Gets a value indicating whether this node requires preservation of the original expression.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the node requires original expression preservation, or <see langword="false" />
        ///     if it can be polynomially-reduced.
        /// </value>
        public override bool RequiresPreservedExpression =>
            this.IsFunction;

        /// <summary>
        ///     Gets a value indicating whether this parameter node is a function.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this parameter node is a function; otherwise, <c>false</c>.
        /// </value>
        public bool IsFunction
        {
            get;
            private set;
        }

        /// <summary>
        ///     Gets or sets the order.
        /// </summary>
        /// <value>
        ///     The order.
        /// </value>
        public int Order
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets the type of the parameter.
        /// </summary>
        /// <value>
        ///     The type of the parameter.
        /// </value>
        public SupportedValueType ParameterType
        {
            get;
            private set;
        }

#endregion

#region Methods

#region Static methods

        private static byte[] GetBinaryValue(in StaticVariableValue value)
        {
            if (!value.TryGetBinary(out var val))
            {
                throw new ExpressionNotValidLogicallyException();
            }

            return val;
        }

        private static bool GetBooleanValue(in StaticVariableValue value)
        {
            if (!value.TryGetBoolean(out var val))
            {
                throw new ExpressionNotValidLogicallyException();
            }

            return val;
        }

        private static long GetIntegerValue(in StaticVariableValue value)
        {
            if (!value.TryGetInteger(out var val))
            {
                throw new ExpressionNotValidLogicallyException();
            }

            return val;
        }

        private static double GetNumericValue(in StaticVariableValue value)
        {
            if (!value.TryGetNumeric(out var val))
            {
                throw new ExpressionNotValidLogicallyException();
            }

            return val;
        }

        private static string GetStringValue(in StaticVariableValue value)
        {
            if (!value.TryGetString(out var val))
            {
                throw new ExpressionNotValidLogicallyException();
            }

            return val;
        }

#endregion

        /// <summary>
        ///     Determines the type of the parameter. This method can only be called once.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <exception cref="ExpressionNotValidLogicallyException">Expression is not valid.</exception>
        public void DetermineParameterType(Type type)
        {
            Type? internalType = Requires.NotNull(
                type,
                nameof(type));

            if (this.parameterType != null)
            {
                throw new MathematicsEngineException();
            }

            SupportedValueType svt;

            if (internalType == typeof(StaticVariableValue))
            {
                svt = SupportedValueType.Unknown;
                this.IsFunction = false;
            }
            else if (internalType == typeof(long))
            {
                svt = SupportedValueType.Integer;
                this.IsFunction = false;
            }
            else if (internalType == typeof(double))
            {
                svt = SupportedValueType.Numeric;
                this.IsFunction = false;
            }
            else if (internalType == typeof(byte[]))
            {
                svt = SupportedValueType.Binary;
                this.IsFunction = false;
            }
            else if (internalType == typeof(bool))
            {
                svt = SupportedValueType.Boolean;
                this.IsFunction = false;
            }
            else if (internalType == typeof(string))
            {
                svt = SupportedValueType.String;
                this.IsFunction = false;
            }
            else if (internalType == typeof(Func<StaticVariableValue>))
            {
                svt = SupportedValueType.Unknown;
                this.IsFunction = true;
            }
            else if (internalType == typeof(Func<long>))
            {
                svt = SupportedValueType.Integer;
                this.IsFunction = true;
            }
            else if (internalType == typeof(Func<double>))
            {
                svt = SupportedValueType.Numeric;
                this.IsFunction = true;
            }
            else if (internalType == typeof(Func<byte[]>))
            {
                svt = SupportedValueType.Binary;
                this.IsFunction = true;
            }
            else if (internalType == typeof(Func<bool>))
            {
                svt = SupportedValueType.Boolean;
                this.IsFunction = true;
            }
            else if (internalType == typeof(Func<string>))
            {
                svt = SupportedValueType.String;
                this.IsFunction = true;
            }
            else
            {
                throw new ExpressionNotValidLogicallyException();
            }

            // Parameter type
            this.ParameterType = svt;
            this.parameterType = internalType;

            // Parameter expression
            this.compiledParameterExpression = Expression.Parameter(
                internalType,
                this.Name);

            // Supported conversions
            this.PossibleReturnType = GetSupportableConversions(
                in svt,
                true);
            this.CalculatedCosts.Clear();
            foreach (SupportedValueType possibleType in GetSupportedTypeOptions(this.PossibleReturnType))
            {
                this.CalculatedCosts[possibleType] = internalType == typeof(StaticVariableValue)
                    ? (0, svt)
                    : (GetStandardConversionStrategyCost(
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
        public override NodeBase Simplify() =>
            this;

        /// <summary>
        ///     Verifies this node and all nodes above it for logical validity.
        /// </summary>
        /// <remarks>
        ///     <para>This method is expected to be overridden, and is a good place to do type restriction verification.</para>
        /// </remarks>
        public override void Verify()
        {
            if (this.parameterType == null)
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
            if (this.compiledParameterValueExpressions.TryGetValue(
                valueType,
                out var value))
            {
                return value;
            }

            // We verify next
            this.Verify();

            Type parameterTypeInternal = this.parameterType!; // Comes after verify, cannot be null
            Expression resultingExpression = this.compiledParameterExpression!;
            if (this.IsFunction)
            {
                // If this is a function, let's invoke it to get the value
                resultingExpression = Expression.Invoke(resultingExpression);
            }

            if (parameterTypeInternal == typeof(StaticVariableValue))
            {
                // The type is a dynamic variable, further processing is needed
                resultingExpression = valueType switch
                {
                    SupportedValueType.Integer => Expression.Call(
                        ((InFunc<StaticVariableValue, long>)GetIntegerValue).Method,
                        resultingExpression),
                    SupportedValueType.Numeric => Expression.Call(
                        ((InFunc<StaticVariableValue, double>)GetNumericValue).Method,
                        resultingExpression),
                    SupportedValueType.Boolean => Expression.Call(
                        ((InFunc<StaticVariableValue, bool>)GetBooleanValue).Method,
                        resultingExpression),
                    SupportedValueType.Binary => Expression.Call(
                        ((InFunc<StaticVariableValue, byte[]>)GetBinaryValue).Method,
                        resultingExpression),
                    SupportedValueType.String => Expression.Call(
                        ((InFunc<StaticVariableValue, string>)GetStringValue).Method,
                        resultingExpression),
                    _ => throw new MathematicsEngineException()
                };
            }

            return this.compiledParameterValueExpressions[valueType] = resultingExpression;
        }

#endregion
    }
}