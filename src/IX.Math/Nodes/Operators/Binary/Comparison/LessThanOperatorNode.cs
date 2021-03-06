// <copyright file="LessThanOperatorNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;

namespace IX.Math.Nodes.Operators.Binary.Comparison
{
    [DebuggerDisplay("{" + nameof(Left) + "} < {" + nameof(Right) + "}")]
    internal sealed class LessThanOperatorNode : InequationOperatorNodeBase
    {
#region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="LessThanOperatorNode" /> class.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public LessThanOperatorNode(
            NodeBase left,
            NodeBase right)
            : base(
                left,
                right,
                false,
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
            new LessThanOperatorNode(
                this.Left.DeepClone(context),
                this.Right.DeepClone(context));

#endregion
    }
}