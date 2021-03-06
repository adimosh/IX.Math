// <copyright file="BinaryNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using IX.Math.Conversion;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Constants
{
    /// <summary>
    ///     A binary value node. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="ConstantNodeBase" />
    [DebuggerDisplay("{" + nameof(Value) + "}")]
    [PublicAPI]
    public sealed class BinaryNode : ConstantNodeBase<byte[]>
    {
#region Internal state

        private long? possibleInteger;

        private double? possibleNumeric;

        private SupportableValueType supportableType;

#endregion

#region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="BinaryNode" /> class.
        /// </summary>
        /// <param name="value">The node's binary value, as an array of bytes.</param>
        public BinaryNode(byte[] value)
            : base(value)
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
            new BinaryNode(this.Value);

        /// <summary>
        ///     Tries to get a byte array value out of this constant node.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the constant can safely be converted to a byte array, <c>false</c> otherwise.</returns>
        public override bool TryGetBinary(out byte[] value)
        {
            value = this.Value;

            return true;
        }

        /// <summary>
        ///     Tries to get an integer value out of this constant node.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the constant can safely be converted to an integer, <c>false</c> otherwise.</returns>
        public override bool TryGetInteger(out long value)
        {
            if (this.possibleInteger.HasValue)
            {
                value = this.possibleInteger.Value;

                return true;
            }

            value = default;

            return false;
        }

        /// <summary>
        ///     Tries to get a numeric value out of this constant node.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the constant can safely be converted to a numeric value, <c>false</c> otherwise.</returns>
        public override bool TryGetNumeric(out double value)
        {
            if (this.possibleNumeric.HasValue)
            {
                value = this.possibleNumeric.Value;

                return true;
            }

            value = default;

            return false;
        }

        /// <summary>
        ///     Gets the supported types.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        ///     The types supported by this constant.
        /// </returns>
        protected override SupportableValueType GetSupportedTypes(byte[] value)
        {
            this.supportableType = SupportableValueType.Binary | SupportableValueType.String;

            if (value.Length <= 8)
            {
                if (value.Length < 8)
                {
                    byte[] bytes = new byte[8];
                    Array.Copy(
                        value,
                        bytes,
                        value.Length);
                    value = bytes;
                }

                // Integer-compatible
                _ = InternalTypeDirectConversions.ToInteger(
                    value,
                    out var i);
                this.possibleInteger = i;
                this.supportableType |= SupportableValueType.Integer;

                // Numeric-compatible
                _ = InternalTypeDirectConversions.ToNumeric(
                    value,
                    out var n);
                this.possibleNumeric = n;
                this.supportableType |= SupportableValueType.Numeric;
            }

            return this.supportableType;
        }

#endregion
    }
}