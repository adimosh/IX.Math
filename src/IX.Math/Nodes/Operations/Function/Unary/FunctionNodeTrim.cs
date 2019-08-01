// <copyright file="FunctionNodeTrim.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Operations.Function.Unary
{
    [DebuggerDisplay("trim({" + nameof(Parameter) + "})")]
    [CallableMathematicsFunction("trim")]
    [UsedImplicitly]
    internal sealed class FunctionNodeTrim : UnaryFunctionNodeBase
    {
        public FunctionNodeTrim(NodeBase parameter)
            : base(parameter)
        {
        }

        public override SupportedValueType ReturnType => SupportedValueType.String;

        public override NodeBase Simplify()
        {
            if (this.Parameter is StringNode stringParam)
            {
                return new StringNode(stringParam.Value.Trim());
            }

            return this;
        }

        public override NodeBase DeepClone(NodeCloningContext context) =>
            new FunctionNodeTrim(this.Parameter.DeepClone(context));

        /// <summary>
        ///     Strongly determines the node's type, if possible.
        /// </summary>
        /// <param name="type">The type to determine to.</param>
        public override void DetermineStrongly(SupportedValueType type)
        {
            if (type != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        /// <summary>
        ///     Weakly determines the node's type, if possible, and, optionally, strongly determines if there is only one possible
        ///     type left.
        /// </summary>
        /// <param name="type">The type or types to determine to.</param>
        public override void DetermineWeakly(SupportableValueType type)
        {
            if ((type & SupportableValueType.String) == 0)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        protected override void EnsureCompatibleParameter(NodeBase parameter)
        {
            parameter.DetermineStrongly(SupportedValueType.String);

            if (parameter.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        protected override Expression GenerateExpressionInternal() =>
            this.GenerateParameterMethodCall<string>(nameof(string.Trim));
    }
}