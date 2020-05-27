// <copyright file="ConstantsGenerator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using IX.Math.Extensibility;
using IX.Math.Formatters;
using IX.Math.Nodes;
using IX.Math.Nodes.Constants;
using IX.StandardExtensions.Contracts;
using IX.StandardExtensions.Globalization;
using IX.System.Collections.Generic;
using JetBrains.Annotations;

namespace IX.Math.Generators
{
    /// <summary>
    ///     A generator for constant values and their like.
    /// </summary>
    public static class ConstantsGenerator
    {
        /// <summary>
        /// Generates a string constant.
        /// </summary>
        /// <param name="constantsTable">The constants table.</param>
        /// <param name="reverseConstantsTable">The reverse constants table.</param>
        /// <param name="originalExpression">The original expression.</param>
        /// <param name="stringIndicator">The string indicator.</param>
        /// <param name="content">The content.</param>
        /// <returns>
        /// The name of the new constant.
        /// </returns>
        public static string GenerateStringConstant(
            [NotNull] IDictionary<string, ConstantNodeBase> constantsTable,
            [NotNull] IDictionary<string, string> reverseConstantsTable,
            [NotNull] string originalExpression,
            [NotNull] string stringIndicator,
            [NotNull] string content)
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
            Requires.NotNullOrWhiteSpace(
                stringIndicator,
                nameof(stringIndicator));
            Requires.NotNullOrWhiteSpace(
                content,
                nameof(content));

            if (reverseConstantsTable.TryGetValue(
                content,
                out var key))
            {
                return key;
            }

            var stringIndicatorLength = stringIndicator.Length;

            var name = GenerateName(
                constantsTable.Keys,
                originalExpression);
            constantsTable.Add(
                name,
                new StringNode(
                    content.Substring(
                        stringIndicatorLength,
                        content.Length - stringIndicatorLength * 2)));
            reverseConstantsTable.Add(
                content,
                name);
            return name;
        }

        /// <summary>
        ///     Generates a numeric constant out of a string.
        /// </summary>
        /// <param name="constantsTable">The constants table.</param>
        /// <param name="reverseConstantsTable">The reverse constants table.</param>
        /// <param name="originalExpression">The original expression.</param>
        /// <param name="content">The content.</param>
        /// <returns>The name of the new constant.</returns>
        public static string GenerateNumericConstant(
            [NotNull] IDictionary<string, ConstantNodeBase> constantsTable,
            [NotNull] IDictionary<string, string> reverseConstantsTable,
            [NotNull] string originalExpression,
            [NotNull] string content)
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
            Requires.NotNullOrWhiteSpace(
                content,
                nameof(content));

            if (reverseConstantsTable.TryGetValue(
                content,
                out var key))
            {
                return key;
            }

            if (!double.TryParse(
                content,
                out var result))
            {
                return null;
            }

            var name = GenerateName(
                constantsTable.Keys,
                originalExpression);
            constantsTable.Add(
                name,
                new NumericNode(result));
            reverseConstantsTable.Add(
                content,
                name);
            return name;
        }

        /// <summary>
        ///     Generates a named numeric symbol.
        /// </summary>
        /// <param name="constantsTable">The constants table.</param>
        /// <param name="reverseConstantsTable">The reverse constants table.</param>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="alternateNames">The alternate names.</param>
        public static void GenerateNamedNumericSymbol(
            [NotNull] IDictionary<string, ConstantNodeBase> constantsTable,
            [NotNull] IDictionary<string, string> reverseConstantsTable,
            string name,
            double value,
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
                new NumericNode(value));
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
            [CanBeNull] string content)
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
                    node = result;
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
                    node = new NumericNode(n);
                }
                else if (ParsingFormatter.ParseByteArray(
                    content,
                    out byte[] ba))
                {
                    node = new ByteArrayNode(ba);
                }
                else if (bool.TryParse(
                    content,
                    out var b))
                {
                    node = new BoolNode(b);
                }
            }

            // Node not recognized
            if (node == null)
            {
                return null;
            }

            // Get the constant a new name
            string name = GenerateName(
                constantsTable.Keys,
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
            [NotNull] IEnumerable<string> keys,
            string originalExpression)
        {
            Requires.NotNull(
                keys,
                nameof(keys));

            var index = int.Parse(
                keys.Where(p => p.InvariantCultureStartsWith("Const") && p.Length > 5).LastOrDefault()?.Substring(5) ?? "0", CultureInfo.CurrentCulture);

            do
            {
                index++;
            }
            while (originalExpression.InvariantCultureContains($"Const{index.ToString(CultureInfo.InvariantCulture)}"));

            return $"Const{index.ToString(CultureInfo.InvariantCulture)}";
        }
    }
}