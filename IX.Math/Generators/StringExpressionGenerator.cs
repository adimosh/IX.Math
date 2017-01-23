// <copyright file="StringExpressionGenerator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

namespace IX.Math.Generators
{
    internal static class StringExpressionGenerator
    {
        internal static void ReplaceStrings(WorkingExpressionSet workingSet)
        {
            string process = workingSet.Expression;
            string stringIndicator = workingSet.Definition.StringIndicator;

            while (true)
            {
                int op = process.IndexOf(stringIndicator);

                if (op == -1)
                {
                    break;
                }

                int cp = process.IndexOf(stringIndicator, op + stringIndicator.Length);

                escapeRoute:
                if (cp == -1 || (cp + stringIndicator.Length) >= process.Length)
                {
                    break;
                }

                if (process.Substring(cp + stringIndicator.Length).StartsWith(stringIndicator))
                {
                    cp = process.IndexOf(stringIndicator, cp + (stringIndicator.Length * 2));
                    goto escapeRoute;
                }

                string itemName = SymbolExpressionGenerator.GenerateSymbolExpression(
                    workingSet,
                    process.Substring(op + stringIndicator.Length, cp - op - stringIndicator.Length),
                    isString: true);

                process = $"{process.Substring(0, op)}{itemName}{process.Substring(cp + stringIndicator.Length)}";
            }

            workingSet.Expression = process;
        }
    }
}