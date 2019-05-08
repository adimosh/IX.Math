// <copyright file="FunctionNodeTrimBody.cs" company="Adrian Mos">
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
    [DebuggerDisplay("trimbody({FirstParameter}, {SecondParameter})")]
    [CallableMathematicsFunction("trimbody")]
    [UsedImplicitly]
    internal sealed class FunctionNodeTrimBody : BinaryFunctionNodeBase
    {
        public FunctionNodeTrimBody(
            NodeBase stringParameter,
            NodeBase numericParameter)
            : base(
                stringParameter?.Simplify(),
                numericParameter?.Simplify())
        {
            if (stringParameter is ParameterNode sp)
            {
                sp.DetermineString();
            }

            if (numericParameter is ParameterNode np)
            {
                np.DetermineString();
            }

            if (stringParameter?.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }

            if (numericParameter?.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public override SupportedValueType ReturnType => SupportedValueType.String;

        public override NodeBase DeepClone(NodeCloningContext context) => new FunctionNodeTrimBody(
            this.FirstParameter.DeepClone(context),
            this.SecondParameter.DeepClone(context));

        public override NodeBase Simplify() =>
            this.FirstParameter is StringNode stringParam && this.SecondParameter is StringNode charParam
                ? new StringNode(
                    stringParam.Value.Replace(
                        charParam.Value,
                        string.Empty))
                : (NodeBase)this;

        protected override void EnsureCompatibleParameters(
            ref NodeBase firstParameter,
            ref NodeBase secondParameter)
        {
            // Nothing needs to be done here
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
                e2,
                Expression.Constant(
                    string.Empty,
                    typeof(string)));
        }
    }
}