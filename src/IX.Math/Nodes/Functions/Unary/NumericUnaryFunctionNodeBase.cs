// <copyright file="NumericUnaryFunctionNodeBase.cs" company="Adrian Mos">
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
    public abstract class NumericUnaryFunctionNodeBase : UnaryFunctionNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NumericUnaryFunctionNodeBase" /> class.
        /// </summary>
        /// <param name="stringFormatters">The string formatters.</param>
        /// <param name="parameter">The parameter.</param>
        protected NumericUnaryFunctionNodeBase(
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
            var (success, value) = this.GetSimplificationExpression();

            if (success)
            {
                return this.GenerateConstantNumeric(this.RepresentedFunction(value));
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
            _ = parameter.VerifyPossibleType(SupportableValueType.Numeric);

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
        protected (bool, double) GetSimplificationExpression()
        {
            if (this.Parameter is ConstantNodeBase fp &&
                fp.TryGetNumeric(out var first))
            {
                return (true, first);
            }

            return (false, default);
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