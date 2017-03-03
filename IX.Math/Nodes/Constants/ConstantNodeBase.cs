// <copyright file="ConstantNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

namespace IX.Math.Nodes.Constants
{
    internal abstract class ConstantNodeBase : NodeBase
    {
        protected ConstantNodeBase()
        {
        }

        public abstract object DistilValue();

        public override NodeBase RefreshParametersRecursive() => this;

        public override NodeBase Simplify() => this;
    }
}