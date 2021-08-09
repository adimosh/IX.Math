// <copyright file="BinaryConvertibleValue.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using IX.Math.Formatters;
using IX.StandardExtensions.Contracts;
using JetBrains.Annotations;

namespace IX.Math.Values
{
    /// <summary>
    ///     A binary convertible value.
    /// </summary>
    [PublicAPI]
    public record BinaryConvertibleValue : ConvertibleValue
    {
#region Internal state

        private readonly long? integerRepresentation;
        private readonly double? numericRepresentation;
        private readonly string stringRepresentation;

#endregion

#region Constructors and destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BinaryConvertibleValue" /> class.
        /// </summary>
        /// <param name="originalValue">The original value.</param>
        public BinaryConvertibleValue(byte[] originalValue)
        {
            this.OriginalValue = Requires.NotNull(
                originalValue,
                nameof(originalValue));

            switch (originalValue.Length)
            {
                case 8:
                {
                    this.integerRepresentation = BitConverter.ToInt64(
                        originalValue,
                        0);
                    this.numericRepresentation = BitConverter.ToDouble(
                        originalValue,
                        0);

                    break;
                }

                case < 8:
                {
                    byte[] paddedValue = new byte[8];
                    Array.Copy(
                        originalValue,
                        paddedValue,
                        BitConverter.IsLittleEndian ? 8 - originalValue.Length : 0);
                    this.integerRepresentation = BitConverter.ToInt64(
                        paddedValue,
                        0);
                    this.numericRepresentation = BitConverter.ToDouble(
                        paddedValue,
                        0);

                    break;
                }

                default:
                {
                    this.integerRepresentation = null;
                    this.numericRepresentation = null;

                    break;
                }
            }

            this.stringRepresentation = StringFormatter.FormatIntoString(originalValue);
        }

#endregion

#region Properties and indexers

        /// <summary>
        ///     Gets a value indicating whether or not this convertible value holds a binary value representation.
        /// </summary>
        public override bool HasBinary => true;

        /// <summary>
        ///     Gets a value indicating whether or not this convertible value holds an integer value representation.
        /// </summary>
        public override bool HasInteger => this.integerRepresentation != null;

        /// <summary>
        ///     Gets a value indicating whether or not this convertible value holds an numeric value representation.
        /// </summary>
        public override bool HasNumeric => this.numericRepresentation != null;

        /// <summary>
        ///     Gets a value indicating whether or not this convertible value holds a string value representation.
        /// </summary>
        public override bool HasString => true;

        /// <summary>
        ///     Gets the original byte array value.
        /// </summary>
        public byte[] OriginalValue { get; }

#endregion

#region Methods

        /// <summary>
        ///     Attempts to get the binary representation of the value.
        /// </summary>
        /// <param name="value">The value representation.</param>
        /// <returns><c>true</c> if the value representation was returned, <c>false</c> otherwise.</returns>
        protected override bool TryGetBinary(out byte[] value)
        {
            value = this.OriginalValue;

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
            if (this.numericRepresentation != null)
            {
                value = this.numericRepresentation.Value;

                return true;
            }

            value = default;

            return false;
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