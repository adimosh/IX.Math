// <copyright file="FunctionNodeTrimBody.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

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
    ///     A node representing a function.
    /// </summary>
    /// <seealso cref="BinaryFunctionNodeBase" />
    [DebuggerDisplay("trimbody({" + nameof(FirstParameter) + "}, {" + nameof(SecondParameter) + "})")]
    [CallableMathematicsFunction("trimbody")]
    [UsedImplicitly]
    internal sealed class FunctionNodeTrimBody : StringBinaryFunctionNodeBase
    {
#region Constructors

        public FunctionNodeTrimBody(
            NodeBase stringParameter,
            NodeBase numericParameter)
            : base(
                stringParameter,
                numericParameter)
        {
        }

#endregion

#region Methods

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>
        ///     A deep clone.
        /// </returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new FunctionNodeTrimBody(
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
            var (success, first, second) = this.GetSimplificationExpressions();

            if (!success)
            {
                return this;
            }

            return new StringNode(
                first.Replace(
                    second,
                    string.Empty));
        }

        /// <summary>
        ///     Generates the expression that this node represents.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="comparisonTolerance">The comparison tolerance.</param>
        /// <returns>A compiled expression, if one is possible.</returns>
        protected override Expression GenerateExpressionInternal(
            in SupportedValueType valueType,
            in ComparisonTolerance comparisonTolerance)
        {
            MethodInfo mi = typeof(string).GetMethodWithExactParameters(
                nameof(string.Replace),
                typeof(string),
                typeof(string));

            if (mi == null)
            {
                throw new MathematicsEngineException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.FunctionCouldNotBeFound,
                        nameof(string.Replace)));
            }

            var (first, second) = this.GetParameters(in comparisonTolerance);

            return Expression.Call(
                first,
                mi,
                second,
                Expression.Constant(
                    string.Empty,
                    typeof(string)));
        }

#endregion
    }
}