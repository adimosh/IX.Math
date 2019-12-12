// <copyright file="LessThanOrEqualNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Nodes.Constants;
using IX.StandardExtensions.Extensions;

namespace IX.Math.Nodes.Operations.Binary
{
    /// <summary>
    ///     A node representing a less than or equal to expression.
    /// </summary>
    /// <seealso cref="IX.Math.Nodes.Operations.Binary.ComparisonOperationNodeBase" />
    [DebuggerDisplay("{" + nameof(Left) + "} <= {" + nameof(Right) + "}")]
    internal sealed class LessThanOrEqualNode : ComparisonOperationNodeBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LessThanOrEqualNode" /> class.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public LessThanOrEqualNode(
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
                NumericNode nnLeft when this.Right is NumericNode nnRight => new BoolNode(
                    Convert.ToDouble(nnLeft.Value) <= Convert.ToDouble(nnRight.Value)),
                StringNode snLeft when this.Right is StringNode snRight => new BoolNode(
                    snLeft.Value.CompareTo(snRight.Value) <= 0),
                BoolNode bnLeft when this.Right is BoolNode bnRight => new BoolNode(!bnLeft.Value || bnRight.Value),
                ByteArrayNode baLeft when this.Right is ByteArrayNode baRight => new BoolNode(
                    baLeft.Value.SequenceCompareWithMsb(baRight.Value) <= 0),
                _ => this
            };

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new LessThanOrEqualNode(
                this.Left.DeepClone(context),
                this.Right.DeepClone(context));

        /// <summary>
        ///     Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>
        ///     The expression.
        /// </returns>
        [SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "We want this to happen.")]
        protected override Expression GenerateExpressionInternal()
        {
            (Expression leftExpression, Expression rightExpression) = this.GetExpressionsOfSameTypeFromOperands();
            if (leftExpression.Type == typeof(string))
            {
                MethodInfo mi = typeof(string).GetMethodWithExactParameters(
                    nameof(string.Compare),
                    typeof(string),
                    typeof(string));
                return Expression.LessThanOrEqual(
                    Expression.Call(
                        mi,
                        this.Left.GenerateStringExpression(),
                        this.Right.GenerateStringExpression()),
                    Expression.Constant(
                        0,
                        typeof(int)));
            }

            if (this.Left.ReturnType == SupportedValueType.Boolean ||
                this.Right.ReturnType == SupportedValueType.Boolean)
            {
                return Expression.Condition(
                    Expression.Equal(
                        leftExpression,
                        Expression.Constant(
                            true,
                            typeof(bool))),
                    rightExpression,
                    Expression.Constant(
                        true,
                        typeof(bool)));
            }

            if (this.Left.ReturnType == SupportedValueType.ByteArray ||
                this.Right.ReturnType == SupportedValueType.ByteArray)
            {
                return Expression.LessThanOrEqual(
                    Expression.Call(
                        typeof(ArrayExtensions).GetMethodWithExactParameters(
                            nameof(ArrayExtensions.SequenceCompareWithMsb),
                            typeof(byte[]),
                            typeof(byte[])),
                        leftExpression,
                        rightExpression),
                    Expression.Constant(
                        0,
                        typeof(int)));
            }

            return Expression.LessThanOrEqual(
                leftExpression,
                rightExpression);
        }

        /// <summary>
        ///     Generates the expression with tolerance that will be compiled into code.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The expression.</returns>
        [SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "We want this to happen.")]
        protected override Expression GenerateExpressionInternal(Tolerance tolerance)
        {
            (Expression leftExpression, Expression rightExpression) =
                this.GetExpressionsOfSameTypeFromOperands(tolerance);
            if (leftExpression.Type == typeof(string))
            {
                MethodInfo mi = typeof(string).GetMethodWithExactParameters(
                    nameof(string.Compare),
                    typeof(string),
                    typeof(string));
                return Expression.LessThanOrEqual(
                    Expression.Call(
                        mi,
                        this.Left.GenerateStringExpression(),
                        this.Right.GenerateStringExpression()),
                    Expression.Constant(
                        0,
                        typeof(int)));
            }

            if (this.Left.ReturnType == SupportedValueType.Boolean ||
                this.Right.ReturnType == SupportedValueType.Boolean)
            {
                return Expression.Condition(
                    Expression.Equal(
                        leftExpression,
                        Expression.Constant(
                            true,
                            typeof(bool))),
                    rightExpression,
                    Expression.Constant(
                        true,
                        typeof(bool)));
            }

            if (this.Left.ReturnType == SupportedValueType.ByteArray ||
                this.Right.ReturnType == SupportedValueType.ByteArray)
            {
                return Expression.LessThanOrEqual(
                    Expression.Call(
                        typeof(ArrayExtensions).GetMethodWithExactParameters(
                            nameof(ArrayExtensions.SequenceCompareWithMsb),
                            typeof(byte[]),
                            typeof(byte[])),
                        leftExpression,
                        rightExpression),
                    Expression.Constant(
                        0,
                        typeof(int)));
            }

            return Expression.LessThanOrEqual(
                leftExpression,
                rightExpression);
        }
    }
}