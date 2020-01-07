// <copyright file="OrNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;

namespace IX.Math.Nodes.Operations.Binary
{
    /// <summary>
    ///     A node representing the or logical operation.
    /// </summary>
    /// <seealso cref="LogicalOperationNodeBase" />
    [DebuggerDisplay("{" + nameof(Left) + "} | {" + nameof(Right) + "}")]
    internal sealed class OrNode : LogicalOperationNodeBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="OrNode" /> class.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public OrNode(
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
                NumericNode nnLeft when this.Right is NumericNode nnRight => new NumericNode(
                    nnLeft.ExtractInteger() | nnRight.ExtractInteger()),
                BoolNode bnLeft when this.Right is BoolNode bnRight => new BoolNode(bnLeft.Value | bnRight.Value),
                _ => this
            };

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new OrNode(
                this.Left.DeepClone(context),
                this.Right.DeepClone(context));

        /// <summary>
        ///     Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>
        ///     The expression.
        /// </returns>
        protected override Expression GenerateExpressionInternal() =>
            Expression.Or(
                this.Left.GenerateExpression(),
                this.Right.GenerateExpression());

        /// <summary>
        ///     Generates the expression with tolerance that will be compiled into code.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The expression.</returns>
        protected override Expression GenerateExpressionInternal(Tolerance tolerance) =>
            Expression.Or(
                this.Left.GenerateExpression(tolerance),
                this.Right.GenerateExpression(tolerance));
    }
}