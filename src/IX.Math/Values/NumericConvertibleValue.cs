// <copyright file="NumericConvertibleValue.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using IX.Math.Formatters;
using JetBrains.Annotations;

namespace IX.Math.Values
{
    /// <summary>
    ///     A numeric convertible value.
    /// </summary>
    [PublicAPI]
    public record NumericConvertibleValue : ConvertibleValue
    {
#region Internal state

        private readonly byte[] binaryRepresentation;
        private readonly bool booleanRepresentation;
        private readonly long? integerRepresentation;
        private readonly string stringRepresentation;

#endregion

#region Constructors and destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="NumericConvertibleValue" /> class.
        /// </summary>
        /// <param name="originalValue">The original value.</param>
        [SuppressMessage(
            "ReSharper",
            "CompareOfFloatsByEqualityOperator",
            Justification = "We aren't particularly interested in this just yet.")]
        [SuppressMessage(
            "CodeQuality",
            "IDE0079:Remove unnecessary suppression",
            Justification = "ReSharper is used in this project.")]
        public NumericConvertibleValue(double originalValue)
        {
            this.OriginalValue = originalValue;
            this.binaryRepresentation = BitConverter.GetBytes(originalValue);
            this.booleanRepresentation = originalValue != 0;
            this.stringRepresentation = StringFormatter.FormatIntoString(originalValue);

            try
            {
                if (global::System.Math.Floor(originalValue) == originalValue ||
                    global::System.Math.Ceiling(originalValue) == originalValue)
                {
                    this.integerRepresentation = Convert.ToInt64(originalValue);
                }
                else
                {
                    this.integerRepresentation = null;
                }
            }
            catch (Exception)
            {
                // It is possible that the case might still fail, as the value is too big.
                this.integerRepresentation = null;
            }
        }

#endregion

#region Properties and indexers

        /// <summary>
        ///     Gets a value indicating whether or not this convertible value holds a binary value representation.
        /// </summary>
        public override bool HasBinary => true;

        /// <summary>
        ///     Gets a value indicating whether or not this convertible value holds a boolean value representation.
        /// </summary>
        public override bool HasBoolean => true;

        /// <summary>
        ///     Gets a value indicating whether or not this convertible value holds an integer value representation.
        /// </summary>
        public override bool HasInteger => this.integerRepresentation != null;

        /// <summary>
        ///     Gets a value indicating whether or not this convertible value holds an numeric value representation.
        /// </summary>
        public override bool HasNumeric => true;

        /// <summary>
        ///     Gets a value indicating whether or not this convertible value holds a string value representation.
        /// </summary>
        public override bool HasString => true;

        /// <summary>
        ///     Gets the original long value.
        /// </summary>
        public double OriginalValue { get; }

#endregion

#region Methods

        /// <summary>
        ///     Attempts to get the binary representation of the value.
        /// </summary>
        /// <param name="value">The value representation.</param>
        /// <returns><c>true</c> if the value representation was returned, <c>false</c> otherwise.</returns>
        protected override bool TryGetBinary(out byte[] value)
        {
            value = this.binaryRepresentation;

            return true;
        }

        /// <summary>
        ///     Attempts to get the boolean representation of the value.
        /// </summary>
        /// <param name="value">The value representation.</param>
        /// <returns><c>true</c> if the value representation was returned, <c>false</c> otherwise.</returns>
        protected override bool TryGetBoolean(out bool value)
        {
            value = this.booleanRepresentation;

            return true;
        }

        /// <summary>
        ///     Attempts to get the integer representation of the value.
        /// </summary>
        /// <param name="value">The value representation.</param>
        /// <returns><c>true</c> if the value representation was returned, <c>false</c> otherwise.</returns>
        protected override bool TryGetInteger(out long value)
        {
            if (this.integerRepresentation != null)
            {
                value = this.integerRepresentation.Value;

                return true;
            }

            value = default;

            return false;
        }

        /// <summary>
        ///     Attempts to get the numeric representation of the value.
        /// </summary>
        /// <param name="value">The value representation.</param>
        /// <returns><c>true</c> if the value representation was returned, <c>false</c> otherwise.</returns>
        protected override bool TryGetNumeric(out double value)
        {
            value = this.OriginalValue;

            return true;
        }

        /// <summary>
        ///     Attempts to get the string representation of the value.
        /// </summary>
        /// <param name="value">The value representation.</param>
        /// <returns><c>true</c> if the value representation was returned, <c>false</c> otherwise.</returns>
        protected override bool TryGetString(out string value)
        {
            value = this.stringRepresentation;

            return true;
        }

#endregion
    }
}