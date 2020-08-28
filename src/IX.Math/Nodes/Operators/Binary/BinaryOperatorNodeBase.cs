// <copyright file="BinaryOperatorNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using IX.StandardExtensions.Contracts;

namespace IX.Math.Nodes.Operators.Binary
{
    /// <summary>
    ///     A node base for binary operations.
    /// </summary>
    /// <seealso cref="NodeBase" />
    internal abstract class BinaryOperatorNodeBase : NodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryOperatorNodeBase" /> class.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        [SuppressMessage(
            "ReSharper",
            "VirtualMemberCallInConstructor",
            Justification = "We specifically want this to happen.")]
        [SuppressMessage(
            "Usage",
            "CA2214:Do not call overridable methods in constructors",
            Justification = "We specifically want this to happen.")]
        protected private BinaryOperatorNodeBase(
            NodeBase left,
            NodeBase right)
        {
            var leftProcessed = Requires.NotNull(
                left,
                nameof(left)).Simplify();
            var rightProcessed = Requires.NotNull(
                right,
                nameof(right)).Simplify();

            this.EnsureCompatibleOperandsAndRefineReturnType(
                leftProcessed,
                rightProcessed);

            this.Left = leftProcessed;
            this.Right = rightProcessed;
        }

        /// <summary>
        ///     Gets a value indicating whether or not this node is actually a constant.
        /// </summary>
        /// <value><see langword="true" /> if the node is a constant, <see langword="false" /> otherwise.</value>
        public override bool IsConstant => this.Left.IsConstant & this.Right.IsConstant;

        /// <summary>
        /// Gets a value indicating whether this node supports tolerance.
        /// </summary>
        /// <value>
        ///   <see langword="true" /> if the node is tolerant, <see langword="false" /> otherwise.
        /// </value>
        public sealed override bool IsTolerant => this.Left.IsTolerant | this.Right.IsTolerant;

        /// <summary>
        ///     Gets a value indicating whether this node requires preservation of the original expression.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the node requires original expression preservation, or <see langword="false" />
        ///     if it can be polynomially-reduced.
        /// </value>
        public sealed override bool RequiresPreservedExpression =>
            this.Left.RequiresPreservedExpression | this.Right.RequiresPreservedExpression;

        /// <summary>
        ///     Gets the left operand.
        /// </summary>
        /// <value>
        ///     The left operand.
        /// </value>
        protected NodeBase Left { get; }

        /// <summary>
        ///     Gets the right operand.
        /// </summary>
        /// <value>
        ///     The right operand.
        /// </value>
        protected NodeBase Right { get; }

        /// <summary>
        /// Verifies this node and all nodes above it for logical validity.
        /// </summary>
        /// <remarks>
        /// <para>This method is expected to be overridden, and is a good place to do type restriction verification.</para>
        /// </remarks>
        public sealed override void Verify()
        {
            this.CalculatedCosts.Clear();

            this.Left.Verify();
            this.Right.Verify();

            this.EnsureCompatibleOperandsAndRefineReturnType(
                this.Left,
                this.Right);
        }

        /// <summary>
        ///     Ensures that the operands are compatible, and refines the return type of this expression based on them.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        protected abstract void EnsureCompatibleOperandsAndRefineReturnType(
            NodeBase left,
            NodeBase right);
    }
}