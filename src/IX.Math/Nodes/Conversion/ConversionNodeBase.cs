// <copyright file="ConversionNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq;
using IX.StandardExtensions.Contracts;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Conversion
{
    /// <summary>
    ///     A base class for an automatic conversion node.
    /// </summary>
    /// <seealso cref="IX.Math.Nodes.NodeBase" />
    [PublicAPI]
    public abstract class ConversionNodeBase : NodeBase
    {
#region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConversionNodeBase" /> class.
        /// </summary>
        /// <param name="sourceNode">The source node.</param>
        /// <param name="destinationType">Type of the destination.</param>
        protected ConversionNodeBase(
            NodeBase sourceNode,
            SupportableValueType destinationType)
        {
            var localSourceNode = Requires.NotNull(
                sourceNode,
                nameof(sourceNode));

            SupportedValueType[] possibleReturns = GetSupportedTypeOptions(in destinationType)
                .SelectMany(p => GetSupportedTypeOptions(GetSupportableConversions(in p)))
                .Distinct()
                .ToArray();

            foreach (SupportedValueType possibleReturn in possibleReturns)
            {
                this.CalculatedCosts[possibleReturn] = (
                    10 + localSourceNode.CalculateStrategyCost(SupportedValueType.String), possibleReturn);
            }

            this.ConvertFromNode = sourceNode;
        }

#endregion

#region Properties and indexers

        /// <summary>
        ///     Gets the node to convert from.
        /// </summary>
        /// <value>
        ///     The node to convert from.
        /// </value>
        public NodeBase ConvertFromNode
        {
            get;
        }

        /// <summary>
        ///     Gets a value indicating whether or not this node is actually a constant.
        /// </summary>
        /// <value><see langword="true" /> if the node is a constant, <see langword="false" /> otherwise.</value>
        public sealed override bool IsConstant =>
            false;

        /// <summary>
        ///     Gets a value indicating whether this node supports tolerance.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the node is tolerant, <see langword="false" /> otherwise.
        /// </value>
        public sealed override bool IsTolerant =>
            this.ConvertFromNode.IsTolerant;

        /// <summary>
        ///     Gets a value indicating whether this node requires preservation of the original expression.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the node requires original expression preservation, or <see langword="false" />
        ///     if it can be polynomially-reduced.
        /// </value>
        public sealed override bool RequiresPreservedExpression =>
            this.ConvertFromNode.RequiresPreservedExpression;

#endregion

#region Methods

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>A simplified node, or this instance.</returns>
        public override NodeBase Simplify() =>
            this;

        /// <summary>
        ///     Verifies this node and all nodes above it for logical validity.
        /// </summary>
        /// <remarks>
        ///     <para>This method is expected to be overridden, and is a good place to do type restriction verification.</para>
        /// </remarks>
        public sealed override void Verify() =>
            this.ConvertFromNode.Verify();

#endregion
    }
}