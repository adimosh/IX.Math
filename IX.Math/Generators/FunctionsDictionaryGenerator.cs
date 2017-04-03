// <copyright file="FunctionsDictionaryGenerator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using IX.Math.Extensibility;
using IX.Math.Nodes;
using IX.Math.PlatformMitigation;

namespace IX.Math.Generators
{
    internal static class FunctionsDictionaryGenerator
    {
        internal static Dictionary<string, Type> GenerateInternalNonaryFunctionsDictionary()
            => GenerateTypeAssignableFrom<NonaryFunctionNodeBase>();

        internal static Dictionary<string, Type> GenerateInternalUnaryFunctionsDictionary()
            => GenerateTypeAssignableFrom<UnaryFunctionNodeBase>();

        internal static Dictionary<string, Type> GenerateInternalBinaryFunctionsDictionary()
            => GenerateTypeAssignableFrom<BinaryFunctionNodeBase>();

        internal static Dictionary<string, Type> GenerateTypeAssignableFrom<T>()
            where T : FunctionNodeBase
        {
            var typeDictionary = new Dictionary<string, Type>();

            typeof(FunctionsDictionaryGenerator).GetTypeInfo().Assembly.DefinedTypes
                .Where(p => typeof(T).GetTypeInfo().IsAssignableFrom(p))
                .ForEach(AddToTypeDictionary);

            void AddToTypeDictionary(TypeInfo p)
            {
                CallableMathematicsFunctionAttribute attr;
                try
                {
                    attr = p.GetCustomAttribute<CallableMathematicsFunctionAttribute>();
                }
                catch
                {
                    // We need not do anything special here.
                    return;
                }

                if (attr == null)
                {
                    return;
                }

                attr.Names.ForEach(q => typeDictionary.Add(q, p.AsType()));
            }

            return typeDictionary;
        }
    }
}