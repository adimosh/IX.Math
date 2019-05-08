// <copyright file="FunctionNodeLogarithm.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Operations.Function.Binary
{
    [DebuggerDisplay("log({FirstParameter}, {SecondParameter})")]
    [CallableMathematicsFunction("log", "logarithm")]
    [UsedImplicitly]
    internal sealed class FunctionNodeLogarithm : NumericBinaryFunctionNodeBase
    {
        public FunctionNodeLogarithm(
            NodeBase firstParameter,
            NodeBase secondParameter)
            : base(
                firstParameter?.Simplify(),
                secondParameter?.Simplify())
        {
        }

        public override NodeBase DeepClone(NodeCloningContext context) => new FunctionNodeLogarithm(
            this.FirstParameter.DeepClone(context),
            this.SecondParameter.DeepClone(context));

        public override NodeBase Simplify() =>
            this.FirstParameter is NumericNode firstParam && this.SecondParameter is NumericNode secondParam
                ? new NumericNode(
                    global::System.Math.Log(
                        firstParam.ExtractFloat(),
                        secondParam.ExtractFloat()))
                : (NodeBase)this;

        protected override Expression GenerateExpressionInternal() => this.GenerateStaticBinaryFunctionCall(
            typeof(global::System.Math),
            nameof(global::System.Math.Log));
    }
}