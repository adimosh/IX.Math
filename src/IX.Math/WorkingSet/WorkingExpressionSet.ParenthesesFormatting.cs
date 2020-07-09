// <copyright file="WorkingExpressionSet.ParenthesesFormatting.cs" company="Adrian Mos">
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

namespace IX.Math.WorkingSet
{
    internal partial class WorkingExpressionSet
    {
        private void FormatParentheses()
        {
            var itemsToProcess = new List<string>();

            KeyValuePair<string, ExpressionSymbol> itemToProcess;

            // Select the first expression that hasn't already been parsed
            while ((itemToProcess = this.symbolTable.Where(
                    (
                            p,
                            itemsToProcessL1) => !itemsToProcessL1.Contains(p.Key) && !p.Value.IsFunctionCall,
                    itemsToProcess).FirstOrDefault()).Value != null)
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
                #region Contracts
                Requires.NotNull(
                    key,
                    nameof(key));
                #endregion

                ExpressionSymbol symbol = this.symbolTable[key];
                if (symbol.IsFunctionCall)
                {
                    return;
                }

                var replacedPreviously = string.Empty;
                var replaced = symbol.Expression;
                while (replaced != replacedPreviously)
                {
                    this.symbolTable[key].Expression = replaced;
                    replacedPreviously = replaced;
                    replaced = ReplaceParanthesis(replaced);
                }

                string ReplaceParanthesis(string source)
                {
                    if (string.IsNullOrWhiteSpace(source))
                    {
                        return string.Empty;
                    }

                    var src = source;

                    var openParenthesis = this.definition.Parentheses.Item1;
                    var closeParenthesis = this.definition.Parentheses.Item2;
                    var openingParanthesisLocation = src.InvariantCultureIndexOf(
                        openParenthesis);
                    var closingParanthesisLocation = src.InvariantCultureIndexOf(
                        closeParenthesis);

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
                                src.Substring(openingParanthesisLocation + openParenthesis.Length));

                            if (openingParanthesisLocation == 0)
                            {
                                src = resultingSubExpression;
                            }
                            else
                            {
                                var expr4 = src.Substring(
                                        0,
                                        openingParanthesisLocation);

                                if (!this.allOperatorsInOrder.Any(
                                    (
                                        p,
                                        expr4L1) => expr4L1.InvariantCultureEndsWith(p), expr4))
                                {
                                    // We have a function call
                                    var inx = this.allOperatorsInOrder.Max(expr4.LastIndexOf);
                                    var expr5 = inx == -1 ? expr4 : expr4.Substring(inx);
                                    var op1 = this.allOperatorsInOrder.OrderByDescending(p => p.Length)
                                        .FirstOrDefault(
                                            (
                                                p,
                                                expr5L1) => expr5L1.InvariantCultureStartsWith(p), expr5);
                                    var expr6 = op1 == null ? expr5 : expr5.Substring(op1.Length);

                                    // ReSharper disable once AssignmentIsFullyDiscarded - We're interested only in having the symbol in the table, and nothing more
                                    _ = SymbolExpressionGenerator.GenerateSymbolExpression(
                                        this.symbolTable,
                                        this.reverseSymbolTable,
                                        $"{expr6}{openParenthesis}item{(this.symbolTable.Count - 1).ToString(CultureInfo.InvariantCulture)}{closeParenthesis}",
                                        false);

                                    expr4 = expr6 == expr4
                                        ? string.Empty
                                        : expr4.Substring(
                                            0,
                                            expr4.Length - expr6.Length);

                                    resultingSubExpression = resultingSubExpression.Replace(
                                        $"item{(this.symbolTable.Count - 1).ToString(CultureInfo.InvariantCulture)}",
                                        $"item{this.symbolTable.Count.ToString(CultureInfo.InvariantCulture)}");
                                }

                                src = $"{expr4}{resultingSubExpression}";
                            }

                            openingParanthesisLocation = src.InvariantCultureIndexOf(
                                openParenthesis);
                            closingParanthesisLocation = src.InvariantCultureIndexOf(
                                closeParenthesis);

                            goto beginning;
                        }

                        return ProcessSubExpression(
                            closingParanthesisLocation,
                            closeParenthesis,
                            src);
                    }

                    if (closingParanthesisLocation == -1)
                    {
                        return src;
                    }

                    return ProcessSubExpression(
                        closingParanthesisLocation,
                        closeParenthesis,
                        src);

                    string ProcessSubExpression(
                        int cp,
                        string closeParenthesisL3,
                        string sourceL3)
                    {
                        var expr1 = sourceL3.Substring(
                            0,
                            cp);

                        var parameterSeparatorL3 = this.definition.ParameterSeparator;
                        string[] parameters = expr1.Split(
                            new[] { parameterSeparatorL3 },
                            StringSplitOptions.None);

                        var parSymbols = new List<string>(parameters.Length);

                        // ReSharper disable once LoopCanBeConvertedToQuery - We are looking for best-performance linearity here
                        foreach (var s in parameters)
                        {
                            parSymbols.Add(
                                SymbolExpressionGenerator.GenerateSymbolExpression(
                                    this.symbolTable,
                                    this.reverseSymbolTable,
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
    }
}