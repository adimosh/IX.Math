// <copyright file="FunctionNodeRandom.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Extensibility;
using IX.Math.Generators;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Function.Nonary
{
    /// <summary>
    ///     A node representing the <see cref="Random.NextDouble" /> function.
    /// </summary>
    /// <seealso cref="NonaryFunctionNodeBase" />
    [DebuggerDisplay("random()")]
    [CallableMathematicsFunction("rand", "random")]
    [UsedImplicitly]
    internal sealed class FunctionNodeRandom : NonaryFunctionNodeBase
    {
        /// <summary>
        ///     Gets the return type of this node.
        /// </summary>
        /// <value>
        ///     The node return type.
        /// </value>
        public override SupportedValueType ReturnType => SupportedValueType.Numeric;

        /// <summary>
        ///     Generates a random number.
        /// </summary>
        /// <returns>A randomly-generated number.</returns>
        [UsedImplicitly]
        public static double GenerateRandom() => RandomNumberGenerator.Generate();

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>
        ///     A simplified node, or this instance.
        /// </returns>
        public override NodeBase Simplify() => this;

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>
        ///     A deep clone.
        /// </returns>
        public override NodeBase DeepClone(NodeCloningContext context) => new FunctionNodeRandom();

        /// <summary>
        ///     Strongly determines the node's type, if possible.
        /// </summary>
        /// <param name="type">The type to determine to.</param>
        public override void DetermineStrongly(SupportedValueType type)
        {
            if (type != SupportedValueType.Numeric)
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
            if ((type & SupportableValueType.Numeric) == 0)
            {
                throw new Exceptions.ExpressionNotValidLogicallyException();
            }
        }

        /// <summary>
        ///     Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>
        ///     The expression.
        /// </returns>
        protected override Expression GenerateExpressionInternal() =>
            this.GenerateStaticNonaryFunctionCall<FunctionNodeRandom>(nameof(GenerateRandom));
    }
}