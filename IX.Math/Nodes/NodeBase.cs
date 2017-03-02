// <copyright file="NodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;

namespace IX.Math.Nodes
{
    internal abstract class NodeBase
    {
        internal NodeBase()
        {
        }

        public abstract SupportedValueType ReturnType { get; }

        public abstract Expression GenerateExpression();

        public abstract Expression GenerateStringExpression();

        public abstract NodeBase RefreshParametersRecursive();

        public abstract NodeBase Simplify();
    }
}