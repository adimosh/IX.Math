// <copyright file="NumericTypeParsingAide.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Globalization;

namespace IX.Math.SimplificationAide
{
    internal static class NumericTypeParsingAide
    {
        private const NumberStyles IntegerNumberStyle =
            NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands | NumberStyles.AllowExponent | NumberStyles.AllowExponent;

        private const NumberStyles UnsignedIntegerNumberStyle =
            NumberStyles.AllowThousands | NumberStyles.AllowExponent | NumberStyles.AllowExponent;

        private const NumberStyles FloatNumberStyle =
            NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands | NumberStyles.AllowExponent | NumberStyles.AllowExponent | NumberStyles.AllowDecimalPoint;

        private const NumberStyles HexNumberStyle = NumberStyles.AllowHexSpecifier;

        internal static bool Parse(string expression, ref Type numericType, out object result)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (expression.StartsWith("0x", StringComparison.CurrentCultureIgnoreCase) || expression.StartsWith("&h", StringComparison.CurrentCultureIgnoreCase))
            {
                if (expression.Length > 2)
                {
                    return ParseHexSpecific(expression.Substring(2), ref numericType, out result);
                }
                else
                {
                    result = null;
                    return false;
                }
            }
            else
            {
                return ParseSpecific(expression, ref numericType, out result);
            }
        }

        private static bool ParseSpecific(string expression, ref Type numericType, out object result)
        {
            IFormatProvider formatProvider = CultureInfo.CurrentCulture;
            Type tempNumericType = numericType;

            if (tempNumericType == typeof(int))
            {
                if (int.TryParse(expression, IntegerNumberStyle, formatProvider, out int intVal))
                {
                    result = intVal;
                    return true;
                }
                else
                {
                    tempNumericType = typeof(long);
                }
            }

            if (tempNumericType == typeof(long))
            {
                if (long.TryParse(expression, IntegerNumberStyle, formatProvider, out long intVal))
                {
                    numericType = tempNumericType;
                    result = intVal;
                    return true;
                }
                else
                {
                    tempNumericType = typeof(float);
                }
            }

            if (tempNumericType == typeof(float))
            {
                if (float.TryParse(expression, FloatNumberStyle, formatProvider, out float floatVal))
                {
                    numericType = tempNumericType;
                    result = floatVal;
                    return true;
                }
                else
                {
                    tempNumericType = typeof(double);
                }
            }

            if (tempNumericType == typeof(double))
            {
                if (double.TryParse(expression, FloatNumberStyle, formatProvider, out double doubleVal))
                {
                    numericType = tempNumericType;
                    result = doubleVal;
                    return true;
                }
            }

            result = null;
            return false;
        }

        private static bool ParseHexSpecific(string expression, ref Type numericType, out object result)
        {
            IFormatProvider formatProvider = CultureInfo.CurrentCulture;
            Type tempNumericType = numericType;

            if (tempNumericType == typeof(int))
            {
                if (int.TryParse(expression, HexNumberStyle, formatProvider, out int intVal))
                {
                    result = intVal;
                    return true;
                }
                else
                {
                    tempNumericType = typeof(long);
                }
            }

            if (tempNumericType == typeof(long))
            {
                if (long.TryParse(expression, HexNumberStyle, formatProvider, out long intVal))
                {
                    numericType = tempNumericType;
                    result = intVal;
                    return true;
                }
            }

            result = null;
            return false;
        }
    }
}