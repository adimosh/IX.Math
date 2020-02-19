// <copyright file="BinaryOperationNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using IX.StandardExtensions.Contracts;

namespace IX.Math.Nodes.Operations.Binary
{
    /// <summary>
    /// A node base for binary operations.
    /// </summary>
    /// <seealso cref="OperationNodeBase" />
    internal abstract class BinaryOperationNodeBase : OperationNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryOperationNodeBase"/> class.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <exception cref="ArgumentNullException">
        /// left
        /// or
        /// right
        /// is <c>null</c> (<c>Nothing</c> in Visual Basic).
        /// </exception>
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "We specifically want this to happen.")]
        [SuppressMessage("Usage", "CA2214:Do not call overridable methods in constructors", Justification = "We specifically want this to happen.")]
        protected BinaryOperationNodeBase(NodeBase left, NodeBase right)
        {
            Contract.RequiresNotNull(
                in left,
                nameof(left));
            Contract.RequiresNotNull(
                in right,
                nameof(right));

            this.EnsureCompatibleOperands(left, right);

            this.Left = left.Simplify();
            this.Right = right.Simplify();
        }

        /// <summary>
        /// Gets the left operand.
        /// </summary>
        /// <value>
        /// The left operand.
        /// </value>
        protected NodeBase Left { get; }

        /// <summary>
        /// Gets the right operand.
        /// </summary>
        /// <value>
        /// The right operand.
        /// </value>
        protected NodeBase Right { get; }

        /// <summary>
        /// Ensures that the operands are compatible.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        protected abstract void EnsureCompatibleOperands(NodeBase left, NodeBase right);

        /// <summary>
        /// Gets the expressions of same type from operands.
        /// </summary>
        /// <returns>The left and right operand expressions.</returns>
        protected Tuple<Expression, Expression> GetExpressionsOfSameTypeFromOperands()
        {
            if (this.Left.ReturnType == SupportedValueType.String || this.Right.ReturnType == SupportedValueType.String)
            {
                return new Tuple<Expression, Expression>(this.Left.GenerateStringExpression(), this.Right.GenerateStringExpression());
            }

            Expression le = this.Left.GenerateExpression();
            Expression re = this.Right.GenerateExpression();

            if (le.Type == typeof(double) && re.Type == typeof(long))
            {
                return new Tuple<Expression, Expression>(le, Expression.Convert(re, typeof(double)));
            }

            if (le.Type == typeof(long) && re.Type == typeof(double))
            {
                return new Tuple<Expression, Expression>(Expression.Convert(le, typeof(double)), re);
            }

            return new Tuple<Expression, Expression>(le, re);
        }

        /// <summary>
        /// Gets the expressions of same type from operands.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The left and right operand expressions.</returns>
        protected Tuple<Expression, Expression> GetExpressionsOfSameTypeFromOperands(Tolerance tolerance)
        {
            if (this.Left.ReturnType == SupportedValueType.String || this.Right.ReturnType == SupportedValueType.String)
            {
                return new Tuple<Expression, Expression>(this.Left.GenerateStringExpression(), this.Right.GenerateStringExpression());
            }

            Expression le = this.Left.GenerateExpression(tolerance);
            Expression re = this.Right.GenerateExpression(tolerance);

            if (le.Type == typeof(double) && re.Type == typeof(long))
            {
                return new Tuple<Expression, Expression>(le, Expression.Convert(re, typeof(double)));
            }

            if (le.Type == typeof(long) && re.Type == typeof(double))
            {
                return new Tuple<Expression, Expression>(Expression.Convert(le, typeof(double)), re);
            }

            return new Tuple<Expression, Expression>(le, re);
        }
    }
}