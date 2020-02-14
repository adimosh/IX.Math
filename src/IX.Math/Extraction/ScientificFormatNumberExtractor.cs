// <copyright file="ScientificFormatNumberExtractor.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using IX.Math.Generators;
using IX.Math.Nodes;
using IX.StandardExtensions.Contracts;

namespace IX.Math.Extraction
{
    /// <summary>
    ///     An extractor for scientific notation of numbers. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="Extensibility.IConstantsExtractor" />
    internal sealed class ScientificFormatNumberExtractor : Extensibility.IConstantsExtractor
    {
        private readonly Regex exponentialNotationRegex = new Regex(@"[0-9.,]+(?:e\+|E\+|e\-|E\-|e|E)[0-9]+");

        /// <summary>
        ///     Extracts the scientific notations constants and replaces them with expression placeholders.
        /// </summary>
        /// <param name="originalExpression">The original expression.</param>
        /// <param name="constantsTable">The constants table.</param>
        /// <param name="reverseConstantsTable">The reverse constants table.</param>
        /// <param name="mathDefinition">The math definition.</param>
        /// <returns>The expression, after replacement.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="constantsTable" />
        ///     or
        ///     <paramref name="mathDefinition" />
        ///     or
        ///     <paramref name="originalExpression" />
        ///     or
        ///     <paramref name="reverseConstantsTable" />
        ///     is <see langword="null" /> (<see langword="Nothing" /> in Visual Basic).
        /// </exception>
        public string ExtractAllConstants(
            string originalExpression,
            IDictionary<string, ConstantNodeBase> constantsTable,
            IDictionary<string, string> reverseConstantsTable,
            MathDefinition mathDefinition)
        {
            Contract.RequiresNotNullOrWhitespacePrivate(
                originalExpression,
                nameof(originalExpression));
            Contract.RequiresNotNullPrivate(
                in constantsTable,
                nameof(constantsTable));
            Contract.RequiresNotNullPrivate(
                in reverseConstantsTable,
                nameof(reverseConstantsTable));
            Contract.RequiresNotNullPrivate(
                in mathDefinition,
                nameof(mathDefinition));

            var process = originalExpression;
            var location = 0;

            while (process.Length > location)
            {
                Match match = this.exponentialNotationRegex.Match(
                    process,
                    location);

                if (!match.Success)
                {
                    break;
                }

                var itemName = ConstantsGenerator.GenerateNumericConstant(
                    constantsTable,
                    reverseConstantsTable,
                    process,
                    match.Value);

                if (!string.IsNullOrWhiteSpace(itemName))
                {
                    process = this.exponentialNotationRegex.Replace(
                        process,
                        itemName,
                        1,
                        location);
                }
                else
                {
                    location = match.Index + match.Length;
                }
            }

            return process;
        }
    }
}