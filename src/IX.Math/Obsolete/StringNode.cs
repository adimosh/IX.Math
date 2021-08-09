// <copyright file="StringNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace IX.Math.Nodes.Constants
{
    /// <summary>
    /// A string node. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="ConstantNodeBase" />
    [DebuggerDisplay("{" + nameof(Value) + "}")]
    [PublicAPI]
    [Obsolete("This type of node is not going to be used anymore.")]
    public sealed class StringNode : ConstantNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StringNode"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public StringNode(string value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public string Value { get; }

        /// <summary>
        /// Gets the return type of this node.
        /// </summary>
        /// <value>Always <see cref="SupportedValueType.String"/>.</value>
        [Obsolete]
        public override SupportedValueType ReturnType => SupportedValueType.String;

        /// <summary>
        /// Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>The expression.</returns>
        public override Expression GenerateCachedExpression() => Expression.Constant(this.Value, typeof(string));

        /// <summary>
        /// Generates the expression that will be compiled into code as a string expression.
        /// </summary>
        /// <returns>The string expression.</returns>
        public override Expression GenerateCachedStringExpression() => this.GenerateExpression();

        /// <summary>
        /// Distills the value into a usable constant.
        /// </summary>
        /// <returns>A usable constant.</returns>
        public override object DistillValue() => this.Value;

        /// <summary>
        /// Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) => new StringNode(this.Value);

        /// <summary>
        /// x.
        /// </summary>
        /// <returns>y.</returns>
        protected override SupportableValueType ResetDetermination() => SupportableValueType.String;
    }
}