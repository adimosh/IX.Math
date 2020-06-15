// <copyright file="PowerNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Exceptions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;

namespace IX.Math.Nodes.Operators.Binary.Mathematic
{
    /// <summary>
    ///     A node for a power operation.
    /// </summary>
    /// <seealso cref="SimpleMathematicalOperationNodeBase" />
    [DebuggerDisplay("{" + nameof(Left) + "} ^ {" + nameof(Right) + "}")]
    internal sealed class PowerNode : SimpleMathematicalOperationNodeBase
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
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>
        ///     A simplified node, or this instance.
        /// </returns>
        public override NodeBase Simplify()
        {
            if (this.Left is NumericNode nnLeft && this.Right is NumericNode nnRight)
            {
                // Numeric
                return this.GenerateConstantNumeric(global::System.Math.Pow(nnLeft.Value, nnRight.Value));
            }

            if (this.Left is IntegerNode inLeft && this.Right is NumericNode niRight)
            {
                // Numeric
                return this.GenerateConstantNumeric(global::System.Math.Pow(inLeft.Value, niRight.Value));
            }

            if (this.Left is NumericNode niLeft && this.Right is IntegerNode inRight)
            {
                // Numeric
                return this.GenerateConstantNumeric(global::System.Math.Pow(niLeft.Value, inRight.Value));
            }

            if (this.Left is IntegerNode iiLeft && this.Right is IntegerNode iiRight)
            {
                // Integer
                return this.GenerateConstantNumeric(global::System.Math.Pow((double)iiLeft.Value, (double)iiRight.Value));
            }

            return this;
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
                return Expression.Power(
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