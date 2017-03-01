// <copyright file="BoolParameterNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.PlatformMitigation;

namespace IX.Math.Nodes.Parameters
{
    [DebuggerDisplay("{ParameterName} (bool)")]
    internal sealed class BoolParameterNode : ParameterNodeBase
    {
        public BoolParameterNode(string parameterName)
            : base(parameterName)
        {
        }

        public override SupportedValueType ReturnType => SupportedValueType.Boolean;

        public override Expression GenerateStringExpression() => Expression.Call(this.GenerateExpression(), typeof(object).GetTypeMethod(nameof(object.ToString)));

        protected override Expression GenerateExpressionInternal()
        {
            return Expression.Parameter(typeof(bool), this.ParameterName);
        }
    }
}