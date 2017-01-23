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

        public abstract Expression GenerateExpression();
    }
}