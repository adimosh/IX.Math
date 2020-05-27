// <copyright file="ITypeFormatter.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using JetBrains.Annotations;

namespace IX.Math.Formatters
{
    /// <summary>
    /// A service contract for a class that is able to format data types.
    /// </summary>
    [PublicAPI]
    [Obsolete("This has been moved to the IX.Math.Extensibility namespace, please use the interface IStringFormatter there. This interface is not used anymore.")]
    public interface ITypeFormatter
    {
        /// <summary>
        /// Formats the specified input.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <param name="input">The input.</param>
        /// <returns>The formatted value, as a string.</returns>
        string Format<TInput>(TInput input);
    }
}