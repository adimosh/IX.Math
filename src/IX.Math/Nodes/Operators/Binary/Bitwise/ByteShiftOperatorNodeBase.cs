// <copyright file="ByteShiftOperatorNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Collections.Generic;
using IX.Math.Exceptions;
using IX.Math.Extensibility;

namespace IX.Math.Nodes.Operators.Binary.Bitwise
{
    /// <summary>
    /// A node base for byte shift operations.
    /// </summary>
    /// <seealso cref="BinaryOperatorNodeBase" />
    internal abstract class ByteShiftOperatorNodeBase : BinaryOperatorNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ByteShiftOperatorNodeBase" /> class.
        /// </summary>
        /// <param name="stringFormatters">The string formatters.</param>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        protected private ByteShiftOperatorNodeBase(
            List<IStringFormatter> stringFormatters,
            NodeBase left,
            NodeBase right)
            : base(
                stringFormatters,
                left,
                right)
        {
        }

        /// <summary>
        /// Ensures that the operands are compatible, and refines the return type of this expression based on them.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        protected sealed override void EnsureCompatibleOperandsAndRefineReturnType(NodeBase left, NodeBase right)
        {
            this.CalculatedCosts.Clear();
            _ = right.VerifyPossibleType(SupportableValueType.Integer);
            var leftType = left.VerifyPossibleType(SupportableValueType.Integer | SupportableValueType.ByteArray);

            int rightCost = right.CalculateStrategyCost(SupportedValueType.Integer);

            int intCost = int.MaxValue, byteArrayCost = int.MaxValue;

            if (leftType == SupportableValueType.Integer)
            {
                this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Integer);
                intCost = left.CalculateStrategyCost(SupportedValueType.Integer) + rightCost;
            }
            else if (leftType == SupportableValueType.ByteArray)
            {
                this.PossibleReturnType = GetSupportableConversions(SupportedValueType.ByteArray);
                byteArrayCost = left.CalculateStrategyCost(SupportedValueType.ByteArray) + rightCost;
            }
            else
            {
                this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Integer) |
                                          GetSupportableConversions(SupportedValueType.ByteArray);
                intCost = left.CalculateStrategyCost(SupportedValueType.Integer) + rightCost;
                byteArrayCost = left.CalculateStrategyCost(SupportedValueType.ByteArray) + rightCost;
            }

            foreach (var supportedOption in GetSupportedTypeOptions(this.PossibleReturnType))
            {
                int intTotalCost;
                if (intCost == int.MaxValue)
                {
                    intTotalCost = int.MaxValue;
                }
                else
                {
                    var conversionCost = GetStandardConversionStrategyCost(
                        SupportedValueType.Integer,
                        in supportedOption);

                    if (conversionCost == int.MaxValue)
                    {
                        intTotalCost = int.MaxValue;
                    }
                    else
                    {
                        intTotalCost = conversionCost + intCost;
                    }
                }

                int byteArrayTotalCost;
                if (byteArrayCost == int.MaxValue)
                {
                    byteArrayTotalCost = int.MaxValue;
                }
                else
                {
                    var conversionCost = GetStandardConversionStrategyCost(
                        SupportedValueType.ByteArray,
                        in supportedOption);

                    if (conversionCost == int.MaxValue)
                    {
                        byteArrayTotalCost = int.MaxValue;
                    }
                    else
                    {
                        byteArrayTotalCost = conversionCost + byteArrayCost;
                    }
                }

                if (intTotalCost != int.MaxValue)
                {
                    // Int preferred
                    if (byteArrayTotalCost < intTotalCost)
                    {
                        // Byte array is cheapest
                        this.CalculatedCosts[supportedOption] = (byteArrayTotalCost, SupportedValueType.ByteArray);
                    }
                    else
                    {
                        // Boolean is cheapest
                        this.CalculatedCosts[supportedOption] = (intTotalCost, SupportedValueType.Integer);
                    }
                }
                else if (byteArrayTotalCost != int.MaxValue)
                {
                    // Boolean if nothing else is available, but it is
                    this.CalculatedCosts[supportedOption] = (byteArrayTotalCost, SupportedValueType.ByteArray);
                }
                else
                {
                    // Nothing else matters
                    throw new MathematicsEngineException();
                }
            }
        }
    }
}