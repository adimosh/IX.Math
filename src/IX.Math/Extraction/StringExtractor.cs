// <copyright file="StringExtractor.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Text;
using IX.Math.Generators;
using IX.Math.Nodes;
using IX.StandardExtensions.Contracts;

namespace IX.Math.Extraction
{
    /// <summary>
    ///     An extractor for strings. This class cannot be inherited.
    /// </summary>
    internal sealed class StringExtractor : Extensibility.IConstantsExtractor
    {
        /// <summary>
        ///     Extracts the string constants and replaces them with expression placeholders.
        /// </summary>
        /// <param name="originalExpression">The original expression.</param>
        /// <param name="constantsTable">The constants table.</param>
        /// <param name="reverseConstantsTable">The reverse constants table.</param>
        /// <param name="mathDefinition">The math definition.</param>
        /// <returns>The expression, after replacement.</returns>
        /// <exception cref="ArgumentNullException">
        ///     <paramref name="originalExpression" />
        ///     or
        ///     <paramref name="constantsTable" />
        ///     or
        ///     <paramref name="reverseConstantsTable" />
        ///     is <see langword="null" /> (<see langword="Nothing" /> in Visual Basic).
        /// </exception>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Usage",
            "PC001:API not supported on all platforms",
            Justification = "This is an analyzer bug, TODO: see https://github.com/dotnet/platform-compat/issues/123")]
#if NETSTANDARD2_1 || NET452
        public
#else
        public unsafe
#endif
        string ExtractAllConstants(
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

            var stringIndicagtorString = mathDefinition.StringIndicator;
            var stringIndicator = mathDefinition.StringIndicator.AsSpan();
            var stringIndicatorLength = stringIndicator.Length;
            var escapeCharacter = mathDefinition.EscapeCharacter.AsSpan();
            var escapeCharacterLength = escapeCharacter.Length;

            var process = originalExpression.AsSpan();
            StringBuilder sb = null;

            while (true)
            {
                var openingPosition = process.IndexOf(
                    stringIndicator,
                    StringComparison.CurrentCulture);

                if (openingPosition == -1)
                {
                    // No string opening
                    break;
                }

                var header = process.Slice(
                    0,
                    openingPosition);

                var rest = process.Slice(openingPosition + stringIndicatorLength);

                int closingPosition;
                ReadOnlySpan<char> body;

                do
                {
                    closingPosition = rest.IndexOf(stringIndicator, StringComparison.CurrentCulture);

                    if (closingPosition != -1)
                    {
                        body = rest.Slice(
                            0,
                            closingPosition);

                        int occurrences = 0;

                        while (body.EndsWith(escapeCharacter))
                        {
                            occurrences++;
                            body = body.Slice(
                                0,
                                body.Length - escapeCharacterLength);
                        }

                        rest = rest.Slice(closingPosition + stringIndicatorLength);

                        if (occurrences % 2 == 0)
                        {
                            break;
                        }
                    }
                }
                while (closingPosition != -1);

                if (closingPosition == -1)
                {
                    // No string closing
                    break;
                }

                // We have a proper string
                body = process.Slice(
                        openingPosition,
                        process.Length - header.Length - rest.Length);

                var itemName = ConstantsGenerator.GenerateStringConstant(
                    constantsTable,
                    reverseConstantsTable,
                    originalExpression,
                    stringIndicagtorString,
                    body.ToString());

                if (sb == null)
                {
                    sb = new StringBuilder(originalExpression.Length);
                }

#if NETSTANDARD2_1
                sb.Append(header);
#else
#if NET452
                sb.Append(header.ToString());
#else
                fixed (char* headerPointer = &header.GetPinnableReference())
                {
                    sb.Append(headerPointer, header.Length);
                }
#endif
#endif

                sb.Append(itemName);

                process = rest;
            }

            if (sb == null)
            {
                return originalExpression;
            }

#if NETSTANDARD2_1
            sb.Append(process);
#else
#if NET452
            sb.Append(process.ToString());
#else
            fixed (char* processPointer = &process.GetPinnableReference())
            {
                sb.Append(processPointer, process.Length);
            }
#endif
#endif

            return sb.ToString();
        }
    }
}