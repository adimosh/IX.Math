// <copyright file="FunctionNodeSubstring.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using IX.StandardExtensions.Extensions;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Function.Ternary
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
        ///     Initializes a new instance of the <see cref="FunctionNodeSubstring" /> class.
        /// </summary>
        /// <param name="stringParameter">The string parameter.</param>
        /// <param name="numericParameter">The numeric parameter.</param>
        /// <param name="secondNumericParameter">The second numeric parameter.</param>
        public FunctionNodeSubstring(
            NodeBase stringParameter,
            NodeBase numericParameter,
            NodeBase secondNumericParameter)
            : base(
                stringParameter?.Simplify(),
                numericParameter?.Simplify(),
                secondNumericParameter?.Simplify())
        {
        }

        /// <summary>
        ///     Gets the return type of this node.
        /// </summary>
        /// <value>
        ///     The node return type.
        /// </value>
        public override SupportedValueType ReturnType => SupportedValueType.String;

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>
        ///     A deep clone.
        /// </returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new FunctionNodeSubstring(
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
            if (this.FirstParameter is StringNode stringParam &&
                this.SecondParameter is NumericNode numericParam &&
                this.ThirdParameter is NumericNode secondNumericParam)
            {
                return new StringNode(
                    stringParam.Value.Substring(
                        numericParam.ExtractInt(),
                        secondNumericParam.ExtractInt()));
            }

            return this;
        }

        /// <summary>
        ///     Strongly determines the node's type, if possible.
        /// </summary>
        /// <param name="type">The type to determine to.</param>
        public override void DetermineStrongly(SupportedValueType type)
        {
            if (type != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        /// <summary>
        ///     Weakly determines the node's type, if possible, and, optionally, strongly determines if there is only one possible
        ///     type left.
        /// </summary>
        /// <param name="type">The type or types to determine to.</param>
        public override void DetermineWeakly(SupportableValueType type)
        {
            if ((type & SupportableValueType.String) == 0)
            {
                throw new ExpressionNotValidLogicallyException();
            }
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
            first.DetermineStrongly(SupportedValueType.String);
            second.DetermineStrongly(SupportedValueType.Numeric);
            third.DetermineStrongly(SupportedValueType.Numeric);

            if (first.ReturnType != SupportedValueType.String ||
                second.ReturnType != SupportedValueType.Numeric ||
                third.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }

            if (second is ParameterNode pn)
            {
                pn.DetermineInteger();
            }

            if (third is ParameterNode pn2)
            {
                pn2.DetermineInteger();
            }
        }

        /// <summary>
        ///     Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>
        ///     The expression.
        /// </returns>
        protected override Expression GenerateExpressionInternal() => this.GenerateExpressionInternal(null);

        /// <summary>
        ///     Generates the expression with tolerance that will be compiled into code.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The expression.</returns>
        protected override Expression GenerateExpressionInternal(Tolerance tolerance)
        {
            Type firstParameterType = typeof(string);
            Type secondParameterType = typeof(int);
            Type thirdParameterType = typeof(int);
            const string functionName = nameof(string.Substring);

            MethodInfo mi = typeof(string).GetMethodWithExactParameters(
                functionName,
                secondParameterType,
                thirdParameterType);

            if (mi == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.FunctionCouldNotBeFound,
                        functionName));
            }

            Expression e1, e2, e3;
            if (tolerance == null)
            {
                e1 = this.FirstParameter.GenerateExpression();
                e2 = this.SecondParameter.GenerateExpression();
                e3 = this.ThirdParameter.GenerateExpression();
            }
            else
            {
                e1 = this.FirstParameter.GenerateExpression(tolerance);
                e2 = this.SecondParameter.GenerateExpression(tolerance);
                e3 = this.ThirdParameter.GenerateExpression(tolerance);
            }

            if (e1.Type != firstParameterType)
            {
                e1 = Expression.Convert(
                    e1,
                    firstParameterType);
            }

            if (e2.Type != secondParameterType)
            {
                e2 = Expression.Convert(
                    e2,
                    secondParameterType);
            }

            if (e3.Type != thirdParameterType)
            {
                e3 = Expression.Convert(
                    e3,
                    thirdParameterType);
            }

            return Expression.Call(
                e1,
                mi,
                e2,
                e3);
        }
    }
}