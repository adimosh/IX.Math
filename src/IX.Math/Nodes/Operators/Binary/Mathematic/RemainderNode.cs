// <copyright file="RemainderNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Exceptions;

namespace IX.Math.Nodes.Operators.Binary.Mathematic
{
    /// <summary>
    ///     A node for a remainder (modulo) operation.
    /// </summary>
    /// <seealso cref="SimpleMathematicalOperationNodeBase" />
    [DebuggerDisplay("{" + nameof(Left) + "} % {" + nameof(Right) + "}")]
    internal sealed class RemainderNode : SimpleMathematicalOperationNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RemainderNode" /> class.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public RemainderNode(
            NodeBase left,
            NodeBase right)
            : base(
                left,
                right)
        {
        }

        /// <summary>
        /// Calculates the constant value.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>Either a long or a double, depending on the circumstances.</returns>
        protected override (bool, long, double) CalculateConstantValue(
            long left,
            long right) =>
            (false, left % right, default);

        /// <summary>
        /// Calculates the constant value.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>Either a long or a double, depending on the circumstances.</returns>
        protected override (bool, long, double) CalculateConstantValue(
            double left,
            double right) =>
            (true, default, left % right);

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new RemainderNode(
                this.Left.DeepClone(context),
                this.Right.DeepClone(context));

        /// <summary>
        /// Generates the expression that this node represents.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="comparisonTolerance">The comparison tolerance.</param>
        /// <returns>A compiled expression, if one is possible.</returns>
        protected override Expression GenerateExpressionInternal(
            in SupportedValueType valueType,
            in ComparisonTolerance comparisonTolerance)
        {
            try
            {
                if (this.Left.CheckSupportedType(SupportableValueType.Integer) &&
                    this.Right.CheckSupportedType(SupportableValueType.Integer))
                {
                    return Expression.Modulo(
                        this.Left.GenerateExpression(
                            SupportedValueType.Integer,
                            in comparisonTolerance),
                        this.Right.GenerateExpression(
                            SupportedValueType.Integer,
                            in comparisonTolerance));
                }

                return Expression.Modulo(
                    this.Left.GenerateExpression(
                        SupportedValueType.Numeric,
                        in comparisonTolerance),
                    this.Right.GenerateExpression(
                        SupportedValueType.Numeric,
                        in comparisonTolerance));
            }
            catch (ExpressionNotValidLogicallyException)
            {
                throw;
            }
            catch (MathematicsEngineException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ExpressionNotValidLogicallyException(ex);
            }
        }
    }
}