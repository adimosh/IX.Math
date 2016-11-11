using System;
using System.Collections.Generic;
using System.Linq;

namespace IX.Math
{
    internal static class FunctionExpressionGenerator
    {
        internal static void ReplaceFunctions(WorkingExpressionSet workingSet)
        {
            ReplaceOneFunction(string.Empty, workingSet);
            for (int i = 1; i < workingSet.SymbolTable.Count; i++)
            {
                ReplaceOneFunction($"item{i}", workingSet);
            }
        }

        private static void ReplaceOneFunction(string key, WorkingExpressionSet workingSet)
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
                replaced = ReplaceFunctions(replaced, workingSet);
            }
        }

        private static string ReplaceFunctions(string source, WorkingExpressionSet workingSet)
        {
            int op = -1;

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

                string functionHeaderCheck = source.Substring(0, op);

                if (workingSet.AllSymbols.Any(p => functionHeaderCheck.EndsWith(p)))
                {
                    continue;
                }

                string functionHeader = functionHeaderCheck.Split(workingSet.AllSymbols, StringSplitOptions.None).Last();

                int oop = source.IndexOf(workingSet.Definition.Parantheses.Item1, op + workingSet.Definition.Parantheses.Item1.Length);
                int cp = source.IndexOf(workingSet.Definition.Parantheses.Item2, op + workingSet.Definition.Parantheses.Item2.Length);

                while (oop < cp && oop != -1 && cp != -1)
                {
                    oop = source.IndexOf(workingSet.Definition.Parantheses.Item1, oop + workingSet.Definition.Parantheses.Item1.Length);
                    cp = source.IndexOf(workingSet.Definition.Parantheses.Item2, cp + workingSet.Definition.Parantheses.Item2.Length);
                }

                if (cp == -1)
                {
                    continue;
                }

                string arguments = source.Substring(op + workingSet.Definition.Parantheses.Item1.Length, cp - op - workingSet.Definition.Parantheses.Item1.Length);
                string originalArguments = arguments;

                string q = arguments;
                while (q != null)
                {
                    arguments = q;
                    q = ReplaceFunctions(q, workingSet);
                }

                List<string> argPlaceholders = new List<string>();
                foreach (var s in arguments.Split(new[] { workingSet.Definition.ParameterSeparator }, StringSplitOptions.None))
                {
                    string sa = SymbolExpressionGenerator.GenerateSymbolExpression(workingSet, s);
                    argPlaceholders.Add(sa);
                }

                string functionCallBody = $"{functionHeader}{workingSet.Definition.Parantheses.Item1}{string.Join(workingSet.Definition.ParameterSeparator, argPlaceholders)}{workingSet.Definition.Parantheses.Item2}";
                string functionCallToReplace = $"{functionHeader}{workingSet.Definition.Parantheses.Item1}{originalArguments}{workingSet.Definition.Parantheses.Item2}";
                string functionCallItem = SymbolExpressionGenerator.GenerateSymbolExpression(workingSet, functionCallBody, isFunction: true);

                return source.Replace(
                    functionCallToReplace,
                    functionCallItem);
            }
        }
    }
}