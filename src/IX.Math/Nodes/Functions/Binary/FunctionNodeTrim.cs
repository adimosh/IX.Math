// <copyright file="FunctionNodeTrim.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Exceptions;
using IX.Math.Extensibility;
using IX.StandardExtensions.Extensions;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Functions.Binary
{
    /// <summary>
    ///     A node representing the trim function.
    /// </summary>
    /// <seealso cref="BinaryFunctionNodeBase" />
    [DebuggerDisplay("trim({" + nameof(FirstParameter) + "}, {" + nameof(SecondParameter) + "})")]
    [CallableMathematicsFunction("trim")]
    [UsedImplicitly]
    internal sealed class FunctionNodeTrim : StringBinaryFunctionNodeBase
    {
        public FunctionNodeTrim(
            List<IStringFormatter> stringFormatters,
            NodeBase stringParameter,
            NodeBase numericParameter)
            : base(
                stringFormatters,
                stringParameter,
                numericParameter)
        {
        }

        /// <summary>
        /// Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>
        /// A deep clone.
        /// </returns>
        public override NodeBase DeepClone(NodeCloningContext context) => new FunctionNodeTrim(
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

            return this.GenerateConstantString(first.Trim(second.ToCharArray()));
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
            MethodInfo mia = typeof(string).GetMethodWithExactParameters(
                nameof(string.ToCharArray),
                Type.EmptyTypes);

            if (mia == null)
            {
                throw new MathematicsEngineException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.FunctionCouldNotBeFound,
                        nameof(string.ToCharArray)));
            }

            MethodInfo mi = typeof(string).GetMethodWithExactParameters(
                nameof(string.Trim),
                typeof(char[]));

            if (mi == null)
            {
                throw new MathematicsEngineException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.FunctionCouldNotBeFound,
                        nameof(string.Trim)));
            }

            var (first, second) = this.GetParameters(in comparisonTolerance);

            return Expression.Call(
                first,
                mi,
                Expression.Call(
                    second,
                    mia));
        }
    }
}