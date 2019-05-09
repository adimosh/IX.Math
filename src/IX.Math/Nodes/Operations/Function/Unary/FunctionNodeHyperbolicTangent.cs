// <copyright file="FunctionNodeHyperbolicTangent.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Operations.Function.Unary
{
    [DebuggerDisplay("tanh({Parameter})")]
    [CallableMathematicsFunction("tanh")]
    [UsedImplicitly]
    internal sealed class FunctionNodeHyperbolicTangent : NumericUnaryFunctionNodeBase
    {
        public FunctionNodeHyperbolicTangent(NodeBase parameter)
            : base(parameter)
        {
        }

        public override NodeBase Simplify()
        {
            if (this.Parameter is NumericNode nodeParam)
            {
                return new NumericNode(global::System.Math.Tanh(nodeParam.ExtractFloat()));
            }

            return this;
        }

        public override NodeBase DeepClone(NodeCloningContext context) =>
            new FunctionNodeHyperbolicTangent(this.Parameter.DeepClone(context));

        protected override Expression GenerateExpressionInternal() => this.GenerateStaticUnaryFunctionCall(
            typeof(global::System.Math),
            nameof(global::System.Math.Tanh));
    }
}