// <copyright file="NumericRoundingUnaryFunctionNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;

namespace IX.Math.Nodes.Functions.Unary
{
    /// <summary>
    ///     A base class for numeric unary functions.
    /// </summary>
    /// <seealso cref="UnaryFunctionNodeBase" />
    public abstract class NumericRoundingUnaryFunctionNodeBase : UnaryFunctionNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NumericRoundingUnaryFunctionNodeBase" /> class.
        /// </summary>
        /// <param name="stringFormatters">The string formatters.</param>
        /// <param name="parameter">The parameter.</param>
        protected NumericRoundingUnaryFunctionNodeBase(
            List<IStringFormatter> stringFormatters,
            NodeBase parameter)
            : base(
                stringFormatters,
                parameter)
        {
        }

        /// <summary>
        /// Gets the function represented by this node.
        /// </summary>
        /// <value>
        /// The represented function.
        /// </value>
        protected abstract Func<double, double> RepresentedFunction { get; }

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>A simplified node, or this instance.</returns>
        public override NodeBase Simplify()
        {
            var (success, isInteger, value, iValue) = this.GetSimplificationExpression();

            if (success)
            {
                if (isInteger)
                {
                    // Constant integers do not need rounding
                    return this.GenerateConstantInteger(iValue);
                }

                return this.GenerateConstantNumeric(this.RepresentedFunction(value));
            }

            if (this.Parameter.PossibleReturnType == SupportableValueType.Integer)
            {
                // If the return type of the parameter can only be an integer, bypass this expression entirely
                return this.Parameter;
            }

            return this;
        }

        /// <summary>
        ///     Ensures that the parameter that is received is compatible with the function, optionally allowing the parameter
        ///     reference to change.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void EnsureCompatibleParameter(NodeBase parameter)
        {
            _ = parameter.VerifyPossibleType(SupportableValueType.Numeric | SupportableValueType.Integer);

            this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Numeric);
            var cost = parameter.CalculateStrategyCost(SupportedValueType.Numeric);

            foreach (var possibleType in GetSupportedTypeOptions(this.PossibleReturnType))
            {
                this.CalculatedCosts[possibleType] = (GetStandardConversionStrategyCost(
                    SupportedValueType.Numeric,
                    in possibleType) + cost, possibleType);
            }
        }

        /// <summary>
        /// Gets the simplification expression.
        /// </summary>
        /// <returns>The success value, along with a constant value if successful.</returns>
        protected (bool, bool, double, long) GetSimplificationExpression()
        {
            if (this.Parameter is ConstantNodeBase fp)
            {
                if (fp.TryGetInteger(out var ifirst))
                {
                    return (true, true, default, ifirst);
                }

                if (fp.TryGetNumeric(out var first))
                {
                    return (true, false, first, default);
                }
            }

            return (false, default, default, default);
        }

        /// <summary>
        /// Generates the expression that this node represents.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="comparisonTolerance">The comparison tolerance.</param>
        /// <returns>A compiled expression, if one is possible.</returns>
        protected override Expression GenerateExpressionInternal(
            in SupportedValueType valueType,
            in ComparisonTolerance comparisonTolerance) =>
            Expression.Call(
                this.RepresentedFunction.Method,
                this.Parameter.GenerateExpression(
                    SupportedValueType.Numeric,
                    in comparisonTolerance));
    }
}