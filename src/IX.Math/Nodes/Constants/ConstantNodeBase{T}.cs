// <copyright file="ConstantNodeBase{T}.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using IX.Math.Extensibility;
using IX.Math.Formatters;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Constants
{
    /// <summary>
    /// A base class for constants.
    /// </summary>
    /// <typeparam name="T">The type of the constant value.</typeparam>
    /// <seealso cref="ConstantNodeBase" />
    [PublicAPI]
    public abstract class ConstantNodeBase<T> : ConstantNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantNodeBase{T}"/> class.
        /// </summary>
        [SuppressMessage(
            "Usage",
            "CA2214:Do not call overridable methods in constructors",
            Justification = "We specifically intend this to happen.")]
        [SuppressMessage(
            "ReSharper",
            "VirtualMemberCallInConstructor",
            Justification = "We specifically intend this to happen.")]
        protected private ConstantNodeBase(List<IStringFormatter> stringFormatters, T value)
            : base(stringFormatters)
        {
            this.Value = value;

            this.ValueAsString = StringFormatter.FormatIntoString(
                value,
                stringFormatters);

            this.PossibleReturnType = this.GetSupportedTypes(value);

            var possibleReturns = GetSupportedTypeOptions(this.PossibleReturnType);

            foreach (var possibleReturn in possibleReturns)
            {
                this.CalculatedCosts[possibleReturn] = (0, SupportedValueType.Unknown);
            }
        }

        /// <summary>
        /// Gets the value of this constant node.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public T Value { get; }

        /// <summary>
        /// Gets the value as object.
        /// </summary>
        /// <value>
        /// The value as object.
        /// </value>
        public override object ValueAsObject => this.Value;

        /// <summary>
        /// Tries to get a string value out of this constant node.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the constant can safely be converted to a string, <c>false</c> otherwise.</returns>
        public sealed override bool TryGetString(out string value)
        {
            value = this.ValueAsString;
            return true;
        }

        /// <summary>
        /// Generates the expression that this node represents.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="comparisonTolerance">The comparison tolerance.</param>
        /// <returns>A compiled expression, if one is possible.</returns>
        [SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "This is intended.")]
        protected override Expression GenerateExpressionInternal(
            in SupportedValueType valueType,
            in ComparisonTolerance comparisonTolerance)
        {
            if (valueType == SupportedValueType.String)
            {
                return Expression.Constant(
                    this.ValueAsString,
                    typeof(string));
            }

            switch (valueType)
            {
                case SupportedValueType.Integer when this.TryGetInteger(out var v):
                    return Expression.Constant(
                        v,
                        typeof(long));
                case SupportedValueType.Numeric when this.TryGetNumeric(out var v):
                    return Expression.Constant(
                        v,
                        typeof(double));
                case SupportedValueType.ByteArray when this.TryGetByteArray(out var v):
                    return Expression.Constant(
                        v,
                        typeof(byte[]));
                case SupportedValueType.Boolean when this.TryGetBoolean(out var v):
                    return Expression.Constant(
                        v,
                        typeof(bool));
                default:
                    return Expression.Constant(
                        this.Value,
                        typeof(T));
            }
        }

        /// <summary>
        /// Gets the supported types.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The types supported by this constant.
        /// </returns>
        protected virtual SupportableValueType GetSupportedTypes(T value) => GetSupportableConversions(typeof(T));
    }
}