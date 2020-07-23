// <copyright file="FunctionNodeLogarithm.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Extensibility;
using JetBrains.Annotations;
using GlobalSystem = System;

namespace IX.Math.Nodes.Functions.Binary
{
    /// <summary>
    ///     A node representing the <see cref="GlobalSystem.Math.Log(double, double)" /> function.
    /// </summary>
    /// <seealso cref="NumericBinaryFunctionNodeBase" />
    [DebuggerDisplay("pow({" + nameof(FirstParameter) + "}, {" + nameof(SecondParameter) + "})")]
    [CallableMathematicsFunction("pow", "power")]
    [UsedImplicitly]
    internal sealed class FunctionNodePower : NumericBinaryFunctionNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionNodePower" /> class.
        /// </summary>
        /// <param name="stringFormatters">The string formatters.</param>
        /// <param name="firstParameter">The first parameter.</param>
        /// <param name="secondParameter">The second parameter.</param>
        public FunctionNodePower(
            List<IStringFormatter> stringFormatters,
            NodeBase firstParameter,
            NodeBase secondParameter)
            : base(
                stringFormatters,
                firstParameter,
                secondParameter)
        {
        }

        /// <summary>
        /// Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>
        /// A deep clone.
        /// </returns>
        public override NodeBase DeepClone(NodeCloningContext context) => new FunctionNodePower(
            this.StringFormatters,
            this.FirstParameter.DeepClone(context),
            this.SecondParameter.DeepClone(context));

        /// <summary>
        /// Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>
        /// A simplified node, or this instance.
        /// </returns>
        public override NodeBase Simplify()
        {
            var (success, first, second) = this.GetSimplificationExpressions();

            if (!success)
            {
                return this;
            }

            return this.GenerateConstantNumeric(
                GlobalSystem.Math.Pow(
                    first,
                    second));
        }

        /// <summary>
        /// Generates the expression that this node represents.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="comparisonTolerance">The comparison tolerance.</param>
        /// <returns>A compiled expression, if one is possible.</returns>
        protected override Expression GenerateExpressionInternal(
            in SupportedValueType valueType,
            in ComparisonTolerance comparisonTolerance)
        {
            var (first, second) = this.GetParameters(in comparisonTolerance);

            return Expression.Call(
                ((GlobalSystem.Func<double, double, double>)GlobalSystem.Math.Pow).Method,
                first,
                second);
        }
    }
}