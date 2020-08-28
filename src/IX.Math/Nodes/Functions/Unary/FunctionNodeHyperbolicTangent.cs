// <copyright file="FunctionNodeHyperbolicTangent.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using IX.Math.Extensibility;
using JetBrains.Annotations;
using GlobalSystem = System;

namespace IX.Math.Nodes.Functions.Unary
{
    /// <summary>
    ///     A node representing the <see cref="GlobalSystem.Math.Tanh(double)" /> function.
    /// </summary>
    /// <seealso cref="NumericUnaryFunctionNodeBase" />
    [DebuggerDisplay("tanh({" + nameof(Parameter) + "})")]
    [CallableMathematicsFunction("tanh")]
    [UsedImplicitly]
    internal sealed class FunctionNodeHyperbolicTangent : NumericUnaryFunctionNodeBase
    {
        private static readonly GlobalSystem.Func<double, double> CachedRepresentedFunction = GlobalSystem.Math.Tanh;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionNodeHyperbolicTangent" /> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public FunctionNodeHyperbolicTangent(
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
            new FunctionNodeHyperbolicTangent(
                this.Parameter.DeepClone(context));
    }
}