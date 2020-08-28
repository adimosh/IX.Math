// <copyright file="EquationNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Exceptions;
using IX.Math.Nodes.Constants;
using IX.StandardExtensions.Extensions;
using IX.StandardExtensions.Globalization;
using DiagCA = System.Diagnostics.CodeAnalysis;

namespace IX.Math.Nodes.Operators.Binary.Comparison
{
    /// <summary>
    ///     A node representing an equation operation.
    /// </summary>
    /// <seealso cref="ComparisonNodeBase" />
    internal abstract class EquationNodeBase : ComparisonNodeBase
    {
        private readonly bool notEqual;

        /// <summary>
        /// Initializes a new instance of the <see cref="EquationNodeBase" /> class.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="notEqual">if set to <c>true</c>, the node represents not equal instead of equal.</param>
        protected private EquationNodeBase(
            NodeBase left,
            NodeBase right,
            bool notEqual)
            : base(
                left,
                right)
        {
            this.notEqual = notEqual;
        }

        /// <summary>
        ///     Gets a value indicating whether or not this node is actually a constant.
        /// </summary>
        /// <value><see langword="true" /> if the node is a constant, <see langword="false" /> otherwise.</value>
        public override bool IsConstant => false;

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>
        ///     A simplified node, or this instance.
        /// </returns>
        public sealed override NodeBase Simplify()
        {
            if (!(this.Left is ConstantNodeBase left) || !(this.Right is ConstantNodeBase right))
            {
                return this;
            }

            bool? equalityValue = null;

            if (left.TryGetBoolean(out bool bvl) && right.TryGetBoolean(out bool bvr))
            {
                // Both boolean
                equalityValue = bvl == bvr;
            }
            else if (left.TryGetByteArray(out byte[] bavl) && right.TryGetByteArray(out byte[] bavr))
            {
                // Both byte array, but not both integer or numeric
                bool bli = left.CheckSupportedType(SupportableValueType.Integer);
                bool bln = left.CheckSupportedType(SupportableValueType.Numeric);
                bool bri = right.CheckSupportedType(SupportableValueType.Integer);
                bool brn = right.CheckSupportedType(SupportableValueType.Numeric);

                if ((bli || bln) && (bri || brn))
                {
                    return this;
                }

                equalityValue = bavl.SequenceEqualsWithMsb(bavr);
            }
            else if (left.TryGetString(out string svl) && right.TryGetString(out string svr))
            {
                // Both string, but not both integer or numeric
                bool bli = left.CheckSupportedType(SupportableValueType.Integer);
                bool bln = left.CheckSupportedType(SupportableValueType.Numeric);
                bool bri = right.CheckSupportedType(SupportableValueType.Integer);
                bool brn = right.CheckSupportedType(SupportableValueType.Numeric);

                if ((bli || bln) && (bri || brn))
                {
                    return this;
                }

                equalityValue = svl.CurrentCultureEquals(svr);
            }

            if (!equalityValue.HasValue)
            {
                return this;
            }

            if (this.notEqual)
            {
                equalityValue = !equalityValue.Value;
            }

            return GenerateConstantBoolean(equalityValue.Value);
        }

        /// <summary>
        /// Generates the expression that this node represents.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="comparisonTolerance">The comparison tolerance.</param>
        /// <returns>A compiled expression, if one is possible.</returns>
        [DiagCA.SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "We want it this way.")]
        protected sealed override Expression GenerateExpressionInternal(
            in SupportedValueType valueType,
            in ComparisonTolerance comparisonTolerance)
        {
            try
            {
                var (leftExpression, rightExpression, innerValueType) =
                    this.GetExpressionArguments(in comparisonTolerance);

                Expression equalExpression;

                switch (innerValueType)
                {
                    case SupportedValueType.ByteArray:
                    {
                        // Byte array comparison
                        equalExpression = Expression.Call(
                            typeof(ArrayExtensions).GetMethodWithExactParameters(
                                nameof(ArrayExtensions.SequenceEqualsWithMsb),
                                typeof(byte[]),
                                typeof(byte[])) ??
                            throw new MathematicsEngineException(),
                            leftExpression,
                            rightExpression);

                        break;
                    }

                    case SupportedValueType.Numeric:
                    case SupportedValueType.Integer:
                    {
                        // Tolerance for numeric comparisons
                        equalExpression = GenerateNumericalToleranceEquateExpression(
                            leftExpression,
                            rightExpression,
                            in comparisonTolerance);

                        static Expression GenerateNumericalToleranceEquateExpression(
                            Expression leftExpression,
                            Expression rightExpression,
                            in ComparisonTolerance tolerance)
                        {
                            if (tolerance.IntegerToleranceRangeLowerBound != null ||
                                tolerance.IntegerToleranceRangeUpperBound != null)
                            {
                                // Integer tolerance
                                MethodInfo mi = typeof(ToleranceFunctions).GetMethodWithExactParameters(
                                                    nameof(ToleranceFunctions.EquateRangeTolerant),
                                                    leftExpression.Type,
                                                    rightExpression.Type,
                                                    typeof(long),
                                                    typeof(long)) ??
                                                throw new MathematicsEngineException();

                                return Expression.Call(
                                    mi,
                                    leftExpression,
                                    rightExpression,
                                    Expression.Constant(
                                        tolerance.IntegerToleranceRangeLowerBound ?? 0L,
                                        typeof(long)),
                                    Expression.Constant(
                                        tolerance.IntegerToleranceRangeUpperBound ?? 0L,
                                        typeof(long)));
                            }

                            if (tolerance.ToleranceRangeLowerBound != null ||
                                tolerance.ToleranceRangeUpperBound != null)
                            {
                                // Floating-point tolerance
                                MethodInfo mi = typeof(ToleranceFunctions).GetMethodWithExactParameters(
                                                    nameof(ToleranceFunctions.EquateRangeTolerant),
                                                    leftExpression.Type,
                                                    rightExpression.Type,
                                                    typeof(double),
                                                    typeof(double)) ??
                                                throw new MathematicsEngineException();

                                return Expression.Call(
                                    mi,
                                    leftExpression,
                                    rightExpression,
                                    Expression.Constant(
                                        tolerance.ToleranceRangeLowerBound ?? 0D,
                                        typeof(double)),
                                    Expression.Constant(
                                        tolerance.ToleranceRangeUpperBound ?? 0D,
                                        typeof(double)));
                            }

                            if (tolerance.ProportionalTolerance != null)
                            {
                                if (tolerance.ProportionalTolerance.Value > 1D)
                                {
                                    // Proportional tolerance
                                    MethodInfo mi = typeof(ToleranceFunctions).GetMethodWithExactParameters(
                                                        nameof(ToleranceFunctions.EquateProportionTolerant),
                                                        leftExpression.Type,
                                                        rightExpression.Type,
                                                        typeof(double)) ??
                                                    throw new MathematicsEngineException();

                                    return Expression.Call(
                                        mi,
                                        leftExpression,
                                        rightExpression,
                                        Expression.Constant(
                                            tolerance.ProportionalTolerance ?? 0D,
                                            typeof(double)));
                                }

                                if (tolerance.ProportionalTolerance.Value < 1D &&
                                    tolerance.ProportionalTolerance.Value > 0D)
                                {
                                    // Percentage tolerance
                                    MethodInfo mi = typeof(ToleranceFunctions).GetMethodWithExactParameters(
                                                        nameof(ToleranceFunctions.EquatePercentageTolerant),
                                                        leftExpression.Type,
                                                        rightExpression.Type,
                                                        typeof(double)) ??
                                                    throw new MathematicsEngineException();

                                    return Expression.Call(
                                        mi,
                                        leftExpression,
                                        rightExpression,
                                        Expression.Constant(
                                            tolerance.ProportionalTolerance ?? 0D,
                                            typeof(double)));
                                }
                            }

                            return Expression.Equal(
                                leftExpression,
                                rightExpression);
                        }

                        break;
                    }

                    case SupportedValueType.Boolean:
                    {
                        // Exact equation for boolean
                        equalExpression = Expression.Equal(
                            leftExpression,
                            rightExpression);
                        break;
                    }

                    case SupportedValueType.String:
                    {
                        // String equation
                        equalExpression = Expression.Call(
                            typeof(string).GetMethodWithExactParameters(
                                nameof(string.Equals),
                                typeof(string),
                                typeof(string),
                                typeof(StringComparison)) ??
                            throw new MathematicsEngineException(),
                            leftExpression,
                            rightExpression,
                            Expression.Constant(
                                StringComparison.Ordinal,
                                typeof(StringComparison)));

                        break;
                    }

                    default:
                        throw new ExpressionNotValidLogicallyException();
                }

                return this.notEqual
                    ? Expression.Equal(
                        equalExpression,
                        Expression.Constant(
                            false,
                            typeof(bool)))
                    : equalExpression;
            }
            catch (ExpressionNotValidLogicallyException)
            {
                throw;
            }
            catch (MathematicsEngineException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ExpressionNotValidLogicallyException(ex);
            }
        }
    }
}