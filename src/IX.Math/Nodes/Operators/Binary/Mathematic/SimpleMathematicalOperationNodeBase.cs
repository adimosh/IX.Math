// <copyright file="SimpleMathematicalOperationNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Collections.Generic;
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

            var leftType = left.VerifyPossibleType(logicalMaximumSupport);
            var rightType = right.VerifyPossibleType(logicalMaximumSupport);

            int cost;
            SupportedValueType svt;

            if (((leftType & SupportableValueType.Integer) == SupportableValueType.Integer) &&
                ((rightType & SupportableValueType.Integer) == SupportableValueType.Integer))
            {
                // Integer only if both support it
                this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Numeric) |
                                          GetSupportableConversions(SupportedValueType.Integer);
                cost = left.CalculateStrategyCost(SupportedValueType.Integer) +
                       right.CalculateStrategyCost(SupportedValueType.Integer);
                svt = SupportedValueType.Integer;
            }
            else
            {
                this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Numeric);
                cost = left.CalculateStrategyCost(SupportedValueType.Numeric) +
                       right.CalculateStrategyCost(SupportedValueType.Numeric);
                svt = SupportedValueType.Numeric;
            }

            foreach (var supportedType in GetSupportedTypeOptions(this.PossibleReturnType))
            {
                this.CalculatedCosts[supportedType] = (GetStandardConversionStrategyCost(
                                                           in svt,
                                                           in supportedType) +
                                                       cost, svt);
            }
        }
    }
}