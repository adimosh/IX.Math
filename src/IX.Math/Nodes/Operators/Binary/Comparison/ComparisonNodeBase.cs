// <copyright file="ComparisonNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;
using IX.Math.Exceptions;

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
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        protected private ComparisonNodeBase(
            NodeBase left,
            NodeBase right)
            : base(
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
            ref NodeBase left,
            ref NodeBase right)
        {
            var commonSupportedTypes = left.PossibleReturnType & right.PossibleReturnType;

            if (commonSupportedTypes == SupportableValueType.None &&
                !(left.CheckSupportedType(SupportableValueType.String) ||
                right.CheckSupportedType(SupportableValueType.String)))
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
            var left = this.Left;
            var right = this.Right;

            var commonSupportedTypes = left.PossibleReturnType & right.PossibleReturnType;

            if ((commonSupportedTypes & SupportableValueType.Integer) != SupportableValueType.None)
            {
                // Integer preferred
                return (left.GenerateExpression(
                    SupportedValueType.Integer,
                    in comparisonTolerance), right.GenerateExpression(
                    SupportedValueType.Integer,
                    in comparisonTolerance), SupportedValueType.Integer);
            }

            if ((commonSupportedTypes & SupportableValueType.Numeric) != SupportableValueType.None)
            {
                // Numeric preferred if integer is not available
                return (left.GenerateExpression(
                    SupportedValueType.Numeric,
                    in comparisonTolerance), right.GenerateExpression(
                    SupportedValueType.Numeric,
                    in comparisonTolerance), SupportedValueType.Numeric);
            }

            if ((commonSupportedTypes & SupportableValueType.ByteArray) != SupportableValueType.None)
            {
                // Byte array preferred if integer and numeric are not available
                return (left.GenerateExpression(
                    SupportedValueType.ByteArray,
                    in comparisonTolerance), right.GenerateExpression(
                    SupportedValueType.ByteArray,
                    in comparisonTolerance), SupportedValueType.ByteArray);
            }

            if ((commonSupportedTypes & SupportableValueType.Boolean) != SupportableValueType.None)
            {
                // Boolean preferred if no multi-bit type is available
                return (left.GenerateExpression(
                    SupportedValueType.Boolean,
                    in comparisonTolerance), right.GenerateExpression(
                    SupportedValueType.Boolean,
                    in comparisonTolerance), SupportedValueType.Boolean);
            }

            // We have a string and an integer or a numeric
            if (left.CheckSupportedType(SupportableValueType.String))
            {
                Expression leftExp = left.GenerateExpression(
                    SupportedValueType.String,
                    in comparisonTolerance);
                Expression rightExp = null;

                if (right.CheckSupportedType(SupportableValueType.Integer))
                {
                    rightExp = right.GenerateExpression(
                        SupportedValueType.Integer,
                        in comparisonTolerance);
                }

                if (right.CheckSupportedType(SupportableValueType.Numeric))
                {
                    rightExp = right.GenerateExpression(
                        SupportedValueType.Numeric,
                        in comparisonTolerance);
                }

                if (rightExp != null)
                {
                    return (leftExp, rightExp, SupportedValueType.Unknown);
                }
            }

            if (right.CheckSupportedType(SupportableValueType.String))
            {
                Expression rightExp = right.GenerateExpression(
                    SupportedValueType.String,
                    in comparisonTolerance);
                Expression leftExp = null;

                if (left.CheckSupportedType(SupportableValueType.Integer))
                {
                    leftExp = left.GenerateExpression(
                        SupportedValueType.Integer,
                        in comparisonTolerance);
                }

                if (left.CheckSupportedType(SupportableValueType.Numeric))
                {
                    leftExp = left.GenerateExpression(
                        SupportedValueType.Numeric,
                        in comparisonTolerance);
                }

                if (leftExp != null)
                {
                    return (leftExp, rightExp, SupportedValueType.Unknown);
                }
            }

            // String is least preferred
            return (left.GenerateExpression(
                SupportedValueType.String,
                in comparisonTolerance), right.GenerateExpression(
                SupportedValueType.String,
                in comparisonTolerance), SupportedValueType.String);
        }
    }
}