// <copyright file="EqualsNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Exceptions;
using IX.Math.Extensibility;
using IX.Math.Formatters;
using IX.Math.Nodes.Constants;
using IX.StandardExtensions.Extensions;
using JetBrains.Annotations;
using DiagCA = System.Diagnostics.CodeAnalysis;

namespace IX.Math.Nodes.Operations.Binary.Comparison
{
    /// <summary>
    ///     A node representing an equation operation.
    /// </summary>
    /// <seealso cref="ComparisonNodeBase" />
    [DebuggerDisplay("{" + nameof(Left) + "} = {" + nameof(Right) + "}")]
    internal sealed class EqualsNode : ComparisonNodeBase
    {
        private readonly bool notEqual;

        /// <summary>
        /// Initializes a new instance of the <see cref="EqualsNode" /> class.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="notEqual">if set to <c>true</c>, the node represents not equal instead of equal.</param>
        public EqualsNode(
            NodeBase left,
            NodeBase right,
            bool notEqual)
            : base(
                left?.Simplify(),
                right?.Simplify())
        {
            this.notEqual = notEqual;
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
                StringNode snLeft when this.Right is StringNode snRight && this.notEqual => new BoolNode(snLeft.Value != snRight.Value),
                StringNode snLeft when this.Right is StringNode snRight && !this.notEqual => new BoolNode(snLeft.Value == snRight.Value),
                BoolNode bnLeft when this.Right is BoolNode bnRight && this.notEqual => new BoolNode(bnLeft.Value != bnRight.Value),
                BoolNode bnLeft when this.Right is BoolNode bnRight && !this.notEqual => new BoolNode(bnLeft.Value == bnRight.Value),
                ByteArrayNode baLeft when this.Right is ByteArrayNode baRight && this.notEqual => new BoolNode(
                    !baLeft.Value.SequenceEqualsWithMsb(baRight.Value)),
                ByteArrayNode baLeft when this.Right is ByteArrayNode baRight && !this.notEqual => new BoolNode(
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
                this.Right.DeepClone(context),
                this.notEqual);

        /// <summary>
        ///     Ensures that the operands are compatible.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        protected override void EnsureCompatibleOperands(
            NodeBase left,
            NodeBase right)
        {
            switch (left.ReturnType)
            {
                case SupportedValueType.Boolean:
                    right.DetermineWeakly(SupportableValueType.Boolean | SupportableValueType.String);
                    break;
                case SupportedValueType.Numeric:
                    right.DetermineWeakly(SupportableValueType.Numeric | SupportableValueType.String);
                    break;
                case SupportedValueType.ByteArray:
                    right.DetermineWeakly(SupportableValueType.ByteArray | SupportableValueType.String);
                    break;
            }

            switch (right.ReturnType)
            {
                case SupportedValueType.Boolean:
                    left.DetermineWeakly(SupportableValueType.Boolean | SupportableValueType.String);
                    break;
                case SupportedValueType.Numeric:
                    left.DetermineWeakly(SupportableValueType.Numeric | SupportableValueType.String);
                    break;
                case SupportedValueType.ByteArray:
                    left.DetermineWeakly(SupportableValueType.ByteArray | SupportableValueType.String);
                    break;
            }
        }

        /// <summary>
        ///     Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>
        ///     The expression.
        /// </returns>
        protected override Expression GenerateExpressionInternal() =>
            this.GenerateExpressionGeneric(in ComparisonTolerance.Empty);

        /// <summary>
        ///     Generates the expression with tolerance that will be compiled into code.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The expression.</returns>
        protected override Expression GenerateExpressionInternal(in ComparisonTolerance tolerance) =>
            this.GenerateExpressionGeneric(in tolerance);

        [DiagCA.SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "We want it this way.")]
        private Expression GenerateExpressionGeneric(in ComparisonTolerance tolerance)
        {
            (Expression leftExpression, Expression rightExpression) = tolerance.IsEmpty
                ? this.GetExpressionsOfSameTypeFromOperands()
                : this.GetExpressionsOfSameTypeFromOperands(in tolerance);

            if (this.Left.ReturnType == SupportedValueType.ByteArray ||
                this.Right.ReturnType == SupportedValueType.ByteArray)
            {
                // Byte array comparison
                var callExpression = Expression.Call(
                    typeof(ArrayExtensions).GetMethodWithExactParameters(
                        nameof(ArrayExtensions.SequenceEqualsWithMsb),
                        typeof(byte[]),
                        typeof(byte[])) ?? throw new MathematicsEngineException(),
                    leftExpression,
                    rightExpression);

                return this.notEqual ? (Expression)Expression.Negate(callExpression) : callExpression;
            }

            if (this.Left.ReturnType == SupportedValueType.ByteArray ||
                this.Right.ReturnType == SupportedValueType.String)
            {
                // Byte array with string
                return CreateStringConversionExpression(
                    rightExpression,
                    leftExpression);
            }

            if (this.Left.ReturnType == SupportedValueType.String ||
                this.Right.ReturnType == SupportedValueType.ByteArray)
            {
                // Byte array with string
                return CreateStringConversionExpression(
                    leftExpression,
                    rightExpression);
            }

            if (this.Left.ReturnType == SupportedValueType.Numeric &&
                this.Right.ReturnType == SupportedValueType.Numeric &&
                !tolerance.IsEmpty)
            {
                var possibleTolerantExpression = this.GenerateNumericalToleranceEquateExpression(
                    leftExpression,
                    rightExpression,
                    in tolerance);

                if (possibleTolerantExpression != null)
                {
                    // Valid tolerance expression
                    return this.notEqual
                        ? Expression.Negate(possibleTolerantExpression)
                        : possibleTolerantExpression;
                }
            }

            if (this.Left.ReturnType == SupportedValueType.Numeric &&
                this.Right.ReturnType == SupportedValueType.String)
            {
                // Numeric with string
                return CreateStringConversionExpression(
                    rightExpression,
                    leftExpression);
            }

            if (this.Left.ReturnType == SupportedValueType.String &&
                this.Right.ReturnType == SupportedValueType.Numeric)
            {
                // String with numeric
                return CreateStringConversionExpression(
                    leftExpression,
                    rightExpression);
            }

            if (this.Left.ReturnType == SupportedValueType.Boolean &&
                this.Right.ReturnType == SupportedValueType.String)
            {
                // Boolean with string
                return CreateStringConversionExpression(
                    rightExpression,
                    leftExpression);
            }

            if (this.Left.ReturnType == SupportedValueType.String &&
                this.Right.ReturnType == SupportedValueType.Boolean)
            {
                // String with boolean
                return CreateStringConversionExpression(
                    leftExpression,
                    rightExpression);
            }

            // Exact equation
            var equalExpression = Expression.Equal(
                leftExpression,
                rightExpression);

            return this.notEqual
                ? (Expression)Expression.Negate(equalExpression)
                : equalExpression;

            // Local functions
            Expression CreateStringConversionExpression(
                Expression stringExpression,
                Expression notStringExpression)
            {
                if (!(this.SpecialObjectRequestFunction?.Invoke(typeof(IStringFormatter)) is List<IStringFormatter> formatters))
                {
                    formatters = null;
                }

                var convertedStringExpression = StringFormatter.CreateStringConversionExpression(notStringExpression, formatters);

                var stringEqualExpression = Expression.Equal(
                    convertedStringExpression,
                    stringExpression);

                return this.notEqual
                    ? (Expression)Expression.Negate(stringEqualExpression)
                    : stringEqualExpression;
            }
        }

        /// <summary>
        ///     Generates the numerical tolerance equation expression.
        /// </summary>
        /// <param name="leftExpression">The left expression.</param>
        /// <param name="rightExpression">The right expression.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>A compilable expression.</returns>
        [DiagCA.SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "We want it this way.")]
        private Expression GenerateNumericalToleranceEquateExpression(
            [NotNull] Expression leftExpression,
            [NotNull] Expression rightExpression,
            in ComparisonTolerance tolerance)
        {
            if (tolerance.IntegerToleranceRangeLowerBound != null || tolerance.IntegerToleranceRangeUpperBound != null)
            {
                // Integer tolerance
                MethodInfo mi = typeof(ToleranceFunctions).GetMethodWithExactParameters(
                    nameof(ToleranceFunctions.EquateRangeTolerant),
                    leftExpression.Type,
                    rightExpression.Type,
                    typeof(long),
                    typeof(long)) ?? throw new MathematicsEngineException();

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
                    typeof(double)) ?? throw new MathematicsEngineException();

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
                        typeof(double)) ?? throw new MathematicsEngineException();

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
                        typeof(double)) ?? throw new MathematicsEngineException();

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