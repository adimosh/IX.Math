// <copyright file="UnaryFunctionNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;

namespace IX.Math.Nodes.Operations.Function.Unary
{
    internal abstract class UnaryFunctionNodeBase : FunctionNodeBase
    {
        protected UnaryFunctionNodeBase(NodeBase parameter)
        {
            this.Parameter = parameter ?? throw new ArgumentNullException(nameof(parameter));
        }

        public NodeBase Parameter { get; private set; }
    }
}