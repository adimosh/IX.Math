// <copyright file="FunctionNodeRandom.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Extensibility;
using IX.Math.Generators;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Functions.Nonary
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
        /// Initializes a new instance of the <see cref="FunctionNodeRandom"/> class.
        /// </summary>
        /// <param name="stringFormatters">The string formatters.</param>
        public FunctionNodeRandom(List<IStringFormatter> stringFormatters)
            : base(stringFormatters)
        {
            this.PossibleReturnType = SupportableValueType.Numeric;

            foreach (var possibleType in GetSupportedTypeOptions(this.PossibleReturnType))
            {
                this.CalculatedCosts[possibleType] = (0, SupportedValueType.Unknown);
            }
        }

        /// <summary>
        ///     Generates a random number.
        /// </summary>
        /// <returns>A randomly-generated number.</returns>
        [UsedImplicitly]
        public static double GenerateRandom() => RandomNumberGenerator.Generate();

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>
        ///     A deep clone.
        /// </returns>
        public override NodeBase DeepClone(NodeCloningContext context) => new FunctionNodeRandom(this.StringFormatters);

        /// <summary>
        /// Generates the expression that this node represents.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="comparisonTolerance">The comparison tolerance.</param>
        /// <returns>A compiled expression, if one is possible.</returns>
        protected override Expression GenerateExpressionInternal(
            in SupportedValueType valueType,
            in ComparisonTolerance comparisonTolerance) =>
            Expression.Call(((Func<double>)GenerateRandom).Method);
    }
}