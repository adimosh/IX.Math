// <copyright file="FunctionNodeAbsolute.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Exceptions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using JetBrains.Annotations;
using GlobalSystem = System;

namespace IX.Math.Nodes.Functions.Unary
{
    /// <summary>
    ///     A node representing the <see cref="GlobalSystem.Math.Abs(double)" /> function.
    /// </summary>
    /// <seealso cref="NumericUnaryFunctionNodeBase" />
    [DebuggerDisplay("abs({" + nameof(Parameter) + "})")]
    [CallableMathematicsFunction(
        "abs",
        "absolute")]
    [UsedImplicitly]
    internal sealed class FunctionNodeAbsolute : UnaryFunctionNodeBase
    {
        private static readonly GlobalSystem.Func<long, long> FuncAbsLong =
            GlobalSystem.Math.Abs;

        private static readonly GlobalSystem.Func<double, double> FuncAbsDouble =
            GlobalSystem.Math.Abs;

        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionNodeAbsolute" /> class.
        /// </summary>
        /// <param name="stringFormatters">The string formatters.</param>
        /// <param name="parameter">The parameter.</param>
        public FunctionNodeAbsolute(
            List<IStringFormatter> stringFormatters,
            NodeBase parameter)
            : base(
                stringFormatters,
                parameter)
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
            if (this.Parameter is ConstantNodeBase c)
            {
                if (c.TryGetInteger(out var i))
                {
                    return this.GenerateConstantInteger(GlobalSystem.Math.Abs(i));
                }

                if (c.TryGetNumeric(out var n))
                {
                    return this.GenerateConstantNumeric(GlobalSystem.Math.Abs(n));
                }
            }

            return this;
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>
        ///     A deep clone.
        /// </returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new FunctionNodeAbsolute(
                this.StringFormatters,
                this.Parameter.DeepClone(context));

        /// <summary>
        ///     Ensures that the parameter that is received is compatible with the function, optionally allowing the parameter
        ///     reference to change.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void EnsureCompatibleParameter(NodeBase parameter)
        {
            this.CalculatedCosts.Clear();

            var firstSupportedType = parameter.VerifyPossibleType(SupportableValueType.Integer | SupportableValueType.Numeric);

            switch (firstSupportedType)
            {
                case SupportableValueType.Numeric:
                    {
                        int cost = parameter.CalculateStrategyCost(SupportedValueType.Numeric);

                        this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Numeric);
                        foreach (var possibleType in GetSupportedTypeOptions(this.PossibleReturnType))
                        {
                            this.CalculatedCosts[possibleType] = (GetStandardConversionStrategyCost(
                                SupportedValueType.Numeric,
                                in possibleType) + cost, SupportedValueType.Numeric);
                        }

                        break;
                    }

                case SupportableValueType.Integer:
                    {
                        int cost = parameter.CalculateStrategyCost(SupportedValueType.Integer);

                        this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Integer);
                        foreach (var possibleType in GetSupportedTypeOptions(this.PossibleReturnType))
                        {
                            this.CalculatedCosts[possibleType] = (GetStandardConversionStrategyCost(
                                SupportedValueType.Integer,
                                in possibleType) + cost, SupportedValueType.Integer);
                        }

                        break;
                    }

                case SupportableValueType.Integer | SupportableValueType.Numeric:
                    {
                        int numericCost = parameter.CalculateStrategyCost(SupportedValueType.Numeric);
                        int integerCost = parameter.CalculateStrategyCost(SupportedValueType.Integer);

                        this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Integer) |
                                                  GetSupportableConversions(SupportedValueType.Numeric);

                        foreach (var possibleType in GetSupportedTypeOptions(this.PossibleReturnType))
                        {
                            int totalIntegerCost = GetStandardConversionStrategyCost(
                                SupportedValueType.Integer,
                                in possibleType);

                            if (totalIntegerCost != int.MaxValue)
                            {
                                totalIntegerCost += integerCost;
                            }

                            int totalNumericCost = GetStandardConversionStrategyCost(
                                SupportedValueType.Numeric,
                                in possibleType);

                            if (totalNumericCost != int.MaxValue)
                            {
                                totalNumericCost += numericCost;
                            }

                            if (totalIntegerCost <= totalNumericCost)
                            {
                                this.CalculatedCosts[possibleType] = (totalIntegerCost, SupportedValueType.Integer);
                            }
                            else
                            {
                                this.CalculatedCosts[possibleType] = (totalNumericCost, SupportedValueType.Numeric);
                            }
                        }

                        break;
                    }

                default:
                    throw new ExpressionNotValidLogicallyException();
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

            return tuple.InternalType switch
            {
                SupportedValueType.Integer => Expression.Call(
                    FuncAbsLong.Method,
                    this.Parameter.GenerateExpression(
                        SupportedValueType.Integer,
                        in comparisonTolerance)),
                SupportedValueType.Numeric => Expression.Call(
                    FuncAbsDouble.Method,
                    this.Parameter.GenerateExpression(
                        SupportedValueType.Numeric,
                        in comparisonTolerance)),
                _ => throw new ExpressionNotValidLogicallyException(),
            };
        }
    }
}