// <copyright file="InequationOperatorNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Conversion;
using IX.Math.Exceptions;
using IX.Math.Formatters;
using IX.Math.Nodes.Constants;
using IX.Math.WorkingSet;
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

        private bool isConstant;

        /// <summary>
        /// Initializes a new instance of the <see cref="InequationOperatorNodeBase" /> class.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <param name="equals">if set to <c>true</c>, make the operator also equate the values.</param>
        /// <param name="lessThan">if set to <c>true</c>, the operator is less than, otherwise it is greater than.</param>
        internal InequationOperatorNodeBase(
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
        ///     Gets a value indicating whether or not this node is actually a constant.
        /// </summary>
        /// <value><see langword="true" /> if the node is a constant, <see langword="false" /> otherwise.</value>
        public sealed override bool IsConstant => this.isConstant;

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>
        ///     A simplified node, or this instance.
        /// </returns>
        public sealed override NodeBase Simplify()
        {
            // Let's check that both are constant
            if (!(this.Left is ConstantNodeBase left) || !(this.Right is ConstantNodeBase right))
            {
                return this;
            }

            // If they are both boolean, we should treat that case first
            if (left.TryGetBoolean(out bool bvl) && right.TryGetBoolean(out bool bvr))
            {
                this.isConstant = true;

                return (this.equals, this.lessThan) switch
                {
                    (true, true) => new BoolNode(!bvl || bvr),
                    (false, true) => new BoolNode(!bvl || bvr),
                    (true, false) => new BoolNode(bvl || !bvr),
                    (false, false) => new BoolNode(bvl || !bvr)
                };
            }

            // If they are both binary, but not both integer or numeric
            if (left.TryGetBinary(out byte[] bavl) &&
                right.TryGetBinary(out byte[] bavr))
            {
                bool bli = left.CheckSupportedType(SupportableValueType.Integer);
                bool bln = left.CheckSupportedType(SupportableValueType.Numeric);
                bool bri = right.CheckSupportedType(SupportableValueType.Integer);
                bool brn = right.CheckSupportedType(SupportableValueType.Numeric);

                if ((bli || bln) && (bri || brn))
                {
                    return this;
                }

                this.isConstant = true;

                return (this.equals, this.lessThan) switch
                {
                    (true, true) => new BoolNode(bavl.SequenceCompareWithMsb(bavr) >= 0),
                    (false, true) => new BoolNode(bavl.SequenceCompareWithMsb(bavr) > 0),
                    (true, false) => new BoolNode(bavl.SequenceCompareWithMsb(bavr) <= 0),
                    (false, false) => new BoolNode(bavl.SequenceCompareWithMsb(bavr) < 0)
                };
            }

            // If they are both string, but not both integer or numeric
            if (left.TryGetString(out string svl) &&
                right.TryGetString(out string svr))
            {
                bool bli = left.CheckSupportedType(SupportableValueType.Integer);
                bool bln = left.CheckSupportedType(SupportableValueType.Numeric);
                bool bri = right.CheckSupportedType(SupportableValueType.Integer);
                bool brn = right.CheckSupportedType(SupportableValueType.Numeric);

                if (bli || bln || bri || brn)
                {
                    return this;
                }

                this.isConstant = true;

                return (this.equals, this.lessThan) switch
                {
                    (true, true) => new BoolNode(svl.CurrentCultureCompareTo(svr) <= 0),
                    (false, true) => new BoolNode(svl.CurrentCultureCompareTo(svr) < 0),
                    (true, false) => new BoolNode(svl.CurrentCultureCompareTo(svr) >= 0),
                    (false, false) => new BoolNode(svl.CurrentCultureCompareTo(svr) > 0)
                };
            }

            return this;
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

                    case SupportedValueType.Binary:
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

                    case SupportedValueType.Unknown when !comparisonTolerance.IsEmpty:
                        {
                            // We check whether we have a string
                            Expression stringExp, numericExp;
                            bool equalsLocal, lessThanLocal;

                            if (leftExpression.Type == typeof(string))
                            {
                                stringExp = leftExpression;
                                numericExp = rightExpression;
                                equalsLocal = this.equals;
                                lessThanLocal = this.lessThan;
                            }
                            else if (rightExpression.Type == typeof(string))
                            {
                                stringExp = rightExpression;
                                numericExp = leftExpression;
                                equalsLocal = !this.equals;
                                lessThanLocal = !this.lessThan;
                            }
                            else
                            {
                                // We don't have a string, this simply means we didn't achieve compatibility
                                throw new ExpressionNotValidLogicallyException();
                            }

                            return GeneratePossibleToleranceExpression(in comparisonTolerance);

                            Expression GeneratePossibleToleranceExpression(in ComparisonTolerance tolerance)
                            {
                                #region Empty tolerance
                                if (tolerance.IsEmpty)
                                {
                                    return Expression.Call(
                                        numericExp.Type == typeof(long) ?
                                            ((Func<string, long, bool>)EqualizeNoToleranceInteger).Method :
                                            ((Func<string, double, bool>)EqualizeNoToleranceNumeric).Method,
                                        stringExp,
                                        numericExp);
                                }
                                #endregion

                                #region Integer range tolerance
                                if (tolerance.IntegerToleranceRangeLowerBound != null ||
                                    tolerance.IntegerToleranceRangeUpperBound != null)
                                {
                                    // Integer range tolerance
                                    MethodInfo methodInfo;
                                    Expression toleranceExpression;

                                    switch ((lessThanLocal, equalsLocal))
                                    {
                                        case (true, true):
                                            toleranceExpression = Expression.Constant(
                                                tolerance.IntegerToleranceRangeUpperBound ?? 0L,
                                                typeof(long));
                                            methodInfo = numericExp.Type == typeof(long)
                                                ? ((Func<string, long, long, bool>)
                                                    LessThanOrEqualIntegerToleranceRangeInteger).Method
                                                : ((Func<string, double, long, bool>)
                                                    LessThanOrEqualIntegerToleranceRangeNumeric).Method;

                                            break;
                                        case (true, false):
                                            toleranceExpression = Expression.Constant(
                                                tolerance.IntegerToleranceRangeUpperBound ?? 0L,
                                                typeof(long));
                                            methodInfo = numericExp.Type == typeof(long)
                                                ? ((Func<string, long, long, bool>)
                                                    LessThanIntegerToleranceRangeInteger).Method
                                                : ((Func<string, double, long, bool>)
                                                    LessThanIntegerToleranceRangeNumeric).Method;

                                            break;
                                        case (false, true):
                                            toleranceExpression = Expression.Constant(
                                                tolerance.IntegerToleranceRangeLowerBound ?? 0L,
                                                typeof(long));
                                            methodInfo = numericExp.Type == typeof(long)
                                                ? ((Func<string, long, long, bool>)
                                                    GreaterThanOrEqualIntegerToleranceRangeInteger).Method
                                                : ((Func<string, double, long, bool>)
                                                    GreaterThanOrEqualIntegerToleranceRangeNumeric).Method;

                                            break;
                                        case (false, false):
                                            toleranceExpression = Expression.Constant(
                                                tolerance.IntegerToleranceRangeLowerBound ?? 0L,
                                                typeof(long));
                                            methodInfo = numericExp.Type == typeof(long)
                                                ? ((Func<string, long, long, bool>)
                                                    GreaterThanIntegerToleranceRangeInteger).Method
                                                : ((Func<string, double, long, bool>)
                                                    GreaterThanIntegerToleranceRangeNumeric).Method;

                                            break;
                                    }

                                    return Expression.Call(
                                        methodInfo,
                                        stringExp,
                                        numericExp,
                                        toleranceExpression);

                                    static bool GreaterThanIntegerToleranceRangeInteger(
                                        string stringValue,
                                        long integerValue,
                                        long lowerTolerance)
                                    {
                                        if (WorkingExpressionSet.TryInterpretStringValue(
                                            stringValue,
                                            out object convertedValue))
                                        {
                                            switch (convertedValue)
                                            {
                                                case long l:
                                                    {
                                                        return ToleranceFunctions.GreaterThanRangeTolerant(
                                                            integerValue,
                                                            l,
                                                            lowerTolerance);
                                                    }

                                                case double d:
                                                    {
                                                        if (InternalTypeDirectConversions.ToInteger(
                                                            d,
                                                            out var l))
                                                        {
                                                            return ToleranceFunctions.GreaterThanRangeTolerant(
                                                                integerValue,
                                                                l,
                                                                lowerTolerance);
                                                        }

                                                        return ToleranceFunctions.GreaterThanRangeTolerant(
                                                            Convert.ToDouble(integerValue),
                                                            l,
                                                            lowerTolerance);
                                                    }
                                            }
                                        }

                                        return string.Compare(
                                                   stringValue,
                                                   StringFormatter.FormatIntoString(integerValue),
                                                   StringComparison.CurrentCulture) >
                                               0;
                                    }

                                    static bool GreaterThanOrEqualIntegerToleranceRangeInteger(
                                        string stringValue,
                                        long integerValue,
                                        long lowerTolerance)
                                    {
                                        if (WorkingExpressionSet.TryInterpretStringValue(
                                            stringValue,
                                            out object convertedValue))
                                        {
                                            switch (convertedValue)
                                            {
                                                case long l:
                                                    {
                                                        return ToleranceFunctions.GreaterThanOrEqualRangeTolerant(
                                                            integerValue,
                                                            l,
                                                            lowerTolerance);
                                                    }

                                                case double d:
                                                    {
                                                        if (InternalTypeDirectConversions.ToInteger(
                                                            d,
                                                            out var l))
                                                        {
                                                            return ToleranceFunctions.GreaterThanOrEqualRangeTolerant(
                                                                integerValue,
                                                                l,
                                                                lowerTolerance);
                                                        }

                                                        return ToleranceFunctions.GreaterThanOrEqualRangeTolerant(
                                                            Convert.ToDouble(integerValue),
                                                            l,
                                                            lowerTolerance);
                                                    }
                                            }
                                        }

                                        return string.Compare(
                                                   stringValue,
                                                   StringFormatter.FormatIntoString(integerValue),
                                                   StringComparison.CurrentCulture) >=
                                               0;
                                    }

                                    static bool LessThanIntegerToleranceRangeInteger(
                                        string stringValue,
                                        long integerValue,
                                        long lowerTolerance)
                                    {
                                        if (WorkingExpressionSet.TryInterpretStringValue(
                                            stringValue,
                                            out object convertedValue))
                                        {
                                            switch (convertedValue)
                                            {
                                                case long l:
                                                    {
                                                        return ToleranceFunctions.LessThanRangeTolerant(
                                                            integerValue,
                                                            l,
                                                            lowerTolerance);
                                                    }

                                                case double d:
                                                    {
                                                        if (InternalTypeDirectConversions.ToInteger(
                                                            d,
                                                            out var l))
                                                        {
                                                            return ToleranceFunctions.LessThanRangeTolerant(
                                                                integerValue,
                                                                l,
                                                                lowerTolerance);
                                                        }

                                                        return ToleranceFunctions.LessThanRangeTolerant(
                                                            Convert.ToDouble(integerValue),
                                                            l,
                                                            lowerTolerance);
                                                    }
                                            }
                                        }

                                        return string.Compare(
                                                   stringValue,
                                                   StringFormatter.FormatIntoString(integerValue),
                                                   StringComparison.CurrentCulture) <
                                               0;
                                    }

                                    static bool LessThanOrEqualIntegerToleranceRangeInteger(
                                        string stringValue,
                                        long integerValue,
                                        long lowerTolerance)
                                    {
                                        if (WorkingExpressionSet.TryInterpretStringValue(
                                            stringValue,
                                            out object convertedValue))
                                        {
                                            switch (convertedValue)
                                            {
                                                case long l:
                                                    {
                                                        return ToleranceFunctions.LessThanOrEqualRangeTolerant(
                                                            integerValue,
                                                            l,
                                                            lowerTolerance);
                                                    }

                                                case double d:
                                                    {
                                                        if (InternalTypeDirectConversions.ToInteger(
                                                            d,
                                                            out var l))
                                                        {
                                                            return ToleranceFunctions.LessThanOrEqualRangeTolerant(
                                                                integerValue,
                                                                l,
                                                                lowerTolerance);
                                                        }

                                                        return ToleranceFunctions.LessThanOrEqualRangeTolerant(
                                                            Convert.ToDouble(integerValue),
                                                            l,
                                                            lowerTolerance);
                                                    }
                                            }
                                        }

                                        return string.Compare(
                                                   stringValue,
                                                   StringFormatter.FormatIntoString(integerValue),
                                                   StringComparison.CurrentCulture) <=
                                               0;
                                    }

                                    static bool GreaterThanIntegerToleranceRangeNumeric(
                                        string stringValue,
                                        double integerValue,
                                        long lowerTolerance)
                                    {
                                        if (WorkingExpressionSet.TryInterpretStringValue(
                                            stringValue,
                                            out object convertedValue))
                                        {
                                            switch (convertedValue)
                                            {
                                                case long l:
                                                    {
                                                        return ToleranceFunctions.GreaterThanRangeTolerant(
                                                            integerValue,
                                                            l,
                                                            lowerTolerance);
                                                    }

                                                case double d:
                                                    {
                                                        if (InternalTypeDirectConversions.ToInteger(
                                                            d,
                                                            out var l))
                                                        {
                                                            return ToleranceFunctions.GreaterThanRangeTolerant(
                                                                integerValue,
                                                                l,
                                                                lowerTolerance);
                                                        }

                                                        return ToleranceFunctions.GreaterThanRangeTolerant(
                                                            Convert.ToDouble(integerValue),
                                                            l,
                                                            lowerTolerance);
                                                    }
                                            }
                                        }

                                        return string.Compare(
                                                   stringValue,
                                                   StringFormatter.FormatIntoString(integerValue),
                                                   StringComparison.CurrentCulture) >
                                               0;
                                    }

                                    static bool GreaterThanOrEqualIntegerToleranceRangeNumeric(
                                        string stringValue,
                                        double integerValue,
                                        long lowerTolerance)
                                    {
                                        if (WorkingExpressionSet.TryInterpretStringValue(
                                            stringValue,
                                            out object convertedValue))
                                        {
                                            switch (convertedValue)
                                            {
                                                case long l:
                                                    {
                                                        return ToleranceFunctions.GreaterThanOrEqualRangeTolerant(
                                                            integerValue,
                                                            l,
                                                            lowerTolerance);
                                                    }

                                                case double d:
                                                    {
                                                        if (InternalTypeDirectConversions.ToInteger(
                                                            d,
                                                            out var l))
                                                        {
                                                            return ToleranceFunctions.GreaterThanOrEqualRangeTolerant(
                                                                integerValue,
                                                                l,
                                                                lowerTolerance);
                                                        }

                                                        return ToleranceFunctions.GreaterThanOrEqualRangeTolerant(
                                                            Convert.ToDouble(integerValue),
                                                            l,
                                                            lowerTolerance);
                                                    }
                                            }
                                        }

                                        return string.Compare(
                                                   stringValue,
                                                   StringFormatter.FormatIntoString(integerValue),
                                                   StringComparison.CurrentCulture) >=
                                               0;
                                    }

                                    static bool LessThanIntegerToleranceRangeNumeric(
                                        string stringValue,
                                        double integerValue,
                                        long lowerTolerance)
                                    {
                                        if (WorkingExpressionSet.TryInterpretStringValue(
                                            stringValue,
                                            out object convertedValue))
                                        {
                                            switch (convertedValue)
                                            {
                                                case long l:
                                                    {
                                                        return ToleranceFunctions.LessThanRangeTolerant(
                                                            integerValue,
                                                            l,
                                                            lowerTolerance);
                                                    }

                                                case double d:
                                                    {
                                                        if (InternalTypeDirectConversions.ToInteger(
                                                            d,
                                                            out var l))
                                                        {
                                                            return ToleranceFunctions.LessThanRangeTolerant(
                                                                integerValue,
                                                                l,
                                                                lowerTolerance);
                                                        }

                                                        return ToleranceFunctions.LessThanRangeTolerant(
                                                            Convert.ToDouble(integerValue),
                                                            l,
                                                            lowerTolerance);
                                                    }
                                            }
                                        }

                                        return string.Compare(
                                                   stringValue,
                                                   StringFormatter.FormatIntoString(integerValue),
                                                   StringComparison.CurrentCulture) <
                                               0;
                                    }

                                    static bool LessThanOrEqualIntegerToleranceRangeNumeric(
                                        string stringValue,
                                        double integerValue,
                                        long lowerTolerance)
                                    {
                                        if (WorkingExpressionSet.TryInterpretStringValue(
                                            stringValue,
                                            out object convertedValue))
                                        {
                                            switch (convertedValue)
                                            {
                                                case long l:
                                                    {
                                                        return ToleranceFunctions.LessThanOrEqualRangeTolerant(
                                                            integerValue,
                                                            l,
                                                            lowerTolerance);
                                                    }

                                                case double d:
                                                    {
                                                        if (InternalTypeDirectConversions.ToInteger(
                                                            d,
                                                            out var l))
                                                        {
                                                            return ToleranceFunctions.LessThanOrEqualRangeTolerant(
                                                                integerValue,
                                                                l,
                                                                lowerTolerance);
                                                        }

                                                        return ToleranceFunctions.LessThanOrEqualRangeTolerant(
                                                            Convert.ToDouble(integerValue),
                                                            l,
                                                            lowerTolerance);
                                                    }
                                            }
                                        }

                                        return string.Compare(
                                                   stringValue,
                                                   StringFormatter.FormatIntoString(integerValue),
                                                   StringComparison.CurrentCulture) <=
                                               0;
                                    }
                                }
                                #endregion

                                #region Numeric range tolerance
                                if (tolerance.ToleranceRangeLowerBound != null ||
                                    tolerance.ToleranceRangeUpperBound != null)
                                {
                                    // Floating-point range tolerance
                                    MethodInfo methodInfo;
                                    Expression toleranceExpression;

                                    switch ((lessThanLocal, equalsLocal))
                                    {
                                        case (true, true):
                                            toleranceExpression = Expression.Constant(
                                                tolerance.ToleranceRangeUpperBound ?? 0L,
                                                typeof(double));
                                            methodInfo = numericExp.Type == typeof(long)
                                                ? ((Func<string, long, double, bool>)
                                                    LessThanOrEqualNumericToleranceRangeInteger).Method
                                                : ((Func<string, double, double, bool>)
                                                    LessThanOrEqualNumericToleranceRangeNumeric).Method;

                                            break;
                                        case (true, false):
                                            toleranceExpression = Expression.Constant(
                                                tolerance.ToleranceRangeUpperBound ?? 0L,
                                                typeof(double));
                                            methodInfo = numericExp.Type == typeof(long)
                                                ? ((Func<string, long, double, bool>)
                                                    LessThanNumericToleranceRangeInteger).Method
                                                : ((Func<string, double, double, bool>)
                                                    LessThanNumericToleranceRangeNumeric).Method;

                                            break;
                                        case (false, true):
                                            toleranceExpression = Expression.Constant(
                                                tolerance.ToleranceRangeLowerBound ?? 0L,
                                                typeof(double));
                                            methodInfo = numericExp.Type == typeof(long)
                                                ? ((Func<string, long, double, bool>)
                                                    GreaterThanOrEqualNumericToleranceRangeInteger).Method
                                                : ((Func<string, double, double, bool>)
                                                    GreaterThanOrEqualNumericToleranceRangeNumeric).Method;

                                            break;
                                        case (false, false):
                                            toleranceExpression = Expression.Constant(
                                                tolerance.ToleranceRangeLowerBound ?? 0L,
                                                typeof(double));
                                            methodInfo = numericExp.Type == typeof(long)
                                                ? ((Func<string, long, double, bool>)
                                                    GreaterThanNumericToleranceRangeInteger).Method
                                                : ((Func<string, double, double, bool>)
                                                    GreaterThanNumericToleranceRangeNumeric).Method;

                                            break;
                                    }

                                    return Expression.Call(
                                        methodInfo,
                                        stringExp,
                                        numericExp,
                                        toleranceExpression);

                                    static bool GreaterThanNumericToleranceRangeInteger(
                                        string stringValue,
                                        long integerValue,
                                        double lowerTolerance)
                                    {
                                        if (WorkingExpressionSet.TryInterpretStringValue(
                                            stringValue,
                                            out object convertedValue))
                                        {
                                            switch (convertedValue)
                                            {
                                                case long l:
                                                    {
                                                        return ToleranceFunctions.GreaterThanRangeTolerant(
                                                            integerValue,
                                                            l,
                                                            lowerTolerance);
                                                    }

                                                case double d:
                                                    {
                                                        if (InternalTypeDirectConversions.ToInteger(
                                                            d,
                                                            out var l))
                                                        {
                                                            return ToleranceFunctions.GreaterThanRangeTolerant(
                                                                integerValue,
                                                                l,
                                                                lowerTolerance);
                                                        }

                                                        return ToleranceFunctions.GreaterThanRangeTolerant(
                                                            Convert.ToDouble(integerValue),
                                                            l,
                                                            lowerTolerance);
                                                    }
                                            }
                                        }

                                        return string.Compare(
                                                   stringValue,
                                                   StringFormatter.FormatIntoString(integerValue),
                                                   StringComparison.CurrentCulture) >
                                               0;
                                    }

                                    static bool GreaterThanOrEqualNumericToleranceRangeInteger(
                                        string stringValue,
                                        long integerValue,
                                        double lowerTolerance)
                                    {
                                        if (WorkingExpressionSet.TryInterpretStringValue(
                                            stringValue,
                                            out object convertedValue))
                                        {
                                            switch (convertedValue)
                                            {
                                                case long l:
                                                    {
                                                        return ToleranceFunctions.GreaterThanOrEqualRangeTolerant(
                                                            integerValue,
                                                            l,
                                                            lowerTolerance);
                                                    }

                                                case double d:
                                                    {
                                                        if (InternalTypeDirectConversions.ToInteger(
                                                            d,
                                                            out var l))
                                                        {
                                                            return ToleranceFunctions.GreaterThanOrEqualRangeTolerant(
                                                                integerValue,
                                                                l,
                                                                lowerTolerance);
                                                        }

                                                        return ToleranceFunctions.GreaterThanOrEqualRangeTolerant(
                                                            Convert.ToDouble(integerValue),
                                                            l,
                                                            lowerTolerance);
                                                    }
                                            }
                                        }

                                        return string.Compare(
                                                   stringValue,
                                                   StringFormatter.FormatIntoString(integerValue),
                                                   StringComparison.CurrentCulture) >=
                                               0;
                                    }

                                    static bool LessThanNumericToleranceRangeInteger(
                                        string stringValue,
                                        long integerValue,
                                        double lowerTolerance)
                                    {
                                        if (WorkingExpressionSet.TryInterpretStringValue(
                                            stringValue,
                                            out object convertedValue))
                                        {
                                            switch (convertedValue)
                                            {
                                                case long l:
                                                    {
                                                        return ToleranceFunctions.LessThanRangeTolerant(
                                                            integerValue,
                                                            l,
                                                            lowerTolerance);
                                                    }

                                                case double d:
                                                    {
                                                        if (InternalTypeDirectConversions.ToInteger(
                                                            d,
                                                            out var l))
                                                        {
                                                            return ToleranceFunctions.LessThanRangeTolerant(
                                                                integerValue,
                                                                l,
                                                                lowerTolerance);
                                                        }

                                                        return ToleranceFunctions.LessThanRangeTolerant(
                                                            Convert.ToDouble(integerValue),
                                                            l,
                                                            lowerTolerance);
                                                    }
                                            }
                                        }

                                        return string.Compare(
                                                   stringValue,
                                                   StringFormatter.FormatIntoString(integerValue),
                                                   StringComparison.CurrentCulture) <
                                               0;
                                    }

                                    static bool LessThanOrEqualNumericToleranceRangeInteger(
                                        string stringValue,
                                        long integerValue,
                                        double lowerTolerance)
                                    {
                                        if (WorkingExpressionSet.TryInterpretStringValue(
                                            stringValue,
                                            out object convertedValue))
                                        {
                                            switch (convertedValue)
                                            {
                                                case long l:
                                                    {
                                                        return ToleranceFunctions.LessThanOrEqualRangeTolerant(
                                                            integerValue,
                                                            l,
                                                            lowerTolerance);
                                                    }

                                                case double d:
                                                    {
                                                        if (InternalTypeDirectConversions.ToInteger(
                                                            d,
                                                            out var l))
                                                        {
                                                            return ToleranceFunctions.LessThanOrEqualRangeTolerant(
                                                                integerValue,
                                                                l,
                                                                lowerTolerance);
                                                        }

                                                        return ToleranceFunctions.LessThanOrEqualRangeTolerant(
                                                            Convert.ToDouble(integerValue),
                                                            l,
                                                            lowerTolerance);
                                                    }
                                            }
                                        }

                                        return string.Compare(
                                                   stringValue,
                                                   StringFormatter.FormatIntoString(integerValue),
                                                   StringComparison.CurrentCulture) <=
                                               0;
                                    }

                                    static bool GreaterThanNumericToleranceRangeNumeric(
                                        string stringValue,
                                        double integerValue,
                                        double lowerTolerance)
                                    {
                                        if (WorkingExpressionSet.TryInterpretStringValue(
                                            stringValue,
                                            out object convertedValue))
                                        {
                                            switch (convertedValue)
                                            {
                                                case long l:
                                                    {
                                                        return ToleranceFunctions.GreaterThanRangeTolerant(
                                                            integerValue,
                                                            l,
                                                            lowerTolerance);
                                                    }

                                                case double d:
                                                    {
                                                        if (InternalTypeDirectConversions.ToInteger(
                                                            d,
                                                            out var l))
                                                        {
                                                            return ToleranceFunctions.GreaterThanRangeTolerant(
                                                                integerValue,
                                                                l,
                                                                lowerTolerance);
                                                        }

                                                        return ToleranceFunctions.GreaterThanRangeTolerant(
                                                            Convert.ToDouble(integerValue),
                                                            l,
                                                            lowerTolerance);
                                                    }
                                            }
                                        }

                                        return string.Compare(
                                                   stringValue,
                                                   StringFormatter.FormatIntoString(integerValue),
                                                   StringComparison.CurrentCulture) >
                                               0;
                                    }

                                    static bool GreaterThanOrEqualNumericToleranceRangeNumeric(
                                        string stringValue,
                                        double integerValue,
                                        double lowerTolerance)
                                    {
                                        if (WorkingExpressionSet.TryInterpretStringValue(
                                            stringValue,
                                            out object convertedValue))
                                        {
                                            switch (convertedValue)
                                            {
                                                case long l:
                                                    {
                                                        return ToleranceFunctions.GreaterThanOrEqualRangeTolerant(
                                                            integerValue,
                                                            l,
                                                            lowerTolerance);
                                                    }

                                                case double d:
                                                    {
                                                        if (InternalTypeDirectConversions.ToInteger(
                                                            d,
                                                            out var l))
                                                        {
                                                            return ToleranceFunctions.GreaterThanOrEqualRangeTolerant(
                                                                integerValue,
                                                                l,
                                                                lowerTolerance);
                                                        }

                                                        return ToleranceFunctions.GreaterThanOrEqualRangeTolerant(
                                                            Convert.ToDouble(integerValue),
                                                            l,
                                                            lowerTolerance);
                                                    }
                                            }
                                        }

                                        return string.Compare(
                                                   stringValue,
                                                   StringFormatter.FormatIntoString(integerValue),
                                                   StringComparison.CurrentCulture) >=
                                               0;
                                    }

                                    static bool LessThanNumericToleranceRangeNumeric(
                                        string stringValue,
                                        double integerValue,
                                        double lowerTolerance)
                                    {
                                        if (WorkingExpressionSet.TryInterpretStringValue(
                                            stringValue,
                                            out object convertedValue))
                                        {
                                            switch (convertedValue)
                                            {
                                                case long l:
                                                    {
                                                        return ToleranceFunctions.LessThanRangeTolerant(
                                                            integerValue,
                                                            l,
                                                            lowerTolerance);
                                                    }

                                                case double d:
                                                    {
                                                        if (InternalTypeDirectConversions.ToInteger(
                                                            d,
                                                            out var l))
                                                        {
                                                            return ToleranceFunctions.LessThanRangeTolerant(
                                                                integerValue,
                                                                l,
                                                                lowerTolerance);
                                                        }

                                                        return ToleranceFunctions.LessThanRangeTolerant(
                                                            Convert.ToDouble(integerValue),
                                                            l,
                                                            lowerTolerance);
                                                    }
                                            }
                                        }

                                        return string.Compare(
                                                   stringValue,
                                                   StringFormatter.FormatIntoString(integerValue),
                                                   StringComparison.CurrentCulture) <
                                               0;
                                    }

                                    static bool LessThanOrEqualNumericToleranceRangeNumeric(
                                        string stringValue,
                                        double integerValue,
                                        double lowerTolerance)
                                    {
                                        if (WorkingExpressionSet.TryInterpretStringValue(
                                            stringValue,
                                            out object convertedValue))
                                        {
                                            switch (convertedValue)
                                            {
                                                case long l:
                                                    {
                                                        return ToleranceFunctions.LessThanOrEqualRangeTolerant(
                                                            integerValue,
                                                            l,
                                                            lowerTolerance);
                                                    }

                                                case double d:
                                                    {
                                                        if (InternalTypeDirectConversions.ToInteger(
                                                            d,
                                                            out var l))
                                                        {
                                                            return ToleranceFunctions.LessThanOrEqualRangeTolerant(
                                                                integerValue,
                                                                l,
                                                                lowerTolerance);
                                                        }

                                                        return ToleranceFunctions.LessThanOrEqualRangeTolerant(
                                                            Convert.ToDouble(integerValue),
                                                            l,
                                                            lowerTolerance);
                                                    }
                                            }
                                        }

                                        return string.Compare(
                                                   stringValue,
                                                   StringFormatter.FormatIntoString(integerValue),
                                                   StringComparison.CurrentCulture) <=
                                               0;
                                    }
                                }
                                #endregion

                                #region Proportion and percentage tolerance
                                if (tolerance.ProportionalTolerance != null)
                                {
                                    var proportionalOrPercentageTolerance = tolerance.ProportionalTolerance.Value;

                                    if (proportionalOrPercentageTolerance > 1D)
                                    {
                                        // Proportional tolerance
                                        MethodInfo methodInfo;

                                        switch ((lessThanLocal, equalsLocal))
                                        {
                                            case (true, true):
                                                methodInfo = numericExp.Type == typeof(long)
                                                    ? ((Func<string, long, double, bool>)
                                                        LessThanOrEqualProportionalToleranceInteger).Method
                                                    : ((Func<string, double, double, bool>)
                                                        LessThanOrEqualProportionalToleranceDouble).Method;

                                                break;
                                            case (true, false):
                                                methodInfo = numericExp.Type == typeof(long)
                                                    ? ((Func<string, long, double, bool>)
                                                        LessThanProportionalToleranceInteger).Method
                                                    : ((Func<string, double, double, bool>)
                                                        LessThanProportionalToleranceNumeric).Method;

                                                break;
                                            case (false, true):
                                                methodInfo = numericExp.Type == typeof(long)
                                                    ? ((Func<string, long, double, bool>)
                                                        GreaterThanOrEqualProportionalToleranceInteger).Method
                                                    : ((Func<string, double, double, bool>)
                                                        GreaterThanOrEqualProportionalToleranceNumeric).Method;

                                                break;
                                            case (false, false):
                                                methodInfo = numericExp.Type == typeof(long)
                                                    ? ((Func<string, long, double, bool>)
                                                        GreaterThanProportionalToleranceInteger).Method
                                                    : ((Func<string, double, double, bool>)
                                                        GreaterThanProportionalToleranceNumeric).Method;

                                                break;
                                        }

                                        return Expression.Call(
                                            methodInfo,
                                            stringExp,
                                            numericExp,
                                            Expression.Constant(
                                                proportionalOrPercentageTolerance,
                                                typeof(double)));

                                        static bool GreaterThanProportionalToleranceInteger(
                                            string stringValue,
                                            long integerValue,
                                            double proportionalTolerance)
                                        {
                                            if (WorkingExpressionSet.TryInterpretStringValue(
                                                stringValue,
                                                out object convertedValue))
                                            {
                                                switch (convertedValue)
                                                {
                                                    case long l:
                                                        {
                                                            return ToleranceFunctions.GreaterThanProportionTolerant(
                                                                integerValue,
                                                                l,
                                                                proportionalTolerance);
                                                        }

                                                    case double d:
                                                        {
                                                            if (InternalTypeDirectConversions.ToInteger(
                                                                d,
                                                                out var l))
                                                            {
                                                                return ToleranceFunctions.GreaterThanProportionTolerant(
                                                                    integerValue,
                                                                    l,
                                                                    proportionalTolerance);
                                                            }

                                                            return ToleranceFunctions.GreaterThanProportionTolerant(
                                                                Convert.ToDouble(integerValue),
                                                                l,
                                                                proportionalTolerance);
                                                        }
                                                }
                                            }

                                            return string.Compare(
                                                       stringValue,
                                                       StringFormatter.FormatIntoString(integerValue),
                                                       StringComparison.CurrentCulture) >
                                                   0;
                                        }

                                        static bool GreaterThanOrEqualProportionalToleranceInteger(
                                            string stringValue,
                                            long integerValue,
                                            double proportionalTolerance)
                                        {
                                            if (WorkingExpressionSet.TryInterpretStringValue(
                                                stringValue,
                                                out object convertedValue))
                                            {
                                                switch (convertedValue)
                                                {
                                                    case long l:
                                                        {
                                                            return ToleranceFunctions.GreaterThanOrEqualProportionTolerant(
                                                                integerValue,
                                                                l,
                                                                proportionalTolerance);
                                                        }

                                                    case double d:
                                                        {
                                                            if (InternalTypeDirectConversions.ToInteger(
                                                                d,
                                                                out var l))
                                                            {
                                                                return ToleranceFunctions.GreaterThanOrEqualProportionTolerant(
                                                                    integerValue,
                                                                    l,
                                                                    proportionalTolerance);
                                                            }

                                                            return ToleranceFunctions.GreaterThanOrEqualProportionTolerant(
                                                                Convert.ToDouble(integerValue),
                                                                l,
                                                                proportionalTolerance);
                                                        }
                                                }
                                            }

                                            return string.Compare(
                                                       stringValue,
                                                       StringFormatter.FormatIntoString(integerValue),
                                                       StringComparison.CurrentCulture) >=
                                                   0;
                                        }

                                        static bool LessThanProportionalToleranceInteger(
                                            string stringValue,
                                            long integerValue,
                                            double proportionalTolerance)
                                        {
                                            if (WorkingExpressionSet.TryInterpretStringValue(
                                                stringValue,
                                                out object convertedValue))
                                            {
                                                switch (convertedValue)
                                                {
                                                    case long l:
                                                        {
                                                            return ToleranceFunctions.LessThanProportionTolerant(
                                                                integerValue,
                                                                l,
                                                                proportionalTolerance);
                                                        }

                                                    case double d:
                                                        {
                                                            if (InternalTypeDirectConversions.ToInteger(
                                                                d,
                                                                out var l))
                                                            {
                                                                return ToleranceFunctions.LessThanProportionTolerant(
                                                                    integerValue,
                                                                    l,
                                                                    proportionalTolerance);
                                                            }

                                                            return ToleranceFunctions.LessThanProportionTolerant(
                                                                Convert.ToDouble(integerValue),
                                                                l,
                                                                proportionalTolerance);
                                                        }
                                                }
                                            }

                                            return string.Compare(
                                                       stringValue,
                                                       StringFormatter.FormatIntoString(integerValue),
                                                       StringComparison.CurrentCulture) <
                                                   0;
                                        }

                                        static bool LessThanOrEqualProportionalToleranceInteger(
                                            string stringValue,
                                            long integerValue,
                                            double proportionalTolerance)
                                        {
                                            if (WorkingExpressionSet.TryInterpretStringValue(
                                                stringValue,
                                                out object convertedValue))
                                            {
                                                switch (convertedValue)
                                                {
                                                    case long l:
                                                        {
                                                            return ToleranceFunctions.LessThanOrEqualProportionTolerant(
                                                                integerValue,
                                                                l,
                                                                proportionalTolerance);
                                                        }

                                                    case double d:
                                                        {
                                                            if (InternalTypeDirectConversions.ToInteger(
                                                                d,
                                                                out var l))
                                                            {
                                                                return ToleranceFunctions.LessThanOrEqualProportionTolerant(
                                                                    integerValue,
                                                                    l,
                                                                    proportionalTolerance);
                                                            }

                                                            return ToleranceFunctions.LessThanOrEqualProportionTolerant(
                                                                Convert.ToDouble(integerValue),
                                                                l,
                                                                proportionalTolerance);
                                                        }
                                                }
                                            }

                                            return string.Compare(
                                                       stringValue,
                                                       StringFormatter.FormatIntoString(integerValue),
                                                       StringComparison.CurrentCulture) <=
                                                   0;
                                        }

                                        static bool GreaterThanProportionalToleranceNumeric(
                                            string stringValue,
                                            double numericValue,
                                            double proportionalTolerance)
                                        {
                                            if (WorkingExpressionSet.TryInterpretStringValue(
                                                stringValue,
                                                out object convertedValue))
                                            {
                                                switch (convertedValue)
                                                {
                                                    case long l:
                                                        {
                                                            return ToleranceFunctions.GreaterThanProportionTolerant(
                                                                numericValue,
                                                                l,
                                                                proportionalTolerance);
                                                        }

                                                    case double d:
                                                        {
                                                            if (InternalTypeDirectConversions.ToInteger(
                                                                d,
                                                                out var l))
                                                            {
                                                                return ToleranceFunctions.GreaterThanProportionTolerant(
                                                                    numericValue,
                                                                    l,
                                                                    proportionalTolerance);
                                                            }

                                                            return ToleranceFunctions.GreaterThanProportionTolerant(
                                                                Convert.ToDouble(numericValue),
                                                                l,
                                                                proportionalTolerance);
                                                        }
                                                }
                                            }

                                            return string.Compare(
                                                       stringValue,
                                                       StringFormatter.FormatIntoString(numericValue),
                                                       StringComparison.CurrentCulture) >
                                                   0;
                                        }

                                        static bool GreaterThanOrEqualProportionalToleranceNumeric(
                                            string stringValue,
                                            double numericValue,
                                            double proportionalTolerance)
                                        {
                                            if (WorkingExpressionSet.TryInterpretStringValue(
                                                stringValue,
                                                out object convertedValue))
                                            {
                                                switch (convertedValue)
                                                {
                                                    case long l:
                                                        {
                                                            return ToleranceFunctions.GreaterThanOrEqualProportionTolerant(
                                                                numericValue,
                                                                l,
                                                                proportionalTolerance);
                                                        }

                                                    case double d:
                                                        {
                                                            if (InternalTypeDirectConversions.ToInteger(
                                                                d,
                                                                out var l))
                                                            {
                                                                return ToleranceFunctions.GreaterThanOrEqualProportionTolerant(
                                                                    numericValue,
                                                                    l,
                                                                    proportionalTolerance);
                                                            }

                                                            return ToleranceFunctions.GreaterThanOrEqualProportionTolerant(
                                                                Convert.ToDouble(numericValue),
                                                                l,
                                                                proportionalTolerance);
                                                        }
                                                }
                                            }

                                            return string.Compare(
                                                       stringValue,
                                                       StringFormatter.FormatIntoString(numericValue),
                                                       StringComparison.CurrentCulture) >=
                                                   0;
                                        }

                                        static bool LessThanProportionalToleranceNumeric(
                                            string stringValue,
                                            double numericValue,
                                            double proportionalTolerance)
                                        {
                                            if (WorkingExpressionSet.TryInterpretStringValue(
                                                stringValue,
                                                out object convertedValue))
                                            {
                                                switch (convertedValue)
                                                {
                                                    case long l:
                                                        {
                                                            return ToleranceFunctions.LessThanProportionTolerant(
                                                                numericValue,
                                                                l,
                                                                proportionalTolerance);
                                                        }

                                                    case double d:
                                                        {
                                                            if (InternalTypeDirectConversions.ToInteger(
                                                                d,
                                                                out var l))
                                                            {
                                                                return ToleranceFunctions.LessThanProportionTolerant(
                                                                    numericValue,
                                                                    l,
                                                                    proportionalTolerance);
                                                            }

                                                            return ToleranceFunctions.LessThanProportionTolerant(
                                                                Convert.ToDouble(numericValue),
                                                                l,
                                                                proportionalTolerance);
                                                        }
                                                }
                                            }

                                            return string.Compare(
                                                       stringValue,
                                                       StringFormatter.FormatIntoString(numericValue),
                                                       StringComparison.CurrentCulture) <
                                                   0;
                                        }

                                        static bool LessThanOrEqualProportionalToleranceDouble(
                                            string stringValue,
                                            double numericValue,
                                            double proportionalTolerance)
                                        {
                                            if (WorkingExpressionSet.TryInterpretStringValue(
                                                stringValue,
                                                out object convertedValue))
                                            {
                                                switch (convertedValue)
                                                {
                                                    case long l:
                                                        {
                                                            return ToleranceFunctions.LessThanOrEqualProportionTolerant(
                                                                numericValue,
                                                                l,
                                                                proportionalTolerance);
                                                        }

                                                    case double d:
                                                        {
                                                            if (InternalTypeDirectConversions.ToInteger(
                                                                d,
                                                                out var l))
                                                            {
                                                                return ToleranceFunctions.LessThanOrEqualProportionTolerant(
                                                                    numericValue,
                                                                    l,
                                                                    proportionalTolerance);
                                                            }

                                                            return ToleranceFunctions.LessThanOrEqualProportionTolerant(
                                                                Convert.ToDouble(numericValue),
                                                                l,
                                                                proportionalTolerance);
                                                        }
                                                }
                                            }

                                            return string.Compare(
                                                       stringValue,
                                                       StringFormatter.FormatIntoString(numericValue),
                                                       StringComparison.CurrentCulture) <=
                                                   0;
                                        }
                                    }

                                    if (proportionalOrPercentageTolerance < 1D &&
                                        proportionalOrPercentageTolerance > 0D)
                                    {
                                        // Percentage tolerance
                                        MethodInfo methodInfo;

                                        switch ((lessThanLocal, equalsLocal))
                                        {
                                            case (true, true):
                                                methodInfo = numericExp.Type == typeof(long)
                                                    ? ((Func<string, long, double, bool>)
                                                        LessThanOrEqualPercentageToleranceInteger).Method
                                                    : ((Func<string, double, double, bool>)
                                                        LessThanOrEqualPercentageToleranceDouble).Method;

                                                break;
                                            case (true, false):
                                                methodInfo = numericExp.Type == typeof(long)
                                                    ? ((Func<string, long, double, bool>)
                                                        LessThanPercentageToleranceInteger).Method
                                                    : ((Func<string, double, double, bool>)
                                                        LessThanPercentageToleranceNumeric).Method;

                                                break;
                                            case (false, true):
                                                methodInfo = numericExp.Type == typeof(long)
                                                    ? ((Func<string, long, double, bool>)
                                                        GreaterThanOrEqualPercentageToleranceInteger).Method
                                                    : ((Func<string, double, double, bool>)
                                                        GreaterThanOrEqualPercentageToleranceNumeric).Method;

                                                break;
                                            case (false, false):
                                                methodInfo = numericExp.Type == typeof(long)
                                                    ? ((Func<string, long, double, bool>)
                                                        GreaterThanPercentageToleranceInteger).Method
                                                    : ((Func<string, double, double, bool>)
                                                        GreaterThanPercentageToleranceNumeric).Method;

                                                break;
                                        }

                                        return Expression.Call(
                                            methodInfo,
                                            stringExp,
                                            numericExp,
                                            Expression.Constant(
                                                proportionalOrPercentageTolerance,
                                                typeof(double)));

                                        static bool GreaterThanPercentageToleranceInteger(
                                            string stringValue,
                                            long integerValue,
                                            double proportionalTolerance)
                                        {
                                            if (WorkingExpressionSet.TryInterpretStringValue(
                                                stringValue,
                                                out object convertedValue))
                                            {
                                                switch (convertedValue)
                                                {
                                                    case long l:
                                                        {
                                                            return ToleranceFunctions.GreaterThanPercentageTolerant(
                                                                integerValue,
                                                                l,
                                                                proportionalTolerance);
                                                        }

                                                    case double d:
                                                        {
                                                            if (InternalTypeDirectConversions.ToInteger(
                                                                d,
                                                                out var l))
                                                            {
                                                                return ToleranceFunctions.GreaterThanPercentageTolerant(
                                                                    integerValue,
                                                                    l,
                                                                    proportionalTolerance);
                                                            }

                                                            return ToleranceFunctions.GreaterThanPercentageTolerant(
                                                                Convert.ToDouble(integerValue),
                                                                l,
                                                                proportionalTolerance);
                                                        }
                                                }
                                            }

                                            return string.Compare(
                                                       stringValue,
                                                       StringFormatter.FormatIntoString(integerValue),
                                                       StringComparison.CurrentCulture) >
                                                   0;
                                        }

                                        static bool GreaterThanOrEqualPercentageToleranceInteger(
                                            string stringValue,
                                            long integerValue,
                                            double proportionalTolerance)
                                        {
                                            if (WorkingExpressionSet.TryInterpretStringValue(
                                                stringValue,
                                                out object convertedValue))
                                            {
                                                switch (convertedValue)
                                                {
                                                    case long l:
                                                        {
                                                            return ToleranceFunctions.GreaterThanOrEqualPercentageTolerant(
                                                                integerValue,
                                                                l,
                                                                proportionalTolerance);
                                                        }

                                                    case double d:
                                                        {
                                                            if (InternalTypeDirectConversions.ToInteger(
                                                                d,
                                                                out var l))
                                                            {
                                                                return ToleranceFunctions.GreaterThanOrEqualPercentageTolerant(
                                                                    integerValue,
                                                                    l,
                                                                    proportionalTolerance);
                                                            }

                                                            return ToleranceFunctions.GreaterThanOrEqualPercentageTolerant(
                                                                Convert.ToDouble(integerValue),
                                                                l,
                                                                proportionalTolerance);
                                                        }
                                                }
                                            }

                                            return string.Compare(
                                                       stringValue,
                                                       StringFormatter.FormatIntoString(integerValue),
                                                       StringComparison.CurrentCulture) >=
                                                   0;
                                        }

                                        static bool LessThanPercentageToleranceInteger(
                                            string stringValue,
                                            long integerValue,
                                            double proportionalTolerance)
                                        {
                                            if (WorkingExpressionSet.TryInterpretStringValue(
                                                stringValue,
                                                out object convertedValue))
                                            {
                                                switch (convertedValue)
                                                {
                                                    case long l:
                                                        {
                                                            return ToleranceFunctions.LessThanPercentageTolerant(
                                                                integerValue,
                                                                l,
                                                                proportionalTolerance);
                                                        }

                                                    case double d:
                                                        {
                                                            if (InternalTypeDirectConversions.ToInteger(
                                                                d,
                                                                out var l))
                                                            {
                                                                return ToleranceFunctions.LessThanPercentageTolerant(
                                                                    integerValue,
                                                                    l,
                                                                    proportionalTolerance);
                                                            }

                                                            return ToleranceFunctions.LessThanPercentageTolerant(
                                                                Convert.ToDouble(integerValue),
                                                                l,
                                                                proportionalTolerance);
                                                        }
                                                }
                                            }

                                            return string.Compare(
                                                       stringValue,
                                                       StringFormatter.FormatIntoString(integerValue),
                                                       StringComparison.CurrentCulture) <
                                                   0;
                                        }

                                        static bool LessThanOrEqualPercentageToleranceInteger(
                                            string stringValue,
                                            long integerValue,
                                            double proportionalTolerance)
                                        {
                                            if (WorkingExpressionSet.TryInterpretStringValue(
                                                stringValue,
                                                out object convertedValue))
                                            {
                                                switch (convertedValue)
                                                {
                                                    case long l:
                                                        {
                                                            return ToleranceFunctions.LessThanOrEqualPercentageTolerant(
                                                                integerValue,
                                                                l,
                                                                proportionalTolerance);
                                                        }

                                                    case double d:
                                                        {
                                                            if (InternalTypeDirectConversions.ToInteger(
                                                                d,
                                                                out var l))
                                                            {
                                                                return ToleranceFunctions.LessThanOrEqualPercentageTolerant(
                                                                    integerValue,
                                                                    l,
                                                                    proportionalTolerance);
                                                            }

                                                            return ToleranceFunctions.LessThanOrEqualPercentageTolerant(
                                                                Convert.ToDouble(integerValue),
                                                                l,
                                                                proportionalTolerance);
                                                        }
                                                }
                                            }

                                            return string.Compare(
                                                       stringValue,
                                                       StringFormatter.FormatIntoString(integerValue),
                                                       StringComparison.CurrentCulture) <=
                                                   0;
                                        }

                                        static bool GreaterThanPercentageToleranceNumeric(
                                            string stringValue,
                                            double numericValue,
                                            double proportionalTolerance)
                                        {
                                            if (WorkingExpressionSet.TryInterpretStringValue(
                                                stringValue,
                                                out object convertedValue))
                                            {
                                                switch (convertedValue)
                                                {
                                                    case long l:
                                                        {
                                                            return ToleranceFunctions.GreaterThanPercentageTolerant(
                                                                numericValue,
                                                                l,
                                                                proportionalTolerance);
                                                        }

                                                    case double d:
                                                        {
                                                            if (InternalTypeDirectConversions.ToInteger(
                                                                d,
                                                                out var l))
                                                            {
                                                                return ToleranceFunctions.GreaterThanPercentageTolerant(
                                                                    numericValue,
                                                                    l,
                                                                    proportionalTolerance);
                                                            }

                                                            return ToleranceFunctions.GreaterThanPercentageTolerant(
                                                                Convert.ToDouble(numericValue),
                                                                l,
                                                                proportionalTolerance);
                                                        }
                                                }
                                            }

                                            return string.Compare(
                                                       stringValue,
                                                       StringFormatter.FormatIntoString(numericValue),
                                                       StringComparison.CurrentCulture) >
                                                   0;
                                        }

                                        static bool GreaterThanOrEqualPercentageToleranceNumeric(
                                            string stringValue,
                                            double numericValue,
                                            double proportionalTolerance)
                                        {
                                            if (WorkingExpressionSet.TryInterpretStringValue(
                                                stringValue,
                                                out object convertedValue))
                                            {
                                                switch (convertedValue)
                                                {
                                                    case long l:
                                                        {
                                                            return ToleranceFunctions.GreaterThanOrEqualPercentageTolerant(
                                                                numericValue,
                                                                l,
                                                                proportionalTolerance);
                                                        }

                                                    case double d:
                                                        {
                                                            if (InternalTypeDirectConversions.ToInteger(
                                                                d,
                                                                out var l))
                                                            {
                                                                return ToleranceFunctions.GreaterThanOrEqualPercentageTolerant(
                                                                    numericValue,
                                                                    l,
                                                                    proportionalTolerance);
                                                            }

                                                            return ToleranceFunctions.GreaterThanOrEqualPercentageTolerant(
                                                                Convert.ToDouble(numericValue),
                                                                l,
                                                                proportionalTolerance);
                                                        }
                                                }
                                            }

                                            return string.Compare(
                                                       stringValue,
                                                       StringFormatter.FormatIntoString(numericValue),
                                                       StringComparison.CurrentCulture) >=
                                                   0;
                                        }

                                        static bool LessThanPercentageToleranceNumeric(
                                            string stringValue,
                                            double numericValue,
                                            double proportionalTolerance)
                                        {
                                            if (WorkingExpressionSet.TryInterpretStringValue(
                                                stringValue,
                                                out object convertedValue))
                                            {
                                                switch (convertedValue)
                                                {
                                                    case long l:
                                                        {
                                                            return ToleranceFunctions.LessThanPercentageTolerant(
                                                                numericValue,
                                                                l,
                                                                proportionalTolerance);
                                                        }

                                                    case double d:
                                                        {
                                                            if (InternalTypeDirectConversions.ToInteger(
                                                                d,
                                                                out var l))
                                                            {
                                                                return ToleranceFunctions.LessThanPercentageTolerant(
                                                                    numericValue,
                                                                    l,
                                                                    proportionalTolerance);
                                                            }

                                                            return ToleranceFunctions.LessThanPercentageTolerant(
                                                                Convert.ToDouble(numericValue),
                                                                l,
                                                                proportionalTolerance);
                                                        }
                                                }
                                            }

                                            return string.Compare(
                                                       stringValue,
                                                       StringFormatter.FormatIntoString(numericValue),
                                                       StringComparison.CurrentCulture) <
                                                   0;
                                        }

                                        static bool LessThanOrEqualPercentageToleranceDouble(
                                            string stringValue,
                                            double numericValue,
                                            double proportionalTolerance)
                                        {
                                            if (WorkingExpressionSet.TryInterpretStringValue(
                                                stringValue,
                                                out object convertedValue))
                                            {
                                                switch (convertedValue)
                                                {
                                                    case long l:
                                                        {
                                                            return ToleranceFunctions.LessThanOrEqualPercentageTolerant(
                                                                numericValue,
                                                                l,
                                                                proportionalTolerance);
                                                        }

                                                    case double d:
                                                        {
                                                            if (InternalTypeDirectConversions.ToInteger(
                                                                d,
                                                                out var l))
                                                            {
                                                                return ToleranceFunctions.LessThanOrEqualPercentageTolerant(
                                                                    numericValue,
                                                                    l,
                                                                    proportionalTolerance);
                                                            }

                                                            return ToleranceFunctions.LessThanOrEqualPercentageTolerant(
                                                                Convert.ToDouble(numericValue),
                                                                l,
                                                                proportionalTolerance);
                                                        }
                                                }
                                            }

                                            return string.Compare(
                                                       stringValue,
                                                       StringFormatter.FormatIntoString(numericValue),
                                                       StringComparison.CurrentCulture) <=
                                                   0;
                                        }
                                    }
                                }
                                #endregion

                                return Expression.Call(
                                    numericExp.Type == typeof(long) ?
                                        ((Func<string, long, bool>)EqualizeNoToleranceInteger).Method :
                                        ((Func<string, double, bool>)EqualizeNoToleranceNumeric).Method,
                                    stringExp,
                                    numericExp);

                                static bool EqualizeNoToleranceInteger(
                                    string stringValue,
                                    long integerValue)
                                {
                                    return string.Equals(stringValue, StringFormatter.FormatIntoString(integerValue), StringComparison.CurrentCulture);
                                }

                                static bool EqualizeNoToleranceNumeric(
                                    string stringValue,
                                    double numericValue)
                                {
                                    return string.Equals(stringValue, StringFormatter.FormatIntoString(numericValue), StringComparison.CurrentCulture);
                                }
                            }
                        }
                }
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
                    string methodName = (this.equals, this.lessThan) switch
                    {
                        (true, true) => nameof(ToleranceFunctions.LessThanOrEqualProportionTolerant),
                        (true, false) => nameof(ToleranceFunctions.GreaterThanOrEqualProportionTolerant),
                        (false, true) => nameof(ToleranceFunctions.LessThanProportionTolerant),
                        (false, false) => nameof(ToleranceFunctions.GreaterThanProportionTolerant),
                    };

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
                    string methodName = (this.equals, this.lessThan) switch
                    {
                        (true, true) => nameof(ToleranceFunctions.LessThanOrEqualPercentageTolerant),
                        (true, false) => nameof(ToleranceFunctions.GreaterThanOrEqualPercentageTolerant),
                        (false, true) => nameof(ToleranceFunctions.LessThanPercentageTolerant),
                        (false, false) => nameof(ToleranceFunctions.GreaterThanPercentageTolerant),
                    };

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