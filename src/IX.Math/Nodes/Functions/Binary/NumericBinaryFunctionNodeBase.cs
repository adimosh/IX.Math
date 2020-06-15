// <copyright file="NumericBinaryFunctionNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Collections.Generic;
using System.Linq.Expressions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;

namespace IX.Math.Nodes.Functions.Binary
{
    /// <summary>
    ///     A base class for numeric binary functions.
    /// </summary>
    /// <seealso cref="BinaryFunctionNodeBase" />
    internal abstract class NumericBinaryFunctionNodeBase : BinaryFunctionNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NumericBinaryFunctionNodeBase" /> class.
        /// </summary>
        /// <param name="stringFormatters">The string formatters.</param>
        /// <param name="firstParameter">The first parameter.</param>
        /// <param name="secondParameter">The second parameter.</param>
        protected NumericBinaryFunctionNodeBase(
            List<IStringFormatter> stringFormatters,
            NodeBase firstParameter,
            NodeBase secondParameter)
            : base(
                stringFormatters,
                firstParameter,
                secondParameter)
        {
        }

        /// <summary>
        ///     Ensures that the parameters that are received are compatible with the function, optionally allowing the parameter
        ///     references to change.
        /// </summary>
        /// <param name="firstParameter">The first parameter.</param>
        /// <param name="secondParameter">The second parameter.</param>
        /// <exception cref="Exceptions.ExpressionNotValidLogicallyException">
        ///     The expression is not valid logically.
        /// </exception>
        protected sealed override void EnsureCompatibleParameters(
            NodeBase firstParameter,
            NodeBase secondParameter)
        {
            this.CalculatedCosts.Clear();

            _ = firstParameter.VerifyPossibleType(SupportableValueType.Numeric);
            _ = secondParameter.VerifyPossibleType(SupportableValueType.Numeric);

            int cost = firstParameter.CalculateStrategyCost(SupportedValueType.Numeric) +
                       secondParameter.CalculateStrategyCost(SupportedValueType.Numeric);

            this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Numeric);
            foreach (var possibleType in GetSupportedTypeOptions(this.PossibleReturnType))
            {
                this.CalculatedCosts[possibleType] = (GetStandardConversionStrategyCost(
                    SupportedValueType.Numeric,
                    in possibleType) + cost, SupportedValueType.Numeric);
            }
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The properly-converted numeric parameters.</returns>
        protected (Expression First, Expression Second) GetParameters(in ComparisonTolerance tolerance) => (this.FirstParameter.GenerateExpression(
                SupportedValueType.Numeric,
                in tolerance), this.SecondParameter.GenerateExpression(
                SupportedValueType.Numeric,
                in tolerance));

        /// <summary>
        /// Gets the simplification expressions, if available.
        /// </summary>
        /// <returns>A success state, and the expressions.</returns>
        protected (bool Success, double FirstValue, double SecondValue) GetSimplificationExpressions()
        {
            if (this.FirstParameter is ConstantNodeBase fp &&
                this.SecondParameter is ConstantNodeBase sp &&
                fp.TryGetNumeric(out var first) &&
                sp.TryGetNumeric(out var second))
            {
                return (true, first, second);
            }

            return (false, default, default);
        }
    }
}