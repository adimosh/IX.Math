// <copyright file="WorkingExpressionSet.ConstantsGeneration.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using IX.Math.Exceptions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using IX.StandardExtensions.Contracts;
using IX.StandardExtensions.Globalization;
using IX.System.Collections.Generic;
using JetBrains.Annotations;

namespace IX.Math.WorkingSet
{
    internal partial class WorkingExpressionSet
    {
        private const NumberStyles IntegerNumberStyle = NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands |
                                                        NumberStyles.AllowExponent | NumberStyles.AllowExponent;

        private const NumberStyles FloatNumberStyle = NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands |
                                                      NumberStyles.AllowExponent | NumberStyles.AllowExponent |
                                                      NumberStyles.AllowDecimalPoint;

        private const NumberStyles HexNumberStyle = NumberStyles.AllowHexSpecifier;

        private static readonly Regex BitRepresentationRegex = new Regex("^[01]{8}$");

        internal static bool ParseNumeric(
            string expression,
            out object result)
        {
            Requires.NotNull(
                expression,
                nameof(expression));

            if (!expression.StartsWith(
                    "0x",
                    StringComparison.CurrentCultureIgnoreCase) && !expression.StartsWith(
                    "&h",
                    StringComparison.CurrentCultureIgnoreCase))
            {
                return ParseSpecific(
                    expression,
                    out result);
            }

            if (expression.Length > 2)
            {
                return ParseHexSpecific(
                    expression.Substring(2),
                    out result);
            }

            result = null;
            return false;

            bool ParseHexSpecific(
                string hexExpression,
                out object hexResult)
            {
                if (long.TryParse(
                    hexExpression,
                    HexNumberStyle,
                    CultureInfo.CurrentCulture,
                    out var intVal))
                {
                    hexResult = intVal;
                    return true;
                }

                hexResult = null;
                return false;
            }

            bool ParseSpecific(
                string specificExpression,
                out object specificResult)
            {
                Requires.NotNull(
                    specificExpression,
                    nameof(specificExpression));

                IFormatProvider formatProvider = CultureInfo.CurrentCulture;

                if (long.TryParse(
                    specificExpression,
                    IntegerNumberStyle,
                    formatProvider,
                    out var intVal))
                {
                    specificResult = intVal;
                    return true;
                }

                if (double.TryParse(
                    specificExpression,
                    FloatNumberStyle,
                    formatProvider,
                    out var doubleVal))
                {
                    specificResult = doubleVal;
                    return true;
                }

                specificResult = null;
                return false;
            }
        }

        internal static bool ParseBinary(
            string expression,
            out byte[] result)
        {
            Requires.NotNull(
                expression,
                nameof(expression));

            if (expression.CurrentCultureStartsWithInsensitive("0b"))
            {
                if (expression.Length > 2)
                {
                    return ParseBinaryLocal(
                        expression.Substring(2),
                        out result);
                }

                result = null;
                return false;
            }

            result = null;
            return false;

            static bool ParseBinaryLocal(
                string binaryExpression,
                out byte[] binaryResult)
            {
                Requires.NotNull(
                    binaryExpression,
                    nameof(binaryExpression));

                binaryExpression = binaryExpression.Replace(
                    "_",
                    string.Empty);
                var stringLength = binaryExpression.Length;
                var byteLength = stringLength / 8;
                if (byteLength < (double)stringLength / 8)
                {
                    byteLength++;
                }

                stringLength = byteLength * 8;
                if (binaryExpression.Length < stringLength)
                {
                    binaryExpression = binaryExpression.PadLeft(
                        stringLength,
                        '0');
                }

                var bytes = new byte[byteLength];

                for (var i = byteLength - 1; i >= 0; i -= 1)
                {
                    var startingIndex = stringLength - (byteLength - i) * 8;

                    var currentByteExpression = binaryExpression.Substring(
                        startingIndex,
                        8);

                    if (!BitRepresentationRegex.IsMatch(currentByteExpression))
                    {
                        binaryResult = null;
                        return false;
                    }

                    bytes[i] = Convert.ToByte(
                        currentByteExpression,
                        2);
                }

                Array.Reverse(bytes);
                binaryResult = bytes;

                return true;
            }
        }

        internal static bool TryInterpretStringValue(
            string content,
            out object value)
        {
            // Go through each interpreter
            var interpreters =
                (LevelDictionary<Type, IConstantInterpreter>)MathematicPortfolio.CurrentContext.Value
                    .ConstantInterpreters;
            foreach (var interpreter in interpreters.KeysByLevel.SelectMany(p => p.Value))
            {
                var (success, result) = interpreters[interpreter]
                    .EvaluateIsConstant(
                        content);
                if (success)
                {
                    value = result;
                    return true;
                }
            }

            // Standard formatters
            if (ParseNumeric(
                content,
                out object n))
            {
                value = n;
                return true;
            }

            if (ParseBinary(
                content,
                out byte[] ba))
            {
                value = ba;
                return true;
            }

            if (bool.TryParse(
                content,
                out var b))
            {
                value = b;
                return true;
            }

            value = default;
            return false;
        }

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
                new NumericNode(value));
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
            var originalExpression = this.expression;

            // We create a copy of our expression first
            var expressionChars = new char[originalExpression.Length * 2];
            var espan = new Span<char>(expressionChars);
            var expressionSpan = originalExpression.AsSpan();

            expressionSpan.CopyTo(espan);

            // We get and order our extractors
            var extractors = this.extractors.KeysByLevel.OrderBy(p => p.Key)
                .SelectMany(p => p.Value)
                .ToArray()
                .Select(p => this.extractors[p])
                .ToArray();

            // The constant name index
            int constantIndex = 0;

            // The span indexes
            int currentIndex = 0, currentFinalIndex = 0;
            for (int i = 0; i < extractors.Length; i++)
            {
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
            }

            return espan.Slice(
                    0,
                    currentFinalIndex + (expressionSpan.Length - currentIndex))
                .ToString();
        }

        /// <summary>
        /// Checks the constant to see if there isn't one already, then tries to guess what type it is, finally adding it to
        /// the constants table if one suitable type is found.
        /// </summary>
        /// <param name="content">The content.</param>
        /// <returns>
        /// The name of the new constant, or <see langword="null" /> (<see langword="Nothing" /> in Visual Basic) if a
        /// suitable type is not found.
        /// </returns>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance",
            "HAA0401:Possible allocation of reference type enumerator",
            Justification = "We're cool with this.")]
        private string CheckAndAdd(
            [CanBeNull] string content)
        {
            // No content
            if (string.IsNullOrWhiteSpace(content))
            {
                return null;
            }

            // Constant has already been evaluated, let's skip
            if (this.reverseConstantsTable.TryGetValue(
                content,
                out var key))
            {
                return key;
            }

            // Let's interpret this
            if (!TryInterpretStringValue(
                content,
                out var value))
            {
                // Could not interpret
                return null;
            }

            // Get the constant a new name
            int tempIndex = 0;
            string name = this.GenerateName(ref tempIndex);

            // Add constant data to tables
            this.constantsTable.Add(
                name,
                this.CreateConstant(value, content));
            this.reverseConstantsTable.Add(
                content,
                name);

            // Return
            return name;
        }

        private string AddExtractedValue(
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
            var node = this.CreateConstant(value, content);

            // Get the constant a new name
            string name = this.GenerateName(ref index);

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

        private ConstantNodeBase CreateConstant(object value, string originalStringValue)
        {
            Requires.NotNull(
                value,
                nameof(value));

            return value switch
            {
                long l => new IntegerNode(
                    l)
                {
                    OriginalStringValue = originalStringValue
                },
                double d => new NumericNode(
                    d)
                {
                    OriginalStringValue = originalStringValue
                },
                bool b => new BoolNode(
                    b)
                {
                    OriginalStringValue = originalStringValue
                },
                byte[] ba => new BinaryNode(
                    ba)
                {
                    OriginalStringValue = originalStringValue
                },
                string s => new StringNode(
                    s)
                {
                    OriginalStringValue = originalStringValue
                },
                _ => throw new MathematicsEngineException()
            };
        }

        private string GenerateName(
            ref int index)
        {
            var nameChars = "C000000000".ToCharArray();
            Span<char> chr = nameChars;
            ReadOnlySpan<char> cc;

            do
            {
                index++;
                ReadOnlySpan<char> nameSpan = index.ToString(CultureInfo.InvariantCulture)
                    .AsSpan();
                nameSpan.CopyTo(chr.Slice(1));
                cc = chr.Slice(
                    0,
                    1 + nameSpan.Length);
            }
            while (this.expression.AsSpan().Contains(
                       cc,
                       StringComparison.OrdinalIgnoreCase) ||
                   this.constantsTable.Keys.Contains(cc.ToString()));

            return cc.ToString();
        }
    }
}