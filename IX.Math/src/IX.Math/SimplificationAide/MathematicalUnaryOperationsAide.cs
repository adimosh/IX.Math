namespace IX.Math.SimplificationAide
{
    /// <summary>
    /// Named method aide for mathematical binary operations.
    /// </summary>
    public static class MathematicalUnaryOperationsAide
    {
        /// <summary>
        /// Negates the value (bitwise).
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The negated value.</returns>
        public static byte Negate(byte value)
        {
            return (byte)~value;
        }

        /// <summary>
        /// Negates the value (bitwise).
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The negated value.</returns>
        public static sbyte Negate(sbyte value)
        {
            return (sbyte)~value;
        }

        /// <summary>
        /// Negates the value (bitwise).
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The negated value.</returns>
        public static char Negate(char value)
        {
            return (char)~value;
        }

        /// <summary>
        /// Negates the value (bitwise).
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The negated value.</returns>
        public static short Negate(short value)
        {
            return (short)~value;
        }

        /// <summary>
        /// Negates the value (bitwise).
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The negated value.</returns>
        public static ushort Negate(ushort value)
        {
            return (ushort)~value;
        }

        /// <summary>
        /// Negates the value (bitwise).
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The negated value.</returns>
        public static int Negate(int value)
        {
            return ~value;
        }

        /// <summary>
        /// Negates the value (bitwise).
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The negated value.</returns>
        public static uint Negate(uint value)
        {
            return ~value;
        }

        /// <summary>
        /// Negates the value (bitwise).
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The negated value.</returns>
        public static long Negate(long value)
        {
            return ~value;
        }

        /// <summary>
        /// Negates the value (bitwise).
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The negated value.</returns>
        public static ulong Negate(ulong value)
        {
            return ~value;
        }

        /// <summary>
        /// Negates the value (bitwise).
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The negated value.</returns>
        public static float Negate(float value)
        {
            return -value;
        }

        /// <summary>
        /// Negates the value (bitwise).
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The negated value.</returns>
        public static double Negate(double value)
        {
            return -value;
        }

        /// <summary>
        /// Negates the value (bitwise).
        /// </summary>
        /// <param name="value">The value to negate.</param>
        /// <returns>The negated value.</returns>
        public static bool Negate(bool value)
        {
            return !value;
        }
    }
}