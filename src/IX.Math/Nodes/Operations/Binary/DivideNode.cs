// <copyright file="DivideNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;

namespace IX.Math.Nodes.Operations.Binary
{
    /// <summary>
    ///     A node representing a division operation.
    /// </summary>
    /// <seealso cref="SimpleMathematicalOperationNodeBase" />
    [DebuggerDisplay("{" + nameof(Left) + "} / {" + nameof(Right) + "}")]
    internal sealed class DivideNode : SimpleMathematicalOperationNodeBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="DivideNode" /> class.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public DivideNode(
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
                return NumericNode.Divide(
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
            new DivideNode(
                this.Left.DeepClone(context),
                this.Right.DeepClone(context));

        /// <summary>
        ///     Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>
        ///     The expression.
        /// </returns>
        protected override Expression GenerateExpressionInternal() =>
            Expression.Divide(
                Expression.Convert(
                    this.Left.GenerateExpression(),
                    typeof(double)),
                Expression.Convert(
                    this.Right.GenerateExpression(),
                    typeof(double)));

        /// <summary>
        ///     Generates the expression with tolerance that will be compiled into code.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The expression.</returns>
        protected override Expression GenerateExpressionInternal(Tolerance tolerance) =>
            Expression.Divide(
                Expression.Convert(
                    this.Left.GenerateExpression(tolerance),
                    typeof(double)),
                Expression.Convert(
                    this.Right.GenerateExpression(tolerance),
                    typeof(double)));
    }
}