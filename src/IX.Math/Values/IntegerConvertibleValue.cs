// <copyright file="IntegerConvertibleValue.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using IX.Math.Formatters;
using JetBrains.Annotations;

namespace IX.Math.Values
{
    /// <summary>
    ///     An integer convertible value.
    /// </summary>
    [PublicAPI]
    public record IntegerConvertibleValue : ConvertibleValue
    {
#region Internal state

        private readonly byte[] binaryRepresentation;
        private readonly bool booleanRepresentation;
        private readonly double numericRepresentation;
        private readonly string stringRepresentation;

#endregion

#region Constructors and destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="IntegerConvertibleValue" /> class.
        /// </summary>
        /// <param name="originalValue">The original value.</param>
        public IntegerConvertibleValue(long originalValue)
        {
            this.OriginalValue = originalValue;
            this.binaryRepresentation = BitConverter.GetBytes(originalValue);
            this.booleanRepresentation = originalValue != 0;
            this.numericRepresentation = originalValue;
            this.stringRepresentation = StringFormatter.FormatIntoString(originalValue);
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
        public override bool HasInteger => true;

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
        public long OriginalValue { get; }

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
            value = this.OriginalValue;

            return true;
        }

        /// <summary>
        ///     Attempts to get the numeric representation of the value.
        /// </summary>
        /// <param name="value">The value representation.</param>
        /// <returns><c>true</c> if the value representation was returned, <c>false</c> otherwise.</returns>
        protected override bool TryGetNumeric(out double value)
        {
            value = this.numericRepresentation;

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