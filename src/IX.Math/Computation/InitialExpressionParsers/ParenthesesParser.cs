// <copyright file="ParenthesesParser.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using IX.Math.ExpressionState;
using IX.Math.Generators;
using IX.StandardExtensions.Contracts;
using IX.StandardExtensions.Extensions;
using IX.StandardExtensions.Globalization;
using JetBrains.Annotations;

namespace IX.Math.Computation.InitialExpressionParsers
{
    internal static class ParenthesesParser
    {
#pragma warning disable HAA0603 // Delegate allocation from a method group - unavoidable
        internal static void FormatParentheses(
             string openParenthesis,
             string closeParenthesis,
             string parameterSeparator,
             string[] allOperatorsInOrder,
             Dictionary<string, ExpressionSymbol> symbolTable,
             Dictionary<string, string> reverseSymbolTable)
        {
            var itemsToProcess = new List<string>();

            KeyValuePair<string, ExpressionSymbol> itemToProcess;

            // Select the first expression that hasn't already been parsed
            while ((itemToProcess = symbolTable.Where(
                    (
                            p,
                            itemsToProcessL1) => !itemsToProcessL1.Contains(p.Key) && !p.Value.IsFunctionCall,
                    itemsToProcess).FirstOrDefault()).Value != null)
            {
                try
                {
                    FormatParenthesis(
                        itemToProcess.Key,
                        openParenthesis,
                        closeParenthesis,
                        parameterSeparator,
                        allOperatorsInOrder,
                        symbolTable,
                        reverseSymbolTable);
                }
                finally
                {
                    itemsToProcess.Add(itemToProcess.Key);
                }
            }

            void FormatParenthesis(
                string key,
                string openParenthesisL1,
                string closeParenthesisL1,
                string parameterSeparatorL1,
                string[] allOperatorsInOrderL1,
                Dictionary<string, ExpressionSymbol> symbolTableL1,
                Dictionary<string, string> reverseSymbolTableL1)
            {
                ExpressionSymbol symbol = symbolTableL1[key];
                if (symbol.IsFunctionCall)
                {
                    return;
                }

                var replacedPreviously = string.Empty;
                var replaced = symbol.Expression;
                while (replaced != replacedPreviously)
                {
                    symbolTableL1[key].Expression = replaced;
                    replacedPreviously = replaced;
                    replaced = ReplaceParanthesis(
                        replaced,
                        openParenthesisL1,
                        closeParenthesisL1,
                        parameterSeparatorL1,
                        allOperatorsInOrderL1,
                        symbolTableL1,
                        reverseSymbolTableL1);
                }

                string ReplaceParanthesis(
                    string source,
                    string openParenthesisL2,
                    string closeParenthesisL2,
                    string parameterSeparatorSymbolL2,
                    string[] allOperatorsInOrderSymbolsL2,
                    Dictionary<string, ExpressionSymbol> symbolTableL2,
                    Dictionary<string, string> reverseSymbolTableL2)
                {
                    if (string.IsNullOrWhiteSpace(source))
                    {
                        return string.Empty;
                    }

                    var src = source;

                    var openingParanthesisLocation = src.InvariantCultureIndexOf(
                        openParenthesisL2);
                    var closingParanthesisLocation = src.InvariantCultureIndexOf(
                        closeParenthesisL2);

                    beginning:
                    if (openingParanthesisLocation != -1)
                    {
                        if (closingParanthesisLocation == -1)
                        {
                            throw new InvalidOperationException();
                        }

                        if (openingParanthesisLocation < closingParanthesisLocation)
                        {
                            var resultingSubExpression = ReplaceParanthesis(
                                src.Substring(openingParanthesisLocation + openParenthesisL2.Length),
                                openParenthesisL2,
                                closeParenthesisL2,
                                parameterSeparatorSymbolL2,
                                allOperatorsInOrderSymbolsL2,
                                symbolTableL2,
                                reverseSymbolTableL2);

                            if (openingParanthesisLocation == 0)
                            {
                                src = resultingSubExpression;
                            }
                            else
                            {
                                var expr4 = openingParanthesisLocation == 0
                                    ? string.Empty
                                    : src.Substring(
                                        0,
                                        openingParanthesisLocation);

                                if (!allOperatorsInOrderSymbolsL2.Any(
                                    (
                                        p,
                                        expr4L1) => expr4L1.InvariantCultureEndsWith(p), expr4))
                                {
                                    // We have a function call
                                    var inx = allOperatorsInOrderSymbolsL2.Max(expr4.LastIndexOf);
                                    var expr5 = inx == -1 ? expr4 : expr4.Substring(inx);
                                    var op1 = allOperatorsInOrderSymbolsL2.OrderByDescending(p => p.Length)
                                        .FirstOrDefault(
                                            (
                                                p,
                                                expr5L1) => expr5L1.InvariantCultureStartsWith(p), expr5);
                                    var expr6 = op1 == null ? expr5 : expr5.Substring(op1.Length);

                                    // ReSharper disable once AssignmentIsFullyDiscarded - We're interested only in having the symbol in the table, and nothing more
                                    _ = SymbolExpressionGenerator.GenerateSymbolExpression(
                                        symbolTableL2,
                                        reverseSymbolTableL2,
                                        $"{expr6}{openParenthesisL2}item{(symbolTableL2.Count - 1).ToString(CultureInfo.InvariantCulture)}{closeParenthesisL2}",
                                        false);

                                    expr4 = expr6 == expr4
                                        ? string.Empty
                                        : expr4.Substring(
                                            0,
                                            expr4.Length - expr6.Length);

                                    resultingSubExpression = resultingSubExpression.Replace(
                                        $"item{(symbolTableL2.Count - 1).ToString(CultureInfo.InvariantCulture)}",
                                        $"item{symbolTableL2.Count.ToString(CultureInfo.InvariantCulture)}");
                                }

                                src = $"{expr4}{resultingSubExpression}";
                            }

                            openingParanthesisLocation = src.InvariantCultureIndexOf(
                                openParenthesisL2);
                            closingParanthesisLocation = src.InvariantCultureIndexOf(
                                closeParenthesisL2);

                            goto beginning;
                        }

                        return ProcessSubExpression(
                            closingParanthesisLocation,
                            closeParenthesisL2,
                            src,
                            parameterSeparatorSymbolL2,
                            symbolTableL2,
                            reverseSymbolTableL2);
                    }

                    if (closingParanthesisLocation == -1)
                    {
                        return src;
                    }

                    return ProcessSubExpression(
                        closingParanthesisLocation,
                        closeParenthesisL2,
                        src,
                        parameterSeparatorSymbolL2,
                        symbolTableL2,
                        reverseSymbolTableL2);

                    string ProcessSubExpression(
                        int cp,
                        string closeParenthesisL3,
                        string sourceL3,
                        string parameterSeparatorL3,
                        Dictionary<string, ExpressionSymbol> symbolTableL3,
                        Dictionary<string, string> reverseSymbolTableL3)
                    {
                        var expr1 = sourceL3.Substring(
                            0,
                            cp);

                        string[] parameters = expr1.Split(
                            new[] { parameterSeparatorL3 },
                            StringSplitOptions.None);

                        var parSymbols = new List<string>(parameters.Length);

                        // ReSharper disable once LoopCanBeConvertedToQuery - We are looking for best-performance linearity here
                        foreach (var s in parameters)
                        {
                            parSymbols.Add(
                                SymbolExpressionGenerator.GenerateSymbolExpression(
                                    symbolTableL3,
                                    reverseSymbolTableL3,
                                    s,
                                    false));
                        }

                        var k = cp + closeParenthesisL3.Length;
                        return
                            $"{string.Join(parameterSeparatorL3, parSymbols)}{(sourceL3.Length == k ? string.Empty : sourceL3.Substring(k))}";
                    }
                }
            }
        }
#pragma warning restore HAA0603 // Delegate allocation from a method group
    }
}