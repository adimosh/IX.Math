// <copyright file="ComputationBody.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using IX.Math.Nodes;
using IX.Math.Registration;
using JetBrains.Annotations;

namespace IX.Math.Computation
{
    internal readonly struct ComputationBody
    {
        internal static readonly ComputationBody Empty = new ComputationBody(
            null,
            null);

        internal readonly NodeBase BodyNode;

        internal readonly IParameterRegistry ParameterRegistry;

        internal ComputationBody(
            NodeBase bodyNode,
            IParameterRegistry parameterRegistry)
        {
            this.BodyNode = bodyNode;
            this.ParameterRegistry = parameterRegistry;
        }

        internal void Deconstruct(
            out NodeBase bodyNode,
            out IParameterRegistry parameterRegistry)
        {
            bodyNode = this.BodyNode;
            parameterRegistry = this.ParameterRegistry;
        }
    }
}