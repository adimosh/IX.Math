// <copyright file="FunctionNodeSubstring.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
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
    ///     A node representing the substring function.
    /// </summary>
    /// <seealso cref="TernaryFunctionNodeBase" />
    [DebuggerDisplay(
        "substring({" +
        nameof(FirstParameter) +
        "}, {" +
        nameof(SecondParameter) +
        "}, {" +
        nameof(ThirdParameter) +
        "})")]
    [CallableMathematicsFunction(
        "substr",
        "substring")]
    [UsedImplicitly]
    internal sealed class FunctionNodeSubstring : TernaryFunctionNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionNodeSubstring" /> class.
        /// </summary>
        /// <param name="stringFormatter">The string formatter.</param>
        /// <param name="stringParameter">The string parameter.</param>
        /// <param name="numericParameter">The numeric parameter.</param>
        /// <param name="secondNumericParameter">The second numeric parameter.</param>
        public FunctionNodeSubstring(
            List<IStringFormatter> stringFormatter,
            NodeBase stringParameter,
            NodeBase numericParameter,
            NodeBase secondNumericParameter)
            : base(
                stringFormatter,
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
            new FunctionNodeSubstring(
                this.StringFormatters,
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
            if (!(this.FirstParameter is ConstantNodeBase stringParam))
            {
                return this;
            }

            if (!(this.SecondParameter is ConstantNodeBase fp &&
                this.ThirdParameter is ConstantNodeBase sp &&
                fp.TryGetInteger(out var secondValue) &&
                sp.TryGetInteger(out var thirdValue)))
            {
                return this;
            }

            return this.GenerateConstantString(
                stringParam.ValueAsString.Substring(
                    Convert.ToInt32(secondValue),
                    Convert.ToInt32(thirdValue)));
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
            _ = second.VerifyPossibleType(SupportableValueType.Integer);
            _ = third.VerifyPossibleType(SupportableValueType.Integer);

            int cost = first.CalculateStrategyCost(SupportedValueType.String) +
                       second.CalculateStrategyCost(SupportedValueType.Integer) +
                       third.CalculateStrategyCost(SupportedValueType.Integer);

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
                nameof(string.Substring),
                typeof(int),
                typeof(int));

            if (mi == null)
            {
                throw new MathematicsEngineException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.FunctionCouldNotBeFound,
                        nameof(string.Replace)));
            }

            var e1 = this.FirstParameter.GenerateExpression(SupportedValueType.String, in comparisonTolerance);
            var e2 = Expression.Call(
                ((Func<long, int>)Convert.ToInt32).Method,
                this.SecondParameter.GenerateExpression(SupportedValueType.Integer, in comparisonTolerance));
            var e3 = Expression.Call(
                ((Func<long, int>)Convert.ToInt32).Method,
                this.ThirdParameter.GenerateExpression(SupportedValueType.Integer, in comparisonTolerance));

            return Expression.Call(
                e1,
                mi,
                e2,
                e3);
        }
    }
}