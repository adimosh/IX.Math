// <copyright file="FunctionNodeStringLength.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Operations.Function.Unary
{
    /// <summary>
    ///     A node representing the <see cref="string.Length" /> property.
    /// </summary>
    /// <seealso cref="UnaryFunctionNodeBase" />
    [DebuggerDisplay("strlen({" + nameof(Parameter) + "})")]
    [CallableMathematicsFunction("strlen")]
    [UsedImplicitly]
    internal sealed class FunctionNodeStringLength : UnaryFunctionNodeBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FunctionNodeStringLength" /> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public FunctionNodeStringLength([NotNull] NodeBase parameter)
            : base(parameter)
        {
        }

        /// <summary>
        ///     Gets the return type of this node.
        /// </summary>
        /// <value>
        ///     The node return type.
        /// </value>
        public override SupportedValueType ReturnType => SupportedValueType.Numeric;

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>
        ///     A simplified node, or this instance.
        /// </returns>
        public override NodeBase Simplify()
        {
            if (this.Parameter is StringNode stringParam)
            {
                return new NumericNode(Convert.ToInt64(stringParam.Value.Length));
            }

            return this;
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>
        ///     A deep clone.
        /// </returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new FunctionNodeStringLength(this.Parameter.DeepClone(context));

        /// <summary>
        ///     Strongly determines the node's type, if possible.
        /// </summary>
        /// <param name="type">The type to determine to.</param>
        public override void DetermineStrongly(SupportedValueType type)
        {
            if (type != SupportedValueType.Numeric)
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
            if ((type & SupportableValueType.Numeric) == 0)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        /// <summary>
        ///     Ensures that the parameter that is received is compatible with the function, optionally allowing the parameter
        ///     reference to change.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <exception cref="ExpressionNotValidLogicallyException">The expression is not logically valid.</exception>
        protected override void EnsureCompatibleParameter(NodeBase parameter)
        {
            parameter.DetermineStrongly(SupportedValueType.String);

            if (parameter.ReturnType != SupportedValueType.String)
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
        protected override Expression GenerateExpressionInternal() =>
            Expression.Convert(
                this.GenerateParameterPropertyCall<string>(nameof(string.Length)),
                typeof(long));

        /// <summary>
        ///     Generates the expression with tolerance that will be compiled into code.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The expression.</returns>
        protected override Expression GenerateExpressionInternal(Tolerance tolerance) =>
            Expression.Convert(
                this.GenerateParameterPropertyCall<string>(
                    nameof(string.Length),
                    tolerance),
                typeof(long));
    }
}