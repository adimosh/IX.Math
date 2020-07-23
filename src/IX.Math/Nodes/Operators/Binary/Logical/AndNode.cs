// <copyright file="AndNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Exceptions;
using IX.Math.Extensibility;

namespace IX.Math.Nodes.Operators.Binary.Logical
{
    /// <summary>
    ///     A node representing a logical and operation.
    /// </summary>
    /// <seealso cref="LogicalOperatorNodeBase" />
    [DebuggerDisplay("{" + nameof(Left) + "} & {" + nameof(Right) + "}")]
    internal sealed class AndNode : LogicalOperatorNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AndNode" /> class.
        /// </summary>
        /// <param name="stringFormatters">The string formatters.</param>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public AndNode(
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
        /// Performs the binary operation.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result of the operation.</returns>
        public static byte[] PerformBinaryOperation(
            byte[] left,
            byte[] right)
        {
            byte[] result = new byte[global::System.Math.Max(
                left.Length,
                right.Length)];
            new BitArray(left).And(new BitArray(right)).CopyTo(result, 0);
            return result;
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new AndNode(
                this.StringFormatters,
                this.Left.DeepClone(context),
                this.Right.DeepClone(context));

        /// <summary>
        /// Generates simplified data.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The resulting simplified data.</returns>
        protected override long GenerateData(
            long left,
            long right) =>
            left & right;

        /// <summary>
        /// Generates simplified data.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The resulting simplified data.</returns>
        protected override bool GenerateData(
            bool left,
            bool right) =>
            left & right;

        /// <summary>
        /// Generates simplified data.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The resulting simplified data.</returns>
        protected override byte[] GenerateData(
            byte[] left,
            byte[] right) =>
            PerformBinaryOperation(
                left,
                right);

        /// <summary>
        /// Generates the expression that this node represents.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="comparisonTolerance">The comparison tolerance.</param>
        /// <returns>A compiled expression, if one is possible.</returns>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance",
            "HAA0603:Delegate allocation from a method group",
            Justification = "This is desirable.")]
        protected override Expression GenerateExpressionInternal(
            in SupportedValueType valueType,
            in ComparisonTolerance comparisonTolerance)
        {
            try
            {
                var (
                    left,
                    right,
                    isBinary) = this.GenerateParameterExpressions(in valueType, in comparisonTolerance);

                if (!isBinary)
                {
                    return Expression.And(
                        left,
                        right);
                }

                Func<byte[], byte[], byte[]> del = PerformBinaryOperation;
                return Expression.Call(
                    del.Method,
                    left,
                    right);
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