// <copyright file="FunctionNodeRound.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using JetBrains.Annotations;
using GlobalSystem = System;

namespace IX.Math.Nodes.Function.Unary
{
    /// <summary>
    ///     A node representing the <see cref="GlobalSystem.Math.Round(double)" /> function.
    /// </summary>
    /// <seealso cref="NumericUnaryFunctionNodeBase" />
    [DebuggerDisplay("round({" + nameof(Parameter) + "})")]
    [CallableMathematicsFunction("round")]
    [UsedImplicitly]
    internal sealed class FunctionNodeRound : NumericUnaryFunctionNodeBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FunctionNodeRound" /> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public FunctionNodeRound(NodeBase parameter)
            : base(parameter)
        {
        }

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>
        ///     A simplified node, or this instance.
        /// </returns>
        public override NodeBase Simplify()
        {
            if (this.Parameter is NumericNode numericParam)
            {
                return new NumericNode(GlobalSystem.Math.Round(numericParam.ExtractFloat()));
            }

            return this;
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>
        ///     A deep clone.
        /// </returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new FunctionNodeRound(this.Parameter.DeepClone(context));

        /// <summary>
        ///     Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>
        ///     The expression.
        /// </returns>
        protected override Expression GenerateExpressionInternal() =>
            this.GenerateStaticUnaryFunctionCall(
                typeof(GlobalSystem.Math),
                nameof(GlobalSystem.Math.Round));

        /// <summary>
        ///     Generates the expression with tolerance that will be compiled into code.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The expression.</returns>
        protected override Expression GenerateExpressionInternal(Tolerance tolerance) =>
            this.GenerateStaticUnaryFunctionCall(
                typeof(GlobalSystem.Math),
                nameof(GlobalSystem.Math.Round),
                tolerance);
    }
}