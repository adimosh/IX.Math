// <copyright file="FunctionNodeceiling.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Operations.Function.Unary
{
    [DebuggerDisplay("ceil({Parameter})")]
    [CallableMathematicsFunction("ceil", "ceiling")]
    [UsedImplicitly]
    internal sealed class FunctionNodeCeiling : NumericUnaryFunctionNodeBase
    {
        public FunctionNodeCeiling(NodeBase parameter)
            : base(parameter)
        {
        }

        public override NodeBase Simplify()
        {
            if (this.Parameter is NumericNode numericParam)
            {
                return new NumericNode(global::System.Math.Ceiling(numericParam.ExtractFloat()));
            }

            return this;
        }

        public override NodeBase DeepClone(NodeCloningContext context) =>
            new FunctionNodeCeiling(this.Parameter.DeepClone(context));

        protected override Expression GenerateExpressionInternal() => this.GenerateStaticUnaryFunctionCall(
            typeof(global::System.Math),
            nameof(global::System.Math.Ceiling));
    }
}