// <copyright file="FunctionNodeSubstring.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Exceptions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using IX.StandardExtensions.Extensions;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Functions.Binary
{
    /// <summary>
    ///     A node representing the substring function.
    /// </summary>
    /// <seealso cref="BinaryFunctionNodeBase" />
    [DebuggerDisplay("substring({" + nameof(FirstParameter) + "}, {" + nameof(SecondParameter) + "})")]
    [CallableMathematicsFunction("substr", "substring")]
    [UsedImplicitly]
    internal sealed class FunctionNodeSubstring : StringOperationBinaryFunctionNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FunctionNodeSubstring" /> class.
        /// </summary>
        /// <param name="stringParameter">The string parameter.</param>
        /// <param name="numericParameter">The numeric parameter.</param>
        public FunctionNodeSubstring(
            NodeBase stringParameter,
            NodeBase numericParameter)
            : base(
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
        public override NodeBase DeepClone(NodeCloningContext context) => new FunctionNodeSubstring(
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

            return new StringNode(first.Substring(Convert.ToInt32(second)));
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
            const string functionName = nameof(string.Substring);

            MethodInfo mi = typeof(string).GetMethodWithExactParameters(
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
                ((Func<long, int>)Convert.ToInt32).Method,
                second);

            return Expression.Call(
                first,
                mi,
                second);
        }
    }
}