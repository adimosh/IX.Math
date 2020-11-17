// <copyright file="FunctionNodeTrim.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Exceptions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Functions.Unary
{
    /// <summary>
    ///     A node representing the <see cref="string.Trim()" /> function.
    /// </summary>
    /// <seealso cref="UnaryFunctionNodeBase" />
    [DebuggerDisplay("trim({" + nameof(Parameter) + "})")]
    [CallableMathematicsFunction("trim")]
    [UsedImplicitly]
    internal sealed class FunctionNodeTrim : UnaryFunctionNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionNodeTrim"/> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public FunctionNodeTrim(
            NodeBase parameter)
            : base(
                parameter)
        {
        }

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>
        ///     A simplified node, or this instance.
        /// </returns>
        public override NodeBase Simplify() =>
            this.Parameter switch
            {
                ConstantNodeBase cn when cn.TryGetString(out var s) => new StringNode(s.Trim()),
                _ => this
            };

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>
        ///     A deep clone.
        /// </returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new FunctionNodeTrim(
                this.Parameter.DeepClone(context));

        /// <summary>
        ///     Ensures that the parameter that is received is compatible with the function, optionally allowing the parameter
        ///     reference to change.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void EnsureCompatibleParameter(NodeBase parameter)
        {
            _ = parameter.VerifyPossibleType(SupportableValueType.String);

            this.PossibleReturnType = GetSupportableConversions(SupportedValueType.String);
            var cost = parameter.CalculateStrategyCost(SupportedValueType.String);

            foreach (var possibleType in GetSupportedTypeOptions(this.PossibleReturnType))
            {
                this.CalculatedCosts[possibleType] = (GetStandardConversionStrategyCost(
                    SupportedValueType.String,
                    in possibleType) + cost, SupportedValueType.String);
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
            if (this.Parameter.CheckSupportedType(SupportableValueType.String))
            {
                return Expression.Call(
                    this.Parameter.GenerateExpression(
                        SupportedValueType.String,
                        in comparisonTolerance),
                    nameof(string.Trim),
                    Array.Empty<Type>());
            }

            throw new ExpressionNotValidLogicallyException();
        }
    }
}