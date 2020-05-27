// <copyright file="RightShiftNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Exceptions;
using IX.Math.Nodes.Constants;
using IX.StandardExtensions.Extensions;

namespace IX.Math.Nodes.Operations.Binary.Bitwise
{
    /// <summary>
    ///     A node for a bitwise right-shift operation.
    /// </summary>
    /// <seealso cref="ByteShiftOperationNodeBase" />
    [DebuggerDisplay("{" + nameof(Left) + "} >> {" + nameof(Right) + "}")]
    internal sealed class RightShiftNode : ByteShiftOperationNodeBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="RightShiftNode" /> class.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public RightShiftNode(
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
                NumericNode nLeft when this.Right is NumericNode nRight => NumericNode.RightShift(
                    nLeft,
                    nRight),
                ByteArrayNode baLeft when this.Right is NumericNode baRight => new ByteArrayNode(
                    baLeft.Value.RightShift(baRight.ExtractInt())),
                _ => this
            };

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new RightShiftNode(
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
                SupportedValueType.Numeric => Expression.RightShift(
                    this.Left.GenerateExpression(),
                    rightExpression),
                SupportedValueType.ByteArray => Expression.Call(
                    typeof(BitwiseExtensions).GetMethodWithExactParameters(
                        nameof(BitwiseExtensions.RightShift),
                        typeof(byte[]),
                        typeof(int)) ?? throw new MathematicsEngineException(),
                    this.Left.GenerateExpression(),
                    rightExpression),
                _ => throw new Exceptions.ExpressionNotValidLogicallyException()
            };
        }

        /// <summary>
        ///     Generates the expression with tolerance that will be compiled into code.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The expression.</returns>
        protected override Expression GenerateExpressionInternal(in ComparisonTolerance tolerance)
        {
            Expression rightExpression = Expression.Convert(
                this.Right.GenerateExpression(in tolerance),
                typeof(int));
            return this.Left.ReturnType switch
            {
                SupportedValueType.Numeric => Expression.RightShift(
                    this.Left.GenerateExpression(in tolerance),
                    rightExpression),
                SupportedValueType.ByteArray => Expression.Call(
                    typeof(BitwiseExtensions).GetMethodWithExactParameters(
                        nameof(BitwiseExtensions.RightShift),
                        typeof(byte[]),
                        typeof(int)) ?? throw new MathematicsEngineException(),
                    this.Left.GenerateExpression(in tolerance),
                    rightExpression),
                _ => throw new Exceptions.ExpressionNotValidLogicallyException()
            };
        }
    }
}