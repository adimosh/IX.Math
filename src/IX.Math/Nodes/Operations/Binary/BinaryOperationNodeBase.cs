// <copyright file="BinaryOperationNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;

namespace IX.Math.Nodes.Operations.Binary
{
    /// <summary>
    /// A node base for binary operations.
    /// </summary>
    /// <seealso cref="IX.Math.Nodes.OperationNodeBase" />
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
        /// </exception>
        protected BinaryOperationNodeBase(NodeBase left, NodeBase right)
        {
            NodeBase leftTemp = left ?? throw new ArgumentNullException(nameof(left));
            NodeBase rightTemp = right ?? throw new ArgumentNullException(nameof(right));

            this.EnsureCompatibleOperands(leftTemp, rightTemp);

            this.Left = leftTemp.Simplify();
            this.Right = rightTemp.Simplify();
        }

        /// <summary>
        /// Gets or sets the left operand.
        /// </summary>
        /// <value>
        /// The left operand.
        /// </value>
        public NodeBase Left { get; protected set; }

        /// <summary>
        /// Gets or sets the right operand.
        /// </summary>
        /// <value>
        /// The right operand.
        /// </value>
        public NodeBase Right { get; protected set; }

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