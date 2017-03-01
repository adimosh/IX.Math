// <copyright file="BoolNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;

namespace IX.Math.Nodes.Constants
{
    [DebuggerDisplay("{Value}")]
    internal sealed class BoolNode : ConstantNodeBase
    {
        private readonly bool value;

        public BoolNode(bool value)
        {
            this.value = value;
        }

        public bool Value => this.value;

        public override SupportedValueType ReturnType => SupportedValueType.Boolean;

        public override object DistilValue() => this.value;

        public override Expression GenerateExpression() => Expression.Constant(this.value, typeof(bool));

        public override Expression GenerateStringExpression() => Expression.Constant(this.value ? "true" : "false", typeof(string));
    }
}