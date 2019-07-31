// <copyright file="FunctionNodeRound.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Operations.Function.Binary
{
    [DebuggerDisplay("round({" + nameof(FirstParameter) + "}, {" + nameof(SecondParameter) + "})")]
    [CallableMathematicsFunction("round")]
    [UsedImplicitly]
    internal sealed class FunctionNodeRound : BinaryFunctionNodeBase
    {
        public FunctionNodeRound(
            NodeBase floatNode,
            NodeBase intNode)
        : base(floatNode?.Simplify(), intNode?.Simplify())
        {
            if (floatNode is ParameterNode fpn)
            {
                fpn.DetermineNumeric();
            }

            if (intNode is ParameterNode ipn)
            {
                ipn.DetermineNumeric().DetermineInteger();
            }

            if (floatNode?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }

            if (intNode?.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        /// <summary>
        /// Gets the return type of this node.
        /// </summary>
        /// <value>The node return type.</value>
        public override SupportedValueType ReturnType => SupportedValueType.Numeric;

        /// <summary>
        /// Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) => new FunctionNodeRound(
            this.FirstParameter.DeepClone(context),
            this.SecondParameter.DeepClone(context));

        /// <summary>
        /// Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>A simplified node, or this instance.</returns>
        public override NodeBase Simplify()
        {
            if (this.FirstParameter is NumericNode fln && this.SecondParameter is NumericNode inn)
            {
                return new NumericNode(global::System.Math.Round(fln.ExtractFloat(), inn.ExtractInt()));
            }

            return this;
        }

        /// <summary>
        /// Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>The expression.</returns>
        protected override Expression GenerateExpressionInternal() =>
            this.GenerateStaticBinaryFunctionCall<double, int>(typeof(global::System.Math), nameof(global::System.Math.Round));

        /// <summary>
        /// Ensures that the parameters that are received are compatible with the function, optionally allowing the parameter references to change.
        /// </summary>
        /// <param name="firstParameter">The first parameter.</param>
        /// <param name="secondParameter">The second parameter.</param>
        protected override void EnsureCompatibleParameters(
            ref NodeBase firstParameter,
            ref NodeBase secondParameter)
        {
            // Nothing to do here
        }
    }
}