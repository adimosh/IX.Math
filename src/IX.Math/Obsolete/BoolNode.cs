// <copyright file="BoolNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Formatters;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace IX.Math.Nodes.Constants
{
    /// <summary>
    /// A boolean node. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="ConstantNodeBase" />
    [DebuggerDisplay("{" + nameof(Value) + "}")]
    [Obsolete("This type of node is not going to be used anymore.")]
    [PublicAPI]
    public sealed class BoolNode : ConstantNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BoolNode"/> class.
        /// </summary>
        /// <param name="value">The node's boolean value.</param>
        public BoolNode(bool value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets a value indicating this <see cref="BoolNode"/>'s value.
        /// </summary>
        /// <value>The node's value.</value>
#pragma warning disable SA1623 // Property summary documentation should match accessors
        public bool Value { get; private set; }
#pragma warning restore SA1623 // Property summary documentation should match accessors

        /// <summary>
        /// Gets the return type of this node.
        /// </summary>
        /// <value>Always <see cref="SupportedValueType.Boolean"/>.</value>
        [Obsolete]
        public override SupportedValueType ReturnType => SupportedValueType.Boolean;

#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation
        /// <summary>
        /// Distills the value into a usable constant.
        /// </summary>
        /// <returns>A usable constant.</returns>
        public override object DistillValue() => this.Value;

        /// <summary>
        /// Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>A <see cref="ConstantExpression"/> with a boolean value.</returns>
        public override Expression GenerateCachedExpression() => Expression.Constant(this.Value, typeof(bool));
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation

        /// <summary>
        /// Generates the expression that will be compiled into code as a string expression.
        /// </summary>
        /// <returns>The string expression.</returns>
        public override Expression GenerateCachedStringExpression() =>
            Expression.Constant(
                StringFormatter.FormatIntoString(this.Value),
                typeof(string));

        /// <summary>
        /// Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) => new BoolNode(this.Value);

        /// <summary>
        /// Resets the determination for supported types on this node, and, presumably, those above it in the tree.
        /// </summary>
        /// <returns>The most expanded supportable types for this node.</returns>
        protected override SupportableValueType ResetDetermination() => SupportableValueType.Boolean;
    }
}