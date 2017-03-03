// <copyright file="StringParameterNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace IX.Math.Nodes.Parameters
{
    [DebuggerDisplay("{ParameterName} (string)")]
    internal sealed class StringParameterNode : ParameterNodeBase
    {
        public StringParameterNode(string parameterName)
            : base(parameterName)
        {
        }

        public override SupportedValueType ReturnType => SupportedValueType.String;

        public override Expression GenerateStringExpression() => this.GenerateExpression();

        protected override Expression GenerateExpressionInternal() => Expression.Parameter(typeof(string), this.ParameterName);
    }
}