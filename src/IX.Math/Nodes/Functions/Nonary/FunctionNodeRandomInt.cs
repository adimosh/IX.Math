// <copyright file="FunctionNodeRandomInt.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Extensibility;
using IX.Math.Generators;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Functions.Nonary
{
    /// <summary>
    ///     A node representing the random integer function.
    /// </summary>
    /// <seealso cref="NonaryFunctionNodeBase" />
    [DebuggerDisplay("randomint()")]
    [CallableMathematicsFunction("randomint")]
    [UsedImplicitly]
    internal sealed class FunctionNodeRandomInt : NonaryFunctionNodeBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="FunctionNodeRandomInt" /> class.
        /// </summary>
        public FunctionNodeRandomInt()
        {
            this.PossibleReturnType = SupportableValueType.Integer;

            foreach (SupportedValueType possibleType in GetSupportedTypeOptions(this.PossibleReturnType))
            {
                this.CalculatedCosts[possibleType] = (0, SupportedValueType.Unknown);
            }
        }

        /// <summary>
        ///     Generates a random number.
        /// </summary>
        /// <returns>A random number.</returns>
        [UsedImplicitly]
        public static long GenerateRandom() => RandomNumberGenerator.GenerateInt();

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>
        ///     A deep clone.
        /// </returns>
        public override NodeBase DeepClone(NodeCloningContext context) => new FunctionNodeRandomInt();

        /// <summary>
        ///     Generates the expression that this node represents.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="comparisonTolerance">The comparison tolerance.</param>
        /// <returns>A compiled expression, if one is possible.</returns>
        protected override Expression GenerateExpressionInternal(
            in SupportedValueType valueType,
            in ComparisonTolerance comparisonTolerance) =>
            Expression.Call(((Func<long>)GenerateRandom).Method);
    }
}