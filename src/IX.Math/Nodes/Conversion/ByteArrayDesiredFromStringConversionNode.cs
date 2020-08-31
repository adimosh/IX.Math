// <copyright file="ByteArrayDesiredFromStringConversionNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using IX.Math.WorkingSet;

namespace IX.Math.Nodes.Conversion
{
    /// <summary>
    /// A conversion node for when a numeric type is desired.
    /// </summary>
    /// <seealso cref="IX.Math.Nodes.Conversion.ConversionNodeBase" />
    public class ByteArrayDesiredFromStringConversionNode : ConversionNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ByteArrayDesiredFromStringConversionNode"/> class.
        /// </summary>
        /// <param name="sourceNode">The source node.</param>
        public ByteArrayDesiredFromStringConversionNode(
            [JetBrains.Annotations.NotNull] NodeBase sourceNode)
        : base(sourceNode, SupportableValueType.ByteArray)
        {
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new ByteArrayDesiredFromStringConversionNode(
                this.ConvertFromNode);

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
            return Expression.Call(
                ((Func<string, byte[]>)ParseNumeric).Method,
                this.ConvertFromNode.GenerateExpression(
                    SupportedValueType.String,
                    in comparisonTolerance));

            static byte[] ParseNumeric(string input)
            {
                if (!WorkingExpressionSet.ParseByteArray(
                    input,
                    out var result))
                {
                    throw new InvalidCastException();
                }

                return result;
            }
        }
    }
}