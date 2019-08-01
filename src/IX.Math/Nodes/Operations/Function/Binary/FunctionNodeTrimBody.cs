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
        public override NodeBase DeepClone(NodeCloningContext context) => new FunctionNodeTrimBody(
            this.FirstParameter.DeepClone(context),
            this.SecondParameter.DeepClone(context));

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>
        ///     A simplified node, or this instance.
        /// </returns>
        public override NodeBase Simplify() =>
            this.FirstParameter is StringNode stringParam && this.SecondParameter is StringNode charParam
                ? new StringNode(
                    stringParam.Value.Replace(
                        charParam.Value,
                        string.Empty))
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
            secondParameter.DetermineStrongly(SupportedValueType.String);

            if (firstParameter.ReturnType != SupportedValueType.String ||
                secondParameter.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        /// <summary>
        ///     Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>
        ///     The expression.
        /// </returns>
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