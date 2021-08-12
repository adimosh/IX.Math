// <copyright file="XorOperator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections;
using System.Linq.Expressions;
using IX.Math.Values;

namespace IX.Math.Nodes.Operators.Binary.Logical
{
    /// <summary>
    /// An exclusive or operator.
    /// </summary>
    internal sealed class XorOperator : LogicalOperatorNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XorOperator"/> class.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        internal XorOperator(
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
            new XorOperator(
                this.LeftOperand.DeepClone(context),
                this.RightOperand.DeepClone(context));

        /// <summary>
        ///     Simplifies this node, if possible, based on a constant operand value.
        /// </summary>
        /// <returns>A simplified node, or this instance.</returns>
        private protected override NodeBase SimplifyOnConvertibleValue(
            ConvertibleValue leftValue,
            ConvertibleValue rightValue)
        {
            if (leftValue.HasBoolean && rightValue.HasBoolean)
            {
                return new ConstantNode(leftValue.GetBoolean() ^ rightValue.GetBoolean());
            }

            if (leftValue.HasInteger && rightValue.HasInteger)
            {
                return new ConstantNode(leftValue.GetInteger() ^ rightValue.GetInteger());
            }

            if (leftValue.HasBinary && rightValue.HasBinary)
            {
                return new ConstantNode(
                    BinaryOperation(
                        leftValue.GetBinary(),
                        rightValue.GetBinary()));
            }

            throw new ExpressionNotValidLogicallyException();
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
            Expression.ExclusiveOr(
                left,
                right);

        /// <summary>
        /// Generates a binary mathematical expression.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <returns>An expression containing the operation.</returns>
        private protected override Expression GenerateBinaryExpression(
            Expression left,
            Expression right)
        {
            var mi = typeof(AndOperator).GetMethod(
                         nameof(this.BinaryOperation),
                         new[]
                         {
                             typeof(byte[]),
                             typeof(byte[])
                         }) ??
                     throw new InvalidOperationException();

            return Expression.Call(
                mi,
                left,
                right);
        }

        /// <summary>
        /// Generates a boolean mathematical expression.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <returns>An expression containing the operation.</returns>
        private protected override Expression GenerateBooleanExpression(
            Expression left,
            Expression right) =>
            Expression.ExclusiveOr(
                left,
                right);

        private static byte[] BinaryOperation(
            byte[] left,
            byte[] right)
        {
            BitArray leftArray = new(left);
            BitArray rightArray = new(right);
            BitArray result;

            if (left.Length > right.Length)
            {
                leftArray.Xor(rightArray);
                result = leftArray;
            }
            else
            {
                rightArray.Xor(leftArray);
                result = rightArray;
            }

            byte[] res = new byte[result.Length / 8];
            result.CopyTo(
                res,
                0);

            return res;
        }
    }
}