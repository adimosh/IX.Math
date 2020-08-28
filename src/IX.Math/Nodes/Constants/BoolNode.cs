// <copyright file="BoolNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Constants
{
    /// <summary>
    /// A boolean node. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="ConstantNodeBase" />
    [DebuggerDisplay("{" + nameof(Value) + "}")]
    [PublicAPI]
    public sealed class BoolNode : ConstantNodeBase<bool>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoolNode" /> class.
        /// </summary>
        /// <param name="value">The node's boolean value.</param>
        internal BoolNode(bool value)
            : base(value)
        {
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) => new BoolNode(this.Value);

        /// <summary>
        /// Tries to get a boolean value out of this constant node.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the constant can safely be converted to a boolean, <c>false</c> otherwise.</returns>
        public override bool TryGetBoolean(out bool value)
        {
            value = this.Value;
            return true;
        }
    }
}