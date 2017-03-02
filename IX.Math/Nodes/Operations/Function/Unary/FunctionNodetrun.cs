// <copyright file="FunctionNodetrun.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Function.Unary
{
    [DebuggerDisplay("trun({Parameter})")]
    internal sealed class FunctionNodetrun : UnaryFunctionNodeBase
    {
        public FunctionNodetrun(NumericNode parameter)
            : base(parameter)
        {
        }

        public FunctionNodetrun(NumericParameterNode parameter)
            : base(parameter)
        {
        }

        public FunctionNodetrun(UndefinedParameterNode parameter)
            : base(parameter?.DetermineNumeric())
        {
        }

        public FunctionNodetrun(OperationNodeBase parameter)
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
                return new NumericNode(System.Math.Truncate(stringParam.ExtractFloat()));
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal() => this.GenerateStaticUnaryFunctionCall(typeof(System.Math), nameof(System.Math.Truncate));
    }
}