﻿// <copyright file="FunctionNodeatan.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Function.Unary
{
    [DebuggerDisplay("atan({Parameter})")]
    internal sealed class FunctionNodeatan : UnaryFunctionNodeBase
    {
        public FunctionNodeatan(NumericNode parameter)
            : base(parameter)
        {
        }

        public FunctionNodeatan(NumericParameterNode parameter)
            : base(parameter)
        {
        }

        public FunctionNodeatan(UndefinedParameterNode parameter)
            : base(parameter?.DetermineNumeric())
        {
        }

        public FunctionNodeatan(OperationNodeBase parameter)
            : base(parameter?.Simplify())
        {
            if (this.Parameter?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public override SupportedValueType ReturnType => SupportedValueType.Numeric;

        public override NodeBase Simplify()
        {
            NumericNode stringParam;
            if ((stringParam = this.Parameter as NumericNode) != null)
            {
                return new NumericNode(System.Math.Atan(stringParam.ExtractFloat()));
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal() => this.GenerateStaticUnaryFunctionCall(typeof(System.Math), nameof(System.Math.Atan));
    }
}