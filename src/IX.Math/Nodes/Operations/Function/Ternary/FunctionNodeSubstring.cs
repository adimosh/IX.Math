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

namespace IX.Math.Nodes.Operations.Function.Ternary
{
    [DebuggerDisplay("substring({FirstParameter}, {SecondParameter}, {ThirdParameter})")]
    [CallableMathematicsFunction("substr", "substring")]
    [UsedImplicitly]
    internal sealed class FunctionNodeSubstring : TernaryFunctionNodeBase
    {
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

        public override SupportedValueType ReturnType => SupportedValueType.String;

        public override NodeBase DeepClone(NodeCloningContext context) => new FunctionNodeSubstring(
            this.FirstParameter.DeepClone(context),
            this.SecondParameter.DeepClone(context),
            this.ThirdParameter.DeepClone(context));

        public override NodeBase Simplify()
        {
            if (this.FirstParameter is StringNode stringParam && this.SecondParameter is NumericNode numericParam &&
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
        /// Ensures the parameters are compatible for this node.
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

            if (first.ReturnType != SupportedValueType.String || second.ReturnType != SupportedValueType.Numeric ||
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

        protected override Expression GenerateExpressionInternal()
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
                        Resources.FunctionCouldNotBeFound,
                        functionName));
            }

            Expression e1 = this.FirstParameter.GenerateExpression();
            Expression e2 = this.SecondParameter.GenerateExpression();
            Expression e3 = this.ThirdParameter.GenerateExpression();

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