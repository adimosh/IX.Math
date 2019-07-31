// <copyright file="FunctionNodeLength.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Operations.Function.Unary
{
    [DebuggerDisplay("strlen({" + nameof(Parameter) + "})")]
    [CallableMathematicsFunction("length")]
    [UsedImplicitly]
    internal sealed class FunctionNodeLength : UnaryFunctionNodeBase
    {
        public FunctionNodeLength(NodeBase parameter)
            : base(parameter)
        {
        }

        public override SupportedValueType ReturnType => SupportedValueType.Numeric;

        public override NodeBase Simplify()
        {
            if (this.Parameter is StringNode stringParam)
            {
                return new NumericNode(Convert.ToInt64(stringParam.Value.Length));
            }

            return this;
        }

        public override NodeBase DeepClone(NodeCloningContext context) =>
            new FunctionNodeStringLength(this.Parameter.DeepClone(context));

        protected override void EnsureCompatibleParameter(ref NodeBase parameter)
        {
            if (parameter is ParameterNode upn)
            {
                parameter = upn.DetermineString();
            }

            if (parameter.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        protected override Expression GenerateExpressionInternal() => Expression.Convert(
            this.GenerateParameterPropertyCall<string>(nameof(string.Length)),
            typeof(long));
    }
}