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
        /// Ensures that the node given as parameter is or can be converted to a specific supported type if no supported types are available.
        /// </summary>
        /// <param name="node">The node.</param>
        /// <param name="typesToEnsure">The types to ensure.</param>
        protected static void EnsureNode(
            ref NodeBase node,
            SupportableValueType typesToEnsure)
        {
            if (!node.CheckSupportedType(typesToEnsure))
            {
                if ((typesToEnsure & SupportableValueType.String) != SupportableValueType.None)
                {
                    typesToEnsure ^= SupportableValueType.String;
                }

                switch (typesToEnsure)
                {
                    case SupportableValueType.Integer:
                        node = GenerateIntegerConversionNode(node);
                        return;
                    case SupportableValueType.Numeric:
                        node = GenerateNumericConversionNode(node);
                        return;
                    case SupportableValueType.Numeric | SupportableValueType.Integer:
                        node = GenerateNumericOrIntegerConversionNode(node);
                        return;
                    case SupportableValueType.ByteArray:
                        node = GenerateByteArrayConversionNode(node);
                        return;
                    default:
                        node = GenerateAnythingConversionNode(node);
                        return;
                }
            }
        }

        /// <summary>
        /// Generates a numeric conversion node from the source node.
        /// </summary>
        /// <param name="derivedNode">The derived node.</param>
        /// <returns>A numeric node, if one is possible.</returns>
        public static NodeBase GenerateAnythingConversionNode(NodeBase derivedNode)
        {
            return new AnythingDesiredFromStringConversionNode(derivedNode);
        }

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