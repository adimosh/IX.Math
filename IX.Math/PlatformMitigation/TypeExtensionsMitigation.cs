// <copyright file="TypeExtensionsMitigation.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

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
            return type.
#if !(NETSTANDARD10 || NETSTANDARD11)
                GetMethods
#else
                GetRuntimeMethods
#endif
#pragma warning disable SA1110 // Opening parenthesis or bracket must be on declaration line
#pragma warning disable SA1008 // Opening parenthesis must be spaced correctly
                ();
#pragma warning restore SA1008 // Opening parenthesis must be spaced correctly
#pragma warning restore SA1110 // Opening parenthesis or bracket must be on declaration line
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
                {
                    return false;
                }

                var pars = p.GetParameters();

                if (pars.Length != parameters.Length)
                {
                    return false;
                }

                for (int i = 0; i < parameters.Length; i++)
                {
                    if (pars[i].ParameterType != parameters[i])
                    {
                        return false;
                    }
                }

                return true;
            });
        }

        internal static MethodInfo GetTypeMethod(this Type type, string name, Type returnType, Type[] parameters)
        {
            return type.GetTypeMethods().SingleOrDefault(p =>
            {
                if (p.Name != name || p.ReturnType != returnType)
                {
                    return false;
                }

                var pars = p.GetParameters();

                if (pars.Length != parameters.Length)
                {
                    return false;
                }

                for (int i = 0; i < parameters.Length; i++)
                {
                    if (pars[i].ParameterType != parameters[i])
                    {
                        return false;
                    }
                }

                return true;
            });
        }

        internal static IEnumerable<PropertyInfo> GetTypeProperties(this Type type)
        {
            return type.
#if !(NETSTANDARD10 || NETSTANDARD11)
                GetProperties
#else
                GetRuntimeProperties
#endif
#pragma warning disable SA1110 // Opening parenthesis or bracket must be on declaration line
#pragma warning disable SA1008 // Opening parenthesis must be spaced correctly
                ();
#pragma warning restore SA1008 // Opening parenthesis must be spaced correctly
#pragma warning restore SA1110 // Opening parenthesis or bracket must be on declaration line
        }
    }
}