// <copyright file="ConstantNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using IX.Math.Values;
using IX.StandardExtensions.Contracts;
using JetBrains.Annotations;

namespace IX.Math.Nodes
{
    /// <summary>
    ///     A constant node.
    /// </summary>
    [PublicAPI]
    public sealed partial class ConstantNode : NodeBase
    {
#region Internal state

        private readonly Expression? binaryExpression;

        private readonly Expression? booleanExpression;
        private readonly Expression? integerExpression;
        private readonly Expression? numericExpression;
        private readonly Expression? stringExpression;
        private readonly SupportableValueType supportableValueTypes;
        private readonly ConvertibleValue value;

#endregion

#region Constructors and destructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConstantNode" /> class.
        /// </summary>
        /// <param name="originalValue">The original constant value.</param>
        public ConstantNode(bool originalValue)
            : this(new BooleanConvertibleValue(originalValue))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConstantNode" /> class.
        /// </summary>
        /// <param name="originalValue">The original constant value.</param>
        public ConstantNode(long originalValue)
            : this(new IntegerConvertibleValue(originalValue))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConstantNode" /> class.
        /// </summary>
        /// <param name="originalValue">The original constant value.</param>
        public ConstantNode(double originalValue)
            : this(new NumericConvertibleValue(originalValue))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConstantNode" /> class.
        /// </summary>
        /// <param name="originalValue">The original constant value.</param>
        public ConstantNode(byte[] originalValue)
            : this(new BinaryConvertibleValue(Requires.NotNull(originalValue, nameof(originalValue))))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConstantNode" /> class.
        /// </summary>
        /// <param name="originalValue">The original constant value.</param>
        public ConstantNode(string originalValue)
        : this(new StringConvertibleValue(Requires.NotNull(originalValue, nameof(originalValue))))
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConstantNode" /> class.
        /// </summary>
        /// <param name="originalValue">The original constant value.</param>
        [SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "This is how expressions work.")]
        [SuppressMessage(
            "ReSharper",
            "HeapView.BoxingAllocation",
            Justification = "This is how expressions work.")]
        public ConstantNode(ConvertibleValue originalValue)
        {
            Requires.NotNull(
                out this.value,
                originalValue,
                nameof(originalValue));

            if (this.value.HasBoolean)
            {
                this.supportableValueTypes |= SupportableValueType.Boolean;
                this.booleanExpression = Expression.Constant(
                    this.value.GetBoolean(),
                    typeof(bool));
            }

            if (this.value.HasInteger)
            {
                this.supportableValueTypes |= SupportableValueType.Integer;
                this.integerExpression = Expression.Constant(
                    this.value.GetInteger(),
                    typeof(long));
            }

            if (this.value.HasNumeric)
            {
                this.supportableValueTypes |= SupportableValueType.Numeric;
                this.numericExpression = Expression.Constant(
                    this.value.GetNumeric(),
                    typeof(double));
            }

            if (this.value.HasBinary)
            {
                this.supportableValueTypes |= SupportableValueType.ByteArray;
                this.binaryExpression = Expression.Constant(
                    this.value.GetBinary(),
                    typeof(byte[]));
            }

            if (this.value.HasString)
            {
                this.supportableValueTypes |= SupportableValueType.String;
                this.stringExpression = Expression.Constant(
                    this.value.GetNumeric(),
                    typeof(string));
            }
        }

        private ConstantNode(
            ConvertibleValue value,
            SupportableValueType supportableValueTypes,
            Expression? booleanExpression,
            Expression? integerExpression,
            Expression? numericExpression,
            Expression? binaryExpression,
            Expression? stringExpression)
        {
            this.value = value;
            this.supportableValueTypes = supportableValueTypes;
            this.booleanExpression = booleanExpression;
            this.binaryExpression = binaryExpression;
            this.integerExpression = integerExpression;
            this.numericExpression = numericExpression;
            this.stringExpression = stringExpression;
        }

#endregion

#region Properties and indexers

        /// <summary>
        /// Gets the convertible value of this constant node.
        /// </summary>
        public ConvertibleValue Value => this.value;

#endregion

#region Methods

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new ConstantNode(
                this.value,
                this.supportableValueTypes,
                this.booleanExpression,
                this.integerExpression,
                this.numericExpression,
                this.binaryExpression,
                this.stringExpression);

        /// <summary>
        /// Calculates all supportable value types, as a result of the node and all nodes above it.
        /// </summary>
        /// <param name="constraints">The constraints to place on the node's supportable types.</param>
        /// <returns>The resulting supportable value type.</returns>
        /// <exception cref="ExpressionNotValidLogicallyException">The expression is not valid, either structurally or given the constraints.</exception>
        public override SupportableValueType CalculateSupportableValueType(
            SupportableValueType constraints = SupportableValueType.All)
        {
            var result = constraints & this.supportableValueTypes;

            if (result == SupportableValueType.None)
            {
                throw new ExpressionNotValidLogicallyException();
            }

            return result;
        }

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
            forType switch
            {
                SupportedValueType.Boolean => this.booleanExpression ??
                                              throw new ExpressionNotValidLogicallyException(),
                SupportedValueType.ByteArray => this.binaryExpression ??
                                                throw new ExpressionNotValidLogicallyException(),
                SupportedValueType.Integer => this.integerExpression ??
                                              throw new ExpressionNotValidLogicallyException(),
                SupportedValueType.Numeric => this.numericExpression ??
                                              throw new ExpressionNotValidLogicallyException(),
                SupportedValueType.String => this.stringExpression ?? throw new ExpressionNotValidLogicallyException(),
                _ => throw new ExpressionNotValidLogicallyException()
            };

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>A simplified node, or this instance.</returns>
        public override NodeBase Simplify() => this;

#endregion
    }
}