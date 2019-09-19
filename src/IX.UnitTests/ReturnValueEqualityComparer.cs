// <copyright file="ReturnValueEqualityComparer.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using IX.StandardExtensions;
using ArrayExtensions = IX.StandardExtensions.Extensions.ArrayExtensions;

namespace IX.UnitTests
{
    internal class ReturnValueEqualityComparer : IEqualityComparer<object>
    {
        /// <summary>Determines whether the specified objects are equal.</summary>
        /// <param name="x">The first object to compare.</param>
        /// <param name="y">The second object to compare.</param>
        /// <returns>true if the specified objects are equal; otherwise, false.</returns>
        /// <exception cref="T:System.ArgumentException"><paramref name="x">x</paramref> and <paramref name="y">y</paramref> are of different types and neither one can handle comparisons with the other.</exception>
        public new bool Equals(
            object x,
            object y)
        {
            switch (x)
            {
                case int ix:
                    return this.Equals(
                        ix,
                        y);
                case long lx:
                    return this.Equals(
                        lx,
                        y);
                case double dx:
                    return this.Equals(
                        dx,
                        y);
                case byte[] bx:
                    {
                        if (!(y is byte[] by))
                        {
                            return false;
                        }

                        return ArrayExtensions.SequenceEquals(bx, by);
                    }

                case string sx:
                    {
                        if (!(y is string sy))
                        {
                            return false;
                        }

                        return sx.Equals(
                            sy,
                            StringComparison.Ordinal);
                    }

                default:
                    return x?.Equals(y) ?? throw new ArgumentNullException(nameof(x));
            }
        }

        /// <summary>Returns a hash code for the specified object.</summary>
        /// <param name="obj">The <see cref="T:System.Object"></see> for which a hash code is to be returned.</param>
        /// <returns>A hash code for the specified object.</returns>
        /// <exception cref="T:System.ArgumentNullException">The type of <paramref name="obj">obj</paramref> is a reference type and <paramref name="obj">obj</paramref> is null.</exception>
        public int GetHashCode(object obj) => obj.GetHashCode();

        private bool Equals(
            long x,
            object y)
        {
            switch (y)
            {
                case long ly:
                    return x == ly;
                case double dy:
                    // ReSharper disable once CompareOfFloatsByEqualityOperator - We are not interested in approximates
                    return Convert.ToDouble(x) == dy;
                case int iy:
                    return x == Convert.ToInt64(iy);
                default:
                    throw new ArgumentInvalidTypeException(nameof(x));
            }
        }

        private bool Equals(
            double x,
            object y)
        {
            switch (y)
            {
                case double dy:
                    // ReSharper disable once CompareOfFloatsByEqualityOperator - We are not interested in approximates
                    return x == dy;
                case long ly:
                    // ReSharper disable once CompareOfFloatsByEqualityOperator - We are not interested in approximates
                    return Convert.ToDouble(x) == ly;
                case int iy:
                    // ReSharper disable once CompareOfFloatsByEqualityOperator - We are not interested in approximates
                    return x == Convert.ToDouble(iy);
                default:
                    throw new ArgumentInvalidTypeException(nameof(x));
            }
        }

        private bool Equals(
            int x,
            object y)
        {
            switch (y)
            {
                case long ly:
                    return Convert.ToInt64(x) == ly;
                case double dy:
                    // ReSharper disable once CompareOfFloatsByEqualityOperator - We are not interested in approximates
                    return Convert.ToDouble(x) == dy;
                case int iy:
                    return x == iy;
                default:
                    throw new ArgumentInvalidTypeException(nameof(x));
            }
        }
    }
}