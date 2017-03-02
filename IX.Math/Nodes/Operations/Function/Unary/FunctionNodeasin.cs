// <copyright file="FunctionNodeasin.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Function.Unary
{
    [DebuggerDisplay("asin({Parameter})")]
    internal sealed class FunctionNodeasin : UnaryFunctionNodeBase
    {
        public FunctionNodeasin(NumericNode parameter)
            : base(parameter)
        {
        }

        public FunctionNodeasin(NumericParameterNode parameter)
            : base(parameter)
        {
        }

        public FunctionNodeasin(UndefinedParameterNode parameter)
            : base(parameter?.DetermineNumeric())
        {
        }

        public FunctionNodeasin(OperationNodeBase parameter)
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
                return new NumericNode(System.Math.Asin(stringParam.ExtractFloat()));
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal() => this.GenerateStaticUnaryFunctionCall(typeof(System.Math), nameof(System.Math.Asin));
    }
}