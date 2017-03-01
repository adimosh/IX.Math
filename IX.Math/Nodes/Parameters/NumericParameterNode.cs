// <copyright file="NumericParameterNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.PlatformMitigation;

namespace IX.Math.Nodes.Parameters
{
    [DebuggerDisplay("{ParameterName} (numeric)")]
    internal sealed class NumericParameterNode : ParameterNodeBase
    {
        public NumericParameterNode(string parameterName)
            : base(parameterName)
        {
        }

        public bool? RequireFloat { get; set; }

        public override SupportedValueType ReturnType => SupportedValueType.Numeric;

        public override Expression GenerateStringExpression() => Expression.Call(this.GenerateExpression(), typeof(object).GetTypeMethod(nameof(object.ToString)));

        protected override Expression GenerateExpressionInternal()
        {
            return (this.RequireFloat ?? true) ?
                Expression.Parameter(typeof(double), this.ParameterName) :
                Expression.Parameter(typeof(long), this.ParameterName);
        }
    }
}