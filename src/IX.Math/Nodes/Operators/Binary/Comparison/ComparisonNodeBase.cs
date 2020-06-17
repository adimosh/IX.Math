// <copyright file="ComparisonNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Collections.Generic;
using System.Linq.Expressions;
using IX.Math.Exceptions;
using IX.Math.Extensibility;

namespace IX.Math.Nodes.Operators.Binary.Comparison
{
    /// <summary>
    ///     A base node for comparison operations.
    /// </summary>
    /// <seealso cref="BinaryOperatorNodeBase" />
    internal abstract class ComparisonNodeBase : BinaryOperatorNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ComparisonNodeBase" /> class.
        /// </summary>
        /// <param name="stringFormatters">The string formatters.</param>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        protected private ComparisonNodeBase(
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
        ///     Gets a value indicating whether or not this node is actually a constant.
        /// </summary>
        /// <value><see langword="true" /> if the node is a constant, <see langword="false" /> otherwise.</value>
        public override bool IsConstant => false;

        /// <summary>
        ///     Ensures that the operands are compatible, and refines the return type of this expression based on them.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        protected sealed override void EnsureCompatibleOperandsAndRefineReturnType(
            NodeBase left,
            NodeBase right)
        {
            this.CalculatedCosts.Clear();
            var commonSupportedTypes = left.PossibleReturnType & right.PossibleReturnType;

            if (commonSupportedTypes == SupportableValueType.None)
            {
                throw new ExpressionNotValidLogicallyException();
            }

            this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Boolean);
            foreach (var supportedType in GetSupportedTypeOptions(this.PossibleReturnType))
            {
                this.CalculatedCosts[supportedType] = (GetStandardConversionStrategyCost(
                    SupportedValueType.Boolean,
                    in supportedType), SupportedValueType.Unknown);
            }
        }

        /// <summary>
        /// Gets the expression arguments.
        /// </summary>
        /// <param name="comparisonTolerance">The comparison tolerance.</param>
        /// <returns>The expression arguments, depending on what those arguments actually are.</returns>
        protected (Expression Left, Expression Right, SupportedValueType ValueType) GetExpressionArguments(
            in ComparisonTolerance comparisonTolerance)
        {
            var commonSupportedTypes = this.Left.PossibleReturnType & this.Right.PossibleReturnType;

            if ((commonSupportedTypes & SupportableValueType.Integer) != SupportableValueType.None)
            {
                // Integer preferred
                return (this.Left.GenerateExpression(
                    SupportedValueType.Integer,
                    in comparisonTolerance), this.Right.GenerateExpression(
                    SupportedValueType.Integer,
                    in comparisonTolerance), SupportedValueType.Integer);
            }

            if ((commonSupportedTypes & SupportableValueType.Numeric) != SupportableValueType.None)
            {
                // Numeric preferred if integer is not available
                return (this.Left.GenerateExpression(
                    SupportedValueType.Numeric,
                    in comparisonTolerance), this.Right.GenerateExpression(
                    SupportedValueType.Numeric,
                    in comparisonTolerance), SupportedValueType.Numeric);
            }

            if ((commonSupportedTypes & SupportableValueType.ByteArray) != SupportableValueType.None)
            {
                // Byte array preferred if integer and numeric are not available
                return (this.Left.GenerateExpression(
                    SupportedValueType.ByteArray,
                    in comparisonTolerance), this.Right.GenerateExpression(
                    SupportedValueType.ByteArray,
                    in comparisonTolerance), SupportedValueType.ByteArray);
            }

            if ((commonSupportedTypes & SupportableValueType.Boolean) != SupportableValueType.None)
            {
                // Boolean preferred if no multi-bit type is available
                return (this.Left.GenerateExpression(
                    SupportedValueType.Boolean,
                    in comparisonTolerance), this.Right.GenerateExpression(
                    SupportedValueType.Boolean,
                    in comparisonTolerance), SupportedValueType.Boolean);
            }

            // String is least preferred
            return (this.Left.GenerateExpression(
                SupportedValueType.Integer,
                in comparisonTolerance), this.Right.GenerateExpression(
                SupportedValueType.Integer,
                in comparisonTolerance), SupportedValueType.Integer);
        }
    }
}