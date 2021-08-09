// <copyright file="StringConvertibleValue.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using IX.StandardExtensions.Contracts;
using JetBrains.Annotations;

namespace IX.Math.Values
{
    /// <summary>
    ///     A string convertible value.
    /// </summary>
    [PublicAPI]
    public record StringConvertibleValue : ConvertibleValue
    {
#region Constructors and destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="StringConvertibleValue" /> class.
        /// </summary>
        /// <param name="originalValue">The original value.</param>
        public StringConvertibleValue(string originalValue)
        {
            this.OriginalValue = Requires.NotNull(
                originalValue,
                nameof(originalValue));
        }

#endregion

#region Properties and indexers

        /// <summary>
        ///     Gets a value indicating whether or not this convertible value holds a string value representation.
        /// </summary>
        public override bool HasString => true;

        /// <summary>
        ///     Gets the original string value.
        /// </summary>
        public string OriginalValue { get; }

#endregion

#region Methods

        /// <summary>
        ///     Attempts to get the string representation of the value.
        /// </summary>
        /// <param name="value">The value representation.</param>
        /// <returns><c>true</c> if the value representation was returned, <c>false</c> otherwise.</returns>
        protected override bool TryGetString(out string value)
        {
            value = this.OriginalValue;

            return true;
        }

#endregion
    }
}