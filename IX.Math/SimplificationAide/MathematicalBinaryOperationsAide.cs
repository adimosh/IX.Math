// <copyright file="MathematicalBinaryOperationsAide.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;

namespace IX.Math.SimplificationAide
{
    /// <summary>
    /// Named method aide for mathematical binary operations.
    /// </summary>
    public static class MathematicalBinaryOperationsAide
    {
        /// <summary>
        /// Adds two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static int Add(int left, int right)
        {
            return left + right;
        }

        /// <summary>
        /// Adds two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static long Add(long left, long right)
        {
            return left + right;
        }

        /// <summary>
        /// Adds two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static float Add(float left, float right)
        {
            return left + right;
        }

        /// <summary>
        /// Adds two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static double Add(double left, double right)
        {
            return left + right;
        }

        /// <summary>
        /// Adds two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static TimeSpan Add(TimeSpan left, TimeSpan right)
        {
            return left + right;
        }

        /// <summary>
        /// Subtracts two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static int Subtract(int left, int right)
        {
            return left - right;
        }

        /// <summary>
        /// Subtracts two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static long Subtract(long left, long right)
        {
            return left - right;
        }

        /// <summary>
        /// Subtracts two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static float Subtract(float left, float right)
        {
            return left - right;
        }

        /// <summary>
        /// Subtracts two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static double Subtract(double left, double right)
        {
            return left - right;
        }

        /// <summary>
        /// Subtracts two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static TimeSpan Subtract(TimeSpan left, TimeSpan right)
        {
            return left - right;
        }

        /// <summary>
        /// Multiplies two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static int Multiply(int left, int right)
        {
            return left * right;
        }

        /// <summary>
        /// Multiplies two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static long Multiply(long left, long right)
        {
            return left * right;
        }

        /// <summary>
        /// Multiplies two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static float Multiply(float left, float right)
        {
            return left * right;
        }

        /// <summary>
        /// Multiplies two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static double Multiply(double left, double right)
        {
            return left * right;
        }

        /// <summary>
        /// Divides two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static int Divide(int left, int right)
        {
            return left / right;
        }

        /// <summary>
        /// Divides two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static long Divide(long left, long right)
        {
            return left / right;
        }

        /// <summary>
        /// Divides two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static float Divide(float left, float right)
        {
            return left / right;
        }

        /// <summary>
        /// Divides two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static double Divide(double left, double right)
        {
            return left / right;
        }

        /// <summary>
        /// Shifts a number left by another number.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static int LeftShift(int left, int right)
        {
            return left << right;
        }

        /// <summary>
        /// Shifts a number left by another number.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static long LeftShift(long left, long right)
        {
            return left << (right > int.MaxValue ? int.MaxValue : (int)right);
        }

        /// <summary>
        /// Shifts a number right by another number.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static int RightShift(int left, int right)
        {
            return left >> right;
        }

        /// <summary>
        /// Shifts a number right by another number.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static long RightShift(long left, long right)
        {
            return left >> (right > int.MaxValue ? int.MaxValue : (int)right);
        }

        /// <summary>
        /// Bitwise masks two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool And(bool left, bool right)
        {
            return left & right;
        }

        /// <summary>
        /// Bitwise masks two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static int And(int left, int right)
        {
            return left & right;
        }

        /// <summary>
        /// Bitwise masks two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static long And(long left, long right)
        {
            return left & right;
        }

        /// <summary>
        /// Bitwise combines two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool Or(bool left, bool right)
        {
            return left | right;
        }

        /// <summary>
        /// Bitwise combines two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static int Or(int left, int right)
        {
            return left | right;
        }

        /// <summary>
        /// Bitwise combines two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static long Or(long left, long right)
        {
            return left | right;
        }

        /// <summary>
        /// Bitwise exclusively combines two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool Xor(bool left, bool right)
        {
            return left ^ right;
        }

        /// <summary>
        /// Bitwise exclusively combines two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static int Xor(int left, int right)
        {
            return left ^ right;
        }

        /// <summary>
        /// Bitwise exclusively combines two numbers.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static long Xor(long left, long right)
        {
            return left ^ right;
        }

        /// <summary>
        /// Determines whether or not two numbers are equal.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool Equal(bool left, bool right)
        {
            return left == right;
        }

        /// <summary>
        /// Determines whether or not two numbers are equal.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool Equal(int left, int right)
        {
            return left == right;
        }

        /// <summary>
        /// Determines whether or not two numbers are equal.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool Equal(long left, long right)
        {
            return left == right;
        }

        /// <summary>
        /// Determines whether or not two numbers are equal.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool Equal(float left, float right)
        {
            return left == right;
        }

        /// <summary>
        /// Determines whether or not two numbers are equal.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool Equal(double left, double right)
        {
            return left == right;
        }

        /// <summary>
        /// Determines whether or not two numbers are equal.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool Equal(TimeSpan left, TimeSpan right)
        {
            return left == right;
        }

        /// <summary>
        /// Determines whether or not two numbers are not equal.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool NotEqual(bool left, bool right)
        {
            return left != right;
        }

        /// <summary>
        /// Determines whether or not two numbers are not equal.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool NotEqual(int left, int right)
        {
            return left != right;
        }

        /// <summary>
        /// Determines whether or not two numbers are not equal.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool NotEqual(long left, long right)
        {
            return left != right;
        }

        /// <summary>
        /// Determines whether or not two numbers are not equal.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool NotEqual(float left, float right)
        {
            return left != right;
        }

        /// <summary>
        /// Determines whether or not two numbers are not equal.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool NotEqual(double left, double right)
        {
            return left != right;
        }

        /// <summary>
        /// Determines whether or not two numbers are not equal.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool NotEqual(TimeSpan left, TimeSpan right)
        {
            return left != right;
        }

        /// <summary>
        /// Determines whether or not two numbers are greater than.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool GreaterThan(int left, int right)
        {
            return left > right;
        }

        /// <summary>
        /// Determines whether or not two numbers are greater than.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool GreaterThan(long left, long right)
        {
            return left > right;
        }

        /// <summary>
        /// Determines whether or not two numbers are greater than.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool GreaterThan(float left, float right)
        {
            return left > right;
        }

        /// <summary>
        /// Determines whether or not two numbers are greater than.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool GreaterThan(double left, double right)
        {
            return left > right;
        }

        /// <summary>
        /// Determines whether or not two numbers are greater than.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool GreaterThan(TimeSpan left, TimeSpan right)
        {
            return left > right;
        }

        /// <summary>
        /// Determines whether or not two numbers are greater than or equal to one another.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool GreaterThanOrEqual(int left, int right)
        {
            return left >= right;
        }

        /// <summary>
        /// Determines whether or not two numbers are greater than or equal to one another.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool GreaterThanOrEqual(long left, long right)
        {
            return left >= right;
        }

        /// <summary>
        /// Determines whether or not two numbers are greater than or equal to one another.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool GreaterThanOrEqual(float left, float right)
        {
            return left >= right;
        }

        /// <summary>
        /// Determines whether or not two numbers are greater than or equal to one another.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool GreaterThanOrEqual(double left, double right)
        {
            return left >= right;
        }

        /// <summary>
        /// Determines whether or not two numbers are greater than or equal to one another.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool GreaterThanOrEqual(TimeSpan left, TimeSpan right)
        {
            return left >= right;
        }

        /// <summary>
        /// Determines whether or not two numbers are less than.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool LessThan(int left, int right)
        {
            return left < right;
        }

        /// <summary>
        /// Determines whether or not two numbers are less than.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool LessThan(long left, long right)
        {
            return left < right;
        }

        /// <summary>
        /// Determines whether or not two numbers are less than.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool LessThan(float left, float right)
        {
            return left < right;
        }

        /// <summary>
        /// Determines whether or not two numbers are less than.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool LessThan(double left, double right)
        {
            return left < right;
        }

        /// <summary>
        /// Determines whether or not two numbers are less than.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool LessThan(TimeSpan left, TimeSpan right)
        {
            return left < right;
        }

        /// <summary>
        /// Determines whether or not two numbers are less than or equal to one another.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool LessThanOrEqual(int left, int right)
        {
            return left <= right;
        }

        /// <summary>
        /// Determines whether or not two numbers are less than or equal to one another.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool LessThanOrEqual(long left, long right)
        {
            return left <= right;
        }

        /// <summary>
        /// Determines whether or not two numbers are less than or equal to one another.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool LessThanOrEqual(float left, float right)
        {
            return left <= right;
        }

        /// <summary>
        /// Determines whether or not two numbers are less than or equal to one another.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool LessThanOrEqual(double left, double right)
        {
            return left <= right;
        }

        /// <summary>
        /// Determines whether or not two numbers are less than or equal to one another.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result.</returns>
        public static bool LessThanOrEqual(TimeSpan left, TimeSpan right)
        {
            return left <= right;
        }
    }
}