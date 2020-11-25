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
#region Methods

#region Static methods

        /// <summary>
        ///     Generates a numeric conversion node from the source node.
        /// </summary>
        /// <param name="derivedNode">The derived node.</param>
        /// <returns>A numeric node, if one is possible.</returns>
        public static NodeBase GenerateAnythingConversionNode(NodeBase derivedNode) =>
            new AnythingDesiredFromStringConversionNode(derivedNode);

        /// <summary>
        ///     Generates a numeric conversion node from the source node.
        /// </summary>
        /// <param name="derivedNode">The derived node.</param>
        /// <returns>A numeric node, if one is possible.</returns>
        public static NodeBase GenerateBinaryConversionNode(NodeBase derivedNode) =>
            new BinaryDesiredFromStringConversionNode(derivedNode);

        /// <summary>
        ///     Generates a numeric conversion node from the source node.
        /// </summary>
        /// <param name="derivedNode">The derived node.</param>
        /// <returns>A numeric node, if one is possible.</returns>
        public static NodeBase GenerateIntegerConversionNode(NodeBase derivedNode) =>
            new IntegerDesiredFromStringConversionNode(derivedNode);

        /// <summary>
        ///     Generates a numeric conversion node from the source node.
        /// </summary>
        /// <param name="derivedNode">The derived node.</param>
        /// <returns>A numeric node, if one is possible.</returns>
        public static NodeBase GenerateNumericConversionNode(NodeBase derivedNode) =>
            new NumericDesiredFromStringConversionNode(derivedNode);

        /// <summary>
        ///     Generates a numeric conversion node from the source node.
        /// </summary>
        /// <param name="derivedNode">The derived node.</param>
        /// <returns>A numeric node, if one is possible.</returns>
        public static NodeBase GenerateNumericOrIntegerConversionNode(NodeBase derivedNode) =>
            new NumericOrIntegerDesiredFromStringConversionNode(derivedNode);

        /// <summary>
        ///     Ensures that the node given as parameter is or can be converted to a specific supported type if no supported types
        ///     are available.
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
                    case SupportableValueType.Binary:
                        node = GenerateBinaryConversionNode(node);

                        return;
                    default:
                        node = GenerateAnythingConversionNode(node);

                        return;
                }
            }
        }

#endregion

#endregion
    }
}