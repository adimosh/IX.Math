using System;
using System.Collections.Generic;
using System.Reflection;

namespace IX.Math.PlatformMitigation
{
    internal static class TypeExtensionsMitigation
    {
        internal static IEnumerable<MethodInfo> GetTypeMethods(this Type type)
        {
#if !(NETSTANDARD12 || NETSTANDARD10 || NETSTANDARD11)
            return type.GetMethods();
#else
            return type.GetRuntimeMethods();
#endif

        }
    }
}