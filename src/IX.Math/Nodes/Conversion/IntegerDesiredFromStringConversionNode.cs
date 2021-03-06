// <copyright file="IntegerDesiredFromStringConversionNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using IX.Math.Conversion;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Conversion
{
    /// <summary>
    ///     A conversion node for when a numeric type is desired.
    /// </summary>
    /// <seealso cref="IX.Math.Nodes.Conversion.ConversionNodeBase" />
    public sealed class IntegerDesiredFromStringConversionNode : ConversionNodeBase
    {
#region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="IntegerDesiredFromStringConversionNode" /> class.
        /// </summary>
        /// <param name="sourceNode">The source node.</param>
        public IntegerDesiredFromStringConversionNode([NotNull] NodeBase sourceNode)
            : base(
                sourceNode,
                SupportableValueType.Integer)
        {
        }

#endregion

#region Methods

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new IntegerDesiredFromStringConversionNode(this.ConvertFromNode.DeepClone(context));

        /// <summary>
        ///     Generates the expression that this node represents.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="comparisonTolerance">The comparison tolerance.</param>
        /// <returns>A compiled expression, if one is possible.</returns>
        protected override Expression GenerateExpressionInternal(
            in SupportedValueType valueType,
            in ComparisonTolerance comparisonTolerance) =>
            Expression.Call(
                ((Func<string, long>)InternalTypeDirectConversions.ParseInteger).Method,
                this.ConvertFromNode.GenerateExpression(
                    SupportedValueType.String,
                    in comparisonTolerance));

#endregion
    }
}