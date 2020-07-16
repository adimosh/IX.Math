// <copyright file="SimpleMathematicalOperationNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Collections.Generic;
using IX.Math.Exceptions;
using IX.Math.Extensibility;

namespace IX.Math.Nodes.Operators.Binary.Mathematic
{
    /// <summary>
    /// A node base for simple mathematical operations.
    /// </summary>
    /// <seealso cref="BinaryOperatorNodeBase" />
    internal abstract class SimpleMathematicalOperationNodeBase : BinaryOperatorNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleMathematicalOperationNodeBase" /> class.
        /// </summary>
        /// <param name="stringFormatters">The string formatters.</param>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        protected private SimpleMathematicalOperationNodeBase(
            List<IStringFormatter> stringFormatters,
            NodeBase left,
            NodeBase right)
            : base(
                stringFormatters,
                left,
                right)
        {
        }

        protected sealed override void EnsureCompatibleOperandsAndRefineReturnType(NodeBase left, NodeBase right)
        {
            const SupportableValueType logicalMaximumSupport =
                SupportableValueType.Integer | SupportableValueType.Numeric;

            var commonType = left.VerifyPossibleType(right.VerifyPossibleType(left.VerifyPossibleType(logicalMaximumSupport)));

            int intCost = int.MaxValue, numericCost = int.MaxValue;

            switch (commonType)
            {
                case SupportableValueType.Integer:
                    this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Integer);
                    checked
                    {
                        intCost = left.CalculateStrategyCost(SupportedValueType.Integer) +
                           right.CalculateStrategyCost(SupportedValueType.Integer);
                    }

                    break;
                case SupportableValueType.Numeric:
                    this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Numeric);
                    checked
                    {
                        numericCost = left.CalculateStrategyCost(SupportedValueType.Numeric) +
                           right.CalculateStrategyCost(SupportedValueType.Numeric);
                    }

                    break;
                case SupportableValueType.Integer | SupportableValueType.Numeric:
                    this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Integer) |
                                              GetSupportableConversions(SupportedValueType.Numeric);
                    checked
                    {
                        intCost = left.CalculateStrategyCost(SupportedValueType.Integer) +
                           right.CalculateStrategyCost(SupportedValueType.Integer);
                        numericCost = left.CalculateStrategyCost(SupportedValueType.Numeric) +
                           right.CalculateStrategyCost(SupportedValueType.Numeric);
                    }

                    break;
                default:
                    throw new ExpressionNotValidLogicallyException();
            }

            foreach (var supportedType in GetSupportedTypeOptions(this.PossibleReturnType))
            {
                int totalIntCost, totalNumericCost;

                checked
                {
                    totalIntCost = this.GetTotalConversionCosts(in intCost, SupportedValueType.Integer, in supportedType);
                    totalNumericCost = this.GetTotalConversionCosts(in numericCost, SupportedValueType.Numeric, in supportedType);
                }

                if (totalNumericCost < totalIntCost)
                {
                    // Numeric if it's more advantageous
                    this.CalculatedCosts[supportedType] = (totalNumericCost, SupportedValueType.Numeric);
                }
                else
                {
                    // Integer otherwise
                    this.CalculatedCosts[supportedType] = (totalIntCost, SupportedValueType.Integer);
                }
            }
        }
    }
}