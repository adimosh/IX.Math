// <copyright file="LogicalOperatorNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;
using IX.Math.Exceptions;
using IX.Math.Nodes.Constants;

namespace IX.Math.Nodes.Operators.Binary.Logical
{
    /// <summary>
    ///     A node base for logical operations.
    /// </summary>
    /// <seealso cref="BinaryOperatorNodeBase" />
    internal abstract class LogicalOperatorNodeBase : BinaryOperatorNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogicalOperatorNodeBase" /> class.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        protected private LogicalOperatorNodeBase(
            NodeBase left,
            NodeBase right)
            : base(
                left,
                right)
        {
        }

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>
        ///     A simplified node, or this instance.
        /// </returns>
        public sealed override NodeBase Simplify()
        {
            if (!(this.Left is ConstantNodeBase left))
            {
                return this;
            }

            if (!(this.Right is ConstantNodeBase right))
            {
                return this;
            }

            if (left.TryGetInteger(out long lint) &&
                right.TryGetInteger(out long rint))
            {
                return new IntegerNode(
                    this.GenerateData(
                        lint,
                        rint));
            }

            if (left.TryGetBoolean(out bool lbool) &&
                right.TryGetBoolean(out bool rbool))
            {
                return new BoolNode(
                    this.GenerateData(
                        lbool,
                        rbool));
            }

            if (left.TryGetBinary(out byte[] lbin) &&
                right.TryGetBinary(out byte[] rbin))
            {
                return new BinaryNode(
                    this.GenerateData(
                        lbin,
                        rbin));
            }

            return this;
        }

        /// <summary>
        /// Generates simplified data.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The resulting simplified data.</returns>
        protected abstract long GenerateData(
                    long left,
                    long right);

        /// <summary>
        /// Generates simplified data.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The resulting simplified data.</returns>
        protected abstract bool GenerateData(
            bool left,
            bool right);

        /// <summary>
        /// Generates simplified data.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The resulting simplified data.</returns>
        protected abstract byte[] GenerateData(
            byte[] left,
            byte[] right);

        /// <summary>
        /// Ensures that the operands are compatible, and refines the return type of this expression based on them.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <exception cref="MathematicsEngineException">
        /// The verification methods of the operands did not behave properly.
        /// </exception>
        protected sealed override void EnsureCompatibleOperandsAndRefineReturnType(
            ref NodeBase left,
            ref NodeBase right)
        {
            const SupportableValueType logicalMaximumSupport =
                SupportableValueType.Integer | SupportableValueType.Boolean | SupportableValueType.Binary;

            this.CalculatedCosts.Clear();

            var typeLeft = left.VerifyPossibleType(logicalMaximumSupport);
            var typeRight = right.VerifyPossibleType(logicalMaximumSupport);

            int intCost = int.MaxValue, boolCost = int.MaxValue, binaryCost = int.MaxValue;

            #region Ensure compatibility
            switch (typeLeft & typeRight)
            {
                case SupportableValueType.Boolean:
                    this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Boolean);
                    boolCost = left.CalculateStrategyCost(SupportedValueType.Boolean) +
                               right.CalculateStrategyCost(SupportedValueType.Boolean);
                    break;
                case SupportableValueType.Integer:
                    this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Integer);
                    intCost = left.CalculateStrategyCost(SupportedValueType.Integer) +
                              right.CalculateStrategyCost(SupportedValueType.Integer);
                    break;
                case SupportableValueType.Binary:
                    this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Binary);
                    binaryCost
 = left.CalculateStrategyCost(SupportedValueType.Binary) +
                                    right.CalculateStrategyCost(SupportedValueType.Binary);
                    break;
                case SupportableValueType.Boolean | SupportableValueType.Integer:
                    this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Boolean) |
                                              GetSupportableConversions(SupportedValueType.Integer);
                    boolCost = left.CalculateStrategyCost(SupportedValueType.Boolean) +
                               right.CalculateStrategyCost(SupportedValueType.Boolean);
                    intCost = left.CalculateStrategyCost(SupportedValueType.Integer) +
                              right.CalculateStrategyCost(SupportedValueType.Integer);
                    break;
                case SupportableValueType.Boolean | SupportableValueType.Binary:
                    this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Boolean) |
                                              GetSupportableConversions(SupportedValueType.Binary);
                    boolCost = left.CalculateStrategyCost(SupportedValueType.Boolean) +
                               right.CalculateStrategyCost(SupportedValueType.Boolean);
                    binaryCost
 = left.CalculateStrategyCost(SupportedValueType.Binary) +
                                    right.CalculateStrategyCost(SupportedValueType.Binary);
                    break;
                case SupportableValueType.Integer | SupportableValueType.Binary:
                    this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Integer) |
                                              GetSupportableConversions(SupportedValueType.Binary);
                    intCost = left.CalculateStrategyCost(SupportedValueType.Integer) +
                              right.CalculateStrategyCost(SupportedValueType.Integer);
                    binaryCost
 = left.CalculateStrategyCost(SupportedValueType.Binary) +
                                    right.CalculateStrategyCost(SupportedValueType.Binary);
                    break;
                case logicalMaximumSupport:
                    this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Boolean) |
                                              GetSupportableConversions(SupportedValueType.Integer) |
                                              GetSupportableConversions(SupportedValueType.Binary);
                    boolCost = left.CalculateStrategyCost(SupportedValueType.Boolean) +
                               right.CalculateStrategyCost(SupportedValueType.Boolean);
                    intCost = left.CalculateStrategyCost(SupportedValueType.Integer) +
                              right.CalculateStrategyCost(SupportedValueType.Integer);
                    binaryCost
 = left.CalculateStrategyCost(SupportedValueType.Binary) +
                                    right.CalculateStrategyCost(SupportedValueType.Binary);
                    break;
                default:
                    throw new ExpressionNotValidLogicallyException();
            }
            #endregion

            foreach (var supportedOption in GetSupportedTypeOptions(this.PossibleReturnType))
            {
                int intTotalCost = GetTotalConversionCosts(
                    in intCost,
                    SupportedValueType.Integer,
                    in supportedOption);

                int byteArrayTotalCost = GetTotalConversionCosts(
                    in binaryCost
,
                    SupportedValueType.Binary,
                    in supportedOption);

                int boolTotalCost = GetTotalConversionCosts(
                    in boolCost,
                    SupportedValueType.Boolean,
                    in supportedOption);

                #region Set smallest cost for this return type
                if (intTotalCost != int.MaxValue)
                {
                    // Integer preferred
                    if (byteArrayTotalCost < intTotalCost)
                    {
                        if (byteArrayTotalCost < boolTotalCost)
                        {
                            // Byte array is cheapest
                            this.CalculatedCosts[supportedOption] = (byteArrayTotalCost, SupportedValueType.Binary);
                        }
                        else
                        {
                            // Boolean is cheapest
                            this.CalculatedCosts[supportedOption] = (boolTotalCost, SupportedValueType.Boolean);
                        }
                    }
                    else
                    {
                        if (boolTotalCost < intTotalCost)
                        {
                            // Boolean is cheapest
                            this.CalculatedCosts[supportedOption] = (boolTotalCost, SupportedValueType.Boolean);
                        }
                        else
                        {
                            // Integer is cheapest
                            this.CalculatedCosts[supportedOption] = (intTotalCost, SupportedValueType.Integer);
                        }
                    }
                }
                else if (byteArrayTotalCost != int.MaxValue)
                {
                    // Byte array preferred if int not available
                    if (byteArrayTotalCost < boolTotalCost)
                    {
                        // Byte array is cheapest
                        this.CalculatedCosts[supportedOption] = (byteArrayTotalCost, SupportedValueType.Binary);
                    }
                    else
                    {
                        // Boolean is cheapest
                        this.CalculatedCosts[supportedOption] = (boolTotalCost, SupportedValueType.Boolean);
                    }
                }
                else if (boolTotalCost != int.MaxValue)
                {
                    // Boolean if nothing else is available, but it is
                    this.CalculatedCosts[supportedOption] = (boolTotalCost, SupportedValueType.Boolean);
                }
                else
                {
                    // Nothing else matters
                    throw new MathematicsEngineException();
                }
                #endregion
            }
        }

        /// <summary>
        /// Generates the parameter expressions based on the best execution strategy.
        /// </summary>
        /// <param name="returnType">Type of the return.</param>
        /// <param name="comparisonTolerance">The comparison tolerance.</param>
        /// <returns>
        /// The parameter expressions.
        /// </returns>
        /// <exception cref="ExpressionNotValidLogicallyException">The expression is not valid logically.</exception>
        protected (Expression Left, Expression Right, bool IsBinary) GenerateParameterExpressions(in SupportedValueType returnType, in ComparisonTolerance comparisonTolerance)
        {
            Expression left, right;
            if (this.CalculatedCosts.TryGetValue(
                returnType,
                out var tuple))
            {
                left = this.Left.GenerateExpression(
                    in tuple.InternalType,
                    in comparisonTolerance);
                right = this.Right.GenerateExpression(
                    in tuple.InternalType,
                    in comparisonTolerance);
            }
            else
            {
                throw new ExpressionNotValidLogicallyException();
            }

            return (left, right, tuple.InternalType == SupportedValueType.Binary);
        }
    }
}