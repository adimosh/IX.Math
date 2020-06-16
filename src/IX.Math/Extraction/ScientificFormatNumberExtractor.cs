// <copyright file="ScientificFormatNumberExtractor.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Text.RegularExpressions;
using IX.Math.Extensibility;
using IX.StandardExtensions.Contracts;

namespace IX.Math.Extraction
{
    /// <summary>
    ///     An extractor for scientific notation of numbers. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="IConstantsExtractor" />
    internal sealed class ScientificFormatNumberExtractor : IConstantsExtractor
    {
        private readonly Regex exponentialNotationRegex = new Regex(@"[0-9.,]+(?:e\+|E\+|e\-|E\-|e|E)[0-9]+");

        /// <summary>
        ///     Extracts a constants, returning its value and placement.
        /// </summary>
        /// <param name="originalExpression">The original expression.</param>
        /// <param name="mathDefinition">The math definition.</param>
        /// <returns>
        ///     A tuple containing a switch indicating success or failure, the extracted value, if any, and the position at which
        ///     it is, as well as its length.
        /// </returns>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "We're fine with this boxing, as the value will be used in a constant and be boxed anyway.")]
        public (bool Success, object Value, int Position, int Length) ExtractConstant(
            in ReadOnlySpan<char> originalExpression,
            MathDefinition mathDefinition)
        {
            Requires.NotNull(
                mathDefinition,
                nameof(mathDefinition));

            Match match = this.exponentialNotationRegex.Match(
                originalExpression.ToString());

            if (!match.Success)
            {
                return (false, default, -1, default);
            }

            int position = match.Index;
            int length = match.Length;
            string content = match.Value;

            if (!double.TryParse(
                content,
                out var val))
            {
                return (false, default, position, default);
            }

            return (true, val, position, length);
        }
    }
}