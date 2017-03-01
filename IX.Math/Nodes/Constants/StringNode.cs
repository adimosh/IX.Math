// <copyright file="StringNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace IX.Math.Nodes.Constants
{
    [DebuggerDisplay("{Value}")]
    internal sealed class StringNode : ConstantNodeBase
    {
        private readonly string value;

        public StringNode(string value)
        {
            this.value = value;
        }

        public string Value => this.value;

        public override SupportedValueType ReturnType => SupportedValueType.String;

        public override Expression GenerateExpression() => Expression.Constant(this.value, typeof(string));

        public override Expression GenerateStringExpression() => this.GenerateExpression();

        public override object DistilValue() => this.value;
    }
}