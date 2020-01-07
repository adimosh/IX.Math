// <copyright file="FunctionNodeLength.cs" company="Adrian Mos">
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
    ///     A node representing the string length function.
    /// </summary>
    /// <seealso cref="UnaryFunctionNodeBase" />
    [DebuggerDisplay("length({" + nameof(Parameter) + "})")]
    [CallableMathematicsFunction("length")]
    [UsedImplicitly]
    internal sealed class FunctionNodeLength : UnaryFunctionNodeBase
    {
        public FunctionNodeLength(NodeBase parameter)
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
        public override NodeBase Simplify() =>
            this.Parameter switch
            {
                StringNode sn => new NumericNode(Convert.ToInt64(sn.Value.Length)),
                ByteArrayNode bn => new NumericNode(Convert.ToInt64(bn.Value.Length)),
                _ => this
            };

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>
        ///     A deep clone.
        /// </returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new FunctionNodeLength(this.Parameter.DeepClone(context));

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
        /// <exception cref="ExpressionNotValidLogicallyException">The parameter expression is not a string or a binary parameter.</exception>
        protected override void EnsureCompatibleParameter(NodeBase parameter)
        {
            parameter.DetermineWeakly(SupportableValueType.String | SupportableValueType.ByteArray);

            if (parameter.ReturnType != SupportedValueType.String &&
                parameter.ReturnType != SupportedValueType.ByteArray)
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
                this.Parameter.ReturnType == SupportedValueType.ByteArray
                    ? this.GenerateParameterPropertyCall<byte[]>(nameof(Array.Length))
                    : this.GenerateParameterPropertyCall<string>(nameof(string.Length)),
                typeof(long));

        /// <summary>
        ///     Generates the expression with tolerance that will be compiled into code.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The expression.</returns>
        protected override Expression GenerateExpressionInternal(Tolerance tolerance) =>
            Expression.Convert(
                this.Parameter.ReturnType == SupportedValueType.ByteArray
                    ? this.GenerateParameterPropertyCall<byte[]>(
                        nameof(Array.Length),
                        tolerance)
                    : this.GenerateParameterPropertyCall<string>(
                        nameof(string.Length),
                        tolerance),
                typeof(long));
    }
}