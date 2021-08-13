// <copyright file="BoolNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Formatters;
using IX.StandardExtensions;
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
        public SupportedValueType ReturnType => SupportedValueType.Boolean;

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
        public override Expression GenerateCachedExpression() =>
            Expression.Constant(
                this.Value,
                typeof(bool));
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
        public override SupportableValueType CalculateSupportableValueType(
            SupportableValueType constraints = SupportableValueType.All) =>
            throw new NotImplementedByDesignException();
    }
}