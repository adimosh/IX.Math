// <copyright file="ParsingFormatter.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Text.RegularExpressions;
using IX.StandardExtensions.Globalization;

namespace IX.Math.Formatters
{
    internal static class ParsingFormatter
    {
        private const NumberStyles IntegerNumberStyle = NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands |
                                                        NumberStyles.AllowExponent | NumberStyles.AllowExponent;

        private const NumberStyles FloatNumberStyle = NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands |
                                                      NumberStyles.AllowExponent | NumberStyles.AllowExponent |
                                                      NumberStyles.AllowDecimalPoint;

        private const NumberStyles HexNumberStyle = NumberStyles.AllowHexSpecifier;

        private static readonly Regex BitRepresentationRegex = new("^[01]{8}$");

        internal static bool ParseNumeric(
            string expression,
#if FRAMEWORK_ADVANCED
            [NotNullWhen(true)]
#endif
            out object? result)
        {
            var eSpan = expression.AsSpan();

            bool success = false;
            long possibleLongOutput = default;
            if (eSpan.Length > 2)
            {
                if ((eSpan[0] == '0' && (eSpan[1] == 'x' || eSpan[1] == 'X')) ||
                    (eSpan[0] == '&' && (eSpan[1] == 'h' || eSpan[1] == 'H')))
                {
                    success = ParseHexSpecific(
                        eSpan.Slice(2),
                        out possibleLongOutput);
                }
            }

            if (success)
            {
                result = possibleLongOutput;

                return true;
            }

            return ParseSpecific(
                #if FRAMEWORK_ADVANCED
                in eSpan,
                #else
                expression,
                #endif
                out result);

            static bool ParseHexSpecific(
                ReadOnlySpan<char> hexExpression,
                out long hexResult)
            {
                if (long.TryParse(
                    #if FRAMEWORK_ADVANCED
                    hexExpression,
                    #else
                    hexExpression.ToString(),
                    #endif
                    HexNumberStyle,
                    CultureInfo.CurrentCulture,
                    out var intVal))
                {
                    hexResult = intVal;
                    return true;
                }

                hexResult = default;
                return false;
            }

            static bool ParseSpecific(
                #if FRAMEWORK_ADVANCED
                in ReadOnlySpan<char> specificExpression,
                #else
                string specificExpression,
                #endif
                out object? specificResult)
            {
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

        internal static bool ParseByteArray(
            string expression,
            out byte[] result)
        {
            if (expression.CurrentCultureStartsWithInsensitive("0b"))
            {
                if (expression.Length > 2)
                {
                    return ParseByteArray(
                        expression.Substring(2),
                        out result);
                }

                result = null;
                return false;
            }

            result = null;
            return false;

            bool ParseByteArray(
                string byteArrayExpression,
                out byte[] byteArrayResult)
            {
                byteArrayExpression = byteArrayExpression.Replace(
                    "_",
                    string.Empty);
                var stringLength = byteArrayExpression.Length;
                var byteLength = stringLength / 8;
                if (byteLength < (double)stringLength / 8)
                {
                    byteLength++;
                }

                stringLength = byteLength * 8;
                if (byteArrayExpression.Length < stringLength)
                {
                    byteArrayExpression = byteArrayExpression.PadLeft(
                        stringLength,
                        '0');
                }

                var bytes = new byte[byteLength];

                for (var i = byteLength - 1; i >= 0; i -= 1)
                {
                    var startingIndex = stringLength - (byteLength - i) * 8;

                    var currentByteExpression = byteArrayExpression.Substring(
                        startingIndex,
                        8);

                    if (!BitRepresentationRegex.IsMatch(currentByteExpression))
                    {
                        byteArrayResult = null;
                        return false;
                    }

                    bytes[i] = Convert.ToByte(
                        currentByteExpression,
                        2);
                }

                Array.Reverse(bytes);
                byteArrayResult = bytes;

                return true;
            }
        }
    }
}