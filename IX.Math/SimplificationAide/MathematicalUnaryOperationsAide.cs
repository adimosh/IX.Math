// <copyright file="MathematicalUnaryOperationsAide.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

namespace IX.Math.SimplificationAide
{
    /// <summary>
    /// Named method aide for mathematical binary operations.
    /// </summary>
    public static class MathematicalUnaryOperationsAide
    {
        /// <summary>
        /// Negates the value.
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The negated value.</returns>
        public static int Negate(int value)
        {
            return -value;
        }

        /// <summary>
        /// Negates the value.
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The negated value.</returns>
        public static long Negate(long value)
        {
            return -value;
        }

        /// <summary>
        /// Negates the value.
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The negated value.</returns>
        public static float Negate(float value)
        {
            return -value;
        }

        /// <summary>
        /// Negates the value.
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The negated value.</returns>
        public static double Negate(double value)
        {
            return -value;
        }

        /// <summary>
        /// Negates the value.
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The negated value.</returns>
        public static bool Negate(bool value)
        {
            return !value;
        }

        /// <summary>
        /// Negates the value (bitwise).
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The negated value.</returns>
        public static int Not(int value)
        {
            return ~value;
        }

        /// <summary>
        /// Negates the value (bitwise).
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The negated value.</returns>
        public static long Not(long value)
        {
            return ~value;
        }

        /// <summary>
        /// Negates the value (bitwise).
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The negated value.</returns>
        public static bool Not(bool value)
        {
            return !value;
        }
    }
}