// <copyright file="ByteArrayNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Formatters;
using IX.StandardExtensions.Contracts;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Constants
{
    /// <summary>
    ///     A binary value node. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="ConstantNodeBase" />
    [DebuggerDisplay("{" + nameof(DisplayValue) + "}")]
    [PublicAPI]
    public class ByteArrayNode : ConstantNodeBase
    {
        private string? cachedDistilledStringValue;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ByteArrayNode" /> class.
        /// </summary>
        /// <param name="value">The value of the constant.</param>
        public ByteArrayNode(byte[] value)
        {
            this.Value = Requires.NotNull(value, nameof(value));
        }

        /// <summary>
        ///     Gets the display value.
        /// </summary>
        public string DisplayValue => this.GetString();

        /// <summary>
        ///     Gets the return type of this node.
        /// </summary>
        /// <value>Always <see cref="SupportedValueType.ByteArray" />.</value>
        public override SupportedValueType ReturnType => SupportedValueType.ByteArray;

        /// <summary>
        ///     Gets the value of the node.
        /// </summary>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance",
            "CA1819:Properties should not return arrays",
            Justification = "We specifically want this here, as this is a binary representation.")]
        public byte[] Value { get; }

        /// <summary>
        ///     Distills the value into a usable constant.
        /// </summary>
        /// <returns>A usable constant.</returns>
        public override object DistillValue() => this.Value;

        /// <summary>
        ///     Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>A <see cref="ConstantExpression" /> with a boolean value.</returns>
        public override Expression GenerateCachedExpression() =>
            Expression.Constant(
                this.Value,
                typeof(byte[]));

        /// <summary>
        ///     Generates the expression that will be compiled into code as a string expression.
        /// </summary>
        /// <returns>The string expression.</returns>
        public override Expression GenerateCachedStringExpression() => Expression.Constant(
                this.GetString(),
                typeof(string));

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) => new ByteArrayNode(this.Value);

        private string GetString() =>
            this.cachedDistilledStringValue ??= StringFormatter.FormatIntoString(this.Value);
    }
}