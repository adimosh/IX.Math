// <copyright file="IReadOnlyParameterRegistry.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using JetBrains.Annotations;

namespace IX.Math.Registration
{
    /// <summary>
    ///     A service contract for a read-only parameter registry.
    /// </summary>
    [PublicAPI]
    public interface IReadOnlyParameterRegistry
    {
        /// <summary>
        ///     Gets a value indicating whether this <see cref="IParameterRegistry" /> is populated.
        /// </summary>
        /// <value><see langword="true" /> if populated; otherwise, <see langword="false" />.</value>
        bool Populated { get; }

        /// <summary>
        ///     Checks whether a specific parameter name exists.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns><see langword="true" /> if the parameter exists, <see langword="false" /> otherwise.</returns>
        bool Exists(string name);

        /// <summary>
        ///     Dumps all parameters.
        /// </summary>
        /// <returns>The existing parameters.</returns>
        ParameterContext[] Dump();

        /// <summary>
        ///     Clones from a previous, unrelated context.
        /// </summary>
        /// <param name="previousContext">The previous context.</param>
        /// <returns>The new parameter context.</returns>
        ParameterContext CloneFrom(ParameterContext previousContext);
    }
}