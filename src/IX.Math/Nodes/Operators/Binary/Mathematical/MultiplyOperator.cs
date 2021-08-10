// <copyright file="MultiplyOperator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using IX.Math.Values;

namespace IX.Math.Nodes.Operators.Binary.Mathematical
{
    /// <summary>
    /// A multiplication binary operator.
    /// </summary>
    internal sealed class MultiplyOperator : MathematicalOperatorNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultiplyOperator"/> class.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        internal MultiplyOperator(
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
            new MultiplyOperator(
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
                return new ConstantNode(leftValue.GetInteger() * rightValue.GetInteger());
            }

            double left = leftValue.HasNumeric ? leftValue.GetNumeric() :
                leftValue.HasInteger ? Convert.ToDouble(leftValue.GetInteger()) :
                throw new ExpressionNotValidLogicallyException();
            double right = rightValue.HasNumeric ? rightValue.GetNumeric() :
                rightValue.HasInteger ? Convert.ToDouble(rightValue.GetInteger()) :
                throw new ExpressionNotValidLogicallyException();

            return new ConstantNode(left * right);
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
            Expression.Multiply(
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
            Expression.Multiply(
                left,
                right);
    }
}