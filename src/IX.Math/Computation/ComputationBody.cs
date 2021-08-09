// <copyright file="ComputationBody.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using IX.Math.Nodes;
using IX.Math.Registration;

namespace IX.Math.Computation
{
    internal readonly struct ComputationBody
    {
        internal static readonly ComputationBody Empty = new(
            null,
            EmptyParametersRegistry.Empty);

        private readonly NodeBase? bodyNode;

        private readonly IReadOnlyParameterRegistry parameterRegistry;

        internal ComputationBody(
            NodeBase? bodyNode,
            IReadOnlyParameterRegistry parameterRegistry)
        {
            this.bodyNode = bodyNode;
            this.parameterRegistry = parameterRegistry;
        }

        [SuppressMessage(
            "ReSharper",
            "ParameterHidesMember",
            Justification = "We're not overly concerned, as these are private members.")]
        [SuppressMessage(
            "CodeQuality",
            "IDE0079:Remove unnecessary suppression",
            Justification = "ReSharper is used in this project.")]
        internal void Deconstruct(
            out NodeBase? bodyNode,
            out IReadOnlyParameterRegistry parameterRegistry)
        {
            bodyNode = this.bodyNode;
            parameterRegistry = this.parameterRegistry;
        }
    }
}