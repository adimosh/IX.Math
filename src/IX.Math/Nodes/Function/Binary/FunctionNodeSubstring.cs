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

namespace IX.Math.Nodes.Function.Binary
{
    /// <summary>
    ///     A node representing the substring function.
    /// </summary>
    /// <seealso cref="BinaryFunctionNodeBase" />
    [DebuggerDisplay("substring({" + nameof(FirstParameter) + "}, {" + nameof(SecondParameter) + "})")]
    [CallableMathematicsFunction("substr", "substring")]
    [UsedImplicitly]
    internal sealed class FunctionNodeSubstring : BinaryFunctionNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionNodeSubstring"/> class.
        /// </summary>
        /// <param name="stringParameter">The string parameter.</param>
        /// <param name="numericParameter">The numeric parameter.</param>
        public FunctionNodeSubstring(
            NodeBase stringParameter,
            NodeBase numericParameter)
            : base(
                stringParameter?.Simplify(),
                numericParameter?.Simplify())
        {
        }

        /// <summary>
        /// Gets the return type of this node.
        /// </summary>
        /// <value>
        /// The node return type.
        /// </value>
        public override SupportedValueType ReturnType => SupportedValueType.String;

        /// <summary>
        /// Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>
        /// A deep clone.
        /// </returns>
        public override NodeBase DeepClone(NodeCloningContext context) => new FunctionNodeSubstring(
            this.FirstParameter.DeepClone(context),
            this.SecondParameter.DeepClone(context));

        /// <summary>
        /// Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>
        /// A simplified node, or this instance.
        /// </returns>
        public override NodeBase Simplify() =>
            this.FirstParameter is StringNode stringParam && this.SecondParameter is NumericNode numericParam
                ? new StringNode(stringParam.Value.Substring(numericParam.ExtractInt()))
                : (NodeBase)this;

        /// <summary>
        ///     Strongly determines the node's type, if possible.
        /// </summary>
        /// <param name="type">The type to determine to.</param>
        public override void DetermineStrongly(SupportedValueType type)
        {
            if (type != SupportedValueType.String)
            {
                throw new Exceptions.ExpressionNotValidLogicallyException();
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
                throw new Exceptions.ExpressionNotValidLogicallyException();
            }
        }

        /// <summary>
        ///     Ensures that the parameters that are received are compatible with the function, optionally allowing the parameter
        ///     references to change.
        /// </summary>
        /// <param name="firstParameter">The first parameter.</param>
        /// <param name="secondParameter">The second parameter.</param>
        /// <exception cref="ExpressionNotValidLogicallyException">The expression is not valid, as its parameters are not strings.</exception>
        protected override void EnsureCompatibleParameters(
            NodeBase firstParameter,
            NodeBase secondParameter)
        {
            firstParameter.DetermineStrongly(SupportedValueType.String);
            secondParameter.DetermineStrongly(SupportedValueType.Numeric);

            if (firstParameter.ReturnType != SupportedValueType.String ||
                secondParameter.ReturnType != SupportedValueType.Numeric)
            {
                throw new Exceptions.ExpressionNotValidLogicallyException();
            }

            if (secondParameter is ParameterNode pn)
            {
                pn.DetermineInteger();
            }
        }

        /// <summary>
        /// Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>
        /// The expression.
        /// </returns>
        protected override Expression GenerateExpressionInternal()
            => this.GenerateExpressionInternal(in ComparisonTolerance.Empty);

        /// <summary>
        /// Generates the expression with tolerance that will be compiled into code.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The expression.</returns>
        protected override Expression GenerateExpressionInternal(in ComparisonTolerance tolerance)
        {
            Type firstParameterType = typeof(string);
            Type secondParameterType = typeof(int);
            const string functionName = nameof(string.Substring);

            MethodInfo mi = typeof(string).GetMethodWithExactParameters(
                functionName,
                secondParameterType);

            if (mi == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.FunctionCouldNotBeFound,
                        functionName));
            }

            Expression e1, e2;
            if (tolerance.IsEmpty)
            {
                e1 = this.FirstParameter.GenerateExpression();
                e2 = this.SecondParameter.GenerateExpression();
            }
            else
            {
                e1 = this.FirstParameter.GenerateExpression(tolerance);
                e2 = this.SecondParameter.GenerateExpression(tolerance);
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

            return Expression.Call(
                e1,
                mi,
                e2);
        }
    }
}