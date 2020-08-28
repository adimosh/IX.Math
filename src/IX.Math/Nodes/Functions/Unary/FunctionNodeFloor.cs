// <copyright file="FunctionNodeFloor.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using IX.Math.Extensibility;
using JetBrains.Annotations;
using GlobalSystem = System;

namespace IX.Math.Nodes.Functions.Unary
{
    /// <summary>
    ///     A node representing the <see cref="GlobalSystem.Math.Floor(double)" /> function.
    /// </summary>
    /// <seealso cref="NumericRoundingUnaryFunctionNodeBase" />
    [DebuggerDisplay("floor({" + nameof(Parameter) + "})")]
    [CallableMathematicsFunction("floor")]
    [UsedImplicitly]
    internal class FunctionNodeFloor : NumericUnaryFunctionNodeBase
    {
        private static readonly GlobalSystem.Func<double, double> CachedRepresentedFunction = GlobalSystem.Math.Floor;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionNodeFloor" /> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public FunctionNodeFloor(
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
            new FunctionNodeFloor(
                this.Parameter.DeepClone(context));
    }
}