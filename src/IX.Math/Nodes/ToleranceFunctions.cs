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
            Contract.Requires(proportion > 1, nameof(proportion));

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
            Contract.Requires(proportion > 1D, nameof(proportion));

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
            Contract.Requires(percentage < 1 && percentage > 0, nameof(percentage));

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
            Contract.Requires(percentage < 1 && percentage > 0, nameof(percentage));

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
    }
}