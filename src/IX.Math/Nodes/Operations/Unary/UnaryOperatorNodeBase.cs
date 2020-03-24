// <copyright file="UnaryOperatorNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Operations.Unary
{
    internal abstract class UnaryOperatorNodeBase : OperationNodeBase
    {
        protected UnaryOperatorNodeBase([NotNull] NodeBase operand)
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

        [NotNull]
        protected NodeBase Operand { get; }
    }
}