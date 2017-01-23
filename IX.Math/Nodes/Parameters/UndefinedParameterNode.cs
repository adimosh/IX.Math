// <copyright file="UndefinedParameterNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;

namespace IX.Math.Nodes.Parameters
{
    internal sealed class UndefinedParameterNode : ParameterNodeBase
    {
        public UndefinedParameterNode(string parameterName)
            : base(parameterName)
        {
        }

        protected override Expression GenerateExpressionInternal()
        {
            throw new InvalidOperationException();
        }
    }
}