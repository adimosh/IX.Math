// <copyright file="IntegerBinaryFunctionNodeBase.cs" company="Adrian Mos">
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
    internal abstract class IntegerBinaryFunctionNodeBase : BinaryFunctionNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerBinaryFunctionNodeBase" /> class.
        /// </summary>
        /// <param name="stringFormatters">The string formatters.</param>
        /// <param name="firstParameter">The first parameter.</param>
        /// <param name="secondParameter">The second parameter.</param>
        protected IntegerBinaryFunctionNodeBase(
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

            _ = firstParameter.VerifyPossibleType(SupportableValueType.Integer);
            _ = secondParameter.VerifyPossibleType(SupportableValueType.Integer);

            int cost = firstParameter.CalculateStrategyCost(SupportedValueType.Integer) +
                       secondParameter.CalculateStrategyCost(SupportedValueType.Integer);

            this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Integer);
            foreach (var possibleType in GetSupportedTypeOptions(this.PossibleReturnType))
            {
                this.CalculatedCosts[possibleType] = (GetStandardConversionStrategyCost(
                    SupportedValueType.Integer,
                    in possibleType) + cost, SupportedValueType.Integer);
            }
        }

        /// <summary>
        /// Gets the parameters.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The properly-converted numeric parameters.</returns>
        protected (Expression First, Expression Second) GetParameters(in ComparisonTolerance tolerance) => (this.FirstParameter.GenerateExpression(
                SupportedValueType.Integer,
                in tolerance), this.SecondParameter.GenerateExpression(
                SupportedValueType.Integer,
                in tolerance));

        /// <summary>
        /// Gets the simplification expressions, if available.
        /// </summary>
        /// <returns>A success state, and the expressions.</returns>
        protected (bool Success, long FirstValue, long SecondValue) GetSimplificationExpressions()
        {
            if (this.FirstParameter is ConstantNodeBase fp &&
                this.SecondParameter is ConstantNodeBase sp &&
                fp.TryGetInteger(out var first) &&
                sp.TryGetInteger(out var second))
            {
                return (true, first, second);
            }

            return (false, default, default);
        }
    }
}