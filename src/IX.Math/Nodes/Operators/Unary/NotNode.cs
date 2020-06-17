// <copyright file="NotNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Exceptions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Operators.Unary
{
    /// <summary>
    ///     A node for a binary negation operation.
    /// </summary>
    /// <seealso cref="UnaryOperatorNodeBase" />
    [DebuggerDisplay("!{" + nameof(Operand) + "}")]
    internal sealed class NotNode : UnaryOperatorNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NotNode" /> class.
        /// </summary>
        /// <param name="stringFormatters">The string formatters.</param>
        /// <param name="operand">The operand.</param>
        public NotNode(
            List<IStringFormatter> stringFormatters,
            [NotNull] NodeBase operand)
            : base(
                stringFormatters,
                operand)
        {
        }

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>
        ///     A simplified node, or this instance.
        /// </returns>
        public override NodeBase Simplify() =>
            this.Operand switch
            {
                IntegerNode integerNode => this.GenerateConstantInteger(~integerNode.Value),
                BoolNode boolNode => this.GenerateConstantBoolean(!boolNode.Value),
                _ => this
            };

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) => new NotNode(
            this.StringFormatters,
            this.Operand.DeepClone(context));

        /// <summary>
        ///     Ensures that the operands are compatible, and refines the return type of this expression based on them.
        /// </summary>
        /// <param name="operand">The operand.</param>
        protected override void EnsureCompatibleOperandsAndRefineReturnType(NodeBase operand)
        {
            this.CalculatedCosts.Clear();

            var supportedType = operand.VerifyPossibleType(SupportableValueType.Boolean | SupportableValueType.Integer);

            if (supportedType == SupportableValueType.Boolean)
            {
                int cost = operand.CalculateStrategyCost(SupportedValueType.Boolean);
                this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Boolean);

                foreach (var svt in GetSupportedTypeOptions(this.PossibleReturnType))
                {
                    this.CalculatedCosts[svt] = (GetStandardConversionStrategyCost(
                                                     SupportedValueType.Boolean,
                                                     in svt) +
                                                 cost, SupportedValueType.Boolean);
                }
            }
            else if (supportedType == SupportableValueType.Integer)
            {
                int cost = operand.CalculateStrategyCost(SupportedValueType.Integer);
                this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Integer);

                foreach (var svt in GetSupportedTypeOptions(this.PossibleReturnType))
                {
                    this.CalculatedCosts[svt] = (GetStandardConversionStrategyCost(
                                                     SupportedValueType.Integer,
                                                     in svt) +
                                                 cost, SupportedValueType.Integer);
                }
            }
            else if (supportedType == (SupportableValueType.Boolean | SupportableValueType.Integer))
            {
                int boolCost = operand.CalculateStrategyCost(SupportedValueType.Boolean);
                int intCost = operand.CalculateStrategyCost(SupportedValueType.Boolean);
                this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Boolean) |
                                          GetSupportableConversions(SupportedValueType.Integer);

                foreach (var svt in GetSupportedTypeOptions(this.PossibleReturnType))
                {
                    int totalBoolCost = GetStandardConversionStrategyCost(
                        SupportedValueType.Boolean,
                        in svt);
                    if (totalBoolCost != int.MaxValue)
                    {
                        totalBoolCost += boolCost;
                    }

                    int totalIntCost = GetStandardConversionStrategyCost(
                        SupportedValueType.Integer,
                        in svt);
                    if (totalIntCost != int.MaxValue)
                    {
                        totalIntCost += intCost;
                    }

                    if (totalIntCost <= totalBoolCost)
                    {
                        this.CalculatedCosts[svt] = (totalIntCost, SupportedValueType.Integer);
                    }
                    else
                    {
                        this.CalculatedCosts[svt] = (totalBoolCost, SupportedValueType.Boolean);
                    }
                }
            }
            else
            {
                throw new MathematicsEngineException();
            }
        }

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
            if (!this.CalculatedCosts.TryGetValue(
                valueType,
                out var tuple))
            {
                throw new ExpressionNotValidLogicallyException();
            }

            Expression operandExpression = this.Operand.GenerateExpression(
                in tuple.InternalType,
                in comparisonTolerance);

            return Expression.Not(operandExpression);
        }
    }
}