// <copyright file="FunctionsDictionaryGenerator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Reflection;
using IX.Math.Extensibility;
using IX.Math.Nodes;
using IX.StandardExtensions.Extensions;
using IX.StandardExtensions.Contracts;

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
            // ReSharper disable once PossibleMultipleEnumeration - False positive
            Contract.RequiresNotNullPrivate(
                in assemblies,
                nameof(assemblies));

            var typeDictionary = new Dictionary<string, Type>();

#pragma warning disable HAA0603 // Delegate allocation from a method group - This is acceptable

            // TODO: Do this in parallel
            // ReSharper disable once PossibleMultipleEnumeration - False positive
            assemblies.GetTypesAssignableFrom<T>().ForEach(
                AddToTypeDictionary,
                typeDictionary);
#pragma warning restore HAA0603 // Delegate allocation from a method group

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
                    // We need not do anything special here.
#pragma warning disable ERP022 // Catching everything considered harmful. - This is acceptable
                    return;
#pragma warning restore ERP022 // Catching everything considered harmful.
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