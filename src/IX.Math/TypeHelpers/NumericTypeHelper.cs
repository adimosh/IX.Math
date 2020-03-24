// <copyright file="NumericTypeHelper.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace IX.Math.TypeHelpers
{
    /// <summary>
    /// Helper for numeric types.
    /// </summary>
    internal static class NumericTypeHelper
    {
        /// <summary>
        /// Extracts floating-point values from undefined numeric types.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The left and right operand, and whether or not the original operands were both integer.</returns>
        /// <exception cref="InvalidCastException">Either operand is neither a valid integer nor a valid floating-point value.</exception>
        internal static (double LeftOperand, double RightOperand, bool IsOriginalInteger) ExtractFloats(
            object left,
            object right) => left switch
            {
                double leftFloat => right switch
                {
                    double rightFloat => (leftFloat, rightFloat, false),
                    long rightInteger => (leftFloat, Convert.ToDouble(rightInteger), false),
                    _ => throw new InvalidCastException(),
                },
                long leftInteger => right switch
                {
                    double rightFloat => (Convert.ToDouble(leftInteger), rightFloat, false),
                    long rightInteger => (Convert.ToDouble(leftInteger), Convert.ToDouble(rightInteger), true),
                    _ => throw new InvalidCastException(),
                },
                _ => throw new InvalidCastException(),
            };

        /// <summary>
        /// Distills an integer value out of an undefined numeric value, if possible, otherwise returns that numeric value.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>A numeric value, possibly an integer if the original value allows it.</returns>
        /// <exception cref="InvalidCastException">The original value is not a correct numeric value.</exception>
        [SuppressMessage(
            "ReSharper",
            "CompareOfFloatsByEqualityOperator",
            Justification = "We can deal with this here, since we're essentially comparing a value with hopefully itself.")]
        [SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "This is essentially what we're aiming for in the end.")]
        internal static object DistillIntegerIfPossible(object value)
        {
            switch (value)
            {
                case long l:
                    return l;
                case double d:
                {
                    if (global::System.Math.Floor(d) != d || d > long.MaxValue || d < long.MinValue)
                    {
                        return d;
                    }

                    return Convert.ToInt64(value, CultureInfo.CurrentCulture);
                }

                default:
                    throw new InvalidCastException();
            }
        }

        /// <summary>
        /// Distills the lowest common type for two numeric values.
        /// </summary>
        /// <param name="left">The left value.</param>
        /// <param name="right">The right value.</param>
        /// <returns>The distilled left and right operand, and whether or not the operands are both integer.</returns>
        /// <exception cref="InvalidCastException">Either operand is neither a valid integer nor a valid floating-point value.</exception>
        [SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "This is essentially what we're aiming for in the end.")]
        internal static (object LeftOperand, object RightOperand, bool IsInteger) DistillLowestCommonType(
            object left,
            object right) => left switch
        {
            double leftFloat => right switch
            {
                // DO NOT remove these casts! If you do, some compiler logic will automatically default to returning
                // (double, double, bool) and will completely mess up the case where the return is long
                double rightFloat => ((object)leftFloat, (object)rightFloat, false),
                long rightInteger => ((object)leftFloat, (object)Convert.ToDouble(rightInteger), false),
                _ => throw new InvalidCastException(),
            },
            long leftInteger => right switch
            {
                // DO NOT remove these casts! If you do, some compiler logic will automatically default to returning
                // (double, double, bool) and will completely mess up the case where the return is long
                double rightFloat => ((object)Convert.ToDouble(leftInteger), (object)rightFloat, false),
                long rightInteger => ((object)leftInteger, (object)rightInteger, true),
                _ => throw new InvalidCastException(),
            },
            _ => throw new InvalidCastException(),
        };
    }
}