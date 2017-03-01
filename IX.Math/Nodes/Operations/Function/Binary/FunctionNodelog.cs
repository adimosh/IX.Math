﻿// <copyright file="FunctionNodelog.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Function.Binary
{
    internal class FunctionNodelog : BinaryFunctionNodeBase
    {
        public FunctionNodelog(NumericNode firstParameter, NumericNode secondParameter)
            : base(firstParameter, secondParameter)
        {
        }

        public FunctionNodelog(NumericNode firstParameter, NumericParameterNode secondParameter)
            : base(firstParameter, secondParameter)
        {
        }

        public FunctionNodelog(NumericNode firstParameter, UndefinedParameterNode secondParameter)
            : base(firstParameter, secondParameter?.DetermineNumeric())
        {
        }

        public FunctionNodelog(NumericNode firstParameter, OperationNodeBase secondParameter)
            : base(firstParameter, secondParameter?.Simplify())
        {
            if (this.SecondParameter?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public FunctionNodelog(NumericParameterNode firstParameter, NumericNode secondParameter)
            : base(firstParameter, secondParameter)
        {
        }

        public FunctionNodelog(NumericParameterNode firstParameter, NumericParameterNode secondParameter)
            : base(firstParameter, secondParameter)
        {
        }

        public FunctionNodelog(NumericParameterNode firstParameter, UndefinedParameterNode secondParameter)
            : base(firstParameter, secondParameter?.DetermineNumeric())
        {
        }

        public FunctionNodelog(NumericParameterNode firstParameter, OperationNodeBase secondParameter)
            : base(firstParameter, secondParameter?.Simplify())
        {
            if (this.SecondParameter?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public FunctionNodelog(UndefinedParameterNode firstParameter, NumericNode secondParameter)
            : base(firstParameter?.DetermineNumeric(), secondParameter)
        {
        }

        public FunctionNodelog(UndefinedParameterNode firstParameter, NumericParameterNode secondParameter)
            : base(firstParameter?.DetermineNumeric(), secondParameter)
        {
        }

        public FunctionNodelog(UndefinedParameterNode firstParameter, UndefinedParameterNode secondParameter)
            : base(firstParameter?.DetermineNumeric(), secondParameter?.DetermineNumeric())
        {
        }

        public FunctionNodelog(UndefinedParameterNode firstParameter, OperationNodeBase secondParameter)
            : base(firstParameter?.DetermineNumeric(), secondParameter?.Simplify())
        {
            if (this.SecondParameter?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public FunctionNodelog(OperationNodeBase firstParameter, NumericNode secondParameter)
            : base(firstParameter?.Simplify(), secondParameter)
        {
            if (this.FirstParameter?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public FunctionNodelog(OperationNodeBase firstParameter, NumericParameterNode secondParameter)
            : base(firstParameter?.Simplify(), secondParameter)
        {
            if (this.FirstParameter?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public FunctionNodelog(OperationNodeBase firstParameter, UndefinedParameterNode secondParameter)
            : base(firstParameter?.Simplify(), secondParameter?.DetermineNumeric())
        {
            if (this.FirstParameter?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public FunctionNodelog(OperationNodeBase firstParameter, OperationNodeBase secondParameter)
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
                return new NumericNode(System.Math.Log(firstParam.ExtractFloat(), secondParam.ExtractFloat()));
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal() => this.GenerateStaticBinaryFunctionCall(typeof(System.Math), nameof(System.Math.Log));
    }
}