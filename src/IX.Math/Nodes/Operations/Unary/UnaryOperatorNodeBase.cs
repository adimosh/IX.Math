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

        [NotNull]
        public NodeBase Operand { get; private set; }
    }
}