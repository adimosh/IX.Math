// <copyright file="ComparisonOperatorNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Formatters;
using IX.Math.Values;

namespace IX.Math.Nodes.Operators.Binary.Comparison
{
    /// <summary>
    /// A base class for a comparison node.
    /// </summary>
    internal abstract class ComparisonOperatorNodeBase : BinaryOperatorNodeBase
    {
        private const SupportableValueType SupportableValueTypes =
            SupportableValueType.ByteArray | SupportableValueType.Integer | SupportableValueType.Numeric | SupportableValueType.String;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComparisonOperatorNodeBase"/> class.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        protected private ComparisonOperatorNodeBase(
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

            SupportableValueType svt = SupportableValueType.None;

            if ((leftType & SupportableValueType.String) != SupportableValueType.None ||
                (rightType & SupportableValueType.String) != SupportableValueType.None)
            {
                // Everything is comparable to string
                svt = SupportableValueType.String;
            }

            if ((leftType &
                 (SupportableValueType.Integer | SupportableValueType.Numeric | SupportableValueType.ByteArray)) !=
                SupportableValueType.None &&
                (rightType & SupportableValueType.ByteArray) != SupportableValueType.None)
            {
                // Numeric, integer and byte arrays are comparable to binary if right operand is binary
                svt |= SupportableValueType.ByteArray;
            }

            if ((rightType &
                 (SupportableValueType.Integer | SupportableValueType.Numeric | SupportableValueType.ByteArray)) !=
                SupportableValueType.None &&
                (leftType & SupportableValueType.ByteArray) != SupportableValueType.None)
            {
                // Numeric, integer and byte arrays are comparable to binary if left operand is binary
                svt |= SupportableValueType.ByteArray;
            }

            if ((leftType & (SupportableValueType.Numeric | SupportableValueType.Integer)) !=
                SupportableValueType.None &&
                (rightType & SupportableValueType.Numeric) != SupportableValueType.None)
            {
                // Numeric and integer are comparable to numeric if right operand is numeric
                svt |= SupportableValueType.Numeric;
            }

            if ((rightType & (SupportableValueType.Numeric | SupportableValueType.Integer)) !=
                SupportableValueType.None &&
                (leftType & SupportableValueType.Numeric) != SupportableValueType.None)
            {
                // Numeric and integer are comparable to numeric if left operand is numeric
                svt |= SupportableValueType.Numeric;
            }

            if ((rightType & leftType & SupportableValueType.Integer) != SupportableValueType.None)
            {
                // Integer comparison is supported if both operands are integer
                svt |= SupportableValueType.Integer;
            }

            return svt;
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
            if (((SupportableValueType)forType & SupportableValueTypes) == SupportableValueType.None)
            {
                // We don't have a supportable return type
                throw new ExpressionNotValidLogicallyException();
            }

            var leftType = this.LeftOperand.CalculateSupportableValueType(SupportableValueTypes);
            var rightType = this.RightOperand.CalculateSupportableValueType(SupportableValueTypes);

            switch (forType)
            {
                case SupportedValueType.ByteArray:
                {
                    // Calculate left expression
                    Expression left;
                    if ((leftType & SupportableValueType.ByteArray) != SupportableValueType.None)
                    {
                        // Binary operand
                        left = this.LeftOperand.GenerateExpression(
                            SupportedValueType.ByteArray,
                            tolerance);
                    }
                    else if ((leftType & SupportableValueType.Numeric) != SupportableValueType.None)
                    {
                        // Numeric operand
                        MethodInfo mi = typeof(BitConverter).GetMethod(
                                            nameof(BitConverter.GetBytes),
                                            new[]
                                            {
                                                typeof(double)
                                            }) ??
                                        throw new PlatformNotSupportedException();
                        left = Expression.Call(
                            mi,
                            this.LeftOperand.GenerateExpression(
                                SupportedValueType.Numeric,
                                tolerance));
                    }
                    else if ((leftType & SupportableValueType.Integer) != SupportableValueType.None)
                    {
                        // Integer operand
                        MethodInfo mi = typeof(BitConverter).GetMethod(
                                            nameof(BitConverter.GetBytes),
                                            new[]
                                            {
                                                typeof(long)
                                            }) ??
                                        throw new PlatformNotSupportedException();
                        left = Expression.Call(
                            mi,
                            this.LeftOperand.GenerateExpression(
                                SupportedValueType.Integer,
                                tolerance));
                    }
                    else
                    {
                        // Unsupported operand
                        throw new ExpressionNotValidLogicallyException();
                    }

                    // Calculate right expression
                    Expression right;
                    if ((rightType & SupportableValueType.ByteArray) != SupportableValueType.None)
                    {
                        // Binary operand
                        right = this.RightOperand.GenerateExpression(
                            SupportedValueType.ByteArray,
                            tolerance);
                    }
                    else if ((rightType & SupportableValueType.Numeric) != SupportableValueType.None)
                    {
                        // Numeric operand
                        MethodInfo mi = typeof(BitConverter).GetMethod(
                                            nameof(BitConverter.GetBytes),
                                            new[]
                                            {
                                                typeof(double)
                                            }) ??
                                        throw new PlatformNotSupportedException();
                        right = Expression.Call(
                            mi,
                            this.RightOperand.GenerateExpression(
                                SupportedValueType.Numeric,
                                tolerance));
                    }
                    else if ((rightType & SupportableValueType.Integer) != SupportableValueType.None)
                    {
                        // Binary operand
                        MethodInfo mi = typeof(BitConverter).GetMethod(
                                            nameof(BitConverter.GetBytes),
                                            new[]
                                            {
                                                typeof(long)
                                            }) ??
                                        throw new PlatformNotSupportedException();
                        right = Expression.Call(
                            mi,
                            this.RightOperand.GenerateExpression(
                                SupportedValueType.Integer,
                                tolerance));
                    }
                    else
                    {
                        throw new ExpressionNotValidLogicallyException();
                    }

                    return this.GenerateBinaryExpression(
                        left,
                        right);
                }

                case SupportedValueType.Numeric:
                {
                    // Calculate left expression
                    Expression left;
                    if ((leftType & SupportableValueType.Numeric) != SupportableValueType.None)
                    {
                        // Numeric operand
                        left = this.LeftOperand.GenerateExpression(
                            SupportedValueType.Numeric,
                            tolerance);
                    }
                    else if ((leftType & SupportableValueType.Integer) != SupportableValueType.None)
                    {
                        // Integer operand
                        left = Expression.Convert(
                            this.LeftOperand.GenerateExpression(
                                SupportedValueType.Integer,
                                tolerance),
                            typeof(double));
                    }
                    else
                    {
                        // Unsupported operand
                        throw new ExpressionNotValidLogicallyException();
                    }

                    // Calculate right expression
                    Expression right;
                    if ((rightType & SupportableValueType.Numeric) != SupportableValueType.None)
                    {
                        // Numeric operand
                        right = this.RightOperand.GenerateExpression(
                            SupportedValueType.Numeric,
                            tolerance);
                    }
                    else if ((rightType & SupportableValueType.Integer) != SupportableValueType.None)
                    {
                        // Binary operand
                        right = Expression.Convert(
                            this.RightOperand.GenerateExpression(
                                SupportedValueType.Integer,
                                tolerance),
                            typeof(double));
                    }
                    else
                    {
                        throw new ExpressionNotValidLogicallyException();
                    }

                    return this.GenerateNumericExpression(
                        left,
                        right);
                }

                case SupportedValueType.Integer:
                    return this.GenerateIntegerExpression(
                        this.LeftOperand.GenerateExpression(
                            SupportedValueType.Integer,
                            tolerance),
                        this.RightOperand.GenerateExpression(
                            SupportedValueType.Integer,
                            tolerance));

                case SupportedValueType.String:
                {
                    // Calculate left expression
                    Expression left;
                    if ((leftType & SupportableValueType.String) != SupportableValueType.None)
                    {
                        // String operand
                        left = StringFormatter.CreateStringConversionExpression(
                            this.LeftOperand.GenerateExpression(
                                SupportedValueType.String,
                                tolerance));
                    }
                    else if ((leftType & SupportableValueType.ByteArray) != SupportableValueType.None)
                    {
                        // Binary operand
                        left = StringFormatter.CreateStringConversionExpression(
                            this.LeftOperand.GenerateExpression(
                                SupportedValueType.ByteArray,
                                tolerance));
                    }
                    else if ((leftType & SupportableValueType.Numeric) != SupportableValueType.None)
                    {
                        // Numeric operand
                        left = StringFormatter.CreateStringConversionExpression(
                            this.LeftOperand.GenerateExpression(
                                SupportedValueType.Numeric,
                                tolerance));
                    }
                    else if ((leftType & SupportableValueType.Integer) != SupportableValueType.None)
                    {
                        // Integer operand
                        left = StringFormatter.CreateStringConversionExpression(
                            this.LeftOperand.GenerateExpression(
                                SupportedValueType.Integer,
                                tolerance));
                    }
                    else if ((leftType & SupportableValueType.Boolean) != SupportableValueType.None)
                    {
                        // Boolean operand
                        left = StringFormatter.CreateStringConversionExpression(
                            this.LeftOperand.GenerateExpression(
                                SupportedValueType.Boolean,
                                tolerance));
                    }
                    else
                    {
                        // Unsupported operand
                        throw new ExpressionNotValidLogicallyException();
                    }

                    // Calculate right expression
                    Expression right;
                    if ((rightType & SupportableValueType.String) != SupportableValueType.None)
                    {
                        // String operand
                        right = StringFormatter.CreateStringConversionExpression(
                            this.RightOperand.GenerateExpression(
                                SupportedValueType.String,
                                tolerance));
                    }
                    else if ((rightType & SupportableValueType.ByteArray) != SupportableValueType.None)
                    {
                        // Binary operand
                        right = StringFormatter.CreateStringConversionExpression(
                            this.RightOperand.GenerateExpression(
                                SupportedValueType.ByteArray,
                                tolerance));
                    }
                    else if ((rightType & SupportableValueType.Numeric) != SupportableValueType.None)
                    {
                        // Numeric operand
                        right = StringFormatter.CreateStringConversionExpression(
                            this.RightOperand.GenerateExpression(
                                SupportedValueType.Numeric,
                                tolerance));
                    }
                    else if ((rightType & SupportableValueType.Integer) != SupportableValueType.None)
                    {
                        // Binary operand
                        right = StringFormatter.CreateStringConversionExpression(
                            this.RightOperand.GenerateExpression(
                                SupportedValueType.Integer,
                                tolerance));
                    }
                    else if ((rightType & SupportableValueType.Boolean) != SupportableValueType.None)
                    {
                        // Binary operand
                        right = StringFormatter.CreateStringConversionExpression(
                            this.RightOperand.GenerateExpression(
                                SupportedValueType.Boolean,
                                tolerance));
                    }
                    else
                    {
                        throw new ExpressionNotValidLogicallyException();
                    }

                    return this.GenerateStringExpression(
                        left,
                        right);
                }

                default:
                    throw new ExpressionNotValidLogicallyException();
            }
        }

        /// <summary>
        ///     Simplifies this node, if possible, based on a constant operand value.
        /// </summary>
        /// <returns>A simplified node, or this instance.</returns>
        private protected sealed override NodeBase SimplifyOnConvertibleValue(
            ConvertibleValue leftValue,
            ConvertibleValue rightValue) =>
            this; // All comparison operators are tolerant

        /// <summary>
        /// Generates an integer mathematical expression.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <param name="tolerance">The tolerance for this operation.</param>
        /// <returns>An expression containing the operation.</returns>
        private protected abstract Expression GenerateIntegerExpression(
            Expression left,
            Expression right,
            Tolerance? tolerance = null);

        /// <summary>
        /// Generates a numeric mathematical expression.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <param name="tolerance">The tolerance for this operation.</param>
        /// <returns>An expression containing the operation.</returns>
        private protected abstract Expression GenerateNumericExpression(
            Expression left,
            Expression right,
            Tolerance? tolerance = null);

        /// <summary>
        /// Generates a binary mathematical expression.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <returns>An expression containing the operation.</returns>
        private protected abstract Expression GenerateBinaryExpression(
            Expression left,
            Expression right);

        /// <summary>
        /// Generates a string mathematical expression.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <returns>An expression containing the operation.</returns>
        private protected abstract Expression GenerateStringExpression(
            Expression left,
            Expression right);

    }
}