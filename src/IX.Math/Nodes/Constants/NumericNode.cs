// <copyright file="NumericNode.cs" company="Adrian Mos">
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
    /// A floating point value node. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="ConstantNodeBase" />
    [DebuggerDisplay("{" + nameof(Value) + "}")]
    [PublicAPI]
    public sealed class NumericNode : ConstantNodeBase<double>
    {
        private byte[] binaryRepresentation;

        private SupportableValueType supportableTypes;

        private long? possibleInteger;

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericNode" /> class.
        /// </summary>
        /// <param name="stringFormatters">The string formatters.</param>
        /// <param name="value">The node's boolean value.</param>
        internal NumericNode(List<IStringFormatter> stringFormatters, double value)
            : base(stringFormatters, value)
        {
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new NumericNode(
                this.StringFormatters,
                this.Value);

        /// <summary>
        /// Tries to get a numeric value out of this constant node.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the constant can safely be converted to a numeric value, <c>false</c> otherwise.</returns>
        public override bool TryGetNumeric(out double value)
        {
            value = this.Value;
            return true;
        }

        /// <summary>
        /// Tries to get a byte array value out of this constant node.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the constant can safely be converted to a byte array, <c>false</c> otherwise.</returns>
        public override bool TryGetByteArray(out byte[] value)
        {
            value = this.binaryRepresentation;
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
        /// Gets the supported types.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>
        /// The types supported by this constant.
        /// </returns>
        protected override SupportableValueType GetSupportedTypes(double value)
        {
            this.supportableTypes = SupportableValueType.Numeric |
                                    SupportableValueType.ByteArray |
                                    SupportableValueType.String;

            this.binaryRepresentation = BitConverter.GetBytes(value);

            if (value <= long.MaxValue && value >= long.MinValue)
            {
                var numericAbs = global::System.Math.Abs(value);
                if (numericAbs - global::System.Math.Floor(numericAbs) < double.Epsilon)
                {
                    this.supportableTypes |= SupportableValueType.Integer;
                    this.possibleInteger = Convert.ToInt64(value);
                }
            }

            return this.supportableTypes;
        }
    }
}