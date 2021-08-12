// <copyright file="LeftShiftOperator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using IX.Math.Values;
using IX.StandardExtensions.Extensions;

namespace IX.Math.Nodes.Operators.Binary.ByteShift
{
    /// <summary>
    /// A left shift binary operator.
    /// </summary>
    internal sealed class LeftShiftOperator : ByteShiftOperatorNodeBase
    {
        private const long LongBitSize = sizeof(long) * 8;

        /// <summary>
        /// Initializes a new instance of the <see cref="LeftShiftOperator"/> class.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        internal LeftShiftOperator(
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
            new LeftShiftOperator(
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
            if (!rightValue.HasInteger)
            {
                throw new ExpressionNotValidLogicallyException();
            }

            var shiftWith = rightValue.GetInteger();

            if (leftValue.HasInteger)
            {
                var shiftInt = leftValue.GetInteger();

                if (shiftWith > LongBitSize)
                {
                    shiftInt = 0;
                }
                else
                {
                    shiftInt <<= (int)shiftWith;
                }

                return new ConstantNode(shiftInt);
            }

            if (leftValue.HasBinary)
            {
                return new ConstantNode(leftValue.GetBinary().LeftShift(Convert.ToInt32(shiftWith)));
            }

            throw new ExpressionNotValidLogicallyException();
        }

        /// <summary>
        /// Generates an integer byte shift expression.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <returns>An expression containing the operation.</returns>
        private protected override Expression GenerateIntegerExpression(
            Expression left,
            Expression right) =>
            Expression.Condition(
                Expression.GreaterThan(
                    left,
                    Expression.Constant(
                        LongBitSize,
                        typeof(long))),
                Expression.Constant(
                    0L,
                    typeof(long)),
                Expression.LeftShift(
                    left,
                    Expression.Convert(
                        right,
                        typeof(int))));

        /// <summary>
        /// Generates a binary byte shift expression.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <returns>An expression containing the operation.</returns>
        private protected override Expression GenerateBinaryExpression(
            Expression left,
            Expression right)
        {
            var leftShiftMethodInfo = typeof(BitwiseExtensions).GetMethod(
                                          nameof(BitwiseExtensions.LeftShift),
                                          new[]
                                          {
                                              typeof(byte[]),
                                              typeof(int)
                                          }) ??
                                      throw new InvalidOperationException();
            var convertMethodInfo = typeof(Convert).GetMethod(
                                        nameof(Convert.ToInt32),
                                        new[]
                                        {
                                            typeof(long)
                                        }) ??
                                    throw new InvalidOperationException();

            return Expression.Call(
                leftShiftMethodInfo,
                left,
                Expression.Call(
                    convertMethodInfo,
                    right));
        }
    }
}