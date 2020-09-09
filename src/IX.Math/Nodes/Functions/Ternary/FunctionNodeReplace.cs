// <copyright file="FunctionNodeReplace.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Exceptions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using IX.StandardExtensions.Extensions;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Functions.Ternary
{
    /// <summary>
    ///     A node representing the string replace function.
    /// </summary>
    /// <seealso cref="TernaryFunctionNodeBase" />
    [DebuggerDisplay(
        "replace({" +
        nameof(FirstParameter) +
        "}, {" +
        nameof(SecondParameter) +
        "}, {" +
        nameof(ThirdParameter) +
        "})")]
    [CallableMathematicsFunction(
        "repl",
        "replace")]
    [UsedImplicitly]
    internal sealed class FunctionNodeReplace : TernaryFunctionNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionNodeReplace" /> class.
        /// </summary>
        /// <param name="stringParameter">The string parameter.</param>
        /// <param name="numericParameter">The numeric parameter.</param>
        /// <param name="secondNumericParameter">The second numeric parameter.</param>
        public FunctionNodeReplace(
            NodeBase stringParameter,
            NodeBase numericParameter,
            NodeBase secondNumericParameter)
            : base(
                stringParameter,
                numericParameter,
                secondNumericParameter)
        {
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>
        ///     A deep clone.
        /// </returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new FunctionNodeReplace(
                this.FirstParameter.DeepClone(context),
                this.SecondParameter.DeepClone(context),
                this.ThirdParameter.DeepClone(context));

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>
        ///     A simplified node, or this instance.
        /// </returns>
        public override NodeBase Simplify()
        {
            if (this.FirstParameter is ConstantNodeBase first &&
                this.SecondParameter is ConstantNodeBase second &&
                this.ThirdParameter is ConstantNodeBase third)
            {
                return new StringNode(
                    first.ValueAsString.Replace(
                        second.ValueAsString,
                        third.ValueAsString));
            }

            return this;
        }

        /// <summary>
        ///     Ensures the parameters are compatible for this node.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <param name="third">The third.</param>
        protected override void EnsureCompatibleParameters(
            NodeBase first,
            NodeBase second,
            NodeBase third)
        {
            this.CalculatedCosts.Clear();

            _ = first.VerifyPossibleType(SupportableValueType.String);
            _ = second.VerifyPossibleType(SupportableValueType.String);
            _ = third.VerifyPossibleType(SupportableValueType.String);

            int cost = first.CalculateStrategyCost(SupportedValueType.String) +
                       second.CalculateStrategyCost(SupportedValueType.String) +
                       third.CalculateStrategyCost(SupportedValueType.String);

            this.PossibleReturnType = GetSupportableConversions(SupportedValueType.String);

            foreach (var possibleType in GetSupportedTypeOptions(this.PossibleReturnType))
            {
                this.CalculatedCosts[possibleType] = (GetStandardConversionStrategyCost(
                                                          SupportedValueType.String,
                                                          in possibleType) +
                                                      cost, SupportedValueType.String);
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
            MethodInfo mi = typeof(string).GetMethodWithExactParameters(
                nameof(string.Replace),
                typeof(string),
                typeof(string));

            if (mi == null)
            {
                throw new MathematicsEngineException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.FunctionCouldNotBeFound,
                        nameof(string.Replace)));
            }

            var e1 = this.FirstParameter.GenerateExpression(SupportedValueType.String, in comparisonTolerance);
            var e2 = this.SecondParameter.GenerateExpression(SupportedValueType.String, in comparisonTolerance);
            var e3 = this.ThirdParameter.GenerateExpression(SupportedValueType.String, in comparisonTolerance);

            return Expression.Call(
                e1,
                mi,
                e2,
                e3);
        }
    }
}