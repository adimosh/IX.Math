﻿// <copyright file="FunctionNodeMax.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Function.Binary
{
    [DebuggerDisplay("max({FirstParameter}, {SecondParameter})")]
    [CallableMathematicsFunction("max", "maximum")]
    internal sealed class FunctionNodeMax : BinaryFunctionNodeBase
    {
        public FunctionNodeMax(NumericNode firstParameter, NumericNode secondParameter)
            : base(firstParameter, secondParameter)
        {
        }

        public FunctionNodeMax(NumericNode firstParameter, NumericParameterNode secondParameter)
            : base(firstParameter, secondParameter)
        {
        }

        public FunctionNodeMax(NumericNode firstParameter, OperationNodeBase secondParameter)
            : base(firstParameter, secondParameter?.Simplify())
        {
            if (this.SecondParameter?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public FunctionNodeMax(NumericParameterNode firstParameter, NumericNode secondParameter)
            : base(firstParameter, secondParameter)
        {
        }

        public FunctionNodeMax(NumericParameterNode firstParameter, NumericParameterNode secondParameter)
            : base(firstParameter, secondParameter)
        {
        }

        public FunctionNodeMax(NumericParameterNode firstParameter, OperationNodeBase secondParameter)
            : base(firstParameter, secondParameter?.Simplify())
        {
            if (this.SecondParameter?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public FunctionNodeMax(OperationNodeBase firstParameter, NumericNode secondParameter)
            : base(firstParameter?.Simplify(), secondParameter)
        {
            if (this.FirstParameter?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public FunctionNodeMax(OperationNodeBase firstParameter, NumericParameterNode secondParameter)
            : base(firstParameter?.Simplify(), secondParameter)
        {
            if (this.FirstParameter?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public FunctionNodeMax(OperationNodeBase firstParameter, OperationNodeBase secondParameter)
            : base(firstParameter?.Simplify(), secondParameter?.Simplify())
        {
            if (this.FirstParameter?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }

            if (this.SecondParameter?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public FunctionNodeMax(UndefinedParameterNode firstParameter, UndefinedParameterNode secondParameter)
            : base(firstParameter?.DetermineNumeric(), secondParameter?.DetermineNumeric())
        {
        }

        public FunctionNodeMax(UndefinedParameterNode firstParameter, NodeBase secondParameter)
            : base(firstParameter, secondParameter?.Simplify())
        {
            if (this.SecondParameter.ReturnType == SupportedValueType.Numeric)
            {
                this.FirstParameter = firstParameter.DetermineNumeric();
            }
            else
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public FunctionNodeMax(NodeBase firstParameter, UndefinedParameterNode secondParameter)
            : base(firstParameter?.Simplify(), secondParameter)
        {
            if (this.FirstParameter.ReturnType == SupportedValueType.Numeric)
            {
                this.SecondParameter = secondParameter.DetermineNumeric();
            }
            else
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public override SupportedValueType ReturnType => SupportedValueType.Numeric;

        public override NodeBase Simplify()
        {
            NumericNode firstParam, secondParam;
            if ((firstParam = this.FirstParameter as NumericNode) != null &&
                (secondParam = this.SecondParameter as NumericNode) != null)
            {
                return new NumericNode(System.Math.Max(firstParam.ExtractFloat(), secondParam.ExtractFloat()));
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal() => this.GenerateStaticBinaryFunctionCall(typeof(System.Math), nameof(System.Math.Max));
    }
}