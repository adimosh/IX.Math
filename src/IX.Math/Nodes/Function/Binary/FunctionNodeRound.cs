// <copyright file="FunctionNodeRound.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using JetBrains.Annotations;
using GlobalSystem = System;

namespace IX.Math.Nodes.Function.Binary
{
    /// <summary>
    ///     A node representing the <see cref="GlobalSystem.Math.Round(double, int)" /> function.
    /// </summary>
    /// <seealso cref="NumericBinaryFunctionNodeBase" />
    [DebuggerDisplay("round({" + nameof(FirstParameter) + "}, {" + nameof(SecondParameter) + "})")]
    [CallableMathematicsFunction("round")]
    [UsedImplicitly]
    internal sealed class FunctionNodeRound : NumericBinaryFunctionNodeBase
    {
        public FunctionNodeRound(
            NodeBase floatNode,
            NodeBase intNode)
            : base(
                floatNode?.Simplify(),
                intNode?.Simplify())
        {
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new FunctionNodeRound(
                this.FirstParameter.DeepClone(context),
                this.SecondParameter.DeepClone(context));

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>A simplified node, or this instance.</returns>
        public override NodeBase Simplify()
        {
            if (this.FirstParameter is NumericNode fln && this.SecondParameter is NumericNode inn)
            {
                return new NumericNode(
                    GlobalSystem.Math.Round(
                        fln.ExtractFloat(),
                        inn.ExtractInt()));
            }

            return this;
        }

        /// <summary>
        ///     Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>The expression.</returns>
        protected override Expression GenerateExpressionInternal() =>
            this.GenerateStaticBinaryFunctionCall<double, int>(
                typeof(GlobalSystem.Math),
                nameof(GlobalSystem.Math.Round));

        /// <summary>
        ///     Generates the expression with tolerance that will be compiled into code.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The expression.</returns>
        protected override Expression GenerateExpressionInternal(in ComparisonTolerance tolerance) =>
            this.GenerateStaticBinaryFunctionCall<double, int>(
                typeof(GlobalSystem.Math),
                nameof(GlobalSystem.Math.Round),
                in tolerance);
    }
}