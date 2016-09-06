using System;
using System.Collections.Generic;
using System.Reflection;

namespace IX.Math.PlatformMitigation
{
#if NETSTANDARD10 || NETSTANDARD11 || NETSTANDARD12
    internal static class MissingListExtensions
    {
        internal static void ForEach<T>(this List<T> listToForEach, Action<T> forEach)
        {
#if DEBUG
            if (listToForEach == null)
                throw new ArgumentNullException(nameof(listToForEach));
            if (forEach == null)
                throw new ArgumentNullException(nameof(forEach));
#endif

            foreach (T element in listToForEach)
                forEach(element);
        }
    }
#endif
}