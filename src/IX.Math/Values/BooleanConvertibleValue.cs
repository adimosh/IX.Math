// <copyright file="BooleanConvertibleValue.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using IX.Math.Formatters;
using JetBrains.Annotations;

namespace IX.Math.Values
{
    /// <summary>
    ///     A boolean convertible value.
    /// </summary>
    [PublicAPI]
    public record BooleanConvertibleValue : ConvertibleValue
    {
#region Internal state

        private string stringRepresentation;

#endregion

#region Constructors and destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BooleanConvertibleValue" /> class.
        /// </summary>
        /// <param name="originalValue">The original value.</param>
        public BooleanConvertibleValue(bool originalValue)
        {
            this.OriginalValue = originalValue;
            this.stringRepresentation = StringFormatter.FormatIntoString(originalValue);
        }

#endregion

#region Properties and indexers

        /// <summary>
        ///     Gets a value indicating whether or not this convertible value holds a boolean value representation.
        /// </summary>
        public override bool HasBoolean => true;

        /// <summary>
        ///     Gets a value indicating whether or not this convertible value holds a string value representation.
        /// </summary>
        public override bool HasString => true;

        /// <summary>
        ///     Gets the original boolean value.
        /// </summary>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "StyleCop.CSharp.DocumentationRules",
            "SA1623:Property summary documentation should match accessors",
            Justification = "The analyzer is rather silly on this one.")]
        public bool OriginalValue { get; }

#endregion

#region Methods

        /// <summary>
        ///     Attempts to get the boolean representation of the value.
        /// </summary>
        /// <param name="value">The value representation.</param>
        /// <returns><c>true</c> if the value representation was returned, <c>false</c> otherwise.</returns>
        protected override bool TryGetBoolean(out bool value)
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