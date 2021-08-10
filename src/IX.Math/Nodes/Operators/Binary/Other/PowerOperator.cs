// <copyright file="PowerOperator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using IX.Math.Values;

namespace IX.Math.Nodes.Operators.Binary.Other
{
    /// <summary>
    /// A raise to power (exponential) binary operator.
    /// </summary>
    internal sealed class PowerOperator : BinaryOperatorNodeBase
    {
        private const SupportableValueType SupportableValueTypes =
            SupportableValueType.Integer | SupportableValueType.Numeric;

        /// <summary>
        /// Initializes a new instance of the <see cref="PowerOperator"/> class.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        internal PowerOperator(
            NodeBase leftOperand,
            NodeBase rightOperand)
            : base(
                leftOperand,
                rightOperand) { }

        /// <summary>
        /// Calculates all supportable value types, as a result of the node and all nodes above it.
        /// </summary>
        /// <param name="constraints">The constraints to place on the node's supportable types.</param>
        /// <returns>The resulting supportable value type.</returns>
        /// <exception cref="ExpressionNotValidLogicallyException">The expression is not valid, either structurally or given the constraints.</exception>
        public override SupportableValueType CalculateSupportableValueType(
            SupportableValueType constraints = SupportableValueType.All)
        {
            if ((constraints & SupportableValueType.Numeric) == SupportableValueType.None)
            {
                return SupportableValueType.None;
            }

            var leftType = this.LeftOperand.CalculateSupportableValueType(SupportableValueTypes);
            if (leftType == SupportableValueType.None)
            {
                return SupportableValueType.None;
            }

            var rightType = this.RightOperand.CalculateSupportableValueType(SupportableValueTypes);
            if (rightType == SupportableValueType.None)
            {
                return SupportableValueType.None;
            }

            return SupportableValueType.Numeric;
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new PowerOperator(
                this.LeftOperand.DeepClone(context),
                this.RightOperand.DeepClone(context));

        /// <summary>
        ///     Generates the expression that will be compiled into code.
        /// </summary>
        /// <param name="forType">What type the expression is generated for.</param>
        /// <param name="tolerance">The tolerance. If this parameter is <c>null</c> (<c>Nothing</c> in Visual Basic), then the operation is exact.</param>
        /// <returns>
        ///     The generated <see cref="Expression" />.
        /// </returns>
        public override Expression GenerateExpression(
            SupportedValueType forType,
            Tolerance? tolerance = null)
        {
            var leftType = this.LeftOperand.CalculateSupportableValueType(SupportableValueTypes);
            var rightType = this.RightOperand.CalculateSupportableValueType(SupportableValueTypes);

            switch (forType)
            {
                case SupportedValueType.Numeric:
                    Expression leftExpression;
                    if (leftType == SupportableValueType.Integer)
                    {
                        leftExpression = Expression.Convert(
                            this.LeftOperand.GenerateExpression(
                                SupportedValueType.Integer,
                                tolerance),
                            typeof(double));
                    }
                    else
                    {
                        leftExpression = this.LeftOperand.GenerateExpression(
                            SupportedValueType.Numeric,
                            tolerance);
                    }

                    Expression rightExpression;
                    if (rightType == SupportableValueType.Integer)
                    {
                        rightExpression = Expression.Convert(
                            this.RightOperand.GenerateExpression(
                                SupportedValueType.Integer,
                                tolerance),
                            typeof(double));
                    }
                    else
                    {
                        rightExpression = this.RightOperand.GenerateExpression(
                            SupportedValueType.Numeric,
                            tolerance);
                    }

                    return Expression.Power(
                        leftExpression,
                        rightExpression);

                default:
                    throw new ExpressionNotValidLogicallyException();
            }
        }

        /// <summary>
        ///     Simplifies this node, if possible, based on a constant operand value.
        /// </summary>
        /// <returns>A simplified node, or this instance.</returns>
        private protected override NodeBase SimplifyOnConvertibleValue(
            ConvertibleValue leftValue,
            ConvertibleValue rightValue)
        {
            double left = leftValue.HasNumeric ? leftValue.GetNumeric() :
                leftValue.HasInteger ? Convert.ToDouble(leftValue.GetInteger()) :
                throw new ExpressionNotValidLogicallyException();
            double right = rightValue.HasNumeric ? rightValue.GetNumeric() :
                rightValue.HasInteger ? Convert.ToDouble(rightValue.GetInteger()) :
                throw new ExpressionNotValidLogicallyException();

            return new ConstantNode(global::System.Math.Pow(left, right));
        }
    }
}