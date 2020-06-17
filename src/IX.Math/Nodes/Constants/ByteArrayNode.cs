// <copyright file="ByteArrayNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using IX.Math.Extensibility;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Constants
{
    /// <summary>
    /// A binary value node. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="ConstantNodeBase" />
    [DebuggerDisplay("{" + nameof(Value) + "}")]
    [PublicAPI]
    public sealed class ByteArrayNode : ConstantNodeBase<byte[]>
    {
        private long? possibleInteger;

        private double? possibleNumeric;

        private SupportableValueType supportableType;

        /// <summary>
        /// Initializes a new instance of the <see cref="ByteArrayNode" /> class.
        /// </summary>
        /// <param name="stringFormatters">The string formatters.</param>
        /// <param name="value">The node's boolean value.</param>
        internal ByteArrayNode(List<IStringFormatter> stringFormatters, byte[] value)
            : base(stringFormatters, value)
        {
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new ByteArrayNode(
                this.StringFormatters,
                this.Value);

        /// <summary>
        /// Tries to get a byte array value out of this constant node.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the constant can safely be converted to a byte array, <c>false</c> otherwise.</returns>
        public override bool TryGetByteArray(out byte[] value)
        {
            value = this.Value;
            return true;
        }

        /// <summary>
        /// Tries to get an integer value out of this constant node.
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
        /// Tries to get a numeric value out of this constant node.
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
        /// Gets the supported types.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The types supported by this constant.
        /// </returns>
        protected override SupportableValueType GetSupportedTypes(byte[] value)
        {
            this.supportableType = SupportableValueType.ByteArray | SupportableValueType.String;

            if (value.Length <= 8)
            {
                if (value.Length < 8)
                {
                    byte[] bytes = new byte[8];
                    Array.Copy(value, bytes, value.Length);
                    value = bytes;
                }

                // Integer-compatible
                this.possibleInteger = BitConverter.ToInt64(
                    value,
                    0);
                this.supportableType |= SupportableValueType.Integer;

                // Numeric-compatible
                this.possibleNumeric = BitConverter.ToDouble(
                    value,
                    0);
                this.supportableType |= SupportableValueType.Numeric;
            }

            return this.supportableType;
        }
    }
}