// <copyright file="FunctionsDictionaryGenerator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Reflection;
using IX.Math.Extensibility;
using IX.Math.Nodes;
using IX.StandardExtensions.Extensions;

namespace IX.Math.Generators;

internal static class FunctionsDictionaryGenerator
{
    internal static void GenerateInternalNonaryFunctionsDictionary(
        this IEnumerable<Assembly> assemblies,
        Dictionary<string, Type> typeDictionary) =>
        GenerateTypeAssignableFrom<NonaryFunctionNodeBase>(assemblies, typeDictionary);

    internal static void GenerateInternalUnaryFunctionsDictionary(
        this IEnumerable<Assembly> assemblies,
        Dictionary<string, Type> typeDictionary) =>
        GenerateTypeAssignableFrom<UnaryFunctionNodeBase>(assemblies, typeDictionary);

    internal static void GenerateInternalBinaryFunctionsDictionary(
        this IEnumerable<Assembly> assemblies,
        Dictionary<string, Type> typeDictionary) =>
        GenerateTypeAssignableFrom<BinaryFunctionNodeBase>(assemblies, typeDictionary);

    internal static void GenerateInternalTernaryFunctionsDictionary(
        this IEnumerable<Assembly> assemblies,
        Dictionary<string, Type> typeDictionary) =>
        GenerateTypeAssignableFrom<TernaryFunctionNodeBase>(assemblies, typeDictionary);

    [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Performance",
        "HAA0603:Delegate allocation from a method group",
        Justification = "This is how LINQ works")]
    private static void GenerateTypeAssignableFrom<T>(IEnumerable<Assembly> assemblies, Dictionary<string, Type> typeDictionary)
        where T : FunctionNodeBase
    {
        // TODO: Do this in parallel
        assemblies.GetTypesAssignableFrom<T>().ForEach(
            AddToTypeDictionary,
            typeDictionary);

        void AddToTypeDictionary(
            TypeInfo p,
            Dictionary<string, Type> td)
        {
            CallableMathematicsFunctionAttribute attr;
            try
            {
                attr = p.GetCustomAttribute<CallableMathematicsFunctionAttribute>();
            }
            catch
            {
                return;
            }

            if (attr == null)
            {
                return;
            }

            foreach (var q in attr.Names)
            {
                if (td.ContainsKey(q))
                {
                    continue;
                }

                td.Add(
                    q,
                    p.AsType());
            }
        }
    }
}