// <copyright file="SimpleNumericOnlyMathematicalOperationNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using IX.Math.Exceptions;
using IX.Math.Nodes.Constants;

namespace IX.Math.Nodes.Operators.Binary.Mathematic
{
    internal abstract class SimpleNumericOnlyMathematicalOperationNodeBase : BinaryOperatorNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SimpleNumericOnlyMathematicalOperationNodeBase" /> class.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        protected private SimpleNumericOnlyMathematicalOperationNodeBase(
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
        /// <returns>A simplified node, or this instance.</returns>
        public sealed override NodeBase Simplify()
        {
            if (!(this.Left is ConstantNodeBase lc) || !(this.Right is ConstantNodeBase rc))
            {
                return this;
            }

            if (lc.TryGetNumeric(out double ldv) && rc.TryGetNumeric(out double rdv))
            {
                return GenerateConstantNumeric(this.CalculateConstantValue(
                    ldv,
                    rdv));
            }

            return this;
        }

        /// <summary>
        /// Calculates the constant value.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>Either a long or a double, depending on the circumstances.</returns>
        protected abstract double CalculateConstantValue(
            double left,
            double right);

        /// <summary>
        /// Generates the expression.
        /// </summary>
        /// <param name="leftOperandExpression">The left operand expression.</param>
        /// <param name="rightOperandExpression">The right operand expression.</param>
        /// <returns>The resulting expression.</returns>
        protected abstract Expression GenerateExpression(
            Expression leftOperandExpression,
            Expression rightOperandExpression);

        /// <summary>
        /// Ensures that the operands are compatible, and refines the return type of this expression based on them.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <exception cref="ExpressionNotValidLogicallyException">There is no way to calculate this expression.</exception>
        protected sealed override void EnsureCompatibleOperandsAndRefineReturnType(NodeBase left, NodeBase right)
        {
            var leftType = left.VerifyPossibleType(SupportableValueType.Numeric);
            var rightType = right.VerifyPossibleType(SupportableValueType.Numeric);

            int cost;
            SupportedValueType svt;

            if (leftType == rightType && leftType != SupportableValueType.None)
            {
                this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Numeric);
                cost = left.CalculateStrategyCost(SupportedValueType.Numeric) +
                       right.CalculateStrategyCost(SupportedValueType.Numeric);
                svt = SupportedValueType.Numeric;
            }
            else
            {
                throw new ExpressionNotValidLogicallyException();
            }

            foreach (var supportedType in GetSupportedTypeOptions(this.PossibleReturnType))
            {
                this.CalculatedCosts[supportedType] = (GetStandardConversionStrategyCost(
                                                           in svt,
                                                           in supportedType) +
                                                       cost, svt);
            }
        }

        /// <summary>
        /// Generates the expression that this node represents.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="comparisonTolerance">The comparison tolerance.</param>
        /// <returns>A compiled expression, if one is possible.</returns>
        protected sealed override Expression GenerateExpressionInternal(
            in SupportedValueType valueType,
            in ComparisonTolerance comparisonTolerance)
        {
            try
            {
                return this.GenerateExpression(
                    this.Left.GenerateExpression(
                        SupportedValueType.Numeric,
                        in comparisonTolerance),
                    this.Right.GenerateExpression(
                        SupportedValueType.Numeric,
                        in comparisonTolerance));
            }
            catch (ExpressionNotValidLogicallyException)
            {
                throw;
            }
            catch (MathematicsEngineException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ExpressionNotValidLogicallyException(ex);
            }
        }
    }
}