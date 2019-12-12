// <copyright file="MultiplyNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;

namespace IX.Math.Nodes.Operations.Binary
{
    /// <summary>
    ///     A node representing a multiplication operation.
    /// </summary>
    /// <seealso cref="IX.Math.Nodes.Operations.Binary.SimpleMathematicalOperationNodeBase" />
    [DebuggerDisplay("{" + nameof(Left) + "} * {" + nameof(Right) + "}")]
    internal sealed class MultiplyNode : SimpleMathematicalOperationNodeBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="MultiplyNode" /> class.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public MultiplyNode(
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
        public override NodeBase Simplify()
        {
            if (this.Left is NumericNode nnLeft && this.Right is NumericNode nnRight)
            {
                return NumericNode.Multiply(
                    nnLeft,
                    nnRight);
            }

            return this;
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new MultiplyNode(
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
            var left = this.Left.GenerateExpression();
            var right = this.Right.GenerateExpression();

            this.EnsureCompatibleNumericExpressions(
                ref left,
                ref right);

            return Expression.Multiply(
                left,
                right);
        }

        /// <summary>
        ///     Generates the expression with tolerance that will be compiled into code.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The expression.</returns>
        protected override Expression GenerateExpressionInternal(Tolerance tolerance)
        {
            var left = this.Left.GenerateExpression(tolerance);
            var right = this.Right.GenerateExpression(tolerance);

            this.EnsureCompatibleNumericExpressions(
                ref left,
                ref right);

            return Expression.Multiply(
                left,
                right);
        }
    }
}