// <copyright file="FunctionNodeSubstring.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using IX.StandardExtensions;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Operations.Function.Binary
{
    [DebuggerDisplay("substring({FirstParameter}, {SecondParameter})")]
    [CallableMathematicsFunction("substr", "substring")]
    [UsedImplicitly]
    internal sealed class FunctionNodeSubstring : BinaryFunctionNodeBase
    {
        public FunctionNodeSubstring(
            NodeBase stringParameter,
            NodeBase numericParameter)
            : base(
                stringParameter?.Simplify(),
                numericParameter?.Simplify())
        {
        }

        public override SupportedValueType ReturnType => SupportedValueType.String;

        public override NodeBase DeepClone(NodeCloningContext context) => new FunctionNodeSubstring(
            this.FirstParameter.DeepClone(context),
            this.SecondParameter.DeepClone(context));

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
                throw new ExpressionNotValidLogicallyException();
            }

            if (secondParameter is ParameterNode pn)
            {
                pn.DetermineInteger();
            }
        }

        protected override Expression GenerateExpressionInternal()
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
                        Resources.FunctionCouldNotBeFound,
                        functionName));
            }

            Expression e1 = this.FirstParameter.GenerateExpression();
            Expression e2 = this.SecondParameter.GenerateExpression();

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