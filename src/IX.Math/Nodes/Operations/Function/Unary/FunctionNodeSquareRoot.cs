// <copyright file="FunctionNodeSquareRoot.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using JetBrains.Annotations;
using GlobalSystem = System;

namespace IX.Math.Nodes.Operations.Function.Unary
{
    /// <summary>
    ///     A node representing the <see cref="GlobalSystem.Math.Ceiling(double)" /> function.
    /// </summary>
    /// <seealso cref="IX.Math.Nodes.Operations.Function.Unary.NumericUnaryFunctionNodeBase" />
    [DebuggerDisplay("sqrt({" + nameof(Parameter) + "})")]
    [CallableMathematicsFunction(
        "sqrt",
        "squareroot")]
    [UsedImplicitly]
    internal sealed class FunctionNodeSquareRoot : NumericUnaryFunctionNodeBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FunctionNodeSquareRoot" /> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public FunctionNodeSquareRoot(NodeBase parameter)
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
                return new NumericNode(GlobalSystem.Math.Sqrt(numericParam.ExtractFloat()));
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
            new FunctionNodeSquareRoot(this.Parameter.DeepClone(context));

        protected override Expression GenerateExpressionInternal() =>
            this.GenerateStaticUnaryFunctionCall(
                typeof(GlobalSystem.Math),
                nameof(GlobalSystem.Math.Sqrt));

        /// <summary>
        ///     Generates the expression with tolerance that will be compiled into code.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The expression.</returns>
        protected override Expression GenerateExpressionInternal(Tolerance tolerance) =>
            this.GenerateStaticUnaryFunctionCall(
                typeof(GlobalSystem.Math),
                nameof(GlobalSystem.Math.Sqrt),
                tolerance);
    }
}