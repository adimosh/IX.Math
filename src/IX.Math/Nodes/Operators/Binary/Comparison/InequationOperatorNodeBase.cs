// <copyright file="InequationOperatorNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Collections.Generic;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Exceptions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using IX.StandardExtensions.Extensions;
using IX.StandardExtensions.Globalization;
using JetBrains.Annotations;
using DiagCA = System.Diagnostics.CodeAnalysis;

namespace IX.Math.Nodes.Operators.Binary.Comparison
{
    internal abstract class InequationOperatorNodeBase : ComparisonNodeBase
    {
        private readonly bool equals;
        private readonly bool lessThan;

        /// <summary>
        /// Initializes a new instance of the <see cref="InequationOperatorNodeBase" /> class.
        /// </summary>
        /// <param name="stringFormatters">The string formatters.</param>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="equals">if set to <c>true</c>, make the operator also equate the values.</param>
        /// <param name="lessThan">if set to <c>true</c>, the operator is less than, otherwise it is greater than.</param>
        internal InequationOperatorNodeBase(
            List<IStringFormatter> stringFormatters,
            NodeBase left,
            NodeBase right,
            bool equals,
            bool lessThan)
            : base(
                stringFormatters,
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
                StringNode snLeft when this.Right is StringNode snRight && this.equals && this.lessThan => this.GenerateConstantBoolean(
                    snLeft.Value.CurrentCultureCompareTo(snRight.Value) <= 0),
                StringNode snLeft when this.Right is StringNode snRight && this.equals && !this.lessThan => this.GenerateConstantBoolean(
                    snLeft.Value.CurrentCultureCompareTo(snRight.Value) >= 0),
                StringNode snLeft when this.Right is StringNode snRight && !this.equals && this.lessThan => this.GenerateConstantBoolean(
                    snLeft.Value.CurrentCultureCompareTo(snRight.Value) < 0),
                StringNode snLeft when this.Right is StringNode snRight && !this.equals && !this.lessThan => this.GenerateConstantBoolean(
                    snLeft.Value.CurrentCultureCompareTo(snRight.Value) > 0),
                #endregion

                #region Boolean simplification
                BoolNode bnLeft when this.Right is BoolNode bnRight && this.equals && this.lessThan => this.GenerateConstantBoolean(
                    !bnLeft.Value || bnRight.Value),
                BoolNode bnLeft when this.Right is BoolNode bnRight && this.equals && !this.lessThan => this.GenerateConstantBoolean(
                    bnLeft.Value || !bnRight.Value),
                BoolNode bnLeft when this.Right is BoolNode bnRight && !this.equals && this.lessThan => this.GenerateConstantBoolean(
                    !bnLeft.Value && bnRight.Value),
                BoolNode bnLeft when this.Right is BoolNode bnRight && !this.equals && !this.lessThan => this.GenerateConstantBoolean(
                    bnLeft.Value && !bnRight.Value),
                #endregion

                #region Binary simplification
                ByteArrayNode baLeft when this.Right is ByteArrayNode baRight && this.equals && this.lessThan => this.GenerateConstantBoolean(
                    baLeft.Value.SequenceCompareWithMsb(baRight.Value) <= 0),
                ByteArrayNode baLeft when this.Right is ByteArrayNode baRight && this.equals && !this.lessThan => this.GenerateConstantBoolean(
                    baLeft.Value.SequenceCompareWithMsb(baRight.Value) >= 0),
                ByteArrayNode baLeft when this.Right is ByteArrayNode baRight && !this.equals && this.lessThan => this.GenerateConstantBoolean(
                    baLeft.Value.SequenceCompareWithMsb(baRight.Value) < 0),
                ByteArrayNode baLeft when this.Right is ByteArrayNode baRight && !this.equals && !this.lessThan => this.GenerateConstantBoolean(
                    baLeft.Value.SequenceCompareWithMsb(baRight.Value) > 0),
                #endregion

                _ => this
            };

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
        protected override Expression GenerateExpressionInternal(
            in SupportedValueType valueType,
            in ComparisonTolerance comparisonTolerance)
        {
            var (leftExpression, rightExpression, internalType) = this.GetExpressionArguments(in comparisonTolerance);

            switch (internalType)
            {
#region Strings

                case SupportedValueType.String:
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
                        leftExpression,
                        rightExpression,
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

                case SupportedValueType.Boolean:
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
                                Expression.Negate(rightExpression));
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
                                Expression.Negate(rightExpression),
                                Expression.Constant(
                                    false,
                                    typeof(bool)));
                        }
                    }
                }

#endregion

#region Byte arrays

                case SupportedValueType.ByteArray:
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

                case SupportedValueType.Integer:
                case SupportedValueType.Numeric:
                {
                    if (!comparisonTolerance.IsEmpty)
                    {
                        var possibleTolerantExpression = this.PossibleToleranceExpression(
                            leftExpression,
                            rightExpression,
                            in comparisonTolerance);

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
                }

#endregion
            }

            throw new MathematicsEngineException();
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