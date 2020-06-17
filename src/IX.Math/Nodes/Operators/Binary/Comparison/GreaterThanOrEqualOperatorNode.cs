// <copyright file="GreaterThanOrEqualOperatorNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics;
using IX.Math.Extensibility;

namespace IX.Math.Nodes.Operators.Binary.Comparison
{
    [DebuggerDisplay("{" + nameof(Left) + "} >= {" + nameof(Right) + "}")]
    internal sealed class GreaterThanOrEqualOperatorNode : InequationOperatorNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GreaterThanOrEqualOperatorNode"/> class.
        /// </summary>
        /// <param name="stringFormatters">The string formatters.</param>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public GreaterThanOrEqualOperatorNode(
            List<IStringFormatter> stringFormatters,
            NodeBase left,
            NodeBase right)
            : base(
                stringFormatters,
                left,
                right,
                true,
                false)
        {
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new GreaterThanOrEqualOperatorNode(
                this.StringFormatters,
                this.Left.DeepClone(context),
                this.Right.DeepClone(context));
    }
}