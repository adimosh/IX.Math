// <copyright file="FunctionNodeCeiling.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics;
using IX.Math.Extensibility;
using JetBrains.Annotations;
using GlobalSystem = System;

namespace IX.Math.Nodes.Functions.Unary
{
    /// <summary>
    ///     A node representing the <see cref="GlobalSystem.Math.Ceiling(double)" /> function.
    /// </summary>
    /// <seealso cref="NumericRoundingUnaryFunctionNodeBase" />
    [DebuggerDisplay("ceil({" + nameof(Parameter) + "})")]
    [CallableMathematicsFunction(
        "ceil",
        "ceiling")]
    [UsedImplicitly]
    internal sealed class FunctionNodeCeiling : NumericUnaryFunctionNodeBase
    {
        private static readonly GlobalSystem.Func<double, double> CachedRepresentedFunction = GlobalSystem.Math.Ceiling;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionNodeCeiling" /> class.
        /// </summary>
        /// <param name="stringFormatters">The string formatters.</param>
        /// <param name="parameter">The parameter.</param>
        public FunctionNodeCeiling(
            List<IStringFormatter> stringFormatters,
            NodeBase parameter)
            : base(
                stringFormatters,
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
            new FunctionNodeCeiling(
                this.StringFormatters,
                this.Parameter.DeepClone(context));
    }
}