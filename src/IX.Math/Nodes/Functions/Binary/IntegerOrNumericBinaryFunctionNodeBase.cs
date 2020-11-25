// <copyright file="IntegerOrNumericBinaryFunctionNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;
using IX.Math.Exceptions;
using IX.Math.Nodes.Constants;

namespace IX.Math.Nodes.Functions.Binary
{
    /// <summary>
    ///     A base class for numeric binary functions.
    /// </summary>
    /// <seealso cref="BinaryFunctionNodeBase" />
    internal abstract class IntegerOrNumericBinaryFunctionNodeBase : BinaryFunctionNodeBase
    {
#region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="IntegerOrNumericBinaryFunctionNodeBase" /> class.
        /// </summary>
        /// <param name="firstParameter">The first parameter.</param>
        /// <param name="secondParameter">The second parameter.</param>
        protected IntegerOrNumericBinaryFunctionNodeBase(
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

            SupportableValueType firstSupportedType =
                firstParameter.VerifyPossibleType(SupportableValueType.Integer | SupportableValueType.Numeric);
            SupportableValueType secondSupportedType =
                secondParameter.VerifyPossibleType(SupportableValueType.Integer | SupportableValueType.Numeric);

            switch (firstSupportedType & secondSupportedType)
            {
                case SupportableValueType.Numeric:
                {
                    var cost = firstParameter.CalculateStrategyCost(SupportedValueType.Numeric) +
                               secondParameter.CalculateStrategyCost(SupportedValueType.Numeric);

                    this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Numeric);
                    foreach (SupportedValueType possibleType in GetSupportedTypeOptions(this.PossibleReturnType))
                    {
                        this.CalculatedCosts[possibleType] = (
                            GetStandardConversionStrategyCost(
                                SupportedValueType.Numeric,
                                in possibleType) +
                            cost, SupportedValueType.Numeric);
                    }

                    break;
                }

                case SupportableValueType.Integer:
                {
                    var cost = firstParameter.CalculateStrategyCost(SupportedValueType.Integer) +
                               secondParameter.CalculateStrategyCost(SupportedValueType.Integer);

                    this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Integer);
                    foreach (SupportedValueType possibleType in GetSupportedTypeOptions(this.PossibleReturnType))
                    {
                        this.CalculatedCosts[possibleType] = (
                            GetStandardConversionStrategyCost(
                                SupportedValueType.Integer,
                                in possibleType) +
                            cost, SupportedValueType.Integer);
                    }

                    break;
                }

                case SupportableValueType.Integer | SupportableValueType.Numeric:
                {
                    var numericCost = firstParameter.CalculateStrategyCost(SupportedValueType.Numeric) +
                                      secondParameter.CalculateStrategyCost(SupportedValueType.Numeric);
                    var integerCost = firstParameter.CalculateStrategyCost(SupportedValueType.Integer) +
                                      secondParameter.CalculateStrategyCost(SupportedValueType.Integer);

                    this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Integer) |
                                              GetSupportableConversions(SupportedValueType.Numeric);

                    foreach (SupportedValueType possibleType in GetSupportedTypeOptions(this.PossibleReturnType))
                    {
                        var totalIntegerCost = GetStandardConversionStrategyCost(
                            SupportedValueType.Integer,
                            in possibleType);

                        if (totalIntegerCost != int.MaxValue)
                        {
                            totalIntegerCost += integerCost;
                        }

                        var totalNumericCost = GetStandardConversionStrategyCost(
                            SupportedValueType.Numeric,
                            in possibleType);

                        if (totalNumericCost != int.MaxValue)
                        {
                            totalNumericCost += numericCost;
                        }

                        if (totalIntegerCost <= totalNumericCost)
                        {
                            this.CalculatedCosts[possibleType] = (totalIntegerCost, SupportedValueType.Integer);
                        }
                        else
                        {
                            this.CalculatedCosts[possibleType] = (totalNumericCost, SupportedValueType.Numeric);
                        }
                    }

                    break;
                }

                default:
                    throw new ExpressionNotValidLogicallyException();
            }
        }

        /// <summary>
        ///     Gets the parameters.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>
        ///     The properly-converted numeric parameters.
        /// </returns>
        /// <exception cref="ExpressionNotValidLogicallyException">The expression is not valid logically.</exception>
        protected (SupportedValueType ValueType, Expression First, Expression Second) GetParameters(
            in SupportedValueType valueType,
            in ComparisonTolerance tolerance)
        {
            if (!this.CalculatedCosts.TryGetValue(
                valueType,
                out (int Cost, SupportedValueType InternalType) tuple))
            {
                throw new ExpressionNotValidLogicallyException();
            }

            SupportedValueType internalType = tuple.InternalType;

            return (internalType, this.FirstParameter.GenerateExpression(
                in internalType,
                in tolerance), this.SecondParameter.GenerateExpression(
                in internalType,
                in tolerance));
        }

        /// <summary>
        ///     Gets the simplification expressions, if available.
        /// </summary>
        /// <returns>A success state, and the expressions.</returns>
        protected (bool Success, bool Integer, double FirstValue, double SecondValue, long FirstIntValue, long
            SecondIntValue) GetSimplificationExpressions()
        {
            if (this.FirstParameter is ConstantNodeBase fp && this.SecondParameter is ConstantNodeBase sp)
            {
                if (fp.TryGetInteger(out var ifirst) && sp.TryGetInteger(out var isecond))
                {
                    return (true, true, default, default, ifirst, isecond);
                }

                if (fp.TryGetNumeric(out var first) && sp.TryGetNumeric(out var second))
                {
                    return (true, false, first, second, default, default);
                }
            }

            return (false, default, default, default, default, default);
        }

#endregion
    }
}