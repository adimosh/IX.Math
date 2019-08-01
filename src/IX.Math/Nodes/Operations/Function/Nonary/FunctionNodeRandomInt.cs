// <copyright file="FunctionNodeRandomInt.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Extensibility;
using IX.Math.Generators;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Operations.Function.Nonary
{
    [DebuggerDisplay("randomint()")]
    [CallableMathematicsFunction("randomint")]
    [UsedImplicitly]
    internal sealed class FunctionNodeRandomInt : NonaryFunctionNodeBase
    {
        public override SupportedValueType ReturnType => SupportedValueType.Numeric;

        public static long GenerateRandom() => RandomNumberGenerator.GenerateInt();

        public override NodeBase Simplify() => this;

        public override NodeBase DeepClone(NodeCloningContext context) => new FunctionNodeRandomInt();

        /// <summary>
        ///     Strongly determines the node's type, if possible.
        /// </summary>
        /// <param name="type">The type to determine to.</param>
        public override void DetermineStrongly(SupportedValueType type)
        {
            if (type != SupportedValueType.Numeric)
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
            if ((type & SupportableValueType.Numeric) == 0)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        protected override Expression GenerateExpressionInternal() => this.GenerateStaticNonaryFunctionCall<FunctionNodeRandomInt>(nameof(GenerateRandom));
    }
}