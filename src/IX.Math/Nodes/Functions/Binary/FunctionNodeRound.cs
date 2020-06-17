// <copyright file="FunctionNodeRound.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Exceptions;
using IX.Math.Extensibility;
using IX.StandardExtensions.Extensions;
using JetBrains.Annotations;
using GlobalSystem = System;

namespace IX.Math.Nodes.Functions.Binary
{
    /// <summary>
    ///     A node representing the <see cref="GlobalSystem.Math.Round(double, int)" /> function.
    /// </summary>
    /// <seealso cref="NumericBinaryFunctionNodeBase" />
    [DebuggerDisplay("round({" + nameof(FirstParameter) + "}, {" + nameof(SecondParameter) + "})")]
    [CallableMathematicsFunction("round")]
    [UsedImplicitly]
    internal sealed class FunctionNodeRound : NumericOperationBinaryFunctionNodeBase
    {
        public FunctionNodeRound(
            List<IStringFormatter> stringFormatters,
            NodeBase floatNode,
            NodeBase intNode)
            : base(
                stringFormatters,
                floatNode,
                intNode)
        {
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new FunctionNodeRound(
                this.StringFormatters,
                this.FirstParameter.DeepClone(context),
                this.SecondParameter.DeepClone(context));

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>A simplified node, or this instance.</returns>
        public override NodeBase Simplify()
        {
            var (success, first, second) = this.GetSimplificationExpressions();

            if (!success)
            {
                return this;
            }

            return this.GenerateConstantNumeric(
                GlobalSystem.Math.Round(
                    first,
                    GlobalSystem.Convert.ToInt32(second)));
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
            const string functionName = nameof(GlobalSystem.Math.Round);

            MethodInfo mi = typeof(GlobalSystem.Math).GetMethodWithExactParameters(
                functionName,
                typeof(int));

            if (mi == null)
            {
                throw new MathematicsEngineException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.FunctionCouldNotBeFound,
                        functionName));
            }

            var (first, second) = this.GetParameters(in comparisonTolerance);

            second = Expression.Call(
                ((GlobalSystem.Func<long, int>)GlobalSystem.Convert.ToInt32).Method,
                second);

            return Expression.Call(
                first,
                mi,
                second);
        }
    }
}