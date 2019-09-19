// <copyright file="FunctionNodeReplace.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using IX.StandardExtensions.Extensions;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Operations.Function.Ternary
{
    [DebuggerDisplay("replace({FirstParameter}, {SecondParameter}, {ThirdParameter})")]
    [CallableMathematicsFunction("repl", "replace")]
    [UsedImplicitly]
    internal sealed class FunctionNodeReplace : TernaryFunctionNodeBase
    {
        public FunctionNodeReplace(
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

        public override NodeBase DeepClone(NodeCloningContext context) => new FunctionNodeReplace(
            this.FirstParameter.DeepClone(context),
            this.SecondParameter.DeepClone(context),
            this.ThirdParameter.DeepClone(context));

        public override NodeBase Simplify() =>
            this.FirstParameter is StringNode stringParam && this.SecondParameter is StringNode numericParam &&
            this.ThirdParameter is StringNode secondNumericParam
                ? new StringNode(
                    stringParam.Value.Replace(
                        numericParam.Value,
                        secondNumericParam.Value))
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
            second.DetermineStrongly(SupportedValueType.String);
            third.DetermineStrongly(SupportedValueType.String);

            if (first.ReturnType != SupportedValueType.String || second.ReturnType != SupportedValueType.String ||
                third.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        protected override Expression GenerateExpressionInternal()
        {
            MethodInfo mi = typeof(string).GetMethodWithExactParameters(
                nameof(string.Replace),
                typeof(string),
                typeof(string));

            if (mi == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        Resources.FunctionCouldNotBeFound,
                        nameof(string.Replace)));
            }

            Expression e1 = this.FirstParameter.GenerateExpression();
            Expression e2 = this.SecondParameter.GenerateExpression();
            Expression e3 = this.ThirdParameter.GenerateExpression();

            if (e1.Type != typeof(string))
            {
                e1 = Expression.Convert(
                    e1,
                    typeof(string));
            }

            if (e2.Type != typeof(string))
            {
                e2 = Expression.Convert(
                    e2,
                    typeof(string));
            }

            if (e3.Type != typeof(string))
            {
                e3 = Expression.Convert(
                    e3,
                    typeof(string));
            }

            return Expression.Call(
                e1,
                mi,
                e2,
                e3);
        }
    }
}