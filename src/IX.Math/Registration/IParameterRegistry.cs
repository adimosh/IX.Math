// <copyright file="IParameterRegistry.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using JetBrains.Annotations;

namespace IX.Math.Registration
{
    /// <summary>
    ///     A service contract for a parameter registry.
    /// </summary>
    [PublicAPI]
    public interface IParameterRegistry : IReadOnlyParameterRegistry
    {
        /// <summary>
        ///     Advertises a potentially new parameter.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>A parameter context.</returns>
        ParameterContext AdvertiseParameter(string name);
    }
}