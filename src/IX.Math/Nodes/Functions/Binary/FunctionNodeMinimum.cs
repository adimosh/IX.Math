// <copyright file="FunctionNodeMinimum.cs" company="Adrian Mos">
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
    ///     A node representing the <see cref="GlobalSystem.Math.Min(long, long)" /> or the <see cref="GlobalSystem.Math.Min(double, double)" /> function.
    /// </summary>
    /// <seealso cref="NumericBinaryFunctionNodeBase" />
    [DebuggerDisplay("min({" + nameof(FirstParameter) + "}, {" + nameof(SecondParameter) + "})")]
    [CallableMathematicsFunction(
        "min",
        "minimum")]
    [UsedImplicitly]
    internal sealed class FunctionNodeMinimum : IntegerOrNumericBinaryFunctionNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionNodeMinimum" /> class.
        /// </summary>
        /// <param name="stringFormatters">The string formatters.</param>
        /// <param name="firstParameter">The first parameter.</param>
        /// <param name="secondParameter">The second parameter.</param>
        public FunctionNodeMinimum(
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
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>
        ///     A deep clone.
        /// </returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new FunctionNodeMinimum(
                this.StringFormatters,
                this.FirstParameter.DeepClone(context),
                this.SecondParameter.DeepClone(context));

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>
        ///     A simplified node, or this instance.
        /// </returns>
        public override NodeBase Simplify()
        {
            var (success, integer, doubleFirst, doubleSecond, intFirst, intSecond) =
                this.GetSimplificationExpressions();

            if (!success)
            {
                return this;
            }

            if (integer)
            {
                return this.GenerateConstantInteger(
                    GlobalSystem.Math.Min(
                        intFirst,
                        intSecond));
            }

            return this.GenerateConstantNumeric(
                GlobalSystem.Math.Min(
                    doubleFirst,
                    doubleSecond));
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
            var (type, first, second) = this.GetParameters(in valueType, in comparisonTolerance);

            return Expression.Call(
                type == SupportedValueType.Integer
                    ? ((GlobalSystem.Func<long, long, long>)GlobalSystem.Math.Min).Method
                    : ((GlobalSystem.Func<double, double, double>)GlobalSystem.Math.Min).Method,
                first,
                second);
        }
    }
}