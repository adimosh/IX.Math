// <copyright file="SimpleMathematicalOperationNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using IX.Math.Exceptions;
using IX.Math.Nodes.Constants;

namespace IX.Math.Nodes.Operators.Binary.Mathematic
{
    /// <summary>
    ///     A node base for simple mathematical operations.
    /// </summary>
    /// <seealso cref="BinaryOperatorNodeBase" />
    internal abstract class SimpleMathematicalOperationNodeBase : BinaryOperatorNodeBase
    {
#region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="SimpleMathematicalOperationNodeBase" /> class.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        protected private SimpleMathematicalOperationNodeBase(
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
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>A simplified node, or this instance.</returns>
        public sealed override NodeBase Simplify()
        {
            if (!(this.Left is ConstantNodeBase lc) || !(this.Right is ConstantNodeBase rc))
            {
                return this;
            }

            if (lc.TryGetInteger(out var llv) && rc.TryGetInteger(out var rlv))
            {
                var (isNumeric, lv, dv) = this.CalculateConstantValue(
                    llv,
                    rlv);

                return isNumeric ? new NumericNode(dv) : (NodeBase)new IntegerNode(lv);
            }

            if (lc.TryGetNumeric(out var ldv) && rc.TryGetNumeric(out var rdv))
            {
                var (isNumeric, lv, dv) = this.CalculateConstantValue(
                    ldv,
                    rdv);

                return isNumeric ? new NumericNode(dv) : (NodeBase)new IntegerNode(lv);
            }

            return this;
        }

        /// <summary>
        ///     Calculates the constant value.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>Either a long or a double, depending on the circumstances.</returns>
        protected abstract (bool IsNumeric, long Integer, double Numeric) CalculateConstantValue(
            long left,
            long right);

        /// <summary>
        ///     Calculates the constant value.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>Either a long or a double, depending on the circumstances.</returns>
        protected abstract (bool IsNumeric, long Integer, double Numeric) CalculateConstantValue(
            double left,
            double right);

        /// <summary>
        ///     Ensures that the operands are compatible, and refines the return type of this expression based on them.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <exception cref="ExpressionNotValidLogicallyException">Calculation on unsupported types.</exception>
        protected sealed override void EnsureCompatibleOperandsAndRefineReturnType(
            ref NodeBase left,
            ref NodeBase right)
        {
            EnsureNode(
                ref left,
                SupportableValueType.Numeric | SupportableValueType.Integer);
            EnsureNode(
                ref right,
                SupportableValueType.Numeric | SupportableValueType.Integer);

            const SupportableValueType logicalMaximumSupport =
                SupportableValueType.Integer | SupportableValueType.Numeric;

            SupportableValueType commonType = left.VerifyPossibleType(
                right.VerifyPossibleType(left.VerifyPossibleType(logicalMaximumSupport)));

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

            foreach (SupportedValueType supportedType in GetSupportedTypeOptions(this.PossibleReturnType))
            {
                int totalIntCost, totalNumericCost;

                checked
                {
                    totalIntCost = GetTotalConversionCosts(
                        in intCost,
                        SupportedValueType.Integer,
                        in supportedType);
                    totalNumericCost = GetTotalConversionCosts(
                        in numericCost,
                        SupportedValueType.Numeric,
                        in supportedType);
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

#endregion
    }
}