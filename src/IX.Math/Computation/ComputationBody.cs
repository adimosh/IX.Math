// <copyright file="ComputationBody.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using IX.Math.Nodes;
using IX.Math.Registration;

namespace IX.Math.Computation
{
    internal readonly struct ComputationBody
    {
        internal static readonly ComputationBody Empty = new(
            null,
            EmptyParametersRegistry.Empty);

        internal readonly NodeBase? BodyNode;

        internal readonly IReadOnlyParameterRegistry ParameterRegistry;

        internal ComputationBody(
            NodeBase? bodyNode,
            IReadOnlyParameterRegistry parameterRegistry)
        {
            this.BodyNode = bodyNode;
            this.ParameterRegistry = parameterRegistry;
        }

        internal void Deconstruct(
            out NodeBase? bodyNode,
            out IReadOnlyParameterRegistry parameterRegistry)
        {
            bodyNode = this.BodyNode;
            parameterRegistry = this.ParameterRegistry;
        }
    }
}