// <copyright file="FunctionNodeMaximum.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Operations.Function.Binary
{
    [DebuggerDisplay("max({FirstParameter}, {SecondParameter})")]
    [CallableMathematicsFunction("max", "maximum")]
    [UsedImplicitly]
    internal sealed class FunctionNodeMaximum : NumericBinaryFunctionNodeBase
    {
        public FunctionNodeMaximum(
            NodeBase firstParameter,
            NodeBase secondParameter)
            : base(
                firstParameter?.Simplify(),
                secondParameter?.Simplify())
        {
        }

        public override NodeBase DeepClone(NodeCloningContext context) => new FunctionNodeMaximum(
            this.FirstParameter.DeepClone(context),
            this.SecondParameter.DeepClone(context));

        public override NodeBase Simplify() =>
            this.FirstParameter is NumericNode firstParam && this.SecondParameter is NumericNode secondParam
                ? new NumericNode(
                    global::System.Math.Max(
                        firstParam.ExtractFloat(),
                        secondParam.ExtractFloat()))
                : (NodeBase)this;

        protected override Expression GenerateExpressionInternal() => this.GenerateStaticBinaryFunctionCall(
            typeof(global::System.Math),
            nameof(global::System.Math.Max));
    }
}