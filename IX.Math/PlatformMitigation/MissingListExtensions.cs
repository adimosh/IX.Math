// <copyright file="MissingListExtensions.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

#if NETSTANDARD10 || NETSTANDARD11
using System;
using System.Collections.Generic;

namespace IX.Math.PlatformMitigation
{
    internal static class MissingListExtensions
    {
        internal static void ForEach<T>(this List<T> listToForEach, Action<T> forEach)
        {
#if DEBUG
            if (listToForEach == null)
            {
                throw new ArgumentNullException(nameof(listToForEach));
            }

            if (forEach == null)
            {
                throw new ArgumentNullException(nameof(forEach));
            }
#endif

            foreach (T element in listToForEach)
            {
                forEach(element);
            }
        }
    }
}
#endif