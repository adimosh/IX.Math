// <copyright file="ForEachMitigation.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;

namespace IX.Math.PlatformMitigation
{
    internal static class ForEachMitigation
    {
        /// <summary>
        /// Until IX.StandardExtensions gets out of alpha, we need this here.
        /// </summary>
        /// <typeparam name="T">The enumerable type.</typeparam>
        /// <param name="source">The enumerable source.</param>
        /// <param name="action">The action to execute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="source" /> or <paramref name="action" /> is <c>null</c> (<c>Nothing</c> in Visual Basic).</exception>
        internal static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
#if DEBUG
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
#endif

            foreach (T item in source)
            {
                action(item);
            }
        }
    }
}