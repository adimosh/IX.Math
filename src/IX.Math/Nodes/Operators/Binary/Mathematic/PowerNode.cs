// <copyright file="PowerNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Extensibility;

namespace IX.Math.Nodes.Operators.Binary.Mathematic
{
    /// <summary>
    ///     A node for a power operation.
    /// </summary>
    /// <seealso cref="SimpleMathematicalOperationNodeBase" />
    [DebuggerDisplay("{" + nameof(Left) + "} # {" + nameof(Right) + "}")]
    internal sealed class PowerNode : SimpleNumericOnlyMathematicalOperationNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IX.Math.Nodes.Operators.Binary.Mathematic.PowerNode" /> class.
        /// </summary>
        /// <param name="stringFormatters">The string formatters.</param>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public PowerNode(
            List<IStringFormatter> stringFormatters,
            NodeBase left,
            NodeBase right)
            : base(
                stringFormatters,
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
            new PowerNode(
                this.StringFormatters,
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
            global::System.Math.Pow(
                left,
                right);

        /// <summary>
        /// Generates the expression.
        /// </summary>
        /// <param name="leftOperandExpression">The left operand expression.</param>
        /// <param name="rightOperandExpression">The right operand expression.</param>
        /// <returns>The resulting expression.</returns>
        protected override Expression GenerateExpression(
            Expression leftOperandExpression,
            Expression rightOperandExpression) =>
            Expression.Power(
                leftOperandExpression,
                rightOperandExpression);
    }
}