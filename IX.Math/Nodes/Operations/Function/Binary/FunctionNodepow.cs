// <copyright file="FunctionNodepow.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Function.Binary
{
    [DebuggerDisplay("pow({FirstParameter}, {SecondParameter})")]
    internal sealed class FunctionNodepow : BinaryFunctionNodeBase
    {
        public FunctionNodepow(NumericNode firstParameter, NumericNode secondParameter)
            : base(firstParameter, secondParameter)
        {
        }

        public FunctionNodepow(NumericNode firstParameter, NumericParameterNode secondParameter)
            : base(firstParameter, secondParameter)
        {
        }

        public FunctionNodepow(NumericNode firstParameter, OperationNodeBase secondParameter)
            : base(firstParameter, secondParameter?.Simplify())
        {
            if (this.SecondParameter?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public FunctionNodepow(NumericParameterNode firstParameter, NumericNode secondParameter)
            : base(firstParameter, secondParameter)
        {
        }

        public FunctionNodepow(NumericParameterNode firstParameter, NumericParameterNode secondParameter)
            : base(firstParameter, secondParameter)
        {
        }

        public FunctionNodepow(NumericParameterNode firstParameter, OperationNodeBase secondParameter)
            : base(firstParameter, secondParameter?.Simplify())
        {
            if (this.SecondParameter?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public FunctionNodepow(OperationNodeBase firstParameter, NumericNode secondParameter)
            : base(firstParameter?.Simplify(), secondParameter)
        {
            if (this.FirstParameter?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public FunctionNodepow(OperationNodeBase firstParameter, NumericParameterNode secondParameter)
            : base(firstParameter?.Simplify(), secondParameter)
        {
            if (this.FirstParameter?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public FunctionNodepow(OperationNodeBase firstParameter, OperationNodeBase secondParameter)
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

        public FunctionNodepow(UndefinedParameterNode firstParameter, UndefinedParameterNode secondParameter)
            : base(firstParameter?.DetermineNumeric(), secondParameter?.DetermineNumeric())
        {
        }

        public FunctionNodepow(UndefinedParameterNode firstParameter, NodeBase secondParameter)
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

        public FunctionNodepow(NodeBase firstParameter, UndefinedParameterNode secondParameter)
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
                return new NumericNode(System.Math.Pow(firstParam.ExtractFloat(), secondParam.ExtractFloat()));
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal() => this.GenerateStaticBinaryFunctionCall(typeof(System.Math), nameof(System.Math.Pow));
    }
}