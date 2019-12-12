// <copyright file="LeftShiftNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.StandardExtensions.Extensions;

namespace IX.Math.Nodes.Operations.Binary
{
    /// <summary>
    ///     A node representing a bitwise left shift operation.
    /// </summary>
    /// <seealso cref="IX.Math.Nodes.Operations.Binary.ByteShiftOperationNodeBase" />
    [DebuggerDisplay("{" + nameof(Left) + "} << {" + nameof(Right) + "}")]
    internal sealed class LeftShiftNode : ByteShiftOperationNodeBase
    {
        public LeftShiftNode(
            NodeBase left,
            NodeBase right)
            : base(
                left?.Simplify(),
                right?.Simplify())
        {
        }

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>
        ///     A simplified node, or this instance.
        /// </returns>
        public override NodeBase Simplify() =>
            this.Left switch
            {
                NumericNode nLeft when this.Right is NumericNode nRight => NumericNode.LeftShift(
                    nLeft,
                    nRight),
                ByteArrayNode baLeft when this.Right is NumericNode baRight => new ByteArrayNode(
                    baLeft.Value.LeftShift(baRight.ExtractInt())),
                _ => this
            };

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new LeftShiftNode(
                this.Left.DeepClone(context),
                this.Right.DeepClone(context));

        /// <summary>
        ///     Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>
        ///     The expression.
        /// </returns>
        protected override Expression GenerateExpressionInternal()
        {
            Expression rightExpression = Expression.Convert(
                this.Right.GenerateExpression(),
                typeof(int));
            return this.Left.ReturnType switch
            {
                SupportedValueType.Numeric => Expression.LeftShift(
                    this.Left.GenerateExpression(),
                    rightExpression),
                SupportedValueType.ByteArray => Expression.Call(
                    typeof(BitwiseExtensions).GetMethodWithExactParameters(
                        nameof(BitwiseExtensions.LeftShift),
                        typeof(byte[]),
                        typeof(int)),
                    this.Left.GenerateExpression(),
                    rightExpression),
                _ => throw new ExpressionNotValidLogicallyException()
            };
        }

        /// <summary>
        ///     Generates the expression with tolerance that will be compiled into code.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The expression.</returns>
        protected override Expression GenerateExpressionInternal(Tolerance tolerance)
        {
            Expression rightExpression = Expression.Convert(
                this.Right.GenerateExpression(tolerance),
                typeof(int));
            return this.Left.ReturnType switch
            {
                SupportedValueType.Numeric => Expression.LeftShift(
                    this.Left.GenerateExpression(tolerance),
                    rightExpression),
                SupportedValueType.ByteArray => Expression.Call(
                    typeof(BitwiseExtensions).GetMethodWithExactParameters(
                        nameof(BitwiseExtensions.LeftShift),
                        typeof(byte[]),
                        typeof(int)),
                    this.Left.GenerateExpression(tolerance),
                    rightExpression),
                _ => throw new ExpressionNotValidLogicallyException()
            };
        }
    }
}