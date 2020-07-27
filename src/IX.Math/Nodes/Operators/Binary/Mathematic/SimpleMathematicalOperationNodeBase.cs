// <copyright file="SimpleMathematicalOperationNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Collections.Generic;
using IX.Math.Exceptions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;

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

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>A simplified node, or this instance.</returns>
        public sealed override NodeBase Simplify()
        {
            if (!(this.Left is ConstantNodeBase lc) || !(this.Right is ConstantNodeBase rc))
            {
                return this;
            }

            if (lc.TryGetInteger(out long llv) && rc.TryGetInteger(out long rlv))
            {
                var (isNumeric, lv, dv) = this.CalculateConstantValue(
                    llv,
                    rlv);

                return isNumeric ? this.GenerateConstantNumeric(dv) : (NodeBase)this.GenerateConstantInteger(lv);
            }

            if (lc.TryGetNumeric(out double ldv) && rc.TryGetNumeric(out double rdv))
            {
                var (isNumeric, lv, dv) = this.CalculateConstantValue(
                    ldv,
                    rdv);

                return isNumeric ? this.GenerateConstantNumeric(dv) : (NodeBase)this.GenerateConstantInteger(lv);
            }

            return this;
        }

        /// <summary>
        /// Calculates the constant value.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>Either a long or a double, depending on the circumstances.</returns>
        protected abstract (bool, long, double) CalculateConstantValue(
            long left,
            long right);

        /// <summary>
        /// Calculates the constant value.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>Either a long or a double, depending on the circumstances.</returns>
        protected abstract (bool, long, double) CalculateConstantValue(
            double left,
            double right);

        /// <summary>
        /// Ensures that the operands are compatible, and refines the return type of this expression based on them.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <exception cref="ExpressionNotValidLogicallyException">Calculation on unsupported types.</exception>
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