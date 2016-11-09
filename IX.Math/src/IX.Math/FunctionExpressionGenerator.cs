using System;
using System.Collections.Generic;
using System.Linq;

namespace IX.Math
{
    internal static class FunctionExpressionGenerator
    {
        internal static void ReplaceFunctions(WorkingExpressionSet workingSet, WorkingDefinition definition)
        {
            ReplaceOneFunction(string.Empty, workingSet, definition);
            for (int i = 1; i < workingSet.SymbolTable.Count; i++)
            {
                ReplaceOneFunction($"item{i}", workingSet, definition);
            }
        }

        private static void ReplaceOneFunction(string key, WorkingExpressionSet workingSet, WorkingDefinition definition)
        {
            var symbol = workingSet.SymbolTable[key];
            if (symbol.IsFunctionCall || symbol.IsString)
            {
                return;
            }

            string replaced = symbol.Expression;
            while (replaced != null)
            {
                workingSet.SymbolTable[key] = new RawExpressionContainer(replaced);
                replaced = ReplaceFunctions(replaced, workingSet, definition);
            }
        }

        private static string ReplaceFunctions(string source, WorkingExpressionSet workingSet, WorkingDefinition definition)
        {
            int op = -1;

            while (true)
            {
                op = source.IndexOf(definition.Definition.Parantheses.Item1, op + definition.Definition.Parantheses.Item1.Length);

                if (op == -1)
                {
                    return null;
                }

                if (op == 0)
                {
                    continue;
                }

                string functionHeaderCheck = source.Substring(0, op);

                if (definition.AllSymbols.Any(p => functionHeaderCheck.EndsWith(p)))
                {
                    continue;
                }

                string functionHeader = functionHeaderCheck.Split(definition.AllSymbols, StringSplitOptions.None).Last();

                int oop = source.IndexOf(definition.Definition.Parantheses.Item1, op + definition.Definition.Parantheses.Item1.Length);
                int cp = source.IndexOf(definition.Definition.Parantheses.Item2, op + definition.Definition.Parantheses.Item2.Length);

                while (oop < cp && oop != -1 && cp != -1)
                {
                    oop = source.IndexOf(definition.Definition.Parantheses.Item1, oop + definition.Definition.Parantheses.Item1.Length);
                    cp = source.IndexOf(definition.Definition.Parantheses.Item2, cp + definition.Definition.Parantheses.Item2.Length);
                }

                if (cp == -1)
                {
                    continue;
                }

                string arguments = source.Substring(op + definition.Definition.Parantheses.Item1.Length, cp - op - definition.Definition.Parantheses.Item1.Length);
                string originalArguments = arguments;

                string q = arguments;
                while (q != null)
                {
                    arguments = q;
                    q = ReplaceFunctions(q, workingSet, definition);
                }

                List<string> argPlaceholders = new List<string>();
                foreach (var s in arguments.Split(new[] { definition.Definition.ParameterSeparator }, StringSplitOptions.None))
                {
                    string sa = SymbolExpressionGenerator.GenerateSymbolExpression(workingSet, s);
                    argPlaceholders.Add(sa);
                }

                string functionCallBody = $"{functionHeader}{definition.Definition.Parantheses.Item1}{string.Join(definition.Definition.ParameterSeparator, argPlaceholders)}{definition.Definition.Parantheses.Item2}";
                string functionCallToReplace = $"{functionHeader}{definition.Definition.Parantheses.Item1}{originalArguments}{definition.Definition.Parantheses.Item2}";
                string functionCallItem = SymbolExpressionGenerator.GenerateSymbolExpression(workingSet, functionCallBody, isFunction: true);

                return source.Replace(
                    functionCallToReplace,
                    functionCallItem);
            }
        }
    }
}