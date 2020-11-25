// <copyright file="LessThanOrEqualOperatorNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;

namespace IX.Math.Nodes.Operators.Binary.Comparison
{
    [DebuggerDisplay("{" + nameof(Left) + "} <= {" + nameof(Right) + "}")]
    internal sealed class LessThanOrEqualOperatorNode : InequationOperatorNodeBase
    {
#region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LessThanOrEqualOperatorNode" /> class.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public LessThanOrEqualOperatorNode(
            NodeBase left,
            NodeBase right)
            : base(
                left,
                right,
                true,
                true)
        {
        }

#endregion

#region Methods

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new LessThanOrEqualOperatorNode(
                this.Left.DeepClone(context),
                this.Right.DeepClone(context));

#endregion
    }
}