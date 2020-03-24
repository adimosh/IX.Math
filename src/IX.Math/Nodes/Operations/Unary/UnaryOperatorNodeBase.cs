// <copyright file="UnaryOperatorNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Operations.Unary
{
    internal abstract class UnaryOperatorNodeBase : OperationNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnaryOperatorNodeBase"/> class.
        /// </summary>
        /// <param name="operand">The operand.</param>
        /// <exception cref="ArgumentNullException">operand
        /// is <c>null</c> (<c>Nothing</c> in Visual Basic).</exception>
        protected private UnaryOperatorNodeBase([NotNull] NodeBase operand)
        {
            this.Operand = operand ?? throw new ArgumentNullException(nameof(operand));
        }

        /// <summary>
        ///     Gets a value indicating whether this node supports tolerance.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is tolerant; otherwise, <c>false</c>.
        /// </value>
        public override bool IsTolerant => this.Operand.IsTolerant;

        /// <summary>
        /// Gets the operand.
        /// </summary>
        /// <value>
        /// The operand.
        /// </value>
        [NotNull]
        protected NodeBase Operand { get; }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public new abstract NodeBase DeepClone(NodeCloningContext context);

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        protected override OperationNodeBase DeepCloneNode(NodeCloningContext context) => (OperationNodeBase)this.DeepClone(context);
    }
}