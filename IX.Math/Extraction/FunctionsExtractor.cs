// <copyright file="FunctionsExtractor.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using IX.Math.Generators;

namespace IX.Math.Extraction
{
    internal static class FunctionsExtractor
    {
        internal static void ReplaceFunctions(WorkingExpressionSet workingSet)
        {
            ReplaceOneFunction(string.Empty, workingSet);
            for (var i = 1; i < workingSet.SymbolTable.Count; i++)
            {
                ReplaceOneFunction($"item{i}", workingSet);
            }
        }

        private static void ReplaceOneFunction(string key, WorkingExpressionSet workingSet)
        {
            RawExpressionContainer symbol = workingSet.SymbolTable[key];
            if (symbol.IsFunctionCall || symbol.IsString)
            {
                return;
            }

            var replaced = symbol.Expression;
            while (replaced != null)
            {
                workingSet.SymbolTable[key] = new RawExpressionContainer(replaced);
                replaced = ReplaceFunctions(replaced, workingSet);
            }
        }

        private static string ReplaceFunctions(string source, WorkingExpressionSet workingSet)
        {
            var op = -1;

            while (true)
            {
                op = source.IndexOf(workingSet.Definition.Parantheses.Item1, op + workingSet.Definition.Parantheses.Item1.Length);

                if (op == -1)
                {
                    return null;
                }

                if (op == 0)
                {
                    continue;
                }

                var functionHeaderCheck = source.Substring(0, op);

                if (workingSet.AllSymbols.Any(p => functionHeaderCheck.EndsWith(p)))
                {
                    continue;
                }

                var functionHeader = functionHeaderCheck.Split(workingSet.AllSymbols, StringSplitOptions.None).Last();

                var oop = source.IndexOf(workingSet.Definition.Parantheses.Item1, op + workingSet.Definition.Parantheses.Item1.Length);
                var cp = source.IndexOf(workingSet.Definition.Parantheses.Item2, op + workingSet.Definition.Parantheses.Item2.Length);

                while (oop < cp && oop != -1 && cp != -1)
                {
                    oop = source.IndexOf(workingSet.Definition.Parantheses.Item1, oop + workingSet.Definition.Parantheses.Item1.Length);
                    cp = source.IndexOf(workingSet.Definition.Parantheses.Item2, cp + workingSet.Definition.Parantheses.Item2.Length);
                }

                if (cp == -1)
                {
                    continue;
                }

                var arguments = source.Substring(op + workingSet.Definition.Parantheses.Item1.Length, cp - op - workingSet.Definition.Parantheses.Item1.Length);
                var originalArguments = arguments;

                var q = arguments;
                while (q != null)
                {
                    arguments = q;
                    q = ReplaceFunctions(q, workingSet);
                }

                var argPlaceholders = new List<string>();
                foreach (var s in arguments.Split(new[] { workingSet.Definition.ParameterSeparator }, StringSplitOptions.None))
                {
                    TablePopulationGenerator.PopulateTables(s, workingSet);

                    // We check whether or not this is actually a constant
                    var sa = ConstantsGenerator.CheckAndAdd(workingSet.ConstantsTable, workingSet.ReverseConstantsTable, workingSet.Expression, s);
                    if (sa == null)
                    {
                        if (workingSet.ParametersTable.ContainsKey(s))
                        {
                            // Not a constant, and also not an already-recognized external parameter, let's generate a symbol
                            sa = SymbolExpressionGenerator.GenerateSymbolExpression(workingSet, s);
                        }
                        else
                        {
                            sa = s;
                        }
                    }

                    argPlaceholders.Add(sa);
                }

                var functionCallBody = $"{functionHeader}{workingSet.Definition.Parantheses.Item1}{string.Join(workingSet.Definition.ParameterSeparator, argPlaceholders)}{workingSet.Definition.Parantheses.Item2}";
                var functionCallToReplace = $"{functionHeader}{workingSet.Definition.Parantheses.Item1}{originalArguments}{workingSet.Definition.Parantheses.Item2}";
                var functionCallItem = SymbolExpressionGenerator.GenerateSymbolExpression(workingSet, functionCallBody, isFunction: true);

                return source.Replace(
                    functionCallToReplace,
                    functionCallItem);
            }
        }
    }
}