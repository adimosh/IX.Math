// <copyright file="SillyConstantsExtractor.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Text.RegularExpressions;
using IX.Math;
using IX.Math.Extensibility;
using JetBrains.Annotations;

namespace IX.UnitTests.ExternalAssemblyCapabilities
{
    /// <summary>
    /// A constants extractor used for testing purposes.
    /// </summary>
    /// <seealso cref="IConstantsExtractor" />
    [UsedImplicitly]
    public class SillyConstantsExtractor : IConstantsExtractor
    {
        private readonly Regex exponentialNotationRegex = new Regex(@"silly");

        /// <summary>
        ///     Extracts a constants, returning its value and placement.
        /// </summary>
        /// <param name="originalExpression">The original expression.</param>
        /// <param name="mathDefinition">The math definition.</param>
        /// <returns>
        ///     A tuple containing a switch indicating success or failure, the extracted value, if any, and the position at which
        ///     it is, as well as its length.
        /// </returns>
        /// <remarks>
        ///     Please note that the Position argument should be returned if a possible match is found, even if it does not
        ///     compute properly.
        /// </remarks>
        public (bool Success, object Value, int Position, int Length) ExtractConstant(
            in ReadOnlySpan<char> originalExpression,
            MathDefinition mathDefinition)
        {
            var match = this.exponentialNotationRegex.Match(originalExpression.ToString());

            if (!match.Success)
            {
                return (false, default, -1, default);
            }

            return (true, "stupid", match.Index, match.Length);
        }
    }
}