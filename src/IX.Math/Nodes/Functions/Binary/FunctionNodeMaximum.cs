// <copyright file="FunctionNodeMaximum.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using JetBrains.Annotations;
using GlobalSystem = System;

namespace IX.Math.Nodes.Functions.Binary
{
    /// <summary>
    ///     A node representing the <see cref="GlobalSystem.Math.Max(long, long)" /> or the <see cref="GlobalSystem.Math.Max(double, double)" /> function.
    /// </summary>
    /// <seealso cref="IntegerOrNumericBinaryFunctionNodeBase" />
    [DebuggerDisplay("max({" + nameof(FirstParameter) + "}, {" + nameof(SecondParameter) + "})")]
    [CallableMathematicsFunction(
        "max",
        "maximum")]
    [UsedImplicitly]
    internal sealed class FunctionNodeMaximum : IntegerOrNumericBinaryFunctionNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionNodeMaximum" /> class.
        /// </summary>
        /// <param name="firstParameter">The first parameter.</param>
        /// <param name="secondParameter">The second parameter.</param>
        public FunctionNodeMaximum(
            NodeBase firstParameter,
            NodeBase secondParameter)
            : base(
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
            new FunctionNodeMaximum(
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
                return new IntegerNode(
                    GlobalSystem.Math.Max(
                        intFirst,
                        intSecond));
            }

            return new NumericNode(
                GlobalSystem.Math.Max(
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
                    ? ((GlobalSystem.Func<long, long, long>)GlobalSystem.Math.Max).Method
                    : ((GlobalSystem.Func<double, double, double>)GlobalSystem.Math.Max).Method,
                first,
                second);
        }
    }
}