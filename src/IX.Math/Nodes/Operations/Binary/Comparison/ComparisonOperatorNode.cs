// <copyright file="ComparisonOperatorNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Exceptions;
using IX.Math.Nodes.Constants;
using IX.StandardExtensions.Extensions;
using IX.StandardExtensions.Globalization;
using JetBrains.Annotations;
using DiagCA = System.Diagnostics.CodeAnalysis;

namespace IX.Math.Nodes.Operations.Binary.Comparison
{
    internal sealed class ComparisonOperatorNode : ComparisonNodeBase
    {
        private readonly bool equals;
        private readonly bool lessThan;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComparisonOperatorNode" /> class.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="equals">if set to <c>true</c>, make the operator also equate the values.</param>
        /// <param name="lessThan">if set to <c>true</c>, the operator is less than, otherwise it is greater than.</param>
        internal ComparisonOperatorNode(
            NodeBase left,
            NodeBase right,
            bool equals,
            bool lessThan)
            : base(
                left,
                right)
        {
            this.equals = equals;
            this.lessThan = lessThan;
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
                #region String simplification
                StringNode snLeft when this.Right is StringNode snRight && this.equals && this.lessThan => new BoolNode(
                    snLeft.Value.CurrentCultureCompareTo(snRight.Value) <= 0),
                StringNode snLeft when this.Right is StringNode snRight && this.equals && !this.lessThan => new BoolNode(
                    snLeft.Value.CurrentCultureCompareTo(snRight.Value) >= 0),
                StringNode snLeft when this.Right is StringNode snRight && !this.equals && this.lessThan => new BoolNode(
                    snLeft.Value.CurrentCultureCompareTo(snRight.Value) < 0),
                StringNode snLeft when this.Right is StringNode snRight && !this.equals && !this.lessThan => new BoolNode(
                    snLeft.Value.CurrentCultureCompareTo(snRight.Value) > 0),
                #endregion

                #region Boolean simplification
                BoolNode bnLeft when this.Right is BoolNode bnRight && this.equals && this.lessThan => new BoolNode(
                    !bnLeft.Value || bnRight.Value),
                BoolNode bnLeft when this.Right is BoolNode bnRight && this.equals && !this.lessThan => new BoolNode(
                    bnLeft.Value || !bnRight.Value),
                BoolNode bnLeft when this.Right is BoolNode bnRight && !this.equals && this.lessThan => new BoolNode(
                    !bnLeft.Value && bnRight.Value),
                BoolNode bnLeft when this.Right is BoolNode bnRight && !this.equals && !this.lessThan => new BoolNode(
                    bnLeft.Value && !bnRight.Value),
                #endregion

                #region Binary simplification
                ByteArrayNode baLeft when this.Right is ByteArrayNode baRight && this.equals && this.lessThan => new BoolNode(
                    baLeft.Value.SequenceCompareWithMsb(baRight.Value) <= 0),
                ByteArrayNode baLeft when this.Right is ByteArrayNode baRight && this.equals && !this.lessThan => new BoolNode(
                    baLeft.Value.SequenceCompareWithMsb(baRight.Value) >= 0),
                ByteArrayNode baLeft when this.Right is ByteArrayNode baRight && !this.equals && this.lessThan => new BoolNode(
                    baLeft.Value.SequenceCompareWithMsb(baRight.Value) < 0),
                ByteArrayNode baLeft when this.Right is ByteArrayNode baRight && !this.equals && !this.lessThan => new BoolNode(
                    baLeft.Value.SequenceCompareWithMsb(baRight.Value) > 0),
                #endregion

                _ => this
            };

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new ComparisonOperatorNode(
                this.Left.DeepClone(context),
                this.Right.DeepClone(context),
                this.equals,
                this.lessThan);

        /// <summary>
        ///     Ensures that the operands are compatible.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <exception cref="Exceptions.ExpressionNotValidLogicallyException">Expression is not valid logically.</exception>
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
                throw new Exceptions.ExpressionNotValidLogicallyException();
            }

            static void DetermineChildren(
                NodeBase parameter,
                NodeBase other)
            {
                if (other.ReturnType == SupportedValueType.Unknown)
                {
                    return;
                }

                parameter.DetermineStrongly(other.ReturnType);
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

            #region Strings
            if (leftExpression.Type == typeof(string))
            {
                MethodInfo mi = typeof(string).GetMethodWithExactParameters(
                                    nameof(string.Compare),
                                    typeof(string),
                                    typeof(string),
                                    typeof(bool),
                                    typeof(CultureInfo)) ??
                                throw new MathematicsEngineException();
                var leftOperand = Expression.Call(
                    mi,
                    this.Left.GenerateStringExpression(),
                    this.Right.GenerateStringExpression(),
                    Expression.Constant(
                        false,
                        typeof(bool)),
                    Expression.Property(
                        null,
                        typeof(CultureInfo),
                        nameof(CultureInfo.CurrentCulture)));
                var rightOperand = Expression.Constant(
                    0,
                    typeof(int));

                if (this.equals)
                {
                    if (this.lessThan)
                    {
                        return Expression.LessThanOrEqual(
                            leftOperand,
                            rightOperand);
                    }
                    else
                    {
                        return Expression.GreaterThanOrEqual(
                            leftOperand,
                            rightOperand);
                    }
                }
                else
                {
                    if (this.lessThan)
                    {
                        return Expression.LessThan(
                            leftOperand,
                            rightOperand);
                    }
                    else
                    {
                        return Expression.GreaterThan(
                            leftOperand,
                            rightOperand);
                    }
                }
            }
            #endregion

            #region Booleans
            if (this.Left.ReturnType == SupportedValueType.Boolean ||
                this.Right.ReturnType == SupportedValueType.Boolean)
            {
                var testExpression = Expression.Equal(
                    leftExpression,
                    Expression.Constant(
                        true,
                        typeof(bool)));

                if (this.equals)
                {
                    if (this.lessThan)
                    {
                        return Expression.Condition(
                            testExpression,
                            rightExpression,
                            Expression.Constant(
                                    true,
                                    typeof(bool)));
                    }
                    else
                    {
                        return Expression.Condition(
                            testExpression,
                            Expression.Constant(
                                true,
                                typeof(bool)),
                            Expression.Negate(
                                rightExpression));
                    }
                }
                else
                {
                    if (this.lessThan)
                    {
                        return Expression.Condition(
                            testExpression,
                            Expression.Constant(
                                false,
                                typeof(bool)),
                            rightExpression);
                    }
                    else
                    {
                        return Expression.Condition(
                            testExpression,
                            Expression.Negate(
                                rightExpression),
                            Expression.Constant(
                                    false,
                                    typeof(bool)));
                    }
                }
            }
            #endregion

            #region Byte arrays
            if (this.Left.ReturnType == SupportedValueType.ByteArray ||
                this.Right.ReturnType == SupportedValueType.ByteArray)
            {
                var mi = typeof(ArrayExtensions).GetMethodWithExactParameters(
                             nameof(ArrayExtensions.SequenceCompareWithMsb),
                             typeof(byte[]),
                             typeof(byte[])) ??
                         throw new MathematicsEngineException();
                var leftOperand = Expression.Call(
                    mi,
                    leftExpression,
                    rightExpression);
                var rightOperand = Expression.Constant(
                    0,
                    typeof(int));

                if (this.equals)
                {
                    if (this.lessThan)
                    {
                        return Expression.LessThanOrEqual(
                            leftOperand,
                            rightOperand);
                    }
                    else
                    {
                        return Expression.GreaterThanOrEqual(
                            leftOperand,
                            rightOperand);
                    }
                }
                else
                {
                    if (this.lessThan)
                    {
                        return Expression.LessThan(
                            leftOperand,
                            rightOperand);
                    }
                    else
                    {
                        return Expression.GreaterThan(
                            leftOperand,
                            rightOperand);
                    }
                }
            }
            #endregion

            #region Numbers

            if (!tolerance.IsEmpty)
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

            if (this.equals)
            {
                if (this.lessThan)
                {
                    return Expression.LessThanOrEqual(
                        leftExpression,
                        rightExpression);
                }
                else
                {
                    return Expression.GreaterThanOrEqual(
                        leftExpression,
                        rightExpression);
                }
            }
            else
            {
                if (this.lessThan)
                {
                    return Expression.LessThan(
                        leftExpression,
                        rightExpression);
                }
                else
                {
                    return Expression.GreaterThan(
                        leftExpression,
                        rightExpression);
                }
            }

            #endregion
        }

        [DiagCA.SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "We want it this way.")]
        private Expression PossibleToleranceExpression(
            [NotNull] Expression leftExpression,
            [NotNull] Expression rightExpression,
            in ComparisonTolerance tolerance)
        {
            #region Range
            if (this.lessThan)
            {
                if (tolerance.IntegerToleranceRangeUpperBound != null)
                {
                    // Integer tolerance
                    MethodInfo mi = typeof(ToleranceFunctions).GetMethodWithExactParameters(
                                        this.equals
                                            ? nameof(ToleranceFunctions.LessThanOrEqualRangeTolerant)
                                            : nameof(ToleranceFunctions.LessThanRangeTolerant),
                                        leftExpression.Type,
                                        rightExpression.Type,
                                        typeof(long)) ??
                                    throw new MathematicsEngineException();

                    return Expression.Call(
                        mi,
                        leftExpression,
                        rightExpression,
                        Expression.Constant(
                            tolerance.IntegerToleranceRangeUpperBound.Value,
                            typeof(long)));
                }

                if (tolerance.ToleranceRangeUpperBound != null)
                {
                    // Floating-point tolerance
                    MethodInfo mi = typeof(ToleranceFunctions).GetMethodWithExactParameters(
                                        this.equals
                                            ? nameof(ToleranceFunctions.LessThanOrEqualRangeTolerant)
                                            : nameof(ToleranceFunctions.LessThanRangeTolerant),
                                        leftExpression.Type,
                                        rightExpression.Type,
                                        typeof(double)) ??
                                    throw new MathematicsEngineException();

                    return Expression.Call(
                        mi,
                        leftExpression,
                        rightExpression,
                        Expression.Constant(
                            tolerance.ToleranceRangeUpperBound.Value,
                            typeof(double)));
                }
            }
            else
            {
                if (tolerance.IntegerToleranceRangeLowerBound != null)
                {
                    // Integer tolerance
                    MethodInfo mi = typeof(ToleranceFunctions).GetMethodWithExactParameters(
                                        this.equals
                                            ? nameof(ToleranceFunctions.GreaterThanOrEqualRangeTolerant)
                                            : nameof(ToleranceFunctions.GreaterThanRangeTolerant),
                                        leftExpression.Type,
                                        rightExpression.Type,
                                        typeof(long)) ??
                                    throw new MathematicsEngineException();

                    return Expression.Call(
                        mi,
                        leftExpression,
                        rightExpression,
                        Expression.Constant(
                            tolerance.IntegerToleranceRangeLowerBound.Value,
                            typeof(long)));
                }

                if (tolerance.ToleranceRangeLowerBound != null)
                {
                    // Floating-point tolerance
                    MethodInfo mi = typeof(ToleranceFunctions).GetMethodWithExactParameters(
                                        this.equals
                                            ? nameof(ToleranceFunctions.GreaterThanOrEqualRangeTolerant)
                                            : nameof(ToleranceFunctions.GreaterThanRangeTolerant),
                                        leftExpression.Type,
                                        rightExpression.Type,
                                        typeof(double)) ??
                                    throw new MathematicsEngineException();

                    return Expression.Call(
                        mi,
                        leftExpression,
                        rightExpression,
                        Expression.Constant(
                            tolerance.ToleranceRangeLowerBound.Value,
                            typeof(double)));
                }
            }
            #endregion

            if (tolerance.ProportionalTolerance != null)
            {
                #region Proportion
                if (tolerance.ProportionalTolerance.Value > 1D)
                {
                    // Proportional tolerance
                    string methodName;
                    if (this.equals)
                    {
                        if (this.lessThan)
                        {
                            methodName = nameof(ToleranceFunctions.LessThanOrEqualProportionTolerant);
                        }
                        else
                        {
                            methodName = nameof(ToleranceFunctions.GreaterThanOrEqualProportionTolerant);
                        }
                    }
                    else
                    {
                        if (this.lessThan)
                        {
                            methodName = nameof(ToleranceFunctions.LessThanProportionTolerant);
                        }
                        else
                        {
                            methodName = nameof(ToleranceFunctions.GreaterThanProportionTolerant);
                        }
                    }

                    MethodInfo mi = typeof(ToleranceFunctions).GetMethodWithExactParameters(
                        methodName,
                        leftExpression.Type,
                        rightExpression.Type,
                        typeof(double)) ?? throw new MathematicsEngineException();

                    return Expression.Call(
                        mi,
                        leftExpression,
                        rightExpression,
                        Expression.Constant(
                            tolerance.ProportionalTolerance.Value,
                            typeof(double)));
                }
                #endregion

                #region Percentage
                if (tolerance.ProportionalTolerance.Value < 1D && tolerance.ProportionalTolerance.Value > 0D)
                {
                    // Percentage tolerance
                    string methodName;
                    if (this.equals)
                    {
                        if (this.lessThan)
                        {
                            methodName = nameof(ToleranceFunctions.LessThanOrEqualPercentageTolerant);
                        }
                        else
                        {
                            methodName = nameof(ToleranceFunctions.GreaterThanOrEqualPercentageTolerant);
                        }
                    }
                    else
                    {
                        if (this.lessThan)
                        {
                            methodName = nameof(ToleranceFunctions.LessThanPercentageTolerant);
                        }
                        else
                        {
                            methodName = nameof(ToleranceFunctions.GreaterThanPercentageTolerant);
                        }
                    }

                    MethodInfo mi = typeof(ToleranceFunctions).GetMethodWithExactParameters(
                        methodName,
                        leftExpression.Type,
                        rightExpression.Type,
                        typeof(double)) ?? throw new MathematicsEngineException();

                    return Expression.Call(
                        mi,
                        leftExpression,
                        rightExpression,
                        Expression.Constant(
                            tolerance.ProportionalTolerance.Value,
                            typeof(double)));
                }
                #endregion
            }

            return null;
        }
    }
}