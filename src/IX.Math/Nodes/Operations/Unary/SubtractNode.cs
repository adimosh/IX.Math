// <copyright file="SubtractNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;

namespace IX.Math.Nodes.Operations.Unary
{
    /// <summary>
    ///     A node for negation operations.
    /// </summary>
    /// <seealso cref="UnaryOperatorNodeBase" />
    [DebuggerDisplay("-{" + nameof(Operand) + "}")]
    internal sealed class SubtractNode : UnaryOperatorNodeBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SubtractNode" /> class.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <exception cref="ExpressionNotValidLogicallyException">The expression is not logically valid.</exception>
        public SubtractNode([JetBrains.Annotations.NotNull] NodeBase operand)
            : base(operand.Simplify())
        {
            operand.DetermineStrongly(SupportedValueType.Numeric);

            if (operand.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        /// <summary>
        ///     Gets the return type of this node.
        /// </summary>
        /// <value>
        ///     The node return type.
        /// </value>
        public override SupportedValueType ReturnType => SupportedValueType.Numeric;

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>
        ///     A simplified node, or this instance.
        /// </returns>
        public override NodeBase Simplify() =>
            this.Operand switch
            {
                NumericNode numericNode => NumericNode.Subtract(
                    new NumericNode(0),
                    numericNode),
                _ => this
            };

        /// <summary>
        ///     Strongly determines the node's type, if possible.
        /// </summary>
        /// <param name="type">The type to determine to.</param>
        public override void DetermineStrongly(SupportedValueType type)
        {
            if (type != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        /// <summary>
        ///     Weakly determines the node's type, if possible, and, optionally, strongly determines if there is only one possible
        ///     type left.
        /// </summary>
        /// <param name="type">The type or types to determine to.</param>
        public override void DetermineWeakly(SupportableValueType type)
        {
            if ((type & SupportableValueType.Numeric) == 0)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new SubtractNode(this.Operand.DeepClone(context));

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
        protected override Expression GenerateExpressionInternal() =>
            Expression.Subtract(
                Expression.Constant(
                    0L,
                    typeof(long)),
                this.Operand.GenerateExpression());

        /// <summary>
        ///     Generates the expression with tolerance that will be compiled into code.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The expression.</returns>
        [SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "We want this to happen.")]
        protected override Expression GenerateExpressionInternal(Tolerance tolerance) =>
            Expression.Subtract(
                Expression.Constant(
                    0L,
                    typeof(long)),
                this.Operand.GenerateExpression(tolerance));
    }
}