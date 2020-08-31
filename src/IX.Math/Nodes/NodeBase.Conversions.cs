// <copyright file="NodeBase.Conversions.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using IX.Math.Nodes.Conversion;
using IX.StandardExtensions;

namespace IX.Math.Nodes
{
    /// <summary>
    ///     A base class for mathematics nodes.
    /// </summary>
    /// <seealso cref="IDeepCloneable{T}" />
    public abstract partial class NodeBase
    {
        /// <summary>
        /// Generates a numeric conversion node from the source node.
        /// </summary>
        /// <param name="derivedNode">The derived node.</param>
        /// <returns>A numeric node, if one is possible.</returns>
        public static NodeBase GenerateNumericConversionNode(NodeBase derivedNode)
        {
            return new NumericDesiredFromStringConversionNode(derivedNode);
        }

        /// <summary>
        /// Generates a numeric conversion node from the source node.
        /// </summary>
        /// <param name="derivedNode">The derived node.</param>
        /// <returns>A numeric node, if one is possible.</returns>
        public static NodeBase GenerateIntegerConversionNode(NodeBase derivedNode)
        {
            return new IntegerDesiredFromStringConversionNode(derivedNode);
        }

        /// <summary>
        /// Generates a numeric conversion node from the source node.
        /// </summary>
        /// <param name="derivedNode">The derived node.</param>
        /// <returns>A numeric node, if one is possible.</returns>
        public static NodeBase GenerateNumericOrIntegerConversionNode(NodeBase derivedNode)
        {
            return new NumericOrIntegerDesiredFromStringConversionNode(derivedNode);
        }

        /// <summary>
        /// Generates a numeric conversion node from the source node.
        /// </summary>
        /// <param name="derivedNode">The derived node.</param>
        /// <returns>A numeric node, if one is possible.</returns>
        public static NodeBase GenerateByteArrayConversionNode(NodeBase derivedNode)
        {
            return new ByteArrayDesiredFromStringConversionNode(derivedNode);
        }
    }
}