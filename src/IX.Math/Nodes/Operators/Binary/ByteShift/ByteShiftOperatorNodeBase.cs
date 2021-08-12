// <copyright file="ByteShiftOperatorNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace IX.Math.Nodes.Operators.Binary.ByteShift
{
    /// <summary>
    /// A base class for a byte shift binary node.
    /// </summary>
    internal abstract class ByteShiftOperatorNodeBase : BinaryOperatorNodeBase
    {
        private const SupportableValueType SupportableValueTypes =
            SupportableValueType.Integer | SupportableValueType.ByteArray;

        /// <summary>
        /// Initializes a new instance of the <see cref="ByteShiftOperatorNodeBase"/> class.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        protected private ByteShiftOperatorNodeBase(
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
        public sealed override SupportableValueType CalculateSupportableValueType(
            SupportableValueType constraints = SupportableValueType.All)
        {
            if ((constraints & SupportableValueTypes) == SupportableValueType.None)
            {
                return SupportableValueType.None;
            }

            return this.RightOperand.CalculateSupportableValueType(SupportableValueType.Integer) ==
                   SupportableValueType.None
                ? SupportableValueType.None
                : this.LeftOperand.CalculateSupportableValueType(SupportableValueTypes);
        }

        /// <summary>
        ///     Generates the expression that will be compiled into code.
        /// </summary>
        /// <param name="forType">What type the expression is generated for.</param>
        /// <param name="tolerance">The tolerance. If this parameter is <c>null</c> (<c>Nothing</c> in Visual Basic), then the operation is exact.</param>
        /// <returns>
        ///     The generated <see cref="Expression" />.
        /// </returns>
        public sealed override Expression GenerateExpression(
            SupportedValueType forType,
            Tolerance? tolerance = null)
        {
            var rightExpression = this.RightOperand.GenerateExpression(
                SupportedValueType.Integer,
                tolerance);

            var leftType = this.LeftOperand.CalculateSupportableValueType(SupportableValueTypes);

            Expression leftExpression;

            switch (forType)
            {
                case SupportedValueType.Integer:
                    if ((leftType & SupportableValueType.Integer) == SupportableValueType.None)
                    {
                        throw new ExpressionNotValidLogicallyException();
                    }

                    leftExpression = this.LeftOperand.GenerateExpression(
                        SupportedValueType.Integer,
                        tolerance);

                    return this.GenerateIntegerExpression(
                        leftExpression,
                        rightExpression);

                case SupportedValueType.ByteArray:
                    if (leftType == SupportableValueType.Integer)
                    {
                        MethodInfo mi = typeof(BitConverter).GetMethod(
                            nameof(BitConverter.GetBytes),
                            new[]
                            {
                                typeof(long)
                            }) ?? throw new PlatformNotSupportedException();

                        leftExpression = Expression.Call(
                            mi,
                            this.LeftOperand.GenerateExpression(
                                SupportedValueType.Integer,
                                tolerance));
                    }
                    else
                    {
                        leftExpression = this.LeftOperand.GenerateExpression(
                            SupportedValueType.ByteArray,
                            tolerance);
                    }

                    return this.GenerateBinaryExpression(
                        leftExpression,
                        rightExpression);

                default:
                    throw new ExpressionNotValidLogicallyException();
            }
        }

        /// <summary>
        /// Generates an integer byte shift expression.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <returns>An expression containing the operation.</returns>
        private protected abstract Expression GenerateIntegerExpression(
            Expression left,
            Expression right);

        /// <summary>
        /// Generates a binary byte shift expression.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <returns>An expression containing the operation.</returns>
        private protected abstract Expression GenerateBinaryExpression(
            Expression left,
            Expression right);
    }
}