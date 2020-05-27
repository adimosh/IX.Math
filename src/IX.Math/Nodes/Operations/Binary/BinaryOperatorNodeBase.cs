// <copyright file="BinaryOperatorNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using IX.Math.Extensibility;
using IX.StandardExtensions.Contracts;

namespace IX.Math.Nodes.Operations.Binary
{
    /// <summary>
    ///     A node base for binary operations.
    /// </summary>
    /// <seealso cref="OperationNodeBase" />
    internal abstract class BinaryOperatorNodeBase : OperationNodeBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="BinaryOperatorNodeBase" /> class.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        [SuppressMessage(
            "ReSharper",
            "VirtualMemberCallInConstructor",
            Justification = "We specifically want this to happen.")]
        [SuppressMessage(
            "Usage",
            "CA2214:Do not call overridable methods in constructors",
            Justification = "We specifically want this to happen.")]
        protected private BinaryOperatorNodeBase(
            NodeBase left,
            NodeBase right)
        {
            Requires.NotNull(
                left,
                nameof(left));
            Requires.NotNull(
                right,
                nameof(right));

            this.EnsureCompatibleOperands(
                left,
                right);

            this.Left = left.Simplify();
            this.Right = right.Simplify();
        }

        /// <summary>
        ///     Gets a value indicating whether this node supports tolerance.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is tolerant; otherwise, <c>false</c>.
        /// </value>
        public override bool IsTolerant => this.Left.IsTolerant || this.Right.IsTolerant;

        /// <summary>
        ///     Gets the left operand.
        /// </summary>
        /// <value>
        ///     The left operand.
        /// </value>
        protected NodeBase Left { get; }

        /// <summary>
        ///     Gets the right operand.
        /// </summary>
        /// <value>
        ///     The right operand.
        /// </value>
        protected NodeBase Right { get; }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public new abstract NodeBase DeepClone(NodeCloningContext context);

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        protected override OperationNodeBase DeepCloneNode(NodeCloningContext context) =>
            (OperationNodeBase)this.DeepClone(context);

        /// <summary>
        ///     Ensures that the operands are compatible.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        protected abstract void EnsureCompatibleOperands(
            NodeBase left,
            NodeBase right);

        /// <summary>
        ///     Sets the special object request function for sub objects.
        /// </summary>
        /// <param name="func">The function.</param>
        protected override void SetSpecialObjectRequestFunctionForSubObjects(Func<Type, object> func)
        {
            if (this.Left is ISpecialRequestNode srnl)
            {
                srnl.SetRequestSpecialObjectFunction(func);
            }

            if (this.Right is ISpecialRequestNode srnr)
            {
                srnr.SetRequestSpecialObjectFunction(func);
            }
        }

        /// <summary>
        ///     Gets the expressions of same type from operands.
        /// </summary>
        /// <returns>The left and right operand expressions.</returns>
        protected (Expression Left, Expression Right) GetExpressionsOfSameTypeFromOperands()
        {
            if (this.Left.ReturnType == SupportedValueType.String || this.Right.ReturnType == SupportedValueType.String)
            {
                return (this.Left.GenerateStringExpression(),
                    this.Right.GenerateStringExpression());
            }

            Expression le = this.Left.GenerateExpression();
            Expression re = this.Right.GenerateExpression();

            if (le.Type == typeof(double) && re.Type == typeof(long))
            {
                return (le,
                    Expression.Convert(
                        re,
                        typeof(double)));
            }

            if (le.Type == typeof(long) && re.Type == typeof(double))
            {
                return (Expression.Convert(
                        le,
                        typeof(double)),
                    re);
            }

            return (le, re);
        }

        /// <summary>
        ///     Gets the expressions of same type from operands.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The left and right operand expressions.</returns>
        protected (Expression Left, Expression Right) GetExpressionsOfSameTypeFromOperands(in ComparisonTolerance tolerance)
        {
            if (this.Left.ReturnType == SupportedValueType.String || this.Right.ReturnType == SupportedValueType.String)
            {
                return (this.Left.GenerateStringExpression(in tolerance),
                    this.Right.GenerateStringExpression(in tolerance));
            }

            Expression le = this.Left.GenerateExpression(in tolerance);
            Expression re = this.Right.GenerateExpression(in tolerance);

            if (le.Type == typeof(double) && re.Type == typeof(long))
            {
                return (le,
                    Expression.Convert(
                        re,
                        typeof(double)));
            }

            if (le.Type == typeof(long) && re.Type == typeof(double))
            {
                return (Expression.Convert(
                        le,
                        typeof(double)),
                    re);
            }

            return (le, re);
        }
    }
}