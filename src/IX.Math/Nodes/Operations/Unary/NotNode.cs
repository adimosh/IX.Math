// <copyright file="NotNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Operations.Unary
{
    /// <summary>
    ///     A node for a binary negation operation.
    /// </summary>
    /// <seealso cref="IX.Math.Nodes.Operations.Unary.UnaryOperatorNodeBase" />
    [DebuggerDisplay("!{" + nameof(Operand) + "}")]
    internal sealed class NotNode : UnaryOperatorNodeBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NotNode" /> class.
        /// </summary>
        /// <param name="operand">The operand.</param>
        public NotNode([NotNull] NodeBase operand)
            : base(operand.Simplify())
        {
            operand.DetermineWeakly(SupportableValueType.Boolean | SupportableValueType.Numeric);

            if (operand is ParameterNode op)
            {
                // If this is or can be a number, it has to be an integer number, as we cannot binary-negate a floating point expression
                op.DetermineInteger();
            }

            EnsureCompatibleOperand(operand);
        }

        /// <summary>
        ///     Gets the return type of this node.
        /// </summary>
        /// <value>
        ///     The node return type.
        /// </value>
        public override SupportedValueType ReturnType => this.Operand.ReturnType;

        /// <summary>
        ///     Ensures that the operand is actually compatible with the operation.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <exception cref="ExpressionNotValidLogicallyException">The expression is not logically valid.</exception>
        private static void EnsureCompatibleOperand(NodeBase operand)
        {
            if (operand.ReturnType == SupportedValueType.Numeric ||
                operand.ReturnType == SupportedValueType.Boolean ||
                operand.ReturnType == SupportedValueType.Unknown)
            {
                return;
            }

            throw new ExpressionNotValidLogicallyException();
        }

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>
        ///     A simplified node, or this instance.
        /// </returns>
        public override NodeBase Simplify() =>
            this.Operand switch
            {
                NumericNode numericNode => new NumericNode(~numericNode.ExtractInteger()),
                BoolNode boolNode => new BoolNode(!boolNode.Value),
                _ => this
            };

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) => new NotNode(this.Operand.DeepClone(context));

        /// <summary>
        ///     Strongly determines the node's type, if possible.
        /// </summary>
        /// <param name="type">The type to determine to.</param>
        public override void DetermineStrongly(SupportedValueType type)
        {
            if (type != SupportedValueType.Boolean && type != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }

            this.Operand.DetermineStrongly(type);

            EnsureCompatibleOperand(this.Operand);
        }

        /// <summary>
        ///     Weakly determines the node's type, if possible, and, optionally, strongly determines if there is only one possible
        ///     type left.
        /// </summary>
        /// <param name="type">The type or types to determine to.</param>
        /// <exception cref="ExpressionNotValidLogicallyException">Expression is not logically valid.</exception>
        public override void DetermineWeakly(SupportableValueType type)
        {
            if ((type & SupportableValueType.Boolean) == 0 && (type & SupportableValueType.Numeric) == 0)
            {
                throw new ExpressionNotValidLogicallyException();
            }

            this.Operand.DetermineWeakly(type);

            EnsureCompatibleOperand(this.Operand);
        }

        /// <summary>
        ///     Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>
        ///     The expression.
        /// </returns>
        protected override Expression GenerateExpressionInternal() => Expression.Not(this.Operand.GenerateExpression());

        /// <summary>
        ///     Generates the expression with tolerance that will be compiled into code.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The expression.</returns>
        protected override Expression GenerateExpressionInternal(Tolerance tolerance) =>
            Expression.Not(this.Operand.GenerateExpression(tolerance));
    }
}