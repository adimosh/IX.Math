// <copyright file="EqualsNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Nodes.Constants;
using IX.StandardExtensions.Extensions;

namespace IX.Math.Nodes.Operations.Binary
{
    /// <summary>
    ///     A node representing an equation operation.
    /// </summary>
    /// <seealso cref="IX.Math.Nodes.Operations.Binary.ComparisonOperationNodeBase" />
    [DebuggerDisplay("{" + nameof(Left) + "} = {" + nameof(Right) + "}")]
    internal sealed class EqualsNode : ComparisonOperationNodeBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EqualsNode" /> class.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public EqualsNode(
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
                NumericNode nnLeft when this.Right is NumericNode nnRight => new BoolNode(
                    Convert.ToDouble(nnLeft.Value) == Convert.ToDouble(nnRight.Value)),
                StringNode snLeft when this.Right is StringNode snRight => new BoolNode(snLeft.Value == snRight.Value),
                BoolNode bnLeft when this.Right is BoolNode bnRight => new BoolNode(bnLeft.Value == bnRight.Value),
                ByteArrayNode baLeft when this.Right is ByteArrayNode baRight => new BoolNode(
                    baLeft.Value.SequenceEqualsWithMsb(baRight.Value)),
                _ => this
            };

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new EqualsNode(
                this.Left.DeepClone(context),
                this.Right.DeepClone(context));

        /// <summary>
        ///     Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>
        ///     The expression.
        /// </returns>
        protected override Expression GenerateExpressionInternal()
        {
            (Expression leftExpression, Expression rightExpression) = this.GetExpressionsOfSameTypeFromOperands();

            if (this.Left.ReturnType == SupportedValueType.ByteArray ||
                this.Right.ReturnType == SupportedValueType.ByteArray)
            {
                return Expression.Call(
                    typeof(ArrayExtensions).GetMethodWithExactParameters(
                        nameof(ArrayExtensions.SequenceEqualsWithMsb),
                        typeof(byte[]),
                        typeof(byte[])),
                    leftExpression,
                    rightExpression);
            }

            return Expression.Equal(
                leftExpression,
                rightExpression);
        }

        /// <summary>
        ///     Generates the expression with tolerance that will be compiled into code.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The expression.</returns>
        [SuppressMessage("Performance", "HAA0601:Value type to reference type conversion causing boxing allocation", Justification = "We want it this way.")]
        protected override Expression GenerateExpressionInternal(Tolerance tolerance)
        {
            (Expression leftExpression, Expression rightExpression) =
                this.GetExpressionsOfSameTypeFromOperands(tolerance);

            if (this.Left.ReturnType == SupportedValueType.ByteArray ||
                this.Right.ReturnType == SupportedValueType.ByteArray)
            {
                return Expression.Call(
                    typeof(ArrayExtensions).GetMethodWithExactParameters(
                        nameof(ArrayExtensions.SequenceEqualsWithMsb),
                        typeof(byte[]),
                        typeof(byte[])),
                    leftExpression,
                    rightExpression);
            }

            if (this.Left.ReturnType == SupportedValueType.Numeric &&
                this.Right.ReturnType == SupportedValueType.Numeric)
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
            }

            // Exact equation
            return Expression.Equal(
                leftExpression,
                rightExpression);
        }
    }
}