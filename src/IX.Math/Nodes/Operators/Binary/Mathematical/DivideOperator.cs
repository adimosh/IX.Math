// <copyright file="DivideOperator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using IX.Math.Values;

namespace IX.Math.Nodes.Operators.Binary.Mathematical
{
    /// <summary>
    /// A division binary operator.
    /// </summary>
    internal sealed class DivideOperator : MathematicalOperatorNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DivideOperator"/> class.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        internal DivideOperator(
            NodeBase leftOperand,
            NodeBase rightOperand)
            : base(
                leftOperand,
                rightOperand) { }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new DivideOperator(
                this.LeftOperand.DeepClone(context),
                this.RightOperand.DeepClone(context));

        /// <summary>
        ///     Simplifies this node, if possible, based on a constant operand value.
        /// </summary>
        /// <returns>A simplified node, or this instance.</returns>
        protected private override NodeBase SimplifyOnConvertibleValue(
            ConvertibleValue leftValue,
            ConvertibleValue rightValue)
        {
            if (leftValue.HasInteger & rightValue.HasInteger)
            {
                return new ConstantNode(leftValue.GetInteger() / rightValue.GetInteger());
            }

            double left = leftValue.HasNumeric ? leftValue.GetNumeric() :
                leftValue.HasInteger ? Convert.ToDouble(leftValue.GetInteger()) :
                throw new ExpressionNotValidLogicallyException();
            double right = rightValue.HasNumeric ? rightValue.GetNumeric() :
                rightValue.HasInteger ? Convert.ToDouble(rightValue.GetInteger()) :
                throw new ExpressionNotValidLogicallyException();

            return new ConstantNode(left / right);
        }

        /// <summary>
        /// Generates an integer mathematical expression.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <returns>An expression containing the operation.</returns>
        private protected override Expression GenerateIntegerExpression(
            Expression left,
            Expression right) =>
            Expression.Divide(
                left,
                right);

        /// <summary>
        /// Generates a numeric mathematical expression.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <returns>An expression containing the operation.</returns>
        private protected override Expression GenerateNumericExpression(
            Expression left,
            Expression right) =>
            Expression.Divide(
                left,
                right);
    }
}