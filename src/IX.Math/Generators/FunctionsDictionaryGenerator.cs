// <copyright file="FunctionsDictionaryGenerator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Reflection;
using IX.Math.Extensibility;
using IX.Math.Nodes.Functions;
using IX.Math.Nodes.Functions.Binary;
using IX.Math.Nodes.Functions.Nonary;
using IX.Math.Nodes.Functions.Ternary;
using IX.Math.Nodes.Functions.Unary;
using IX.StandardExtensions.Contracts;
using IX.StandardExtensions.Extensions;

namespace IX.Math.Generators
{
    internal static class FunctionsDictionaryGenerator
    {
        internal static Dictionary<string, Type> GenerateInternalNonaryFunctionsDictionary(
            IEnumerable<Assembly> assemblies) => GenerateTypeAssignableFrom<NonaryFunctionNodeBase>(assemblies);

        internal static Dictionary<string, Type> GenerateInternalUnaryFunctionsDictionary(
            IEnumerable<Assembly> assemblies) => GenerateTypeAssignableFrom<UnaryFunctionNodeBase>(assemblies);

        internal static Dictionary<string, Type> GenerateInternalBinaryFunctionsDictionary(
            IEnumerable<Assembly> assemblies) => GenerateTypeAssignableFrom<BinaryFunctionNodeBase>(assemblies);

        internal static Dictionary<string, Type> GenerateInternalTernaryFunctionsDictionary(
            IEnumerable<Assembly> assemblies) => GenerateTypeAssignableFrom<TernaryFunctionNodeBase>(assemblies);

        private static Dictionary<string, Type> GenerateTypeAssignableFrom<T>(IEnumerable<Assembly> assemblies)
            where T : FunctionNodeBase
        {
            Requires.NotNull(
                assemblies,
                nameof(assemblies));

            var typeDictionary = new Dictionary<string, Type>();

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

            return typeDictionary;
        }
    }
}