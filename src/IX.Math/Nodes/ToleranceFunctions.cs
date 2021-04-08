// <copyright file="ToleranceFunctions.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using IX.StandardExtensions.Contracts;
using JetBrains.Annotations;

namespace IX.Math.Nodes
{
    /// <summary>
    /// Contains functions related to tolerance.
    /// </summary>
    [PublicAPI]
    public static class ToleranceFunctions
    {
        #region Equation

        /// <summary>
        /// Equates two integer operands while being integer-range-tolerant.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <returns><c>true</c> if the two operands are within range, <c>false</c> otherwise.</returns>
        public static bool EquateRangeTolerant(
            long leftOperand,
            long rightOperand,
            long lowerBound,
            long upperBound) => (leftOperand >= (rightOperand - lowerBound)) && (leftOperand <= (rightOperand + upperBound));

        /// <summary>
        /// Equates two integer operands while being floating-point-range-tolerant.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <returns><c>true</c> if the two operands are within range, <c>false</c> otherwise.</returns>
        public static bool EquateRangeTolerant(
            long leftOperand,
            long rightOperand,
            double lowerBound,
            double upperBound) => ((double)leftOperand >= ((double)rightOperand - lowerBound)) && ((double)leftOperand <= ((double)rightOperand + upperBound));

        /// <summary>
        /// Equates two floating-point operands while being integer-range-tolerant.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <returns><c>true</c> if the two operands are within range, <c>false</c> otherwise.</returns>
        public static bool EquateRangeTolerant(
            double leftOperand,
            double rightOperand,
            long lowerBound,
            long upperBound) => (leftOperand >= (rightOperand - (double)lowerBound)) && (leftOperand <= (rightOperand + (double)upperBound));

        /// <summary>
        /// Equates two floating-point operands while being floating-point-range-tolerant.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="lowerBound">The lower bound.</param>
        /// <param name="upperBound">The upper bound.</param>
        /// <returns><c>true</c> if the two operands are within range, <c>false</c> otherwise.</returns>
        public static bool EquateRangeTolerant(
            double leftOperand,
            double rightOperand,
            double lowerBound,
            double upperBound) => (leftOperand >= (rightOperand - lowerBound)) && (leftOperand <= (rightOperand + upperBound));

        /// <summary>
        /// Equates two integer operands while being proportion-tolerant.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="proportion">The proportion.</param>
        /// <returns><c>true</c> if the two operands are within proportion, <c>false</c> otherwise.</returns>
        public static bool EquateProportionTolerant(
            long leftOperand,
            long rightOperand,
            double proportion)
        {
            Requires.True(proportion > 1, nameof(proportion));

            double directProportion = ((double)rightOperand) * proportion;
            double inverseProportion = ((double)rightOperand) * (1D / proportion);

            return (double)leftOperand >=
                    global::System.Math.Min(
                        directProportion,
                        inverseProportion) &&
                    (double)rightOperand <=
                    global::System.Math.Max(
                        directProportion,
                        inverseProportion);
        }

        /// <summary>
        /// Equates two floating-point operands while being proportion-tolerant.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="proportion">The proportion.</param>
        /// <returns><c>true</c> if the two operands are within proportion, <c>false</c> otherwise.</returns>
        public static bool EquateProportionTolerant(
            double leftOperand,
            double rightOperand,
            double proportion)
        {
            Requires.True(proportion > 1D, nameof(proportion));

            double directProportion = rightOperand * proportion;
            double inverseProportion = rightOperand * (1D / proportion);

            return leftOperand >=
                   global::System.Math.Min(
                       directProportion,
                       inverseProportion) &&
                   rightOperand <=
                   global::System.Math.Max(
                       directProportion,
                       inverseProportion);
        }

        /// <summary>
        /// Equates two integer operands while being percentage-tolerant.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="percentage">The percentage.</param>
        /// <returns><c>true</c> if the two operands are within percentage, <c>false</c> otherwise.</returns>
        public static bool EquatePercentageTolerant(
            long leftOperand,
            long rightOperand,
            double percentage)
        {
            Requires.True(percentage < 1 && percentage > 0, nameof(percentage));

            double directPercentage = ((double)rightOperand) * (1D - percentage);
            double inversePercentage = ((double)rightOperand) * (1D + percentage);

            return (double)leftOperand >=
                    global::System.Math.Min(
                        directPercentage,
                        inversePercentage) &&
                    (double)rightOperand <=
                    global::System.Math.Max(
                        directPercentage,
                        inversePercentage);
        }

        /// <summary>
        /// Equates two floating-point operands while being percentage-tolerant.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="percentage">The percentage.</param>
        /// <returns><c>true</c> if the two operands are within percentage, <c>false</c> otherwise.</returns>
        public static bool EquatePercentageTolerant(
            double leftOperand,
            double rightOperand,
            double percentage)
        {
            Requires.True(percentage < 1 && percentage > 0, nameof(percentage));

            double directPercentage = ((double)rightOperand) * (1D - percentage);
            double inversePercentage = ((double)rightOperand) * (1D + percentage);

            return leftOperand >=
                   global::System.Math.Min(
                       directPercentage,
                       inversePercentage) &&
                   rightOperand <=
                   global::System.Math.Max(
                       directPercentage,
                       inversePercentage);
        }
        #endregion

        #region Greater Than

        /// <summary>
        /// Establishes whether or not the left operand is greater than the right operand in a range-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="range">The range.</param>
        /// <returns><c>true</c> if the left is greater than the right, <c>false</c> otherwise.</returns>
        public static bool GreaterThanRangeTolerant(
            long leftOperand,
            long rightOperand,
            long range) =>
            leftOperand > rightOperand - range;

        /// <summary>
        /// Establishes whether or not the left operand is greater than the right operand in a range-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="range">The range.</param>
        /// <returns><c>true</c> if the left is greater than the right, <c>false</c> otherwise.</returns>
        public static bool GreaterThanRangeTolerant(
            long leftOperand,
            long rightOperand,
            double range) =>
            (double)leftOperand > (double)rightOperand - range;

        /// <summary>
        /// Establishes whether or not the left operand is greater than the right operand in a range-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="range">The range.</param>
        /// <returns><c>true</c> if the left is greater than the right, <c>false</c> otherwise.</returns>
        public static bool GreaterThanRangeTolerant(
            double leftOperand,
            double rightOperand,
            long range) =>
            leftOperand > rightOperand - (double)range;

        /// <summary>
        /// Establishes whether or not the left operand is greater than the right operand in a range-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="range">The range.</param>
        /// <returns><c>true</c> if the left is greater than the right, <c>false</c> otherwise.</returns>
        public static bool GreaterThanRangeTolerant(
            double leftOperand,
            double rightOperand,
            double range) =>
            leftOperand > rightOperand - range;

        /// <summary>
        /// Establishes whether or not the left operand is greater than the right operand in a proportion-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="proportion">The proportion.</param>
        /// <returns><c>true</c> if the two operands are within proportion, <c>false</c> otherwise.</returns>
        public static bool GreaterThanProportionTolerant(
            long leftOperand,
            long rightOperand,
            double proportion)
        {
            Requires.True(proportion > 1, nameof(proportion));

            double directProportion = ((double)rightOperand) * proportion;
            double inverseProportion = ((double)rightOperand) * (1D / proportion);

            return (double)leftOperand >
                    global::System.Math.Min(
                        directProportion,
                        inverseProportion);
        }

        /// <summary>
        /// Establishes whether or not the left operand is greater than the right operand in a proportion-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="proportion">The proportion.</param>
        /// <returns><c>true</c> if the two operands are within proportion, <c>false</c> otherwise.</returns>
        public static bool GreaterThanProportionTolerant(
            double leftOperand,
            double rightOperand,
            double proportion)
        {
            Requires.True(proportion > 1D, nameof(proportion));

            double directProportion = rightOperand * proportion;
            double inverseProportion = rightOperand * (1D / proportion);

            return leftOperand >
                   global::System.Math.Min(
                       directProportion,
                       inverseProportion);
        }

        /// <summary>
        /// Establishes whether or not the left operand is greater than the right operand in a percentage-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="percentage">The percentage.</param>
        /// <returns><c>true</c> if the two operands are within percentage, <c>false</c> otherwise.</returns>
        public static bool GreaterThanPercentageTolerant(
            long leftOperand,
            long rightOperand,
            double percentage)
        {
            Requires.True(percentage < 1 && percentage > 0, nameof(percentage));

            double directPercentage = ((double)rightOperand) * (1D - percentage);
            double inversePercentage = ((double)rightOperand) * (1D + percentage);

            return (double)leftOperand >
                    global::System.Math.Min(
                        directPercentage,
                        inversePercentage);
        }

        /// <summary>
        /// Establishes whether or not the left operand is greater than the right operand in a percentage-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="percentage">The percentage.</param>
        /// <returns><c>true</c> if the two operands are within percentage, <c>false</c> otherwise.</returns>
        public static bool GreaterThanPercentageTolerant(
            double leftOperand,
            double rightOperand,
            double percentage)
        {
            Requires.True(percentage < 1 && percentage > 0, nameof(percentage));

            double directPercentage = ((double)rightOperand) * (1D - percentage);
            double inversePercentage = ((double)rightOperand) * (1D + percentage);

            return leftOperand >
                   global::System.Math.Min(
                       directPercentage,
                       inversePercentage);
        }
        #endregion

        #region Greater Than OrEqual

        /// <summary>
        /// Establishes whether or not the left operand is greater than or equal to the right operand in a range-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="range">The range.</param>
        /// <returns><c>true</c> if the left is greater than the right, <c>false</c> otherwise.</returns>
        public static bool GreaterThanOrEqualRangeTolerant(
            long leftOperand,
            long rightOperand,
            long range) =>
            leftOperand >= rightOperand - range;

        /// <summary>
        /// Establishes whether or not the left operand is greater than or equal to the right operand in a range-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="range">The range.</param>
        /// <returns><c>true</c> if the left is greater than the right, <c>false</c> otherwise.</returns>
        public static bool GreaterThanOrEqualRangeTolerant(
            long leftOperand,
            long rightOperand,
            double range) =>
            (double)leftOperand >= (double)rightOperand - range;

        /// <summary>
        /// Establishes whether or not the left operand is greater than or equal to the right operand in a range-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="range">The range.</param>
        /// <returns><c>true</c> if the left is greater than the right, <c>false</c> otherwise.</returns>
        public static bool GreaterThanOrEqualRangeTolerant(
            double leftOperand,
            double rightOperand,
            long range) =>
            leftOperand >= rightOperand - (double)range;

        /// <summary>
        /// Establishes whether or not the left operand is greater than or equal to the right operand in a range-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="range">The range.</param>
        /// <returns><c>true</c> if the left is greater than the right, <c>false</c> otherwise.</returns>
        public static bool GreaterThanOrEqualRangeTolerant(
            double leftOperand,
            double rightOperand,
            double range) =>
            leftOperand >= rightOperand - range;

        /// <summary>
        /// Establishes whether or not the left operand is greater than or equal to the right operand in a proportion-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="proportion">The proportion.</param>
        /// <returns><c>true</c> if the two operands are within proportion, <c>false</c> otherwise.</returns>
        public static bool GreaterThanOrEqualProportionTolerant(
            long leftOperand,
            long rightOperand,
            double proportion)
        {
            Requires.True(proportion > 1, nameof(proportion));

            double directProportion = ((double)rightOperand) * proportion;
            double inverseProportion = ((double)rightOperand) * (1D / proportion);

            return (double)leftOperand >=
                    global::System.Math.Min(
                        directProportion,
                        inverseProportion);
        }

        /// <summary>
        /// Establishes whether or not the left operand is greater than or equal to the right operand in a proportion-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="proportion">The proportion.</param>
        /// <returns><c>true</c> if the two operands are within proportion, <c>false</c> otherwise.</returns>
        public static bool GreaterThanOrEqualProportionTolerant(
            double leftOperand,
            double rightOperand,
            double proportion)
        {
            Requires.True(proportion > 1D, nameof(proportion));

            double directProportion = rightOperand * proportion;
            double inverseProportion = rightOperand * (1D / proportion);

            return leftOperand >=
                   global::System.Math.Min(
                       directProportion,
                       inverseProportion);
        }

        /// <summary>
        /// Establishes whether or not the left operand is greater than or equal to the right operand in a percentage-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="percentage">The percentage.</param>
        /// <returns><c>true</c> if the two operands are within percentage, <c>false</c> otherwise.</returns>
        public static bool GreaterThanOrEqualPercentageTolerant(
            long leftOperand,
            long rightOperand,
            double percentage)
        {
            Requires.True(percentage < 1 && percentage > 0, nameof(percentage));

            double directPercentage = ((double)rightOperand) * (1D - percentage);
            double inversePercentage = ((double)rightOperand) * (1D + percentage);

            return (double)leftOperand >=
                    global::System.Math.Min(
                        directPercentage,
                        inversePercentage);
        }

        /// <summary>
        /// Establishes whether or not the left operand is greater than or equal to the right operand in a percentage-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="percentage">The percentage.</param>
        /// <returns><c>true</c> if the two operands are within percentage, <c>false</c> otherwise.</returns>
        public static bool GreaterThanOrEqualPercentageTolerant(
            double leftOperand,
            double rightOperand,
            double percentage)
        {
            Requires.True(percentage < 1 && percentage > 0, nameof(percentage));

            double directPercentage = ((double)rightOperand) * (1D - percentage);
            double inversePercentage = ((double)rightOperand) * (1D + percentage);

            return leftOperand >=
                   global::System.Math.Min(
                       directPercentage,
                       inversePercentage);
        }
        #endregion

        #region Less Than

        /// <summary>
        /// Establishes whether or not the left operand is less than the right operand in a range-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="range">The range.</param>
        /// <returns><c>true</c> if the left is less than the right, <c>false</c> otherwise.</returns>
        public static bool LessThanRangeTolerant(
            long leftOperand,
            long rightOperand,
            long range) =>
            leftOperand < rightOperand + range;

        /// <summary>
        /// Establishes whether or not the left operand is less than the right operand in a range-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="range">The range.</param>
        /// <returns><c>true</c> if the left is less than the right, <c>false</c> otherwise.</returns>
        public static bool LessThanRangeTolerant(
            long leftOperand,
            long rightOperand,
            double range) =>
            (double)leftOperand < (double)rightOperand + range;

        /// <summary>
        /// Establishes whether or not the left operand is less than the right operand in a range-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="range">The range.</param>
        /// <returns><c>true</c> if the left is less than the right, <c>false</c> otherwise.</returns>
        public static bool LessThanRangeTolerant(
            double leftOperand,
            double rightOperand,
            long range) =>
            leftOperand < rightOperand + (double)range;

        /// <summary>
        /// Establishes whether or not the left operand is less than the right operand in a range-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="range">The range.</param>
        /// <returns><c>true</c> if the left is less than the right, <c>false</c> otherwise.</returns>
        public static bool LessThanRangeTolerant(
            double leftOperand,
            double rightOperand,
            double range) =>
            leftOperand < rightOperand + range;

        /// <summary>
        /// Establishes whether or not the left operand is less than the right operand in a proportion-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="proportion">The proportion.</param>
        /// <returns><c>true</c> if the two operands are within proportion, <c>false</c> otherwise.</returns>
        public static bool LessThanProportionTolerant(
            long leftOperand,
            long rightOperand,
            double proportion)
        {
            Requires.True(proportion > 1, nameof(proportion));

            double directProportion = ((double)rightOperand) * proportion;
            double inverseProportion = ((double)rightOperand) * (1D / proportion);

            return (double)leftOperand <
                    global::System.Math.Max(
                        directProportion,
                        inverseProportion);
        }

        /// <summary>
        /// Establishes whether or not the left operand is less than the right operand in a proportion-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="proportion">The proportion.</param>
        /// <returns><c>true</c> if the two operands are within proportion, <c>false</c> otherwise.</returns>
        public static bool LessThanProportionTolerant(
            double leftOperand,
            double rightOperand,
            double proportion)
        {
            Requires.True(proportion > 1D, nameof(proportion));

            double directProportion = rightOperand * proportion;
            double inverseProportion = rightOperand * (1D / proportion);

            return leftOperand <
                   global::System.Math.Max(
                       directProportion,
                       inverseProportion);
        }

        /// <summary>
        /// Establishes whether or not the left operand is less than the right operand in a percentage-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="percentage">The percentage.</param>
        /// <returns><c>true</c> if the two operands are within percentage, <c>false</c> otherwise.</returns>
        public static bool LessThanPercentageTolerant(
            long leftOperand,
            long rightOperand,
            double percentage)
        {
            Requires.True(percentage < 1 && percentage > 0, nameof(percentage));

            double directPercentage = ((double)rightOperand) * (1D - percentage);
            double inversePercentage = ((double)rightOperand) * (1D + percentage);

            return (double)leftOperand <
                    global::System.Math.Max(
                        directPercentage,
                        inversePercentage);
        }

        /// <summary>
        /// Establishes whether or not the left operand is less than the right operand in a percentage-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="percentage">The percentage.</param>
        /// <returns><c>true</c> if the two operands are within percentage, <c>false</c> otherwise.</returns>
        public static bool LessThanPercentageTolerant(
            double leftOperand,
            double rightOperand,
            double percentage)
        {
            Requires.True(percentage < 1 && percentage > 0, nameof(percentage));

            double directPercentage = ((double)rightOperand) * (1D - percentage);
            double inversePercentage = ((double)rightOperand) * (1D + percentage);

            return leftOperand <
                   global::System.Math.Max(
                       directPercentage,
                       inversePercentage);
        }
        #endregion

        #region Less Than OrEqual

        /// <summary>
        /// Establishes whether or not the left operand is less than or equal to the right operand in a range-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="range">The range.</param>
        /// <returns><c>true</c> if the left is less than the right, <c>false</c> otherwise.</returns>
        public static bool LessThanOrEqualRangeTolerant(
            long leftOperand,
            long rightOperand,
            long range) =>
            leftOperand <= rightOperand + range;

        /// <summary>
        /// Establishes whether or not the left operand is less than or equal to the right operand in a range-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="range">The range.</param>
        /// <returns><c>true</c> if the left is less than the right, <c>false</c> otherwise.</returns>
        public static bool LessThanOrEqualRangeTolerant(
            long leftOperand,
            long rightOperand,
            double range) =>
            (double)leftOperand <= (double)rightOperand + range;

        /// <summary>
        /// Establishes whether or not the left operand is less than or equal to the right operand in a range-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="range">The range.</param>
        /// <returns><c>true</c> if the left is less than the right, <c>false</c> otherwise.</returns>
        public static bool LessThanOrEqualRangeTolerant(
            double leftOperand,
            double rightOperand,
            long range) =>
            leftOperand <= rightOperand + (double)range;

        /// <summary>
        /// Establishes whether or not the left operand is less than or equal to the right operand in a range-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="range">The range.</param>
        /// <returns><c>true</c> if the left is less than the right, <c>false</c> otherwise.</returns>
        public static bool LessThanOrEqualRangeTolerant(
            double leftOperand,
            double rightOperand,
            double range) =>
            leftOperand <= rightOperand + range;

        /// <summary>
        /// Establishes whether or not the left operand is less than or equal to the right operand in a proportion-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="proportion">The proportion.</param>
        /// <returns><c>true</c> if the two operands are within proportion, <c>false</c> otherwise.</returns>
        public static bool LessThanOrEqualProportionTolerant(
            long leftOperand,
            long rightOperand,
            double proportion)
        {
            Requires.True(proportion > 1, nameof(proportion));

            double directProportion = ((double)rightOperand) * proportion;
            double inverseProportion = ((double)rightOperand) * (1D / proportion);

            return (double)leftOperand <=
                    global::System.Math.Max(
                        directProportion,
                        inverseProportion);
        }

        /// <summary>
        /// Establishes whether or not the left operand is less than or equal to the right operand in a proportion-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="proportion">The proportion.</param>
        /// <returns><c>true</c> if the two operands are within proportion, <c>false</c> otherwise.</returns>
        public static bool LessThanOrEqualProportionTolerant(
            double leftOperand,
            double rightOperand,
            double proportion)
        {
            Requires.GreaterThan(in proportion, 1D, nameof(proportion));

            double directProportion = rightOperand * proportion;
            double inverseProportion = rightOperand * (1D / proportion);

            return leftOperand <=
                   global::System.Math.Max(
                       directProportion,
                       inverseProportion);
        }

        /// <summary>
        /// Establishes whether or not the left operand is less than or equal to the right operand in a percentage-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="percentage">The percentage.</param>
        /// <returns><c>true</c> if the two operands are within percentage, <c>false</c> otherwise.</returns>
        public static bool LessThanOrEqualPercentageTolerant(
            long leftOperand,
            long rightOperand,
            double percentage)
        {
            Requires.True(percentage < 1 && percentage > 0, nameof(percentage));

            double directPercentage = ((double)rightOperand) * (1D - percentage);
            double inversePercentage = ((double)rightOperand) * (1D + percentage);

            return (double)leftOperand <=
                    global::System.Math.Max(
                        directPercentage,
                        inversePercentage);
        }

        /// <summary>
        /// Establishes whether or not the left operand is less than or equal to the right operand in a percentage-tolerant way.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        /// <param name="percentage">The percentage.</param>
        /// <returns><c>true</c> if the two operands are within percentage, <c>false</c> otherwise.</returns>
        public static bool LessThanOrEqualPercentageTolerant(
            double leftOperand,
            double rightOperand,
            double percentage)
        {
            Requires.True(percentage < 1 && percentage > 0, nameof(percentage));

            double directPercentage = ((double)rightOperand) * (1D - percentage);
            double inversePercentage = ((double)rightOperand) * (1D + percentage);

            return leftOperand <=
                   global::System.Math.Max(
                       directPercentage,
                       inversePercentage);
        }
        #endregion
    }
}