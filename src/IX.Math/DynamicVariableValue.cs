// <copyright file="DynamicVariableValue.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using IX.StandardExtensions.Contracts;
using IX.StandardExtensions.Extensions;
using JetBrains.Annotations;
using DiagCA = System.Diagnostics.CodeAnalysis;

namespace IX.Math
{
    /// <summary>
    ///     A value in the variable system that accepts conversions to everything.
    /// </summary>
    [PublicAPI]
    public readonly struct DynamicVariableValue : IEquatable<DynamicVariableValue>
    {
        private readonly string? stringEquivalentValue;

        private readonly long? integerEquivalentValue;

        private readonly double? numericEquivalentValue;

        private readonly byte[]? binaryEquivalentValue;

        private readonly bool? booleanEquivalentValue;

        private readonly string stringDisplay;

        private readonly int hashCode;

        /*
         * Conversion equivalents order:
         * 1. integer
         * 2. numeric
         * 3. binary
         * 4. boolean
         * 5. string
         */

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicVariableValue" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public DynamicVariableValue(string value)
        {
            MathematicPortfolio registry = MathematicPortfolio.CurrentContext?.Value ??
                                           throw new InvalidOperationException(
                                               Resources.ErrorMathematicPortfolioNotAvailable);
            DisplayList displayStrings = new DisplayList();
            bool success;

            Requires.NotNull(
                out this.stringEquivalentValue,
                value,
                nameof(value));

            (success, this.integerEquivalentValue) = registry.Convert<string, long>(value);
            if (success)
            {
                displayStrings.Add(
                    $"{Resources.IntegerValueTag}: {registry.Display(this.integerEquivalentValue.Value)}");
            }
            else
            {
                this.integerEquivalentValue = null;
            }

            (success, this.numericEquivalentValue) = registry.Convert<string, double>(value);
            if (success)
            {
                displayStrings.Add(
                    $"{Resources.NumericValueTag}: {registry.Display(this.numericEquivalentValue.Value)}");
            }
            else
            {
                this.numericEquivalentValue = null;
            }

            (success, this.binaryEquivalentValue) = registry.Convert<string, byte[]>(value);
            if (success)
            {
                displayStrings.Add($"{Resources.BinaryValueTag}: {registry.Display(this.binaryEquivalentValue)}");
            }
            else
            {
                this.binaryEquivalentValue = null;
            }

            (success, this.booleanEquivalentValue) = registry.Convert<string, bool>(value);
            if (success)
            {
                displayStrings.Add(
                    $"{Resources.BooleanValueTag}: {registry.Display(this.booleanEquivalentValue.Value)}");
            }
            else
            {
                this.booleanEquivalentValue = null;
            }

            displayStrings.Add($"{Resources.StringValueTag} {Resources.OriginalValueMarker}: {value}");

            this.stringDisplay = string.Join(
                ";",
                displayStrings.Where(p => p != null));
            this.hashCode = this.stringDisplay.GetHashCode();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicVariableValue" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public DynamicVariableValue(long value)
        {
            MathematicPortfolio registry = MathematicPortfolio.CurrentContext?.Value ??
                                           throw new InvalidOperationException(
                                               Resources.ErrorMathematicPortfolioNotAvailable);
            DisplayList displayStrings = new DisplayList();
            bool success;

            this.integerEquivalentValue = value;
            displayStrings.Add(
                $"{Resources.IntegerValueTag} {Resources.OriginalValueMarker}: {registry.Display(this.integerEquivalentValue.Value)}");

            (success, this.numericEquivalentValue) = registry.Convert<long, double>(value);
            if (success)
            {
                displayStrings.Add(
                    $"{Resources.NumericValueTag}: {registry.Display(this.numericEquivalentValue.Value)}");
            }
            else
            {
                this.numericEquivalentValue = null;
            }

            (success, this.binaryEquivalentValue) = registry.Convert<long, byte[]>(value);
            if (success)
            {
                displayStrings.Add($"{Resources.BinaryValueTag}: {registry.Display(this.binaryEquivalentValue)}");
            }
            else
            {
                this.binaryEquivalentValue = null;
            }

            (success, this.booleanEquivalentValue) = registry.Convert<long, bool>(value);
            if (success)
            {
                displayStrings.Add(
                    $"{Resources.BooleanValueTag}: {registry.Display(this.booleanEquivalentValue.Value)}");
            }
            else
            {
                this.booleanEquivalentValue = null;
            }

            (success, this.stringEquivalentValue) = registry.Convert<long, string>(value);
            if (success)
            {
                displayStrings.Add($"{Resources.StringValueTag}: {registry.Display(this.stringEquivalentValue)}");
            }
            else
            {
                this.stringEquivalentValue = null;
            }

            this.stringDisplay = string.Join(
                ";",
                displayStrings.Where(p => p != null));
            this.hashCode = this.stringDisplay.GetHashCode();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicVariableValue" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public DynamicVariableValue(double value)
        {
            MathematicPortfolio registry = MathematicPortfolio.CurrentContext?.Value ??
                                           throw new InvalidOperationException(
                                               Resources.ErrorMathematicPortfolioNotAvailable);
            DisplayList displayStrings = new DisplayList();
            bool success;

            (success, this.integerEquivalentValue) = registry.Convert<double, long>(value);
            if (success)
            {
                displayStrings.Add(
                    $"{Resources.IntegerValueTag}: {registry.Display(this.integerEquivalentValue.Value)}");
            }
            else
            {
                this.integerEquivalentValue = null;
            }

            this.numericEquivalentValue = value;
            displayStrings.Add(
                $"{Resources.NumericValueTag} {Resources.OriginalValueMarker}: {registry.Display(value)}");

            (success, this.binaryEquivalentValue) = registry.Convert<double, byte[]>(value);
            if (success)
            {
                displayStrings.Add($"{Resources.BinaryValueTag}: {registry.Display(this.binaryEquivalentValue)}");
            }
            else
            {
                this.binaryEquivalentValue = null;
            }

            (success, this.booleanEquivalentValue) = registry.Convert<double, bool>(value);
            if (success)
            {
                displayStrings.Add(
                    $"{Resources.BooleanValueTag}: {registry.Display(this.booleanEquivalentValue.Value)}");
            }
            else
            {
                this.booleanEquivalentValue = null;
            }

            (success, this.stringEquivalentValue) = registry.Convert<double, string>(value);
            if (success)
            {
                displayStrings.Add($"{Resources.StringValueTag}: {registry.Display(this.stringEquivalentValue)}");
            }
            else
            {
                this.stringEquivalentValue = null;
            }

            this.stringDisplay = string.Join(
                ";",
                displayStrings.Where(p => p != null));
            this.hashCode = this.stringDisplay.GetHashCode();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicVariableValue" /> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public DynamicVariableValue(byte[] value)
        {
            MathematicPortfolio registry = MathematicPortfolio.CurrentContext?.Value ??
                                           throw new InvalidOperationException(
                                               Resources.ErrorMathematicPortfolioNotAvailable);
            DisplayList displayStrings = new DisplayList();
            bool success;

            this.binaryEquivalentValue = Requires.NotNull(
                value,
                nameof(value));

            (success, this.integerEquivalentValue) = registry.Convert<byte[], long>(value);
            if (success)
            {
                displayStrings.Add(
                    $"{Resources.IntegerValueTag}: {registry.Display(this.integerEquivalentValue.Value)}");
            }
            else
            {
                this.integerEquivalentValue = null;
            }

            (success, this.numericEquivalentValue) = registry.Convert<byte[], double>(value);
            if (success)
            {
                displayStrings.Add(
                    $"{Resources.NumericValueTag}: {registry.Display(this.numericEquivalentValue.Value)}");
            }
            else
            {
                this.numericEquivalentValue = null;
            }

            displayStrings.Add(
                $"{Resources.BinaryValueTag} {Resources.OriginalValueMarker}: {registry.Display(this.binaryEquivalentValue)}");

            (success, this.booleanEquivalentValue) = registry.Convert<byte[], bool>(value);
            if (success)
            {
                displayStrings.Add(
                    $"{Resources.BooleanValueTag}: {registry.Display(this.booleanEquivalentValue.Value)}");
            }
            else
            {
                this.booleanEquivalentValue = null;
            }

            (success, this.stringEquivalentValue) = registry.Convert<byte[], string>(value);
            if (success)
            {
                displayStrings.Add($"{Resources.StringValueTag}: {registry.Display(this.stringEquivalentValue)}");
            }
            else
            {
                this.stringEquivalentValue = null;
            }

            this.stringDisplay = string.Join(
                ";",
                displayStrings.Where(p => p != null));
            this.hashCode = this.stringDisplay.GetHashCode();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DynamicVariableValue" /> struct.
        /// </summary>
        /// <param name="value">The boolean value.</param>
        public DynamicVariableValue(bool value)
        {
            MathematicPortfolio registry = MathematicPortfolio.CurrentContext?.Value ??
                                           throw new InvalidOperationException(
                                               Resources.ErrorMathematicPortfolioNotAvailable);
            DisplayList displayStrings = new DisplayList();
            bool success;

            (success, this.integerEquivalentValue) = registry.Convert<bool, long>(value);
            if (success)
            {
                displayStrings.Add(
                    $"{Resources.IntegerValueTag}: {registry.Display(this.integerEquivalentValue.Value)}");
            }
            else
            {
                this.integerEquivalentValue = null;
            }

            (success, this.numericEquivalentValue) = registry.Convert<bool, double>(value);
            if (success)
            {
                displayStrings.Add(
                    $"{Resources.NumericValueTag}: {registry.Display(this.numericEquivalentValue.Value)}");
            }
            else
            {
                this.numericEquivalentValue = null;
            }

            (success, this.binaryEquivalentValue) = registry.Convert<bool, byte[]>(value);
            if (success)
            {
                displayStrings.Add($"{Resources.BinaryValueTag}: {registry.Display(this.binaryEquivalentValue)}");
            }
            else
            {
                this.binaryEquivalentValue = null;
            }

            this.booleanEquivalentValue = value;
            displayStrings.Add(
                $"{Resources.BooleanValueTag} {Resources.OriginalValueMarker}: {registry.Display(this.booleanEquivalentValue.Value)}");

            (success, this.stringEquivalentValue) = registry.Convert<bool, string>(value);
            if (success)
            {
                displayStrings.Add($"{Resources.StringValueTag}: {registry.Display(this.stringEquivalentValue)}");
            }
            else
            {
                this.stringEquivalentValue = null;
            }

            this.stringDisplay = string.Join(
                ";",
                displayStrings.Where(p => p != null));
            this.hashCode = this.stringDisplay.GetHashCode();
        }

        /// <summary>
        ///     Performs an implicit conversion from <see cref="string" /> to <see cref="DynamicVariableValue" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator DynamicVariableValue(string value) =>
            new DynamicVariableValue(value);

        /// <summary>
        ///     Performs an implicit conversion from <see cref="long" /> to <see cref="DynamicVariableValue" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator DynamicVariableValue(long value) =>
            new DynamicVariableValue(value);

        /// <summary>
        ///     Performs an implicit conversion from <see cref="double" /> to <see cref="DynamicVariableValue" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator DynamicVariableValue(double value) =>
            new DynamicVariableValue(value);

        /// <summary>
        ///     Performs an implicit conversion from an array of <see cref="byte" />s to <see cref="DynamicVariableValue" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator DynamicVariableValue(byte[] value) =>
            new DynamicVariableValue(value);

        /// <summary>
        ///     Performs an implicit conversion from <see cref="bool" /> to <see cref="DynamicVariableValue" />.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The result of the conversion.
        /// </returns>
        public static implicit operator DynamicVariableValue(bool value) =>
            new DynamicVariableValue(value);

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(
            DynamicVariableValue left,
            DynamicVariableValue right) =>
            left.Equals(right);

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(
            DynamicVariableValue left,
            DynamicVariableValue right) =>
            !(left == right);

        /// <summary>
        ///     Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator ==(
            DynamicVariableValue? left,
            DynamicVariableValue? right) =>
            right.HasValue ? (left?.Equals(right.Value) ?? false) : left.HasValue;

        /// <summary>
        ///     Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        ///     The result of the operator.
        /// </returns>
        public static bool operator !=(
            DynamicVariableValue? left,
            DynamicVariableValue? right) =>
            !(left == right);

        /// <summary>
        ///     Tries to the get an integer value.
        /// </summary>
        /// <param name="integer">The integer value, if one exists.</param>
        /// <returns><c>true</c> if the value has an integer representation, <c>false</c> otherwise.</returns>
        public bool TryGetInteger(out long integer)
        {
            if (this.integerEquivalentValue.HasValue)
            {
                integer = this.integerEquivalentValue.Value;

                return true;
            }

            integer = default;

            return false;
        }

        /// <summary>
        ///     Tries to the get an integer value.
        /// </summary>
        /// <param name="integerExpression">The integer expression of value, if one exists.</param>
        /// <returns><c>true</c> if the value has an integer representation, <c>false</c> otherwise.</returns>
        [DiagCA.SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "Expressions cannot work in any other way.")]
        public bool TryGetIntegerExpression(
            [DiagCA.MaybeNullWhen(false)]
            out Expression integerExpression)
        {
            if (this.integerEquivalentValue.HasValue)
            {
                integerExpression = Expression.Constant(
                    this.integerEquivalentValue.Value,
                    typeof(long));

                return true;
            }

            integerExpression = default;

            return false;
        }

        /// <summary>
        ///     Tries to the get a numeric value.
        /// </summary>
        /// <param name="numeric">The numeric value, if one exists.</param>
        /// <returns><c>true</c> if the value has a numeric representation, <c>false</c> otherwise.</returns>
        public bool TryGetNumeric(out double numeric)
        {
            if (this.numericEquivalentValue.HasValue)
            {
                numeric = this.numericEquivalentValue.Value;

                return true;
            }

            numeric = default;

            return false;
        }

        /// <summary>
        ///     Tries to the get a binary value.
        /// </summary>
        /// <param name="binary">The binary value, if one exists.</param>
        /// <returns><c>true</c> if the value has a binary representation, <c>false</c> otherwise.</returns>
        public bool TryGetBinary(
            [DiagCA.MaybeNullWhen(false)]
            out byte[] binary)
        {
            if (this.binaryEquivalentValue != null)
            {
                binary = this.binaryEquivalentValue;

                return true;
            }

            binary = default;

            return false;
        }

        /// <summary>
        ///     Tries to the get a boolean value.
        /// </summary>
        /// <param name="boolean">The boolean value, if one exists.</param>
        /// <returns><c>true</c> if the value has a boolean representation, <c>false</c> otherwise.</returns>
        public bool TryGetBoolean(out bool boolean)
        {
            if (this.booleanEquivalentValue.HasValue)
            {
                boolean = this.booleanEquivalentValue.Value;

                return true;
            }

            boolean = default;

            return false;
        }

        /// <summary>
        ///     Tries to the get a string value.
        /// </summary>
        /// <param name="str">The string value, if one exists.</param>
        /// <returns><c>true</c> if the value has a string representation, <c>false</c> otherwise.</returns>
        public bool TryGetString(
            [DiagCA.MaybeNullWhen(false)]
            out string str)
        {
            if (this.stringEquivalentValue != null)
            {
                str = this.stringEquivalentValue;

                return true;
            }

            str = default;

            return false;
        }

        /// <summary>Returns the fully qualified type name of this instance.</summary>
        /// <returns>The fully qualified type name.</returns>
        public override string ToString() =>
            this.stringDisplay;

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode() =>
            this.hashCode;

        /// <summary>
        ///     Determines whether the specified <see cref="object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="object" /> to compare with this instance.</param>
        /// <returns>
        ///     <c>true</c> if the specified <see cref="object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object? obj)
        {
            if (!(obj is DynamicVariableValue other) || this.hashCode == other.hashCode)
            {
                return false;
            }

            return this.binaryEquivalentValue.SequenceEquals(other.binaryEquivalentValue) &&
                   this.booleanEquivalentValue == other.booleanEquivalentValue &&
                   this.integerEquivalentValue == other.integerEquivalentValue &&
                   this.numericEquivalentValue == other.numericEquivalentValue &&
                   this.stringEquivalentValue == other.stringEquivalentValue;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///   <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.
        /// </returns>
        public bool Equals(DynamicVariableValue other) =>
            this.binaryEquivalentValue.SequenceEquals(other.binaryEquivalentValue) &&
            this.booleanEquivalentValue == other.booleanEquivalentValue &&
            this.integerEquivalentValue == other.integerEquivalentValue &&
            this.numericEquivalentValue == other.numericEquivalentValue &&
            this.stringEquivalentValue == other.stringEquivalentValue;

        private class DisplayList : List<string>
        {
#region Constructors

            public DisplayList()
                : base(5)
            {
            }

#endregion
        }
    }
}