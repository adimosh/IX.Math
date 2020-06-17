// <copyright file="ConstantsGenerator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading;
using IX.Math.Exceptions;
using IX.Math.Extensibility;
using IX.Math.Formatters;
using IX.Math.Nodes.Constants;
using IX.StandardExtensions.Contracts;
using IX.System.Collections.Generic;
using JetBrains.Annotations;

namespace IX.Math.Generators
{
    /// <summary>
    ///     A generator for constant values and their like.
    /// </summary>
    internal static class ConstantsGenerator
    {
        /// <summary>
        /// Generates a named numeric symbol.
        /// </summary>
        /// <param name="constantsTable">The constants table.</param>
        /// <param name="reverseConstantsTable">The reverse constants table.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="stringFormatters">The string formatters.</param>
        /// <param name="alternateNames">The alternate names.</param>
        internal static void GenerateNamedNumericSymbol(
            [NotNull] IDictionary<string, ConstantNodeBase> constantsTable,
            [NotNull] IDictionary<string, string> reverseConstantsTable,
            string name,
            double value,
            [NotNull] List<IStringFormatter> stringFormatters,
            params string[] alternateNames)
        {
            Requires.NotNullOrWhiteSpace(
                name,
                nameof(name));
            Requires.NotNull(
                constantsTable,
                nameof(constantsTable));
            Requires.NotNull(
                reverseConstantsTable,
                nameof(reverseConstantsTable));

            if (reverseConstantsTable.TryGetValue(
                name,
                out _))
            {
                return;
            }

            constantsTable.Add(
                name,
                new NumericNode(stringFormatters, value));
            reverseConstantsTable.Add(
                value.ToString(CultureInfo.CurrentCulture),
                name);

            foreach (var alternateName in alternateNames)
            {
                reverseConstantsTable.Add(
                    alternateName,
                    name);
            }
        }

        /// <summary>
        /// Checks the constant to see if there isn't one already, then tries to guess what type it is, finally adding it to
        /// the constants table if one suitable type is found.
        /// </summary>
        /// <param name="constantsTable">The constants table.</param>
        /// <param name="reverseConstantsTable">The reverse constants table.</param>
        /// <param name="interpreters">The constant interpreters.</param>
        /// <param name="originalExpression">The original expression.</param>
        /// <param name="content">The content.</param>
        /// <param name="stringFormatters">The string formatters.</param>
        /// <returns>
        /// The name of the new constant, or <see langword="null" /> (<see langword="Nothing" /> in Visual Basic) if a
        /// suitable type is not found.
        /// </returns>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance",
            "HAA0401:Possible allocation of reference type enumerator",
            Justification = "We're cool with this.")]
        internal static string CheckAndAdd(
            [NotNull] IDictionary<string, ConstantNodeBase> constantsTable,
            [NotNull] IDictionary<string, string> reverseConstantsTable,
            [NotNull] LevelDictionary<Type, IConstantInterpreter> interpreters,
            [NotNull] string originalExpression,
            [CanBeNull] string content,
            [NotNull] List<IStringFormatter> stringFormatters)
        {
            // Contract validation
            Requires.NotNullOrWhiteSpace(
                originalExpression,
                nameof(originalExpression));
            Requires.NotNull(
                constantsTable,
                nameof(constantsTable));
            Requires.NotNull(
                reverseConstantsTable,
                nameof(reverseConstantsTable));
            Requires.NotNull(
                interpreters,
                nameof(interpreters));

            // No content
            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }

            // Constant has already been evaluated, let's skip
            if (reverseConstantsTable.TryGetValue(
                content,
                out var key))
            {
                return key;
            }

            ConstantNodeBase node = null;

            // Go through each interpreter
            foreach (var interpreter in interpreters.KeysByLevel.SelectMany(p => p.Value))
            {
                var (success, result) = interpreters[interpreter].EvaluateIsConstant(content);
                if (success)
                {
                    node = CreateConstant(result, stringFormatters);
                    break;
                }
            }

            // Standard formatters
            if (node == null)
            {
                if (ParsingFormatter.ParseNumeric(
                    content,
                    out object n))
                {
                    if (n is double d)
                    {
                        node = new NumericNode(
                            stringFormatters,
                            d);
                    }
                    else if (n is long i)
                    {
                        node = new IntegerNode(
                            stringFormatters,
                            i);
                    }
                }
                else if (ParsingFormatter.ParseByteArray(
                    content,
                    out byte[] ba))
                {
                    node = new ByteArrayNode(stringFormatters, ba);
                }
                else if (bool.TryParse(
                    content,
                    out var b))
                {
                    node = new BoolNode(stringFormatters, b);
                }
            }

            // Node not recognized
            if (node == null)
            {
                return null;
            }

            // Get the constant a new name
            int tempIndex = 0;
            string name = GenerateName(
                ref tempIndex,
                constantsTable,
                originalExpression.AsSpan());

            // Add constant data to tables
            constantsTable.Add(
                name,
                node);
            reverseConstantsTable.Add(
                content,
                name);

            // Return
            return name;
        }

        internal static ConstantNodeBase CreateConstant(
            object value,
            [NotNull] List<IStringFormatter> stringFormatters)
        {
            Requires.NotNull(
                stringFormatters,
                nameof(stringFormatters));
            Requires.NotNull(
                value,
                nameof(value));

            return value switch
            {
                long l => new IntegerNode(
                    stringFormatters,
                    l),
                double d => new NumericNode(
                    stringFormatters,
                    d),
                bool b => new BoolNode(
                    stringFormatters,
                    b),
                byte[] ba => new ByteArrayNode(
                    stringFormatters,
                    ba),
                string s => new StringNode(
                    stringFormatters,
                    s),
                _ => throw new MathematicsEngineException()
            };
        }

        internal static string ExtractConstants(
            [NotNull] IDictionary<string, ConstantNodeBase> constantsTable,
            [NotNull] IDictionary<string, string> reverseConstantsTable,
            [NotNull] string originalExpression,
            [NotNull] LevelDictionary<Type, IConstantsExtractor> constantExtractors,
            [NotNull] MathDefinition mathDefinition,
            [NotNull] List<IStringFormatter> stringFormatters,
            CancellationToken cancellationToken)
        {
            Requires.NotNullOrWhiteSpace(
                originalExpression,
                nameof(originalExpression));
            Requires.NotNull(
                constantsTable,
                nameof(constantsTable));
            Requires.NotNull(
                reverseConstantsTable,
                nameof(reverseConstantsTable));
            Requires.NotNull(
                constantExtractors,
                nameof(constantExtractors));
            Requires.NotNull(
                stringFormatters,
                nameof(stringFormatters));

            // We create a copy of our expression first
            var expressionChars = new char[originalExpression.Length * 2];
            var espan = new Span<char>(expressionChars);
            var expressionSpan = originalExpression.AsSpan();

            expressionSpan.CopyTo(espan);

            // We get and order our extractors
            var extractors = constantExtractors.KeysByLevel.OrderBy(p => p.Key)
                .SelectMany(p => p.Value)
                .ToArray()
                .Select(p => constantExtractors[p])
                .ToArray();

            // The constant name index
            int constantIndex = 0;

            // The span indexes
            int currentIndex = 0, currentFinalIndex = 0;
            for (int i = 0; i < extractors.Length; i++)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    // Cancellation time
                    return null;
                }

                // We call the constant extractor
                var ce = extractors[i];
                var (success, value, index, length) = ce.ExtractConstant(
                    expressionSpan.Slice(currentIndex),
                    mathDefinition);

                if (index < 0)
                {
                    // Constant extractor did not find anything
                    expressionSpan = espan.Slice(
                            0,
                            currentFinalIndex + (expressionSpan.Length - currentIndex))
                        .ToString()
                        .AsSpan();
                    currentIndex = 0;
                    currentFinalIndex = 0;
                    continue;
                }

                if (!success)
                {
                    // Constant extractor found what it thought might be a constant, but wasn't
                    currentIndex += index + 1;
                    currentFinalIndex += index + 1;
                    i--;
                    continue;
                }

                // We have a successfully-extracted constant
                currentIndex += index;
                var name = AddExtractedValue(
                    constantsTable,
                    reverseConstantsTable,
                    expressionSpan,
                    expressionSpan.Slice(
                            currentIndex,
                            length)
                        .ToString(),
                    value,
                    stringFormatters,
                    ref constantIndex);

                // Let's set the value in the span
                currentFinalIndex += index;
                name.AsSpan()
                    .CopyTo(
                        espan.Slice(
                            currentFinalIndex,
                            name.Length));

                // Let's increase the indexes
                currentFinalIndex += name.Length;
                currentIndex += length;

                // Let's set the rest of the string
                expressionSpan.Slice(currentIndex).CopyTo(espan.Slice(currentFinalIndex));

                // Returning to the same extractor until there ain't no more matches
                i--;
                continue;
            }

            return espan.Slice(
                    0,
                    currentFinalIndex + (expressionSpan.Length - currentIndex))
                .ToString();
        }

        private static string AddExtractedValue(
            [NotNull] IDictionary<string, ConstantNodeBase> constantsTable,
            [NotNull] IDictionary<string, string> reverseConstantsTable,
            ReadOnlySpan<char> originalExpression,
            [NotNull] string content,
            [NotNull] object value,
            [NotNull] List<IStringFormatter> stringFormatters,
            ref int index)
        {
            if (reverseConstantsTable.TryGetValue(
                content,
                out var key))
            {
                // We already have a constant like this
                return key;
            }

            // Create the constant
            var node = CreateConstant(
                value,
                stringFormatters);

            // Get the constant a new name
            string name = GenerateName(
                ref index,
                constantsTable,
                originalExpression);

            // Add constant data to tables
            constantsTable.Add(
                name,
                node);
            reverseConstantsTable.Add(
                content,
                name);

            // Return
            return name;
        }

        private static string GenerateName(
            ref int index,
            IDictionary<string, ConstantNodeBase> constantsTable,
            ReadOnlySpan<char> originalExpression)
        {
            var nameChars = "C000000000".ToCharArray();
            Span<char> chr = nameChars;
            ReadOnlySpan<char> cc;

            do
            {
                index++;
                ReadOnlySpan<char> nameSpan = index.ToString(CultureInfo.InvariantCulture).AsSpan();
                nameSpan.CopyTo(chr.Slice(1));
                cc = chr.Slice(
                    0,
                    1 + nameSpan.Length);
            }
            while (originalExpression.Contains(cc, StringComparison.OrdinalIgnoreCase) || constantsTable.Keys.Contains(cc.ToString()));

            return cc.ToString();
        }
    }
}