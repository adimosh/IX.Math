// <copyright file="EquationNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Conversion;
using IX.Math.Exceptions;
using IX.Math.Formatters;
using IX.Math.Nodes.Constants;
using IX.Math.WorkingSet;
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
#region Internal state

        private readonly bool notEqual;

        private bool isConstant;

#endregion

#region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="EquationNodeBase" /> class.
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

#endregion

#region Properties and indexers

        /// <summary>
        ///     Gets a value indicating whether or not this node is actually a constant.
        /// </summary>
        /// <value><see langword="true" /> if the node is a constant, <see langword="false" /> otherwise.</value>
        public sealed override bool IsConstant =>
            this.isConstant;

#endregion

#region Methods

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

            if (left.TryGetBoolean(out var bvl) && right.TryGetBoolean(out var bvr))
            {
                // Both boolean
                equalityValue = bvl == bvr;
            }
            else if (left.TryGetBinary(out byte[] bavl) && right.TryGetBinary(out byte[] bavr))
            {
                // Both byte array, but not both integer or numeric
                var bli = left.CheckSupportedType(SupportableValueType.Integer);
                var bln = left.CheckSupportedType(SupportableValueType.Numeric);
                var bri = right.CheckSupportedType(SupportableValueType.Integer);
                var brn = right.CheckSupportedType(SupportableValueType.Numeric);

                if ((bli || bln) && (bri || brn))
                {
                    return this;
                }

                equalityValue = bavl.SequenceEqualsWithMsb(bavr);
            }
            else if (left.TryGetString(out string svl) && right.TryGetString(out string svr))
            {
                // Both string, but not either integer or numeric
                var bli = left.CheckSupportedType(SupportableValueType.Integer);
                var bln = left.CheckSupportedType(SupportableValueType.Numeric);
                var bri = right.CheckSupportedType(SupportableValueType.Integer);
                var brn = right.CheckSupportedType(SupportableValueType.Numeric);

                if (bli || bln || bri || brn)
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

            this.isConstant = true;

            return new BoolNode(equalityValue.Value);
        }

        /// <summary>
        ///     Generates the expression that this node represents.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="comparisonTolerance">The comparison tolerance.</param>
        /// <returns>A compiled expression, if one is possible.</returns>
        [DiagCA.SuppressMessageAttribute(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "We want it this way.")]
        protected sealed override Expression GenerateExpressionInternal(
            in SupportedValueType valueType,
            in ComparisonTolerance comparisonTolerance)
        {
            try
            {
                (var leftExpression, var rightExpression, SupportedValueType innerValueType) =
                    this.GetExpressionArguments(in comparisonTolerance);

                Expression equalExpression;

                switch (innerValueType)
                {
                    case SupportedValueType.Binary:
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
                        equalExpression = GenerateNumericalToleranceEquateExpression(in comparisonTolerance);

                        Expression GenerateNumericalToleranceEquateExpression(in ComparisonTolerance tolerance)
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

                    case SupportedValueType.Unknown when !comparisonTolerance.IsEmpty:
                    {
                        // We check whether we have a string
                        Expression stringExp, numericExp;

                        if (leftExpression.Type == typeof(string))
                        {
                            stringExp = leftExpression;
                            numericExp = rightExpression;
                        }
                        else if (rightExpression.Type == typeof(string))
                        {
                            stringExp = rightExpression;
                            numericExp = leftExpression;
                        }
                        else
                        {
                            // We don't have a string, this simply means we didn't achieve compatibility
                            throw new ExpressionNotValidLogicallyException();
                        }

                        equalExpression = GeneratePossibleToleranceExpression(in comparisonTolerance);

                        Expression GeneratePossibleToleranceExpression(in ComparisonTolerance tolerance)
                        {
#region Empty tolerance

                            if (tolerance.IsEmpty)
                            {
                                return Expression.Call(
                                    numericExp.Type == typeof(long)
                                        ? ((Func<string, long, bool>)EqualizeNoToleranceInteger).Method
                                        : ((Func<string, double, bool>)EqualizeNoToleranceNumeric).Method,
                                    stringExp,
                                    numericExp);
                            }

#endregion

#region Integer range tolerance

                            if (tolerance.IntegerToleranceRangeLowerBound != null ||
                                tolerance.IntegerToleranceRangeUpperBound != null)
                            {
                                // Integer range tolerance
                                return Expression.Call(
                                    numericExp.Type == typeof(long)
                                        ? ((Func<string, long, long, long, bool>)EqualizeIntegerToleranceRangeInteger)
                                        .Method
                                        : ((Func<string, double, long, long, bool>)EqualizeIntegerToleranceRangeNumeric)
                                        .Method,
                                    stringExp,
                                    numericExp,
                                    Expression.Constant(
                                        tolerance.IntegerToleranceRangeLowerBound ?? 0L,
                                        typeof(long)),
                                    Expression.Constant(
                                        tolerance.IntegerToleranceRangeUpperBound ?? 0L,
                                        typeof(long)));

                                static bool EqualizeIntegerToleranceRangeInteger(
                                    string stringValue,
                                    long integerValue,
                                    long lowerTolerance,
                                    long upperTolerance)
                                {
                                    if (WorkingExpressionSet.TryInterpretStringValue(
                                        stringValue,
                                        out object convertedValue))
                                    {
                                        switch (convertedValue)
                                        {
                                            case long l:
                                            {
                                                return ToleranceFunctions.EquateRangeTolerant(
                                                    integerValue,
                                                    l,
                                                    lowerTolerance,
                                                    upperTolerance);
                                            }

                                            case double d:
                                            {
                                                if (InternalTypeDirectConversions.ToInteger(
                                                    d,
                                                    out var l))
                                                {
                                                    return ToleranceFunctions.EquateRangeTolerant(
                                                        integerValue,
                                                        l,
                                                        lowerTolerance,
                                                        upperTolerance);
                                                }

                                                return ToleranceFunctions.EquateRangeTolerant(
                                                    Convert.ToDouble(integerValue),
                                                    l,
                                                    lowerTolerance,
                                                    upperTolerance);
                                            }
                                        }
                                    }

                                    return string.Equals(
                                        stringValue,
                                        StringFormatter.FormatIntoString(integerValue),
                                        StringComparison.CurrentCulture);
                                }

                                static bool EqualizeIntegerToleranceRangeNumeric(
                                    string stringValue,
                                    double numericValue,
                                    long lowerTolerance,
                                    long upperTolerance)
                                {
                                    if (WorkingExpressionSet.TryInterpretStringValue(
                                        stringValue,
                                        out object convertedValue))
                                    {
                                        switch (convertedValue)
                                        {
                                            case long l:
                                            {
                                                return ToleranceFunctions.EquateRangeTolerant(
                                                    numericValue,
                                                    Convert.ToDouble(l),
                                                    lowerTolerance,
                                                    upperTolerance);
                                            }

                                            case double d:
                                            {
                                                return ToleranceFunctions.EquateRangeTolerant(
                                                    numericValue,
                                                    d,
                                                    lowerTolerance,
                                                    upperTolerance);
                                            }
                                        }
                                    }

                                    return string.Equals(
                                        stringValue,
                                        StringFormatter.FormatIntoString(numericValue),
                                        StringComparison.CurrentCulture);
                                }
                            }

#endregion

#region Numeric range tolerance

                            if (tolerance.ToleranceRangeLowerBound != null ||
                                tolerance.ToleranceRangeUpperBound != null)
                            {
                                // Floating-point range tolerance
                                return Expression.Call(
                                    numericExp.Type == typeof(long)
                                        ? ((Func<string, long, double, double, bool>)EqualizeToleranceRangeInteger)
                                        .Method
                                        : ((Func<string, double, double, double, bool>)EqualizeToleranceRangeNumeric)
                                        .Method,
                                    stringExp,
                                    numericExp,
                                    Expression.Constant(
                                        tolerance.ToleranceRangeLowerBound ?? 0L,
                                        typeof(double)),
                                    Expression.Constant(
                                        tolerance.ToleranceRangeUpperBound ?? 0L,
                                        typeof(double)));

                                static bool EqualizeToleranceRangeInteger(
                                    string stringValue,
                                    long integerValue,
                                    double lowerTolerance,
                                    double upperTolerance)
                                {
                                    if (WorkingExpressionSet.TryInterpretStringValue(
                                        stringValue,
                                        out object convertedValue))
                                    {
                                        switch (convertedValue)
                                        {
                                            case long l:
                                            {
                                                return ToleranceFunctions.EquateRangeTolerant(
                                                    integerValue,
                                                    l,
                                                    lowerTolerance,
                                                    upperTolerance);
                                            }

                                            case double d:
                                            {
                                                if (InternalTypeDirectConversions.ToInteger(
                                                    d,
                                                    out var l))
                                                {
                                                    return ToleranceFunctions.EquateRangeTolerant(
                                                        integerValue,
                                                        l,
                                                        lowerTolerance,
                                                        upperTolerance);
                                                }

                                                return ToleranceFunctions.EquateRangeTolerant(
                                                    Convert.ToDouble(integerValue),
                                                    l,
                                                    lowerTolerance,
                                                    upperTolerance);
                                            }
                                        }
                                    }

                                    return string.Equals(
                                        stringValue,
                                        StringFormatter.FormatIntoString(integerValue),
                                        StringComparison.CurrentCulture);
                                }

                                static bool EqualizeToleranceRangeNumeric(
                                    string stringValue,
                                    double numericValue,
                                    double lowerTolerance,
                                    double upperTolerance)
                                {
                                    if (WorkingExpressionSet.TryInterpretStringValue(
                                        stringValue,
                                        out object convertedValue))
                                    {
                                        switch (convertedValue)
                                        {
                                            case long l:
                                            {
                                                return ToleranceFunctions.EquateRangeTolerant(
                                                    numericValue,
                                                    Convert.ToDouble(l),
                                                    lowerTolerance,
                                                    upperTolerance);
                                            }

                                            case double d:
                                            {
                                                return ToleranceFunctions.EquateRangeTolerant(
                                                    numericValue,
                                                    d,
                                                    lowerTolerance,
                                                    upperTolerance);
                                            }
                                        }
                                    }

                                    return string.Equals(
                                        stringValue,
                                        StringFormatter.FormatIntoString(numericValue),
                                        StringComparison.CurrentCulture);
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
                                    return Expression.Call(
                                        numericExp.Type == typeof(long)
                                            ? ((Func<string, long, double, bool>)EqualizeProportionalRangeInteger)
                                            .Method
                                            : ((Func<string, double, double, bool>)EqualizeProportionalRangeNumeric)
                                            .Method,
                                        stringExp,
                                        numericExp,
                                        Expression.Constant(
                                            proportionalOrPercentageTolerance,
                                            typeof(double)));

                                    static bool EqualizeProportionalRangeInteger(
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
                                                    return ToleranceFunctions.EquateProportionTolerant(
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
                                                        return ToleranceFunctions.EquateProportionTolerant(
                                                            integerValue,
                                                            l,
                                                            proportionalTolerance);
                                                    }

                                                    return ToleranceFunctions.EquateProportionTolerant(
                                                        Convert.ToDouble(integerValue),
                                                        l,
                                                        proportionalTolerance);
                                                }
                                            }
                                        }

                                        return string.Equals(
                                            stringValue,
                                            StringFormatter.FormatIntoString(integerValue),
                                            StringComparison.CurrentCulture);
                                    }

                                    static bool EqualizeProportionalRangeNumeric(
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
                                                    return ToleranceFunctions.EquateProportionTolerant(
                                                        numericValue,
                                                        Convert.ToDouble(l),
                                                        proportionalTolerance);
                                                }

                                                case double d:
                                                {
                                                    return ToleranceFunctions.EquateProportionTolerant(
                                                        numericValue,
                                                        d,
                                                        proportionalTolerance);
                                                }
                                            }
                                        }

                                        return string.Equals(
                                            stringValue,
                                            StringFormatter.FormatIntoString(numericValue),
                                            StringComparison.CurrentCulture);
                                    }
                                }

                                if (tolerance.ProportionalTolerance.Value < 1D &&
                                    tolerance.ProportionalTolerance.Value > 0D)
                                {
                                    // Percentage tolerance
                                    return Expression.Call(
                                        numericExp.Type == typeof(long)
                                            ? ((Func<string, long, double, bool>)EqualizePercentageRangeInteger).Method
                                            : ((Func<string, double, double, bool>)EqualizePercentageRangeNumeric)
                                            .Method,
                                        stringExp,
                                        numericExp,
                                        Expression.Constant(
                                            proportionalOrPercentageTolerance,
                                            typeof(double)));

                                    static bool EqualizePercentageRangeInteger(
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
                                                    return ToleranceFunctions.EquatePercentageTolerant(
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
                                                        return ToleranceFunctions.EquatePercentageTolerant(
                                                            integerValue,
                                                            l,
                                                            proportionalTolerance);
                                                    }

                                                    return ToleranceFunctions.EquatePercentageTolerant(
                                                        Convert.ToDouble(integerValue),
                                                        l,
                                                        proportionalTolerance);
                                                }
                                            }
                                        }

                                        return string.Equals(
                                            stringValue,
                                            StringFormatter.FormatIntoString(integerValue),
                                            StringComparison.CurrentCulture);
                                    }

                                    static bool EqualizePercentageRangeNumeric(
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
                                                    return ToleranceFunctions.EquatePercentageTolerant(
                                                        numericValue,
                                                        Convert.ToDouble(l),
                                                        proportionalTolerance);
                                                }

                                                case double d:
                                                {
                                                    return ToleranceFunctions.EquatePercentageTolerant(
                                                        numericValue,
                                                        d,
                                                        proportionalTolerance);
                                                }
                                            }
                                        }

                                        return string.Equals(
                                            stringValue,
                                            StringFormatter.FormatIntoString(numericValue),
                                            StringComparison.CurrentCulture);
                                    }
                                }
                            }

#endregion

                            return Expression.Call(
                                numericExp.Type == typeof(long)
                                    ? ((Func<string, long, bool>)EqualizeNoToleranceInteger).Method
                                    : ((Func<string, double, bool>)EqualizeNoToleranceNumeric).Method,
                                stringExp,
                                numericExp);

                            static bool EqualizeNoToleranceInteger(
                                string stringValue,
                                long integerValue)
                            {
                                return string.Equals(
                                    stringValue,
                                    StringFormatter.FormatIntoString(integerValue),
                                    StringComparison.CurrentCulture);
                            }

                            static bool EqualizeNoToleranceNumeric(
                                string stringValue,
                                double numericValue)
                            {
                                return string.Equals(
                                    stringValue,
                                    StringFormatter.FormatIntoString(numericValue),
                                    StringComparison.CurrentCulture);
                            }
                        }

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

#endregion
    }
}