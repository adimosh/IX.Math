// <copyright file="ByteShiftOperatorNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using IX.Math.Exceptions;

namespace IX.Math.Nodes.Operators.Binary.Bitwise
{
    /// <summary>
    ///     A node base for byte shift operations.
    /// </summary>
    /// <seealso cref="BinaryOperatorNodeBase" />
    internal abstract class ByteShiftOperatorNodeBase : BinaryOperatorNodeBase
    {
#region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ByteShiftOperatorNodeBase" /> class.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        protected private ByteShiftOperatorNodeBase(
            NodeBase left,
            NodeBase right)
            : base(
                left,
                right)
        {
        }

#endregion

#region Methods

        /// <summary>
        ///     Ensures that the operands are compatible, and refines the return type of this expression based on them.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        protected sealed override void EnsureCompatibleOperandsAndRefineReturnType(
            ref NodeBase left,
            ref NodeBase right)
        {
            this.CalculatedCosts.Clear();

            EnsureNode(
                ref left,
                SupportableValueType.Integer | SupportableValueType.Binary);
            EnsureNode(
                ref right,
                SupportableValueType.Integer);

            _ = right.VerifyPossibleType(SupportableValueType.Integer);
            SupportableValueType leftType =
                left.VerifyPossibleType(SupportableValueType.Integer | SupportableValueType.Binary);

            var rightCost = right.CalculateStrategyCost(SupportedValueType.Integer);

            int intCost = int.MaxValue, binaryCost = int.MaxValue;

            if (leftType == SupportableValueType.Integer)
            {
                this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Integer);
                intCost = left.CalculateStrategyCost(SupportedValueType.Integer) + rightCost;
            }
            else if (leftType == SupportableValueType.Binary)
            {
                this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Binary);
                binaryCost = left.CalculateStrategyCost(SupportedValueType.Binary) + rightCost;
            }
            else
            {
                this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Integer) |
                                          GetSupportableConversions(SupportedValueType.Binary);
                intCost = left.CalculateStrategyCost(SupportedValueType.Integer) + rightCost;
                binaryCost = left.CalculateStrategyCost(SupportedValueType.Binary) + rightCost;
            }

            foreach (SupportedValueType supportedOption in GetSupportedTypeOptions(this.PossibleReturnType))
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
                if (binaryCost == int.MaxValue)
                {
                    byteArrayTotalCost = int.MaxValue;
                }
                else
                {
                    var conversionCost = GetStandardConversionStrategyCost(
                        SupportedValueType.Binary,
                        in supportedOption);

                    if (conversionCost == int.MaxValue)
                    {
                        byteArrayTotalCost = int.MaxValue;
                    }
                    else
                    {
                        byteArrayTotalCost = conversionCost + binaryCost;
                    }
                }

                if (intTotalCost != int.MaxValue)
                {
                    // Int preferred
                    if (byteArrayTotalCost < intTotalCost)
                    {
                        // Byte array is cheapest
                        this.CalculatedCosts[supportedOption] = (byteArrayTotalCost, SupportedValueType.Binary);
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
                    this.CalculatedCosts[supportedOption] = (byteArrayTotalCost, SupportedValueType.Binary);
                }
                else
                {
                    // Nothing else matters
                    throw new MathematicsEngineException();
                }
            }
        }

#endregion
    }
}