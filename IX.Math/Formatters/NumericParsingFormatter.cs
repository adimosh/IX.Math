// <copyright file="NumericParsingFormatter.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Globalization;

namespace IX.Math.Formatters
{
    internal static class NumericParsingFormatter
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
            else
            {
                return ParseSpecific(expression, out result);
            }
        }

        private static bool ParseSpecific(string expression, out object result)
        {
            IFormatProvider formatProvider = CultureInfo.CurrentCulture;

            if (long.TryParse(expression, IntegerNumberStyle, formatProvider, out long intVal))
            {
                result = intVal;
                return true;
            }
            else if (double.TryParse(expression, FloatNumberStyle, formatProvider, out double doubleVal))
            {
                result = doubleVal;
                return true;
            }

            result = null;
            return false;
        }

        private static bool ParseHexSpecific(string expression, out object result)
        {
            if (long.TryParse(expression, HexNumberStyle, CultureInfo.CurrentCulture, out long intVal))
            {
                result = intVal;
                return true;
            }

            result = null;
            return false;
        }
    }
}