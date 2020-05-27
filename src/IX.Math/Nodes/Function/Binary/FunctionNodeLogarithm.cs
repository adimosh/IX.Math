// <copyright file="FunctionNodeLogarithm.cs" company="Adrian Mos">
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
    ///     A node representing the <see cref="GlobalSystem.Math.Log(double, double)" /> function.
    /// </summary>
    /// <seealso cref="NumericBinaryFunctionNodeBase" />
    [DebuggerDisplay("log({" + nameof(FirstParameter) + "}, {" + nameof(SecondParameter) + "})")]
    [CallableMathematicsFunction("log", "logarithm")]
    [UsedImplicitly]
    internal sealed class FunctionNodeLogarithm : NumericBinaryFunctionNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionNodeLogarithm"/> class.
        /// </summary>
        /// <param name="firstParameter">The first parameter.</param>
        /// <param name="secondParameter">The second parameter.</param>
        public FunctionNodeLogarithm(
            NodeBase firstParameter,
            NodeBase secondParameter)
            : base(
                firstParameter?.Simplify(),
                secondParameter?.Simplify())
        {
        }

        /// <summary>
        /// Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>
        /// A deep clone.
        /// </returns>
        public override NodeBase DeepClone(NodeCloningContext context) => new FunctionNodeLogarithm(
            this.FirstParameter.DeepClone(context),
            this.SecondParameter.DeepClone(context));

        /// <summary>
        /// Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>
        /// A simplified node, or this instance.
        /// </returns>
        public override NodeBase Simplify() =>
            this.FirstParameter is NumericNode firstParam && this.SecondParameter is NumericNode secondParam
                ? new NumericNode(
                    GlobalSystem.Math.Log(
                        firstParam.ExtractFloat(),
                        secondParam.ExtractFloat()))
                : (NodeBase)this;

        /// <summary>
        /// Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>
        /// The expression.
        /// </returns>
        protected override Expression GenerateExpressionInternal() =>
            this.GenerateStaticBinaryFunctionCall(
                typeof(GlobalSystem.Math),
                nameof(GlobalSystem.Math.Log));

        /// <summary>
        /// Generates the expression with tolerance that will be compiled into code.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The expression.</returns>
        protected override Expression GenerateExpressionInternal(in ComparisonTolerance tolerance) =>
            this.GenerateStaticBinaryFunctionCall(
                typeof(GlobalSystem.Math),
                nameof(GlobalSystem.Math.Log),
                in tolerance);
    }
}