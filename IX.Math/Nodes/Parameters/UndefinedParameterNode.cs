// <copyright file="UndefinedParameterNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Generators;

namespace IX.Math.Nodes.Parameters
{
    [DebuggerDisplay("{ParameterName} (undefined)")]
    internal sealed class UndefinedParameterNode : ParameterNodeBase
    {
        private IDictionary<string, ParameterNodeBase> parametersTable;

        public UndefinedParameterNode(string parameterName, IDictionary<string, ParameterNodeBase> parametersTable)
            : base(parameterName)
        {
            this.parametersTable = parametersTable;
        }

        public override SupportedValueType ReturnType => SupportedValueType.Unknown;

        public NumericParameterNode DetermineNumeric() => ParametersGenerator.DetermineNumeric(this.parametersTable, this.ParameterName);

        public BoolParameterNode DetermineBool() => ParametersGenerator.DetermineBool(this.parametersTable, this.ParameterName);

        public StringParameterNode DetermineString() => ParametersGenerator.DetermineString(this.parametersTable, this.ParameterName);

        public override Expression GenerateStringExpression()
        {
            throw new InvalidOperationException();
        }

        protected override Expression GenerateExpressionInternal()
        {
            throw new InvalidOperationException();
        }
    }
}