// <copyright file="StringBinaryFunctionNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;
using IX.Math.Nodes.Constants;

namespace IX.Math.Nodes.Functions.Binary
{
    /// <summary>
    ///     A base class for numeric binary functions.
    /// </summary>
    /// <seealso cref="BinaryFunctionNodeBase" />
    internal abstract class StringBinaryFunctionNodeBase : BinaryFunctionNodeBase
    {
#region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="StringBinaryFunctionNodeBase" /> class.
        /// </summary>
        /// <param name="firstParameter">The first parameter.</param>
        /// <param name="secondParameter">The second parameter.</param>
        protected StringBinaryFunctionNodeBase(
            NodeBase firstParameter,
            NodeBase secondParameter)
            : base(
                firstParameter,
                secondParameter)
        {
        }

#endregion

#region Methods

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

            _ = firstParameter.VerifyPossibleType(SupportableValueType.String);
            _ = secondParameter.VerifyPossibleType(SupportableValueType.String);

            var cost = firstParameter.CalculateStrategyCost(SupportedValueType.String) +
                       secondParameter.CalculateStrategyCost(SupportedValueType.String);

            this.PossibleReturnType = GetSupportableConversions(SupportedValueType.String);
            foreach (SupportedValueType possibleType in GetSupportedTypeOptions(this.PossibleReturnType))
            {
                this.CalculatedCosts[possibleType] = (GetStandardConversionStrategyCost(
                                                          SupportedValueType.String,
                                                          in possibleType) +
                                                      cost, SupportedValueType.String);
            }
        }

        /// <summary>
        ///     Gets the parameters.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The properly-converted numeric parameters.</returns>
        protected (Expression First, Expression Second) GetParameters(in ComparisonTolerance tolerance) =>
            (this.FirstParameter.GenerateExpression(
                SupportedValueType.String,
                in tolerance), this.SecondParameter.GenerateExpression(
                SupportedValueType.String,
                in tolerance));

        /// <summary>
        ///     Gets the simplification expressions, if available.
        /// </summary>
        /// <returns>A success state, and the expressions.</returns>
        protected (bool Success, string FirstValue, string SecondValue) GetSimplificationExpressions()
        {
            if (this.FirstParameter is ConstantNodeBase nnFirst && this.SecondParameter is ConstantNodeBase nnSecond)
            {
                return (true, nnFirst.ValueAsString, nnSecond.ValueAsString);
            }

            return (false, default, default);
        }

#endregion
    }
}