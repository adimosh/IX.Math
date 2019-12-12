// <copyright file="ComparisonOperationNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;
using System.Reflection;
using IX.StandardExtensions.Extensions;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Operations.Binary
{
    internal abstract class ComparisonOperationNodeBase : BinaryOperationNodeBase
    {
        protected ComparisonOperationNodeBase(
            NodeBase left,
            NodeBase right)
            : base(
                left,
                right)
        {
        }

        public override SupportedValueType ReturnType => SupportedValueType.Boolean;

        private static void DetermineChildren(
            NodeBase parameter,
            NodeBase other)
        {
            if (other.ReturnType == SupportedValueType.Unknown)
            {
                return;
            }

            parameter.DetermineStrongly(other.ReturnType);
        }

        /// <summary>
        ///     Strongly determines the node's type, if possible.
        /// </summary>
        /// <param name="type">The type to determine to.</param>
        public override void DetermineStrongly(SupportedValueType type)
        {
            if (type != SupportedValueType.Boolean)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        /// <summary>
        ///     Weakly determines the node's type, if possible, and, optionally, strongly determines if there is only one possible
        ///     type left.
        /// </summary>
        /// <param name="type">The type or types to determine to.</param>
        public override void DetermineWeakly(SupportableValueType type)
        {
            if ((type & SupportableValueType.Boolean) == 0)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        protected override void EnsureCompatibleOperands(
            NodeBase left,
            NodeBase right)
        {
            DetermineChildren(
                left,
                right);
            DetermineChildren(
                right,
                left);
            DetermineChildren(
                left,
                right);
            DetermineChildren(
                right,
                left);

            if (left.ReturnType != right.ReturnType)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "We want it this way.")]
        protected Expression GenerateNumericalToleranceEquateExpression([NotNull] Expression leftExpression, [NotNull] Expression rightExpression, [NotNull] Tolerance tolerance)
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
                    typeof(long));

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

            if (tolerance.ToleranceRangeLowerBound != null || tolerance.ToleranceRangeUpperBound != null)
            {
                // Floating-point tolerance
                MethodInfo mi = typeof(ToleranceFunctions).GetMethodWithExactParameters(
                    nameof(ToleranceFunctions.EquateRangeTolerant),
                    leftExpression.Type,
                    rightExpression.Type,
                    typeof(double),
                    typeof(double));

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
                        typeof(double));

                    return Expression.Call(
                        mi,
                        leftExpression,
                        rightExpression,
                        Expression.Constant(
                            tolerance.ProportionalTolerance ?? 0D,
                            typeof(double)));
                }

                if (tolerance.ProportionalTolerance.Value < 1D && tolerance.ProportionalTolerance.Value > 0D)
                {
                    // Percentage tolerance
                    MethodInfo mi = typeof(ToleranceFunctions).GetMethodWithExactParameters(
                        nameof(ToleranceFunctions.EquatePercentageTolerant),
                        leftExpression.Type,
                        rightExpression.Type,
                        typeof(double));

                    return Expression.Call(
                        mi,
                        leftExpression,
                        rightExpression,
                        Expression.Constant(
                            tolerance.ProportionalTolerance ?? 0D,
                            typeof(double)));
                }
            }

            return null;
        }
    }
}