// <copyright file="NotOperator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;
using IX.Math.Values;

namespace IX.Math.Nodes.Operators.Unary
{
    /// <summary>
    /// A negation unary operator.
    /// </summary>
    internal class NotOperator : UnaryOperatorNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotOperator"/> class.
        /// </summary>
        /// <param name="operand">The operand.</param>
        internal NotOperator(NodeBase operand)
            : base(operand) { }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new NotOperator(this.Operand.DeepClone(context));

        /// <summary>
        /// Calculates all supportable value types, as a result of the node and all nodes above it.
        /// </summary>
        /// <param name="constraints">The constraints to place on the node's supportable types.</param>
        /// <returns>The resulting supportable value type.</returns>
        /// <exception cref="ExpressionNotValidLogicallyException">The expression is not valid, either structurally or given the constraints.</exception>
        public override SupportableValueType CalculateSupportableValueType(
            SupportableValueType constraints = SupportableValueType.All)
        {
            if (this.Operand.CalculateSupportableValueType(
                    SupportableValueType.Boolean | SupportableValueType.Integer) ==
                SupportableValueType.None)
            {
                return SupportableValueType.None;
            }

            return constraints & (SupportableValueType.Integer | SupportableValueType.Boolean);
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
            Tolerance? tolerance = null)
        {
            Expression operandExpression = forType switch
            {
                SupportedValueType.Integer => this.Operand.GenerateExpression(
                    SupportedValueType.Integer,
                    tolerance),
                SupportedValueType.Boolean => this.Operand.GenerateExpression(
                    SupportedValueType.Boolean,
                    tolerance),
                _ => throw new ExpressionNotValidLogicallyException()
            };

            return Expression.Not(operandExpression);
        }

        /// <summary>
        ///     Simplifies this node, if possible, based on a constant operand value.
        /// </summary>
        /// <returns>A simplified node, or this instance.</returns>
        private protected override NodeBase SimplifyOnConvertibleValue(ConvertibleValue value)
        {
            if (value.HasInteger)
            {
                return new ConstantNode(~value.GetInteger());
            }

            if (value.HasBoolean)
            {
                return new ConstantNode(!value.GetBoolean());
            }

            throw new ExpressionNotValidLogicallyException();
        }
    }
}