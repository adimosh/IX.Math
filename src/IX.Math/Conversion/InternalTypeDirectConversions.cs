// <copyright file="InternalTypeDirectConversions.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using IX.Math.WorkingSet;

namespace IX.Math.Conversion
{
    internal static class InternalTypeDirectConversions
    {
        #region To integer
        internal static bool ToInteger(double numeric, out long integer)
        {
            if (numeric <= long.MaxValue && numeric >= long.MinValue)
            {
                var numericAbs = global::System.Math.Abs(numeric);
                if (numericAbs - global::System.Math.Floor(numericAbs) < double.Epsilon)
                {
                    integer = Convert.ToInt64(numeric);
                    return true;
                }
            }

            integer = default;
            return false;
        }

        internal static bool ToInteger(byte[] byteArray, out long integer)
        {
            if (byteArray.Length <= 8)
            {
                if (byteArray.Length < 8)
                {
                    byte[] bytes = new byte[8];
                    Array.Copy(byteArray, bytes, byteArray.Length);
                    byteArray = bytes;
                }

                integer = BitConverter.ToInt64(
                    byteArray,
                    0);

                return true;
            }

            integer = default;
            return false;
        }

        internal static long ParseInteger(string input)
        {
            if (!WorkingExpressionSet.TryInterpretStringValue(
                input,
                out var result))
            {
                throw new InvalidCastException();
            }

            return result switch
            {
                long l => l,
                double d => NumericConversion(d),
                byte[] ba => ByteArrayConversion(ba),
                _ => throw new InvalidCastException()
            };

            static long NumericConversion(double d)
            {
                if (!ToInteger(
                    d,
                    out var i))
                {
                    throw new InvalidCastException();
                }

                return i;
            }

            static long ByteArrayConversion(byte[] ba)
            {
                if (!ToInteger(
                    ba,
                    out var i))
                {
                    throw new InvalidCastException();
                }

                return i;
            }
        }
        #endregion

        #region To numeric
        internal static bool ToNumeric(byte[] byteArray, out double numeric)
        {
            if (byteArray.Length <= 8)
            {
                if (byteArray.Length < 8)
                {
                    byte[] bytes = new byte[8];
                    Array.Copy(byteArray, bytes, byteArray.Length);
                    byteArray = bytes;
                }

                numeric = BitConverter.ToDouble(
                    byteArray,
                    0);

                return true;
            }

            numeric = default;
            return false;
        }

        internal static double ParseNumeric(string input)
        {
            if (!WorkingExpressionSet.TryInterpretStringValue(
                input,
                out var result))
            {
                throw new InvalidCastException();
            }

            return result switch
            {
                long l => Convert.ToDouble(l),
                double d => d,
                byte[] ba => ByteArrayConversion(ba),
                _ => throw new InvalidCastException()
            };

            static double ByteArrayConversion(byte[] ba)
            {
                if (!ToNumeric(
                    ba,
                    out var i))
                {
                    throw new InvalidCastException();
                }

                return i;
            }
        }
        #endregion

        #region To byte array
        internal static byte[] ParseByteArray(string input)
        {
            if (!WorkingExpressionSet.TryInterpretStringValue(
                input,
                out var result))
            {
                throw new InvalidCastException();
            }

            return result switch
            {
                long l => BitConverter.GetBytes(l),
                double d => BitConverter.GetBytes(d),
                byte[] ba => ba,
                bool b => BitConverter.GetBytes(b),
                _ => throw new InvalidCastException()
            };
        }
        #endregion

        #region To boolean
        internal static bool ParseBoolean(string input)
        {
            if (!WorkingExpressionSet.TryInterpretStringValue(
                input,
                out var result))
            {
                throw new InvalidCastException();
            }

            return result switch
            {
                bool b => b,
                _ => throw new InvalidCastException()
            };
        }
        #endregion
    }
}