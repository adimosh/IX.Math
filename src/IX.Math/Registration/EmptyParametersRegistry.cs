// <copyright file="EmptyParametersRegistry.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;

namespace IX.Math.Registration
{
    internal class EmptyParametersRegistry : IReadOnlyParameterRegistry
    {
        public static IReadOnlyParameterRegistry Empty = new EmptyParametersRegistry();

        /// <summary>
        ///     Gets a value indicating whether this <see cref="IParameterRegistry" /> is populated.
        /// </summary>
        /// <value><see langword="true" /> if populated; otherwise, <see langword="false" />.</value>
        public bool Populated => false;

        /// <summary>
        ///     Checks whether a specific parameter name exists.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><see langword="true" /> if the parameter exists, <see langword="false" /> otherwise.</returns>
        public bool Exists(string name) => false;

        /// <summary>
        ///     Dumps all parameters.
        /// </summary>
        /// <returns>The existing parameters.</returns>
        public ParameterContext[] Dump() => Array.Empty<ParameterContext>();

        /// <summary>
        ///     Clones from a previous, unrelated context.
        /// </summary>
        /// <param name="previousContext">The previous context.</param>
        /// <returns>The new parameter context.</returns>
        public ParameterContext CloneFrom(ParameterContext previousContext) => previousContext;
    }
}