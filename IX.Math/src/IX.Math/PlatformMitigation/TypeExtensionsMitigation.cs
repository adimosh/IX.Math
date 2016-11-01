using System;
using System.Collections.Generic;
using System.Linq;
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

        internal static MethodInfo GetTypeMethod(this Type type, string name)
        {
            return type.GetTypeMethods().Where(p => p.Name == name).OrderBy(p => p.GetParameters().Length).FirstOrDefault();
        }

        internal static MethodInfo GetTypeMethod(this Type type, string name, Type[] parameters)
        {
            return type.GetTypeMethods().SingleOrDefault(p =>
            {
                if (p.Name != name)
                    return false;

                var pars = p.GetParameters();

                if (pars.Length != parameters.Length)
                    return false;

                for (int i = 0; i < parameters.Length; i++)
                {
                    if (pars[i].ParameterType != parameters[i])
                        return false;
                }

                return true;
            });
        }

        internal static MethodInfo GetTypeMethod(this Type type, string name, Type returnType, Type[] parameters)
        {
            return type.GetTypeMethods().SingleOrDefault(p =>
            {
                if (p.Name != name || p.ReturnType != returnType)
                    return false;

                var pars = p.GetParameters();

                if (pars.Length != parameters.Length)
                    return false;

                for (int i = 0; i < parameters.Length; i++)
                {
                    if (pars[i].ParameterType != parameters[i])
                        return false;
                }

                return true;
            });
        }
    }
}