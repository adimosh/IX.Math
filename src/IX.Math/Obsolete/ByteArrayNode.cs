// <copyright file="ByteArrayNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Formatters;
using IX.StandardExtensions;
using IX.StandardExtensions.Contracts;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace IX.Math.Nodes.Constants
{
    /// <summary>
    ///     A binary value node. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="ConstantNodeBase" />
    [DebuggerDisplay("{" + nameof(DisplayValue) + "}")]
    [PublicAPI]
    [Obsolete("This type of node is not going to be used anymore.")]
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
        [Obsolete]
        public SupportedValueType ReturnType => SupportedValueType.ByteArray;

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

        /// <summary>
        ///     Generates the expression that will be compiled into code.
        /// </summary>
        /// <param name="forType">What type the expression is generated for.</param>
        /// <param name="tolerance">The tolerance. If this parameter is <c>null</c> (<c>Nothing</c> in Visual Basic), then the operation is exact.</param>
        /// <returns>
        ///     The generated <see cref="Expression" />.
        /// </returns>
        public override Expression GenerateExpression(
            SupportedValueType forType,
            Tolerance? tolerance = null) =>
            throw new NotImplementedByDesignException();

        /// <summary>
        /// Calculates all supportable value types, as a result of the node and all nodes above it.
        /// </summary>
        /// <param name="constraints">The constraints to place on the node's supportable types.</param>
        /// <returns>The resulting supportable value type.</returns>
        /// <exception cref="ExpressionNotValidLogicallyException">The expression is not valid, either structurally or given the constraints.</exception>
        public override SupportableValueType CalculateSupportableValueType(SupportableValueType constraints = SupportableValueType.All) => throw new NotImplementedByDesignException();

        /// <summary>
        /// x.
        /// </summary>
        /// <returns>y.</returns>
        protected SupportableValueType ResetDetermination() => SupportableValueType.ByteArray;

        private string GetString() =>
            this.cachedDistilledStringValue ??= StringFormatter.FormatIntoString(this.Value);
    }
}