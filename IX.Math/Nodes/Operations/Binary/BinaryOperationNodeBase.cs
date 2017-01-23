// <copyright file="BinaryOperationNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;

namespace IX.Math.Nodes.Operations.Binary
{
    internal abstract class BinaryOperationNodeBase : OperationNodeBase
    {
        protected BinaryOperationNodeBase(NodeBase left, NodeBase right)
        {
            this.Left = left ?? throw new ArgumentNullException(nameof(left));
            this.Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public NodeBase Left { get; private set; }

        public NodeBase Right { get; private set; }
    }
}