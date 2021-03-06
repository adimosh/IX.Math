// <copyright file="NumericOperationBinaryFunctionNodeBase.cs" company="Adrian Mos">
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
    internal abstract class NumericOperationBinaryFunctionNodeBase : BinaryFunctionNodeBase
    {
#region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="NumericOperationBinaryFunctionNodeBase" /> class.
        /// </summary>
        /// <param name="firstParameter">The first parameter.</param>
        /// <param name="secondParameter">The second parameter.</param>
        protected NumericOperationBinaryFunctionNodeBase(
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

            _ = firstParameter.VerifyPossibleType(SupportableValueType.Numeric);
            _ = secondParameter.VerifyPossibleType(SupportableValueType.Integer);

            var cost = firstParameter.CalculateStrategyCost(SupportedValueType.Numeric) +
                       secondParameter.CalculateStrategyCost(SupportedValueType.Integer);

            this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Numeric);
            foreach (SupportedValueType possibleType in GetSupportedTypeOptions(this.PossibleReturnType))
            {
                this.CalculatedCosts[possibleType] = (GetStandardConversionStrategyCost(
                                                          SupportedValueType.Numeric,
                                                          in possibleType) +
                                                      cost, SupportedValueType.Numeric);
            }
        }

        /// <summary>
        ///     Gets the parameters.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The properly-converted numeric parameters.</returns>
        protected (Expression First, Expression Second) GetParameters(in ComparisonTolerance tolerance) =>
            (this.FirstParameter.GenerateExpression(
                SupportedValueType.Numeric,
                in tolerance), this.SecondParameter.GenerateExpression(
                SupportedValueType.Integer,
                in tolerance));

        /// <summary>
        ///     Gets the simplification expressions, if available.
        /// </summary>
        /// <returns>A success state, and the expressions.</returns>
        protected (bool Success, double FirstValue, long SecondValue) GetSimplificationExpressions()
        {
            if (this.FirstParameter is ConstantNodeBase fp &&
                this.SecondParameter is ConstantNodeBase sp &&
                fp.TryGetNumeric(out var first) &&
                sp.TryGetInteger(out var second))
            {
                return (true, first, second);
            }

            return (false, default, default);
        }

#endregion
    }
}