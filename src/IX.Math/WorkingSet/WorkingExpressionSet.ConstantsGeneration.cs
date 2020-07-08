// <copyright file="WorkingExpressionSet.ConstantsGeneration.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Globalization;
using System.Linq;
using IX.Math.Generators;
using IX.Math.Nodes.Constants;
using IX.StandardExtensions.Contracts;
using JetBrains.Annotations;

namespace IX.Math.WorkingSet
{
    internal partial class WorkingExpressionSet
    {
        /// <summary>
        /// Generates a named numeric symbol.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="alternateNames">The alternate names.</param>
        private void GenerateNamedNumericSymbol(
            string name,
            double value,
            params string[] alternateNames)
        {
            Requires.NotNullOrWhiteSpace(
                name,
                nameof(name));

            if (this.reverseConstantsTable.TryGetValue(
                name,
                out _))
            {
                return;
            }

            this.constantsTable.Add(
                name,
                new NumericNode(this.StringFormatters, value));
            this.reverseConstantsTable.Add(
                value.ToString(CultureInfo.CurrentCulture),
                name);

            foreach (var alternateName in alternateNames)
            {
                this.reverseConstantsTable.Add(
                    alternateName,
                    name);
            }
        }

        private string ExtractConstants()
        {
            var originalExpression = this.Expression;

            // We create a copy of our expression first
            var expressionChars = new char[originalExpression.Length * 2];
            var espan = new Span<char>(expressionChars);
            var expressionSpan = originalExpression.AsSpan();

            expressionSpan.CopyTo(espan);

            // We get and order our extractors
            var extractors = this.Extractors.KeysByLevel.OrderBy(p => p.Key)
                .SelectMany(p => p.Value)
                .ToArray()
                .Select(p => this.Extractors[p])
                .ToArray();

            // The constant name index
            int constantIndex = 0;

            // The span indexes
            int currentIndex = 0, currentFinalIndex = 0;
            for (int i = 0; i < extractors.Length; i++)
            {
                if (this.cancellationToken.IsCancellationRequested)
                {
                    // Cancellation time
                    return null;
                }

                // We call the constant extractor
                var ce = extractors[i];
                var (success, value, index, length) = ce.ExtractConstant(
                    expressionSpan.Slice(currentIndex),
                    this.definition);

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
                var name = this.AddExtractedValue(
                    expressionSpan,
                    expressionSpan.Slice(
                            currentIndex,
                            length)
                        .ToString(),
                    value,
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

        private string AddExtractedValue(
            ReadOnlySpan<char> originalExpression,
            [NotNull] string content,
            [NotNull] object value,
            ref int index)
        {
            if (this.reverseConstantsTable.TryGetValue(
                content,
                out var key))
            {
                // We already have a constant like this
                return key;
            }

            // Create the constant
            var node = ConstantsGenerator.CreateConstant(
                value,
                this.StringFormatters);

            // Get the constant a new name
            string name = ConstantsGenerator.GenerateName(
                ref index,
                this.constantsTable,
                originalExpression);

            // Add constant data to tables
            this.constantsTable.Add(
                name,
                node);
            this.reverseConstantsTable.Add(
                content,
                name);

            // Return
            return name;
        }
    }
}
