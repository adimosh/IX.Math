// <copyright file="Mitigation.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;

namespace IX.Math.Obsolete
{
#if NET452
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1649:File name should match first type name",
        Justification = "This file contains obsolete mitigation stuff, let's not worry about that for now.")]
    internal static class DotNet452Mitigation
    {
        internal static readonly Type[] EmptyTypeArray = new Type[0];

        internal static readonly byte[] EmptyByteAttay = new byte[0];

        internal static readonly string[] EmptyStringArray = new string[0];
    }

    /// <summary>
    /// A thread-static .NET 4.5.2-only object to make coding easier for higher-framework counterparts.
    /// </summary>
    /// <typeparam name="T">The type of object in the ThreadStatic.</typeparam>
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleCop.CSharp.MaintainabilityRules",
        "SA1402:File may only contain a single type",
        Justification = "This file contains obsolete mitigation stuff, let's not worry about that for now.")]
    public class ThreadStatic<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ThreadStatic{T}"/> class.
        /// </summary>
        /// <param name="value">The value.</param>
        public ThreadStatic(T value)
        {
            this.Value = value;
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>
        /// The value.
        /// </value>
        public T Value { get; }
    }
    #endif
}