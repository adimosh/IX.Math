// <copyright file="FunctionNodeTruncate.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using IX.Math.Extensibility;
using JetBrains.Annotations;
using GlobalSystem = System;

namespace IX.Math.Nodes.Functions.Unary
{
    /// <summary>
    ///     A node representing the <see cref="GlobalSystem.Math.Truncate(double)" /> function.
    /// </summary>
    /// <seealso cref="NumericUnaryFunctionNodeBase" />
    [DebuggerDisplay("trun({" + nameof(Parameter) + "})")]
    [CallableMathematicsFunction(
        "trun",
        "truncate")]
    [UsedImplicitly]
    internal sealed class FunctionNodeTruncate : NumericUnaryFunctionNodeBase
    {
#region Internal state

        private static readonly GlobalSystem.Func<double, double>
            CachedRepresentedFunction = GlobalSystem.Math.Truncate;

#endregion

#region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FunctionNodeTruncate" /> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public FunctionNodeTruncate(NodeBase parameter)
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
            new FunctionNodeTruncate(this.Parameter.DeepClone(context));

#endregion
    }
}