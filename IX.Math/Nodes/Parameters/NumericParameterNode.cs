﻿// <copyright file="NumericParameterNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;

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

        protected override Expression GenerateExpressionInternal()
        {
            return (this.RequireFloat ?? false) ?
                Expression.Parameter(typeof(double), this.ParameterName) :
                Expression.Parameter(typeof(long), this.ParameterName);
        }
    }
}