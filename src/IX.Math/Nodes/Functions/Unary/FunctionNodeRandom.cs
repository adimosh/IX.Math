// <copyright file="FunctionNodeRandom.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using IX.Math.Extensibility;
using IX.Math.Generators;
using JetBrains.Annotations;
using GlobalSystem = System;

namespace IX.Math.Nodes.Functions.Unary
{
    /// <summary>
    ///     A node representing the <see cref="GlobalSystem.Random.Next(int)" /> function.
    /// </summary>
    /// <seealso cref="NumericUnaryFunctionNodeBase" />
    [DebuggerDisplay("random({" + nameof(Parameter) + "})")]
    [CallableMathematicsFunction(
        "rand",
        "random")]
    [UsedImplicitly]
    internal sealed class FunctionNodeRandom : NumericUnaryFunctionNodeBase
    {
#region Internal state

        private static readonly GlobalSystem.Func<double, double> CachedRepresentedFunction =
            RandomNumberGenerator.Generate;

#endregion

#region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FunctionNodeRandom" /> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public FunctionNodeRandom(NodeBase parameter)
            : base(parameter)
        {
        }

#endregion

#region Properties and indexers

        /// <summary>
        ///     Gets the function represented by this node.
        /// </summary>
        /// <value>
        ///     The represented function.
        /// </value>
        protected override GlobalSystem.Func<double, double> RepresentedFunction =>
            CachedRepresentedFunction;

#endregion

#region Methods

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>
        ///     A deep clone.
        /// </returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new FunctionNodeRandom(this.Parameter.DeepClone(context));

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>
        ///     A simplified node, or this instance.
        /// </returns>
        public override NodeBase Simplify() =>
            this;

#endregion
    }
}