// <copyright file="AddNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Nodes.Constants;
using IX.StandardExtensions.Extensions;

namespace IX.Math.Nodes.Operations.Binary
{
    /// <summary>
    ///     A node representing an addition operation.
    /// </summary>
    /// <seealso cref="IX.Math.Nodes.Operations.Binary.BinaryOperationNodeBase" />
    [DebuggerDisplay("{" + nameof(Left) + "} + {" + nameof(Right) + "}")]
    internal sealed class AddNode : BinaryOperationNodeBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="AddNode" /> class.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public AddNode(
            NodeBase left,
            NodeBase right)
            : base(
                left?.Simplify(),
                right?.Simplify())
        {
        }

        /// <summary>
        ///     Gets the return type of this node.
        /// </summary>
        /// <value>
        ///     The node return type.
        /// </value>
        public override SupportedValueType ReturnType
        {
            get
            {
                if (this.Left.ReturnType == SupportedValueType.String ||
                    this.Right.ReturnType == SupportedValueType.String)
                {
                    return SupportedValueType.String;
                }

                switch (this.Left.ReturnType)
                {
                    case SupportedValueType.ByteArray:
                        return this.Right.ReturnType == SupportedValueType.ByteArray
                            ? SupportedValueType.ByteArray
                            : SupportedValueType.Unknown;
                    case SupportedValueType.Numeric:
                        return this.Right.ReturnType == SupportedValueType.Numeric
                            ? SupportedValueType.Numeric
                            : SupportedValueType.Unknown;
                    default:
                        return SupportedValueType.Unknown;
                }
            }
        }

        /// <summary>
        ///     Determines the children in the correct types.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <param name="other">The other.</param>
        private static void DetermineChildren(
            NodeBase parameter,
            NodeBase other)
        {
            switch (other.ReturnType)
            {
                case SupportedValueType.Boolean:
                    parameter.DetermineStrongly(SupportedValueType.String);
                    break;
                case SupportedValueType.Numeric:
                    parameter.DetermineWeakly(SupportableValueType.Numeric | SupportableValueType.String);
                    break;
                case SupportedValueType.ByteArray:
                    parameter.DetermineWeakly(SupportableValueType.ByteArray | SupportableValueType.String);
                    break;
            }
        }

        /// <summary>
        ///     Stitches the specified byte arrays.
        /// </summary>
        /// <param name="left">The left array operand.</param>
        /// <param name="right">The right array operand.</param>
        /// <returns>A stitched array of bytes.</returns>
        private static byte[] Stitch(
            byte[] left,
            byte[] right)
        {
            var r = new byte[left.Length + right.Length];
            Array.Copy(
                left,
                r,
                left.Length);
            Array.Copy(
                right,
                0,
                r,
                left.Length,
                right.Length);
            return r;
        }

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>
        ///     A simplified node, or this instance.
        /// </returns>
        public override NodeBase Simplify()
        {
            if (this.Left.IsConstant != this.Right.IsConstant)
            {
                return this;
            }

            if (!this.Left.IsConstant)
            {
                return this;
            }

            return this.Left switch
            {
                NumericNode nn1Left when this.Right is NumericNode nn1Right => NumericNode.Add(
                    nn1Left,
                    nn1Right),
                StringNode sn1Left when this.Right is StringNode sn1Right => new StringNode(
                    sn1Left.Value + sn1Right.Value),
                NumericNode nn2Left when this.Right is StringNode sn2Right => new StringNode(
                    $"{nn2Left.Value}{sn2Right.Value}"),
                StringNode sn3Left when this.Right is NumericNode nn3Right => new StringNode(
                    $"{sn3Left.Value}{nn3Right.Value}"),
                BoolNode bn4Left when this.Right is StringNode sn4Right => new StringNode(
                    $"{bn4Left.Value.ToString()}{sn4Right.Value}"),
                StringNode sn5Left when this.Right is BoolNode bn5Right => new StringNode(
                    $"{sn5Left.Value}{bn5Right.Value.ToString()}"),
                ByteArrayNode ban5Left when this.Right is ByteArrayNode ban5Right => new ByteArrayNode(
                    Stitch(
                        ban5Left.Value,
                        ban5Right.Value)),
                _ => this
            };
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new AddNode(
                this.Left.DeepClone(context),
                this.Right.DeepClone(context));

        /// <summary>
        ///     Strongly determines the node's type, if possible.
        /// </summary>
        /// <param name="type">The type to determine to.</param>
        public override void DetermineStrongly(SupportedValueType type)
        {
            switch (type)
            {
                case SupportedValueType.Boolean:
                    throw new ExpressionNotValidLogicallyException();
                case SupportedValueType.ByteArray:
                {
                    this.Left.DetermineStrongly(SupportedValueType.ByteArray);
                    this.Right.DetermineStrongly(SupportedValueType.ByteArray);
                }

                    break;

                case SupportedValueType.Numeric:
                {
                    this.Left.DetermineStrongly(SupportedValueType.Numeric);
                    this.Right.DetermineStrongly(SupportedValueType.Numeric);
                }

                    break;
            }

            this.EnsureCompatibleOperands(
                this.Left,
                this.Right);
        }

        /// <summary>
        ///     Weakly determines the node's type, if possible, and, optionally, strongly determines if there is only one possible
        ///     type left.
        /// </summary>
        /// <param name="type">The type or types to determine to.</param>
        public override void DetermineWeakly(SupportableValueType type)
        {
            if ((type & SupportableValueType.Boolean) != 0)
            {
                type ^= SupportableValueType.Boolean;
            }

            this.Left.DetermineWeakly(type);
            this.Right.DetermineWeakly(type);

            this.EnsureCompatibleOperands(
                this.Left,
                this.Right);
        }

        /// <summary>
        ///     Ensures that the operands are compatible.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        protected override void EnsureCompatibleOperands(
            NodeBase left,
            NodeBase right)
        {
            DetermineChildren(
                left,
                right);
            DetermineChildren(
                right,
                left);
            DetermineChildren(
                left,
                right);
            DetermineChildren(
                right,
                left);

            switch (left.ReturnType)
            {
                case SupportedValueType.Numeric:
                    if (right.ReturnType != SupportedValueType.Numeric &&
                        right.ReturnType != SupportedValueType.String &&
                        right.ReturnType != SupportedValueType.Unknown)
                    {
                        throw new ExpressionNotValidLogicallyException();
                    }

                    break;

                case SupportedValueType.Boolean:
                    if (right.ReturnType != SupportedValueType.String)
                    {
                        throw new ExpressionNotValidLogicallyException();
                    }

                    break;

                case SupportedValueType.ByteArray:
                    if (right.ReturnType != SupportedValueType.ByteArray &&
                        right.ReturnType != SupportedValueType.String &&
                        right.ReturnType != SupportedValueType.Unknown)
                    {
                        throw new ExpressionNotValidLogicallyException();
                    }

                    break;

                case SupportedValueType.String:
                case SupportedValueType.Unknown:
                    break;
            }

            switch (right.ReturnType)
            {
                case SupportedValueType.Numeric:
                    if (left.ReturnType != SupportedValueType.Numeric &&
                        left.ReturnType != SupportedValueType.String &&
                        left.ReturnType != SupportedValueType.Unknown)
                    {
                        throw new ExpressionNotValidLogicallyException();
                    }

                    break;

                case SupportedValueType.Boolean:
                    if (left.ReturnType != SupportedValueType.String)
                    {
                        throw new ExpressionNotValidLogicallyException();
                    }

                    break;

                case SupportedValueType.ByteArray:
                    if (left.ReturnType != SupportedValueType.ByteArray &&
                        left.ReturnType != SupportedValueType.String &&
                        left.ReturnType != SupportedValueType.Unknown)
                    {
                        throw new ExpressionNotValidLogicallyException();
                    }

                    break;

                case SupportedValueType.String:
                case SupportedValueType.Unknown:
                    break;
            }
        }

        /// <summary>
        ///     Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>
        ///     The expression.
        /// </returns>
        protected override Expression GenerateExpressionInternal()
        {
            (Expression leftExpression, Expression rightExpression) = this.GetExpressionsOfSameTypeFromOperands();

            switch (this.ReturnType)
            {
                case SupportedValueType.String:
                    MethodInfo mi1 = typeof(string).GetMethodWithExactParameters(
                        nameof(string.Concat),
                        typeof(string),
                        typeof(string));
                    return Expression.Call(
                        mi1,
                        leftExpression,
                        rightExpression);
                case SupportedValueType.ByteArray:
                    MethodInfo mi2 = typeof(AddNode).GetMethodWithExactParameters(
                        nameof(Stitch),
                        typeof(byte[]),
                        typeof(byte[]));
                    return Expression.Call(
                        mi2,
                        leftExpression,
                        rightExpression);
                default:
                    return Expression.Add(
                        leftExpression,
                        rightExpression);
            }
        }

        /// <summary>
        ///     Generates the expression with tolerance that will be compiled into code.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The expression.</returns>
        protected override Expression GenerateExpressionInternal(Tolerance tolerance)
        {
            (Expression leftExpression, Expression rightExpression) =
                this.GetExpressionsOfSameTypeFromOperands(tolerance);

            switch (this.ReturnType)
            {
                case SupportedValueType.String:
                    MethodInfo mi1 = typeof(string).GetMethodWithExactParameters(
                        nameof(string.Concat),
                        typeof(string),
                        typeof(string));
                    return Expression.Call(
                        mi1,
                        leftExpression,
                        rightExpression);
                case SupportedValueType.ByteArray:
                    MethodInfo mi2 = typeof(AddNode).GetMethodWithExactParameters(
                        nameof(Stitch),
                        typeof(byte[]),
                        typeof(byte[]));
                    return Expression.Call(
                        mi2,
                        leftExpression,
                        rightExpression);
                default:
                    return Expression.Add(
                        leftExpression,
                        rightExpression);
            }
        }
    }
}