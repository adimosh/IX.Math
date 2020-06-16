// <copyright file="StringExtractor.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Text.RegularExpressions;
using System.Threading;
using IX.Math.Extensibility;

namespace IX.Math.Extraction
{
    /// <summary>
    ///     An extractor for strings. This class cannot be inherited.
    /// </summary>
    internal sealed class StringExtractor : IConstantsExtractor
    {
        private string stringIndicator;
        private string escapeCharacter;

        private Regex quotationMarksRegex;

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
            var input = originalExpression.ToString();

            if (this.quotationMarksRegex == null ||
                this.stringIndicator != mathDefinition.StringIndicator ||
                this.escapeCharacter != mathDefinition.EscapeCharacter)
            {
                var si = this.stringIndicator = mathDefinition.StringIndicator;
                var ec = this.escapeCharacter = mathDefinition.EscapeCharacter;

                if (ec == "\\")
                {
                    ec = "\\\\";
                }

                Interlocked.Exchange(
                    ref this.quotationMarksRegex,
                    new Regex(
                        $@"(?<!{ec})(?:{ec}{{2}})*(?<constant>{si}(?<content>(?:(?<!{ec})(?:{ec}{{2}})*{ec}{si}|[^{si}])+(?<!{ec})(?:{ec}{{2}})*){si})"));
            }

            var match = this.quotationMarksRegex.Match(input);

            if (match.Success)
            {
                return (false, default, -1, default);
            }

            var grp = match.Groups["constant"];
            int index = grp.Index;
            int length = grp.Length;
            string content = match.Groups["content"]
                .Value;

            return (true, content, index, length);
        }
    }
}