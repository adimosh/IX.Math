// <copyright file="IntegerUnaryFunctionNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;

namespace IX.Math.Nodes.Functions.Unary
{
    /// <summary>
    ///     A base class for numeric unary functions.
    /// </summary>
    /// <seealso cref="UnaryFunctionNodeBase" />
    internal abstract class IntegerUnaryFunctionNodeBase : UnaryFunctionNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="IntegerUnaryFunctionNodeBase" /> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected IntegerUnaryFunctionNodeBase(
            NodeBase parameter)
            : base(
                parameter)
        {
        }

        /// <summary>
        /// Gets the function represented by this node.
        /// </summary>
        /// <value>
        /// The represented function.
        /// </value>
        protected abstract Func<long, long> RepresentedFunction { get; }

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>A simplified node, or this instance.</returns>
        public override NodeBase Simplify()
        {
            var (success, value) = this.GetSimplificationExpression();

            if (success)
            {
                return GenerateConstantInteger(this.RepresentedFunction(value));
            }

            return this;
        }

        /// <summary>
        ///     Ensures that the parameter that is received is compatible with the function, optionally allowing the parameter
        ///     reference to change.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void EnsureCompatibleParameter(NodeBase parameter)
        {
            this.CalculatedCosts.Clear();

            _ = parameter.VerifyPossibleType(SupportableValueType.Integer);

            this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Integer);
            var cost = parameter.CalculateStrategyCost(SupportedValueType.Integer);

            foreach (var possibleType in GetSupportedTypeOptions(this.PossibleReturnType))
            {
                this.CalculatedCosts[possibleType] = (GetStandardConversionStrategyCost(
                    SupportedValueType.Integer,
                    in possibleType) + cost, possibleType);
            }
        }

        /// <summary>
        /// Gets the simplification expression.
        /// </summary>
        /// <returns>The success value, along with a constant value if successful.</returns>
        protected (bool, long) GetSimplificationExpression()
        {
            if (this.Parameter is ConstantNodeBase fp &&
                fp.TryGetInteger(out var first))
            {
                return (true, first);
            }

            return (false, default);
        }

        /// <summary>
        /// Generates the expression that this node represents.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="comparisonTolerance">The comparison tolerance.</param>
        /// <returns>A compiled expression, if one is possible.</returns>
        protected override Expression GenerateExpressionInternal(
            in SupportedValueType valueType,
            in ComparisonTolerance comparisonTolerance) =>
            Expression.Call(
                this.RepresentedFunction.Method,
                this.Parameter.GenerateExpression(
                    SupportedValueType.Integer,
                    in comparisonTolerance));
    }
}