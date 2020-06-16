// <copyright file="UnaryOperatorNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using IX.Math.Extensibility;
using IX.StandardExtensions.Contracts;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Operators.Unary
{
    /// <summary>
    ///     A base node for unary operators.
    /// </summary>
    /// <seealso cref="IX.Math.Nodes.NodeBase" />
    internal abstract class UnaryOperatorNodeBase : NodeBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UnaryOperatorNodeBase" /> class.
        /// </summary>
        /// <param name="stringFormatters">The string formatters.</param>
        /// <param name="operand">The operand.</param>
        /// <exception cref="ArgumentNullException">
        ///     operand
        ///     is <c>null</c> (<c>Nothing</c> in Visual Basic).
        /// </exception>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "ReSharper",
            "VirtualMemberCallInConstructor",
            Justification = "We specifically want this to happen.")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Usage",
            "CA2214:Do not call overridable methods in constructors",
            Justification = "We want this here.")]
        protected private UnaryOperatorNodeBase(
            List<IStringFormatter> stringFormatters,
            [NotNull] NodeBase operand)
            : base(stringFormatters)
        {
            NodeBase tempOperand = Requires.NotNull(
                operand,
                nameof(operand));

            this.EnsureCompatibleOperandsAndRefineReturnType(tempOperand);

            this.Operand = tempOperand;
        }

        /// <summary>
        ///     Gets a value indicating whether or not this node is actually a constant.
        /// </summary>
        /// <value><see langword="true" /> if the node is a constant, <see langword="false" /> otherwise.</value>
        public sealed override bool IsConstant => this.Operand.IsConstant;

        /// <summary>
        ///     Gets a value indicating whether this node supports tolerance.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is tolerant; otherwise, <c>false</c>.
        /// </value>
        public sealed override bool IsTolerant => this.Operand.IsTolerant;

        /// <summary>
        ///     Gets a value indicating whether this node requires preservation of the original expression.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the node requires original expression preservation, or <see langword="false" />
        ///     if it can be polynomially-reduced.
        /// </value>
        public sealed override bool RequiresPreservedExpression => this.Operand.RequiresPreservedExpression;

        /// <summary>
        ///     Gets the operand.
        /// </summary>
        /// <value>
        ///     The operand.
        /// </value>
        [NotNull]
        protected NodeBase Operand { get; }

        /// <summary>
        ///     Verifies this node and all nodes above it for logical validity.
        /// </summary>
        /// <remarks>
        ///     <para>This method is expected to be overridden, and is a good place to do type restriction verification.</para>
        /// </remarks>
        public sealed override void Verify()
        {
            this.CalculatedCosts.Clear();

            this.Operand.Verify();

            this.EnsureCompatibleOperandsAndRefineReturnType(this.Operand);
        }

        /// <summary>
        ///     Ensures that the operands are compatible, and refines the return type of this expression based on them.
        /// </summary>
        /// <param name="operand">The operand.</param>
        protected abstract void EnsureCompatibleOperandsAndRefineReturnType(NodeBase operand);
    }
}