// <copyright file="GreaterThanOperator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using System.Reflection;
using IX.StandardExtensions.Extensions;

namespace IX.Math.Nodes.Operators.Binary.Comparison
{
    /// <summary>
    /// A greater than binary operator.
    /// </summary>
    internal sealed class GreaterThanOperator : ComparisonOperatorNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GreaterThanOperator"/> class.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        internal GreaterThanOperator(
            NodeBase leftOperand,
            NodeBase rightOperand)
            : base(
                leftOperand,
                rightOperand) { }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new GreaterThanOperator(
                this.LeftOperand.DeepClone(context),
                this.RightOperand.DeepClone(context));

        /// <summary>
        /// Generates an integer mathematical expression.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <param name="tolerance">The tolerance for this operation.</param>
        /// <returns>An expression containing the operation.</returns>
        private protected override Expression GenerateIntegerExpression(
            Expression left,
            Expression right,
            Tolerance? tolerance = null)
        {
            if (tolerance == null)
            {
                // No tolerance
                return Expression.GreaterThan(
                    left,
                    right);
            }

            if (tolerance.IntegerToleranceRangeLowerBound != null)
            {
                // Integer tolerance
                MethodInfo mi = typeof(ToleranceFunctions).GetMethodWithExactParameters(
                                    nameof(ToleranceFunctions.GreaterThanRangeTolerant),
                                    typeof(long),
                                    typeof(long),
                                    typeof(long)) ??
                                throw new PlatformNotSupportedException();

                return Expression.Call(
                    mi,
                    left,
                    right,
                    Expression.Constant(
                        tolerance.IntegerToleranceRangeLowerBound.Value,
                        typeof(long)));
            }

            if (tolerance.ToleranceRangeLowerBound != null)
            {
                // Floating-point tolerance
                MethodInfo mi = typeof(ToleranceFunctions).GetMethodWithExactParameters(
                                    nameof(ToleranceFunctions.GreaterThanRangeTolerant),
                                    typeof(long),
                                    typeof(long),
                                    typeof(double)) ??
                                throw new PlatformNotSupportedException();

                return Expression.Call(
                    mi,
                    left,
                    right,
                    Expression.Constant(
                        tolerance.ToleranceRangeLowerBound.Value,
                        typeof(double)));
            }

            if (tolerance.ProportionalTolerance != null)
            {
                if (tolerance.ProportionalTolerance.Value > 1D)
                {
                    // Proportional tolerance
                    MethodInfo mi = typeof(ToleranceFunctions).GetMethodWithExactParameters(
                        nameof(ToleranceFunctions.GreaterThanProportionTolerant),
                        typeof(long),
                        typeof(long),
                        typeof(double)) ?? throw new PlatformNotSupportedException();

                    return Expression.Call(
                        mi,
                        left,
                        right,
                        Expression.Constant(
                            tolerance.ProportionalTolerance.Value,
                            typeof(double)));
                }

                if (tolerance.ProportionalTolerance.Value < 1D && tolerance.ProportionalTolerance.Value > 0D)
                {
                    // Percentage tolerance
                    MethodInfo mi = typeof(ToleranceFunctions).GetMethodWithExactParameters(
                        nameof(ToleranceFunctions.GreaterThanPercentageTolerant),
                        typeof(long),
                        typeof(long),
                        typeof(double)) ?? throw new PlatformNotSupportedException();

                    return Expression.Call(
                        mi,
                        left,
                        right,
                        Expression.Constant(
                            tolerance.ProportionalTolerance.Value,
                            typeof(double)));
                }
            }

            // No discernible tolerance value
            return Expression.GreaterThan(
                left,
                right);
        }

        /// <summary>
        /// Generates a numeric mathematical expression.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <param name="tolerance">The tolerance for this operation.</param>
        /// <returns>An expression containing the operation.</returns>
        protected private override Expression GenerateNumericExpression(
            Expression left,
            Expression right,
            Tolerance? tolerance = null)
        {
            if (tolerance == null)
            {
                // No tolerance
                return Expression.GreaterThan(
                    left,
                    right);
            }

            if (tolerance.IntegerToleranceRangeLowerBound != null)
            {
                // Integer tolerance
                MethodInfo mi = typeof(ToleranceFunctions).GetMethodWithExactParameters(
                                    nameof(ToleranceFunctions.GreaterThanRangeTolerant),
                                    typeof(double),
                                    typeof(double),
                                    typeof(long)) ??
                                throw new PlatformNotSupportedException();

                return Expression.Call(
                    mi,
                    left,
                    right,
                    Expression.Constant(
                        tolerance.IntegerToleranceRangeLowerBound.Value,
                        typeof(long)));
            }

            if (tolerance.ToleranceRangeLowerBound != null)
            {
                // Floating-point tolerance
                MethodInfo mi = typeof(ToleranceFunctions).GetMethodWithExactParameters(
                                    nameof(ToleranceFunctions.GreaterThanRangeTolerant),
                                    typeof(double),
                                    typeof(double),
                                    typeof(double)) ??
                                throw new PlatformNotSupportedException();

                return Expression.Call(
                    mi,
                    left,
                    right,
                    Expression.Constant(
                        tolerance.ToleranceRangeLowerBound.Value,
                        typeof(double)));
            }

            if (tolerance.ProportionalTolerance != null)
            {
                if (tolerance.ProportionalTolerance.Value > 1D)
                {
                    // Proportional tolerance
                    MethodInfo mi = typeof(ToleranceFunctions).GetMethodWithExactParameters(
                        nameof(ToleranceFunctions.GreaterThanProportionTolerant),
                        typeof(double),
                        typeof(double),
                        typeof(double)) ?? throw new PlatformNotSupportedException();

                    return Expression.Call(
                        mi,
                        left,
                        right,
                        Expression.Constant(
                            tolerance.ProportionalTolerance.Value,
                            typeof(double)));
                }

                if (tolerance.ProportionalTolerance.Value < 1D && tolerance.ProportionalTolerance.Value > 0D)
                {
                    // Percentage tolerance
                    MethodInfo mi = typeof(ToleranceFunctions).GetMethodWithExactParameters(
                        nameof(ToleranceFunctions.GreaterThanPercentageTolerant),
                        typeof(double),
                        typeof(double),
                        typeof(double)) ?? throw new PlatformNotSupportedException();

                    return Expression.Call(
                        mi,
                        left,
                        right,
                        Expression.Constant(
                            tolerance.ProportionalTolerance.Value,
                            typeof(double)));
                }
            }

            // No discernible tolerance value
            return Expression.GreaterThan(
                left,
                right);
        }

        /// <summary>
        /// Generates a binary mathematical expression.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <returns>An expression containing the operation.</returns>
        private protected override Expression GenerateBinaryExpression(
            Expression left,
            Expression right) =>
            Expression.GreaterThan(
                Expression.Call(
                    typeof(ArrayExtensions).GetMethodWithExactParameters(
                        nameof(ArrayExtensions.SequenceCompareWithMsb),
                        typeof(byte[]),
                        typeof(byte[])) ??
                    throw new PlatformNotSupportedException(),
                    left,
                    right),
                Expression.Constant(
                    0,
                    typeof(int)));

        /// <summary>
        /// Generates a string mathematical expression.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <returns>An expression containing the operation.</returns>
        private protected override Expression GenerateStringExpression(
            Expression left,
            Expression right)
        {
            var mi = typeof(string).GetMethod(
                    nameof(string.CompareOrdinal),
                    new[]
                    {
                        typeof(string),
                        typeof(string)
                    }) ??
                throw new PlatformNotSupportedException();

            return Expression.GreaterThan(
                Expression.Call(
                    mi,
                    left,
                    right),
                Expression.Constant(
                    0,
                    typeof(int)));
        }
    }
}