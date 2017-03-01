// <copyright file="FunctionNodetan.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Function.Unary
{
    internal class FunctionNodetan : UnaryFunctionNodeBase
    {
        public FunctionNodetan(NumericNode parameter)
            : base(parameter)
        {
        }

        public FunctionNodetan(NumericParameterNode parameter)
            : base(parameter)
        {
        }

        public FunctionNodetan(UndefinedParameterNode parameter)
            : base(parameter?.DetermineNumeric())
        {
        }

        public FunctionNodetan(OperationNodeBase parameter)
            : base(parameter?.Simplify())
        {
            if (parameter?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public override SupportedValueType ReturnType => SupportedValueType.Numeric;

        public override NodeBase Simplify()
        {
            NumericNode stringParam;
            if ((stringParam = this.Parameter as NumericNode) != null)
            {
                return new NumericNode(System.Math.Tan(stringParam.ExtractFloat()));
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal() => this.GenerateStaticUnaryFunctionCall(typeof(System.Math), nameof(System.Math.Tan));
    }
}