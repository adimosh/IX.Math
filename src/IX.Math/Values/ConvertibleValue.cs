// <copyright file="ConvertibleValue.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace IX.Math.Values
{
    /// <summary>
    /// A base record for a convertible value that can hold multiple representations of the same value.
    /// </summary>
    [PublicAPI]
    public abstract record ConvertibleValue
    {
        /// <summary>
        /// Gets a value indicating whether or not this convertible value holds an integer value representation.
        /// </summary>
        public virtual bool HasInteger => false;

        /// <summary>
        /// Gets a value indicating whether or not this convertible value holds an numeric value representation.
        /// </summary>
        public virtual bool HasNumeric => false;

        /// <summary>
        /// Gets a value indicating whether or not this convertible value holds a binary value representation.
        /// </summary>
        public virtual bool HasBinary => false;

        /// <summary>
        /// Gets a value indicating whether or not this convertible value holds a boolean value representation.
        /// </summary>
        public virtual bool HasBoolean => false;

        /// <summary>
        /// Gets a value indicating whether or not this convertible value holds a string value representation.
        /// </summary>
        public virtual bool HasString => false;

        /// <summary>
        /// Attempts to get the integer representation of the value.
        /// </summary>
        /// <param name="value">The value representation.</param>
        /// <returns><c>true</c> if the value representation was returned, <c>false</c> otherwise.</returns>
        protected virtual bool TryGetInteger(out long value)
        {
            value = default;

            return false;
        }

        /// <summary>
        /// Attempts to get the numeric representation of the value.
        /// </summary>
        /// <param name="value">The value representation.</param>
        /// <returns><c>true</c> if the value representation was returned, <c>false</c> otherwise.</returns>
        protected virtual bool TryGetNumeric(out double value)
        {
            value = default;

            return false;
        }

        /// <summary>
        /// Attempts to get the binary representation of the value.
        /// </summary>
        /// <param name="value">The value representation.</param>
        /// <returns><c>true</c> if the value representation was returned, <c>false</c> otherwise.</returns>
        protected virtual bool TryGetBinary([MaybeNullWhen(false)] out byte[] value)
        {
            value = default;

            return false;
        }

        /// <summary>
        /// Attempts to get the boolean representation of the value.
        /// </summary>
        /// <param name="value">The value representation.</param>
        /// <returns><c>true</c> if the value representation was returned, <c>false</c> otherwise.</returns>
        protected virtual bool TryGetBoolean(out bool value)
        {
            value = default;

            return false;
        }

        /// <summary>
        /// Attempts to get the string representation of the value.
        /// </summary>
        /// <param name="value">The value representation.</param>
        /// <returns><c>true</c> if the value representation was returned, <c>false</c> otherwise.</returns>
        protected virtual bool TryGetString([MaybeNullWhen(false)] out string value)
        {
            value = default;

            return false;
        }

        /// <summary>
        /// Gets an integer value from this convertible value. Use this method in expressions.
        /// </summary>
        /// <returns>The value to be used in expressions.</returns>
        public long GetInteger()
        {
            if (!this.HasInteger || !this.TryGetInteger(out long value))
            {
                throw new InvalidCastException("The current convertible value cannot be cast to an integer value.");
            }

            return value;
        }

        /// <summary>
        /// Gets a numeric value from this convertible value. Use this method in expressions.
        /// </summary>
        /// <returns>The value to be used in expressions.</returns>
        public double GetNumeric()
        {
            if (!this.HasNumeric || !this.TryGetNumeric(out double value))
            {
                throw new InvalidCastException("The current convertible value cannot be cast to a numeric value.");
            }

            return value;
        }

        /// <summary>
        /// Gets a boolean value from this convertible value. Use this method in expressions.
        /// </summary>
        /// <returns>The value to be used in expressions.</returns>
        public bool GetBoolean()
        {
            if (!this.HasBoolean || !this.TryGetBoolean(out bool value))
            {
                throw new InvalidCastException("The current convertible value cannot be cast to a boolean value.");
            }

            return value;
        }

        /// <summary>
        /// Gets a binary value from this convertible value. Use this method in expressions.
        /// </summary>
        /// <returns>The value to be used in expressions.</returns>
        public byte[] GetBinary()
        {
            if (!this.HasBinary || !this.TryGetBinary(out var value))
            {
                throw new InvalidCastException("The current convertible value cannot be cast to a binary value.");
            }

            return value;
        }

        /// <summary>
        /// Gets a string value from this convertible value. Use this method in expressions.
        /// </summary>
        /// <returns>The value to be used in expressions.</returns>
        public string GetString()
        {
            if (!this.HasString || !this.TryGetString(out var value))
            {
                throw new InvalidCastException("The current convertible value cannot be cast to a string value.");
            }

            return value;
        }
    }
}