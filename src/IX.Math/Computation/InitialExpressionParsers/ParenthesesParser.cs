// <copyright file="ParenthesesParser.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using IX.Math.ExpressionState;
using IX.Math.Generators;
using IX.Math.Interpretation;
using IX.StandardExtensions.Extensions;
using IX.StandardExtensions.Globalization;

namespace IX.Math.Computation.InitialExpressionParsers
{
    internal static class ParenthesesParser
    {
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance",
            "HAA0302:Display class allocation to capture closure",
            Justification =
                "A major closure is preferred, since the closure can be optimized and is much cheaper than otherwise")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance",
            "HAA0301:Closure Allocation Source",
            Justification =
                "A major closure is preferred, since the closure can be optimized and is much cheaper than otherwise")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance",
            "HAA0603:Delegate allocation from a method group",
            Justification = "Unavoidable")]
        internal static void FormatParentheses()
        {
            // Context symbols
            var context = InterpretationContext.Current;
            var symbolTable = context.SymbolTable;
            var definition = context.Definition;
            var (openParenthesis, closeParenthesis) = definition.Parentheses;
            var parameterSeparatorSymbol = definition.ParameterSeparator;

            KeyValuePair<string, ExpressionSymbol> itemToProcess;
            var itemsToProcess = new List<string>();

            // Select the first expression that hasn't already been parsed
            while ((itemToProcess = context.SymbolTable.Where(
                           (
                               p,
                               itemsToProcessL1) => !itemsToProcessL1.Contains(p.Key) && !p.Value.IsFunctionCall,
                           itemsToProcess)
                       .FirstOrDefault()).Value !=
                   null)
            {
                try
                {
                    FormatParenthesis(itemToProcess.Key);
                }
                finally
                {
                    itemsToProcess.Add(itemToProcess.Key);
                }
            }

            void FormatParenthesis(string key)
            {
                ExpressionSymbol symbol = symbolTable[key];
                if (symbol.IsFunctionCall)
                {
                    return;
                }

                var replacedPreviously = string.Empty;
                var replaced = symbol.Expression;
                while (replaced != replacedPreviously)
                {
                    symbolTable[key] = new ExpressionSymbol(
                        symbol.Name,
                        replaced,
                        symbol.IsFunctionCall);
                    replacedPreviously = replaced;
                    replaced = ReplaceParenthesis(replaced);
                }

                string ReplaceParenthesis(string? source)
                {
                    if (string.IsNullOrWhiteSpace(source))
                    {
                        return string.Empty;
                    }

                    var src = source!;

                    var openingParenthesisLocation = src.InvariantCultureIndexOf(openParenthesis);
                    var closingParenthesisLocation = src.InvariantCultureIndexOf(closeParenthesis);

                    beginning:
                    if (openingParenthesisLocation != -1)
                    {
                        if (closingParenthesisLocation == -1)
                        {
                            throw new InvalidOperationException();
                        }

                        if (openingParenthesisLocation < closingParenthesisLocation)
                        {
                            var resultingSubExpression = ReplaceParenthesis(
                                src.Substring(openingParenthesisLocation + openParenthesis.Length));

                            if (openingParenthesisLocation == 0)
                            {
                                src = resultingSubExpression;
                            }
                            else
                            {
                                var expr4 = src.Substring(
                                    0,
                                    openingParenthesisLocation);

                                var allOperatorsInOrder = context.AllOperatorsInOrder;
                                if (!allOperatorsInOrder.Any(
                                    (
                                        p,
                                        expr4L1) => expr4L1.InvariantCultureEndsWith(p),
                                    expr4))
                                {
                                    // We have a function call
                                    var inx = allOperatorsInOrder.Max(expr4.LastIndexOf);
                                    var expr5 = inx == -1 ? expr4 : expr4.Substring(inx);
                                    var op1 = allOperatorsInOrder.OrderByDescending(p => p.Length)
                                        .FirstOrDefault(
                                            (
                                                p,
                                                expr5L1) => expr5L1.InvariantCultureStartsWith(p),
                                            expr5);
                                    var expr6 = op1 == null ? expr5 : expr5.Substring(op1.Length);

                                    // ReSharper disable once AssignmentIsFullyDiscarded - We're interested only in having the symbol in the table, and nothing more
                                    _ = SymbolExpressionGenerator.GenerateSymbolExpression(
                                        $"{expr6}{openParenthesis}item{(symbolTable.Count - 1).ToString(CultureInfo.InvariantCulture)}{closeParenthesis}",
                                        false);

                                    expr4 = expr6 == expr4
                                        ? string.Empty
                                        : expr4.Substring(
                                            0,
                                            expr4.Length - expr6.Length);

                                    resultingSubExpression = resultingSubExpression.Replace(
                                        $"item{(symbolTable.Count - 1).ToString(CultureInfo.InvariantCulture)}",
                                        $"item{symbolTable.Count.ToString(CultureInfo.InvariantCulture)}");
                                }

                                src = $"{expr4}{resultingSubExpression}";
                            }

                            openingParenthesisLocation = src.InvariantCultureIndexOf(openParenthesis);
                            closingParenthesisLocation = src.InvariantCultureIndexOf(closeParenthesis);

                            goto beginning;
                        }

                        return ProcessSubExpression(
                            closingParenthesisLocation,
                            src);
                    }

                    if (closingParenthesisLocation == -1)
                    {
                        return src;
                    }

                    return ProcessSubExpression(
                        closingParenthesisLocation,
                        src);

                    string ProcessSubExpression(
                        int cp,
                        string sourceL3)
                    {
                        var expr1 = sourceL3.Substring(
                            0,
                            cp);

                        string[] parameters = expr1.Split(
                            new[]
                            {
                                parameterSeparatorSymbol
                            },
                            StringSplitOptions.None);

                        var parSymbols = new List<string>(parameters.Length);

                        // ReSharper disable once LoopCanBeConvertedToQuery - We are looking for best-performance linearity here
                        foreach (var s in parameters)
                        {
                            parSymbols.Add(
                                SymbolExpressionGenerator.GenerateSymbolExpression(
                                    s,
                                    false));
                        }

                        var k = cp + InterpretationContext.Current.Definition.Parentheses.Right.Length;

                        return
                            $"{string.Join(parameterSeparatorSymbol, parSymbols)}{(sourceL3.Length == k ? string.Empty : sourceL3.Substring(k))}";
                    }
                }
            }
        }
    }
}