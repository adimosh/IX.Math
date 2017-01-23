// <copyright file="ParanthesesExpressionGenerator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace IX.Math.Generators
{
    internal static class ParanthesesExpressionGenerator
    {
        internal static void FormatParantheses(WorkingExpressionSet workingSet)
        {
            FormatParanthesis(string.Empty, workingSet);
            for (int i = 1; i < workingSet.SymbolTable.Count; i++)
            {
                FormatParanthesis($"item{i}", workingSet);
            }
        }

        private static void FormatParanthesis(string key, WorkingExpressionSet workingSet)
        {
            var symbol = workingSet.SymbolTable[key];
            if (symbol.IsFunctionCall || symbol.IsString)
            {
                return;
            }

            string replacedPreviously = string.Empty;
            string replaced = symbol.Expression;
            while (replaced != replacedPreviously)
            {
                workingSet.SymbolTable[key] = new RawExpressionContainer(replaced);
                replacedPreviously = replaced;
                replaced = ReplaceParanthesis(replaced, workingSet);
            }
        }

        private static string ReplaceParanthesis(string source, WorkingExpressionSet workingSet)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return string.Empty;
            }

            int openingParanthesisLocation = source.IndexOf(workingSet.Definition.Parantheses.Item1);
            int closingParanthesisLocation = source.IndexOf(workingSet.Definition.Parantheses.Item2);

            beginning:
            if (openingParanthesisLocation != -1)
            {
                if (closingParanthesisLocation != -1)
                {
                    if (openingParanthesisLocation < closingParanthesisLocation)
                    {
                        string resultingSubExpression = ReplaceParanthesis(
                            source.Substring(openingParanthesisLocation + workingSet.Definition.Parantheses.Item1.Length),
                            workingSet);

                        if (openingParanthesisLocation == 0)
                        {
                            source = resultingSubExpression;
                        }
                        else
                        {
                            string expr4 = openingParanthesisLocation == 0 ? string.Empty : source.Substring(0, openingParanthesisLocation);

                            if (!workingSet.AllOperatorsInOrder.Any(p => expr4.EndsWith(p)))
                            {
                                // We have a function call
                                int inx = workingSet.AllOperatorsInOrder.Max(p => expr4.LastIndexOf(p));
                                var expr5 = inx == -1 ? expr4 : expr4.Substring(inx);
                                string op1 = workingSet.AllOperatorsInOrder.OrderByDescending(p => p.Length).FirstOrDefault(p => expr5.StartsWith(p));
                                var expr6 = op1 == null ? expr5 : expr5.Substring(op1.Length);

                                string expr2 = SymbolExpressionGenerator.GenerateSymbolExpression(
                                    workingSet,
                                    $"{expr6}{workingSet.Definition.Parantheses.Item1}item{workingSet.SymbolTable.Count - 1}{workingSet.Definition.Parantheses.Item2}");

                                if (expr6 == expr4)
                                {
                                    expr4 = string.Empty;
                                }
                                else
                                {
                                    expr4 = expr4.Substring(0, expr4.Length - expr6.Length);
                                }

                                resultingSubExpression = resultingSubExpression.Replace($"item{workingSet.SymbolTable.Count - 1}", $"item{workingSet.SymbolTable.Count}");
                            }

                            source = $"{expr4}{resultingSubExpression}";
                        }

                        openingParanthesisLocation = source.IndexOf(workingSet.Definition.Parantheses.Item1);
                        closingParanthesisLocation = source.IndexOf(workingSet.Definition.Parantheses.Item2);

                        goto beginning;
                    }

                    return ProcessSubExpression(source, closingParanthesisLocation, workingSet);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                if (closingParanthesisLocation == -1)
                {
                    return source;
                }
                else
                {
                    return ProcessSubExpression(source, closingParanthesisLocation, workingSet);
                }
            }
        }

        private static string ProcessSubExpression(
            string source,
            int cp,
            WorkingExpressionSet workingSet)
        {
            string expr1 = source.Substring(0, cp);

            string[] parameters = expr1.Split(new string[] { workingSet.Definition.ParameterSeparator }, StringSplitOptions.None);

            List<string> parSymbols = new List<string>();
            foreach (string s in parameters)
            {
                string expr2 = SymbolExpressionGenerator.GenerateSymbolExpression(workingSet, s);
                parSymbols.Add(expr2);
            }

            int k = cp + workingSet.Definition.Parantheses.Item2.Length;
            return $"{string.Join(workingSet.Definition.ParameterSeparator, parSymbols)}{(source.Length == k ? string.Empty : source.Substring(k))}";
        }
    }
}