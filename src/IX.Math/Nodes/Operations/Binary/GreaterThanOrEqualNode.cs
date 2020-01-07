// <copyright file="GreaterThanOrEqualNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Nodes.Constants;
using IX.StandardExtensions.Extensions;
using JetBrains.Annotations;
using SuppressMessage = System.Diagnostics.CodeAnalysis.SuppressMessageAttribute;

namespace IX.Math.Nodes.Operations.Binary
{
    /// <summary>
    ///     A node representing a greater than or equal to operation.
    /// </summary>
    /// <seealso cref="ComparisonOperationNodeBase" />
    [DebuggerDisplay("{" + nameof(Left) + "} >= {" + nameof(Right) + "}")]
    internal sealed class GreaterThanOrEqualNode : ComparisonOperationNodeBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="GreaterThanOrEqualNode" /> class.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public GreaterThanOrEqualNode(
            NodeBase left,
            NodeBase right)
            : base(
                left?.Simplify(),
                right?.Simplify())
        {
        }

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>
        ///     A simplified node, or this instance.
        /// </returns>
        public override NodeBase Simplify() =>
            this.Left switch
            {
                // NumericNode nnLeft when this.Right is NumericNode nnRight => new BoolNode(
                //    Convert.ToDouble(nnLeft.Value) >= Convert.ToDouble(nnRight.Value)),
                StringNode snLeft when this.Right is StringNode snRight => new BoolNode(
                    snLeft.Value.CompareTo(snRight.Value) >= 0),
                BoolNode bnLeft when this.Right is BoolNode bnRight => new BoolNode(
                    bnLeft.Value || !bnRight.Value),
                ByteArrayNode baLeft when this.Right is ByteArrayNode baRight => new BoolNode(
                    baLeft.Value.SequenceCompareWithMsb(baRight.Value) >= 0),
                _ => this
            };

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new GreaterThanOrEqualNode(
                this.Left.DeepClone(context),
                this.Right.DeepClone(context));

        /// <summary>
        ///     Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>
        ///     The expression.
        /// </returns>
        [SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "We want it this way.")]
        protected override Expression GenerateExpressionInternal()
        {
            (Expression leftExpression, Expression rightExpression) = this.GetExpressionsOfSameTypeFromOperands();
            if (leftExpression.Type == typeof(string))
            {
                MethodInfo mi = typeof(string).GetMethodWithExactParameters(
                    nameof(string.Compare),
                    typeof(string),
                    typeof(string));
                return Expression.GreaterThanOrEqual(
                    Expression.Call(
                        mi,
                        this.Left.GenerateStringExpression(),
                        this.Right.GenerateStringExpression()),
                    Expression.Constant(
                        0,
                        typeof(int)));
            }

            if (this.Left.ReturnType == SupportedValueType.Boolean ||
                this.Right.ReturnType == SupportedValueType.Boolean)
            {
                return Expression.Condition(
                    Expression.Equal(
                        leftExpression,
                        Expression.Constant(
                            true,
                            typeof(bool))),
                    Expression.Constant(
                        true,
                        typeof(bool)),
                    Expression.Negate(rightExpression));
            }

            if (this.Left.ReturnType == SupportedValueType.ByteArray ||
                this.Right.ReturnType == SupportedValueType.ByteArray)
            {
                return Expression.GreaterThanOrEqual(
                    Expression.Call(
                        typeof(ArrayExtensions).GetMethodWithExactParameters(
                            nameof(ArrayExtensions.SequenceCompareWithMsb),
                            typeof(byte[]),
                            typeof(byte[])),
                        leftExpression,
                        rightExpression),
                    Expression.Constant(
                        0,
                        typeof(int)));
            }

            return Expression.GreaterThanOrEqual(
                leftExpression,
                rightExpression);
        }

        /// <summary>
        ///     Generates the expression with tolerance that will be compiled into code.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The expression.</returns>
        [SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "We want it this way.")]
        protected override Expression GenerateExpressionInternal(Tolerance tolerance)
        {
            (Expression leftExpression, Expression rightExpression) = this.GetExpressionsOfSameTypeFromOperands(tolerance);
            if (leftExpression.Type == typeof(string))
            {
                MethodInfo mi = typeof(string).GetMethodWithExactParameters(
                    nameof(string.Compare),
                    typeof(string),
                    typeof(string));
                return Expression.GreaterThanOrEqual(
                    Expression.Call(
                        mi,
                        this.Left.GenerateStringExpression(),
                        this.Right.GenerateStringExpression()),
                    Expression.Constant(
                        0,
                        typeof(int)));
            }

            if (this.Left.ReturnType == SupportedValueType.Boolean ||
                this.Right.ReturnType == SupportedValueType.Boolean)
            {
                return Expression.Condition(
                    Expression.Equal(
                        leftExpression,
                        Expression.Constant(
                            true,
                            typeof(bool))),
                    Expression.Constant(
                        true,
                        typeof(bool)),
                    Expression.Negate(rightExpression));
            }

            if (this.Left.ReturnType == SupportedValueType.ByteArray ||
                this.Right.ReturnType == SupportedValueType.ByteArray)
            {
                return Expression.GreaterThanOrEqual(
                    Expression.Call(
                        typeof(ArrayExtensions).GetMethodWithExactParameters(
                            nameof(ArrayExtensions.SequenceCompareWithMsb),
                            typeof(byte[]),
                            typeof(byte[])),
                        leftExpression,
                        rightExpression),
                    Expression.Constant(
                        0,
                        typeof(int)));
            }

            if (this.Left.ReturnType == SupportedValueType.Numeric &&
                this.Right.ReturnType == SupportedValueType.Numeric)
            {
                var possibleTolerantExpression = this.PossibleToleranceExpression(
                    leftExpression,
                    rightExpression,
                    tolerance);

                if (possibleTolerantExpression != null)
                {
                    // Valid tolerance expression
                    return possibleTolerantExpression;
                }
            }

            return Expression.GreaterThanOrEqual(
                leftExpression,
                rightExpression);
        }

        [SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "We want it this way.")]
        private Expression PossibleToleranceExpression([NotNull] Expression leftExpression, [NotNull] Expression rightExpression, [NotNull] Tolerance tolerance)
        {
            if (tolerance.IntegerToleranceRangeLowerBound != null)
            {
                // Integer tolerance
                MethodInfo mi = typeof(ToleranceFunctions).GetMethodWithExactParameters(
                    nameof(ToleranceFunctions.GreaterThanOrEqualRangeTolerant),
                    leftExpression.Type,
                    rightExpression.Type,
                    typeof(long));

                return Expression.Call(
                    mi,
                    leftExpression,
                    rightExpression,
                    Expression.Constant(
                        tolerance.IntegerToleranceRangeLowerBound ?? 0L,
                        typeof(long)));
            }

            if (tolerance.ToleranceRangeLowerBound != null)
            {
                // Floating-point tolerance
                MethodInfo mi = typeof(ToleranceFunctions).GetMethodWithExactParameters(
                    nameof(ToleranceFunctions.GreaterThanOrEqualRangeTolerant),
                    leftExpression.Type,
                    rightExpression.Type,
                    typeof(double));

                return Expression.Call(
                    mi,
                    leftExpression,
                    rightExpression,
                    Expression.Constant(
                        tolerance.ToleranceRangeLowerBound ?? 0D,
                        typeof(double)));
            }

            if (tolerance.ProportionalTolerance != null)
            {
                if (tolerance.ProportionalTolerance.Value > 1D)
                {
                    // Proportional tolerance
                    MethodInfo mi = typeof(ToleranceFunctions).GetMethodWithExactParameters(
                        nameof(ToleranceFunctions.GreaterThanOrEqualProportionTolerant),
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
                        nameof(ToleranceFunctions.GreaterThanOrEqualPercentageTolerant),
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