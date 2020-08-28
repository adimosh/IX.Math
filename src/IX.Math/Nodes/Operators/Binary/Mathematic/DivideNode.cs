// <copyright file="DivideNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;

namespace IX.Math.Nodes.Operators.Binary.Mathematic
{
    /// <summary>
    ///     A node for a division operation.
    /// </summary>
    /// <seealso cref="SimpleMathematicalOperationNodeBase" />
    [DebuggerDisplay("{" + nameof(Left) + "} / {" + nameof(Right) + "}")]
    internal sealed class DivideNode : SimpleNumericOnlyMathematicalOperationNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IX.Math.Nodes.Operators.Binary.Mathematic.DivideNode" /> class.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public DivideNode(
            NodeBase left,
            NodeBase right)
            : base(
                left,
                right)
        {
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new DivideNode(
                this.Left.DeepClone(context),
                this.Right.DeepClone(context));

        /// <summary>
        /// Calculates the constant value.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>Either a long or a double, depending on the circumstances.</returns>
        protected override double CalculateConstantValue(
            double left,
            double right) =>
            left / right;

        /// <summary>
        /// Generates the expression.
        /// </summary>
        /// <param name="leftOperandExpression">The left operand expression.</param>
        /// <param name="rightOperandExpression">The right operand expression.</param>
        /// <returns>The resulting expression.</returns>
        protected override Expression GenerateExpression(
            Expression leftOperandExpression,
            Expression rightOperandExpression) =>
            Expression.Divide(
                leftOperandExpression,
                rightOperandExpression);
    }
}