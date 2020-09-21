// <copyright file="IntegerNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Constants
{
    /// <summary>
    /// An integer node. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="ConstantNodeBase" />
    [DebuggerDisplay("{" + nameof(Value) + "}")]
    [PublicAPI]
    public sealed class IntegerNode : ConstantNodeBase<long>
    {
        private readonly double numericRepresentation;

        private readonly byte[] binaryRepresentation;

        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerNode" /> class.
        /// </summary>
        /// <param name="value">The node's boolean value.</param>
        public IntegerNode(long value)
            : base(value)
        {
            this.numericRepresentation = Convert.ToDouble(value);

            this.binaryRepresentation = BitConverter.GetBytes(value);
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) => new IntegerNode(this.Value);

        /// <summary>
        /// Tries to get an integer value out of this constant node.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the constant can safely be converted to an integer, <c>false</c> otherwise.</returns>
        public override bool TryGetInteger(out long value)
        {
            value = this.Value;
            return true;
        }

        /// <summary>
        /// Tries to get a numeric value out of this constant node.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the constant can safely be converted to a numeric value, <c>false</c> otherwise.</returns>
        public override bool TryGetNumeric(out double value)
        {
            value = this.numericRepresentation;
            return true;
        }

        /// <summary>
        /// Tries to get a byte array value out of this constant node.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the constant can safely be converted to a byte array, <c>false</c> otherwise.</returns>
        public override bool TryGetBinary(out byte[] value)
        {
            value = this.binaryRepresentation;
            return false;
        }
    }
}