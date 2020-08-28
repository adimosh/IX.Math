// <copyright file="SubtractNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Exceptions;
using IX.Math.Nodes.Constants;
using JetBrains.Annotations;
using DiagCA = System.Diagnostics.CodeAnalysis;

namespace IX.Math.Nodes.Operators.Unary
{
    /// <summary>
    ///     A node for negation operations.
    /// </summary>
    /// <seealso cref="UnaryOperatorNodeBase" />
    [DebuggerDisplay("-{" + nameof(Operand) + "}")]
    internal sealed class SubtractNode : UnaryOperatorNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubtractNode" /> class.
        /// </summary>
        /// <param name="operand">The operand.</param>
        public SubtractNode(
            [NotNull] NodeBase operand)
            : base(
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
                IntegerNode integerNode => GenerateConstantInteger(-integerNode.Value),
                NumericNode numericNode => GenerateConstantNumeric(-numericNode.Value),
                _ => this
            };

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new SubtractNode(this.Operand.DeepClone(context));

        /// <summary>
        ///     Ensures that the operands are compatible, and refines the return type of this expression based on them.
        /// </summary>
        /// <param name="operand">The operand.</param>
        protected override void EnsureCompatibleOperandsAndRefineReturnType(NodeBase operand)
        {
            this.CalculatedCosts.Clear();

            var supportedType = operand.VerifyPossibleType(SupportableValueType.Numeric | SupportableValueType.Integer);

            if (supportedType == SupportableValueType.Numeric)
            {
                int cost = operand.CalculateStrategyCost(SupportedValueType.Numeric);
                this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Numeric);

                foreach (var svt in GetSupportedTypeOptions(this.PossibleReturnType))
                {
                    this.CalculatedCosts[svt] = (GetStandardConversionStrategyCost(
                                                     SupportedValueType.Numeric,
                                                     in svt) +
                                                 cost, SupportedValueType.Numeric);
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
            else if (supportedType == (SupportableValueType.Numeric | SupportableValueType.Integer))
            {
                int boolCost = operand.CalculateStrategyCost(SupportedValueType.Numeric);
                int intCost = operand.CalculateStrategyCost(SupportedValueType.Integer);
                this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Numeric) |
                                          GetSupportableConversions(SupportedValueType.Integer);

                foreach (var svt in GetSupportedTypeOptions(this.PossibleReturnType))
                {
                    int totalBoolCost = GetStandardConversionStrategyCost(
                        SupportedValueType.Numeric,
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
                        this.CalculatedCosts[svt] = (totalBoolCost, SupportedValueType.Numeric);
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
        [DiagCA.SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "We want this to happen.")]
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

            return Expression.Subtract(
                Expression.Constant(
                    tuple.InternalType == SupportedValueType.Integer ? 0L : 0D,
                    tuple.InternalType == SupportedValueType.Integer ? typeof(long) : typeof(double)),
                operandExpression);
        }
    }
}