﻿// <copyright file="FunctionNodeHyperbolicCosine.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Function.Unary
{
    [DebuggerDisplay("cosh({Parameter})")]
    [CallableMathematicsFunction("cosh")]
    internal sealed class FunctionNodeHyperbolicCosine : UnaryFunctionNodeBase
    {
        public FunctionNodeHyperbolicCosine(NumericNode parameter)
            : base(parameter)
        {
        }

        public FunctionNodeHyperbolicCosine(NumericParameterNode parameter)
            : base(parameter)
        {
        }

        public FunctionNodeHyperbolicCosine(UndefinedParameterNode parameter)
            : base(parameter?.DetermineNumeric())
        {
        }

        public FunctionNodeHyperbolicCosine(OperationNodeBase parameter)
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
                return new NumericNode(System.Math.Cosh(stringParam.ExtractFloat()));
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal() => this.GenerateStaticUnaryFunctionCall(typeof(System.Math), nameof(System.Math.Cosh));
    }
}