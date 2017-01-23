// <copyright file="StringNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;

namespace IX.Math.Nodes.Constants
{
    internal sealed class StringNode : ConstantNodeBase
    {
        private readonly string value;

        public StringNode(string value)
        {
            this.value = value;
        }

        public string Value => this.value;

        public override Expression GenerateExpression() => Expression.Constant(this.value, typeof(string));
    }
}