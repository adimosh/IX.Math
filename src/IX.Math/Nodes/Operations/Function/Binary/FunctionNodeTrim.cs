// <copyright file="FunctionNodeTrim.cs" company="Adrian Mos">
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
    [DebuggerDisplay("trim({FirstParameter}, {SecondParameter})")]
    [CallableMathematicsFunction("trim")]
    [UsedImplicitly]
    internal sealed class FunctionNodeTrim : BinaryFunctionNodeBase
    {
        public FunctionNodeTrim(
            NodeBase stringParameter,
            NodeBase numericParameter)
            : base(
                stringParameter?.Simplify(),
                numericParameter?.Simplify())
        {
        }

        public override SupportedValueType ReturnType => SupportedValueType.String;

        public override NodeBase DeepClone(NodeCloningContext context) => new FunctionNodeTrim(
            this.FirstParameter.DeepClone(context),
            this.SecondParameter.DeepClone(context));

        public override NodeBase Simplify() =>
            this.FirstParameter is StringNode stringParam && this.SecondParameter is StringNode charParam
                ? (NodeBase)new StringNode(stringParam.Value.Trim(charParam.Value.ToCharArray()))
                : this;

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
            secondParameter.DetermineStrongly(SupportedValueType.String);

            if (firstParameter.ReturnType != SupportedValueType.String ||
                secondParameter.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        protected override Expression GenerateExpressionInternal()
        {
            MethodInfo mia = typeof(string).GetMethodWithExactParameters(
                nameof(string.ToCharArray),
                new Type[0]);

            if (mia == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        Resources.FunctionCouldNotBeFound,
                        nameof(string.ToCharArray)));
            }

            MethodInfo mi = typeof(string).GetMethodWithExactParameters(
                nameof(string.Trim),
                typeof(char[]));

            if (mi == null)
            {
                throw new InvalidOperationException(
                    string.Format(
                        Resources.FunctionCouldNotBeFound,
                        nameof(string.Trim)));
            }

            Expression e1 = this.FirstParameter.GenerateExpression();
            Expression e2 = this.SecondParameter.GenerateExpression();

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

            return Expression.Call(
                e1,
                mi,
                Expression.Call(
                    e2,
                    mia));
        }
    }
}