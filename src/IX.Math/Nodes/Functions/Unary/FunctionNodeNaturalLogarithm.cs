// <copyright file="FunctionNodeNaturalLogarithm.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using IX.Math.Extensibility;
using JetBrains.Annotations;
using GlobalSystem = System;

namespace IX.Math.Nodes.Functions.Unary
{
    /// <summary>
    ///     A node representing the <see cref="GlobalSystem.Math.Log(double)" /> function, with e as base.
    /// </summary>
    /// <seealso cref="NumericUnaryFunctionNodeBase" />
    [DebuggerDisplay("ln({" + nameof(Parameter) + "})")]
    [CallableMathematicsFunction("ln")]
    [UsedImplicitly]
    internal sealed class FunctionNodeNaturalLogarithm : NumericUnaryFunctionNodeBase
    {
        private static readonly GlobalSystem.Func<double, double> CachedRepresentedFunction = GlobalSystem.Math.Log;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionNodeNaturalLogarithm" /> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public FunctionNodeNaturalLogarithm(
            NodeBase parameter)
            : base(
                parameter)
        {
        }

        /// <summary>
        /// Gets the function represented by this node.
        /// </summary>
        /// <value>
        /// The represented function.
        /// </value>
        protected override GlobalSystem.Func<double, double> RepresentedFunction => CachedRepresentedFunction;

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>
        ///     A deep clone.
        /// </returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new FunctionNodeNaturalLogarithm(
                this.Parameter.DeepClone(context));
    }
}