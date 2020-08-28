// <copyright file="ComputationBody.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using IX.Math.Nodes;
using IX.Math.Nodes.Parameters;
using IX.StandardExtensions.Efficiency;
using JetBrains.Annotations;

namespace IX.Math.Computation
{
    internal readonly struct ComputationBody
    {
        internal static readonly ComputationBody Empty = new ComputationBody(
            null,
            null);

        [CanBeNull]
        internal readonly NodeBase BodyNode;

        [CanBeNull]
        internal readonly ConcurrentDictionary<string, ExternalParameterNode> ParameterRegistry;

        internal ComputationBody(
            NodeBase bodyNode,
            ConcurrentDictionary<string, ExternalParameterNode> parameterRegistry)
        {
            this.BodyNode = bodyNode;
            this.ParameterRegistry = parameterRegistry;
        }

        [UsedImplicitly]
        internal void Deconstruct(
            out NodeBase bodyNode,
            out ConcurrentDictionary<string, ExternalParameterNode> parameterRegistry)
        {
            bodyNode = this.BodyNode;
            parameterRegistry = this.ParameterRegistry;
        }
    }
}