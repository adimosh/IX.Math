// <copyright file="FunctionNodemin.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Function.Binary
{
    internal class FunctionNodemin : BinaryFunctionNodeBase
    {
        public FunctionNodemin(NumericNode firstParameter, NumericNode secondParameter)
            : base(firstParameter, secondParameter)
        {
        }

        public FunctionNodemin(NumericNode firstParameter, NumericParameterNode secondParameter)
            : base(firstParameter, secondParameter)
        {
        }

        public FunctionNodemin(NumericNode firstParameter, UndefinedParameterNode secondParameter)
            : base(firstParameter, secondParameter?.DetermineNumeric())
        {
        }

        public FunctionNodemin(NumericNode firstParameter, OperationNodeBase secondParameter)
            : base(firstParameter, secondParameter?.Simplify())
        {
            if (this.SecondParameter?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public FunctionNodemin(NumericParameterNode firstParameter, NumericNode secondParameter)
            : base(firstParameter, secondParameter)
        {
        }

        public FunctionNodemin(NumericParameterNode firstParameter, NumericParameterNode secondParameter)
            : base(firstParameter, secondParameter)
        {
        }

        public FunctionNodemin(NumericParameterNode firstParameter, UndefinedParameterNode secondParameter)
            : base(firstParameter, secondParameter?.DetermineNumeric())
        {
        }

        public FunctionNodemin(NumericParameterNode firstParameter, OperationNodeBase secondParameter)
            : base(firstParameter, secondParameter?.Simplify())
        {
            if (this.SecondParameter?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public FunctionNodemin(UndefinedParameterNode firstParameter, NumericNode secondParameter)
            : base(firstParameter?.DetermineNumeric(), secondParameter)
        {
        }

        public FunctionNodemin(UndefinedParameterNode firstParameter, NumericParameterNode secondParameter)
            : base(firstParameter?.DetermineNumeric(), secondParameter)
        {
        }

        public FunctionNodemin(UndefinedParameterNode firstParameter, UndefinedParameterNode secondParameter)
            : base(firstParameter?.DetermineNumeric(), secondParameter?.DetermineNumeric())
        {
        }

        public FunctionNodemin(UndefinedParameterNode firstParameter, OperationNodeBase secondParameter)
            : base(firstParameter?.DetermineNumeric(), secondParameter?.Simplify())
        {
            if (this.SecondParameter?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public FunctionNodemin(OperationNodeBase firstParameter, NumericNode secondParameter)
            : base(firstParameter?.Simplify(), secondParameter)
        {
            if (this.FirstParameter?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public FunctionNodemin(OperationNodeBase firstParameter, NumericParameterNode secondParameter)
            : base(firstParameter?.Simplify(), secondParameter)
        {
            if (this.FirstParameter?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public FunctionNodemin(OperationNodeBase firstParameter, UndefinedParameterNode secondParameter)
            : base(firstParameter?.Simplify(), secondParameter?.DetermineNumeric())
        {
            if (this.FirstParameter?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public FunctionNodemin(OperationNodeBase firstParameter, OperationNodeBase secondParameter)
            : base(firstParameter?.Simplify(), secondParameter?.Simplify())
        {
            if (this.FirstParameter?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }

            if (this.SecondParameter?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public override SupportedValueType ReturnType => SupportedValueType.Numeric;

        public override NodeBase Simplify()
        {
            NumericNode firstParam, secondParam;
            if ((firstParam = this.FirstParameter as NumericNode) != null &&
                (secondParam = this.SecondParameter as NumericNode) != null)
            {
                return new NumericNode(System.Math.Min(firstParam.ExtractFloat(), secondParam.ExtractFloat()));
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal() => this.GenerateStaticBinaryFunctionCall(typeof(System.Math), nameof(System.Math.Min));
    }
}