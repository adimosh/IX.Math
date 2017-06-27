// <copyright file="ParsingFormatter.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Globalization;

namespace IX.Math.Formatters
{
    internal static class ParsingFormatter
    {
        private const NumberStyles IntegerNumberStyle =
            NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands | NumberStyles.AllowExponent | NumberStyles.AllowExponent;

        private const NumberStyles UnsignedIntegerNumberStyle =
            NumberStyles.AllowThousands | NumberStyles.AllowExponent | NumberStyles.AllowExponent;

        private const NumberStyles FloatNumberStyle =
            NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands | NumberStyles.AllowExponent | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint;

        private const NumberStyles HexNumberStyle = NumberStyles.AllowHexSpecifier;

        internal static bool Parse(string expression, out object result)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (expression.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase) || expression.StartsWith("&h", StringComparison.CurrentCultureIgnoreCase))
            {
                if (expression.Length > 2)
                {
                    return ParseHexSpecific(expression.Substring(2), out result);
                }
                else
                {
                    result = null;
                    return false;
                }
            }
            else if (expression.StartsWith("0b", StringComparison.CurrentCultureIgnoreCase))
            {
                if (expression.Length > 2)
                {
                    return ParseByteArray(expression.Substring(2), out result);
                }
                else
                {
                    result = null;
                    return false;
                }
            }
            else
            {
                return ParseSpecific(expression, out result);
            }

            bool ParseHexSpecific(string hexExpression, out object hexResult)
            {
                if (long.TryParse(hexExpression, HexNumberStyle, CultureInfo.CurrentCulture, out long intVal))
                {
                    hexResult = intVal;
                    return true;
                }

                hexResult = null;
                return false;
            }

            bool ParseSpecific(string specificExpression, out object specificResult)
            {
                IFormatProvider formatProvider = CultureInfo.CurrentCulture;

                if (long.TryParse(specificExpression, IntegerNumberStyle, formatProvider, out long intVal))
                {
                    specificResult = intVal;
                    return true;
                }
                else if (double.TryParse(specificExpression, FloatNumberStyle, formatProvider, out double doubleVal))
                {
                    specificResult = doubleVal;
                    return true;
                }

                specificResult = null;
                return false;
            }

            bool ParseByteArray(string byteArrayExpression, out object byteArrayResult)
            {
                var stringLength = byteArrayExpression.Length;

                byte[] bytes = new byte[stringLength / 2];

                try
                {
                    for (var i = 0; i < stringLength; i += 2)
                    {
                        bytes[i / 2] = Convert.ToByte(byteArrayExpression.Substring(i, 2), 16);
                    }

                    byteArrayResult = bytes;

                    return true;
                }
                catch
                {
                    byteArrayResult = null;
                    return false;
                }
            }
        }
    }
}