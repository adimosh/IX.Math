// <copyright file="FunctionsExtractor.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using IX.Math.ExpressionState;
using IX.Math.Generators;
using IX.Math.Interpretation;
using IX.Math.Registration;
using IX.StandardExtensions.Extensions;
using IX.StandardExtensions.Globalization;

namespace IX.Math.Extraction
{
    /// <summary>
    ///     A class to handle function extraction.
    /// </summary>
    internal static class FunctionsExtractor
    {
        /// <summary>
        ///     Replaces functions calls with expression placeholders.
        /// </summary>
        /// <param name="expression">The expression before processing.</param>
        internal static void ReplaceFunctions(string expression)
        {
            // Replace the main expression
            ReplaceOneFunction(
                string.Empty,
                expression);

            for (var i = 1; i < InterpretationContext.Current.SymbolTable.Count; i++)
            {
                // Replace sub-expressions
                ReplaceOneFunction(
                    $"item{i.ToString(CultureInfo.InvariantCulture).PadLeft(4, '0')}",
                    expression);
            }

            static void ReplaceOneFunction(
                string key,
                string outerExpressionSymbol)
            {
                var symbolTable = InterpretationContext.Current.SymbolTable;
                ExpressionSymbol symbol = symbolTable[key];
                if (symbol.IsFunctionCall)
                {
                    return;
                }

                var replaced = symbol.Expression;
                while (replaced != null)
                {
                    symbolTable[key].Expression = replaced;
                    replaced = ReplaceFunctionsInternal(
                        replaced,
                        outerExpressionSymbol);
                }

                static string? ReplaceFunctionsInternal(
                    string source,
                    string expressionSymbol)
                {
                    var op = -1;
                    var context = InterpretationContext.Current;
                    var definition = context.Definition;
                    var (openParenthesisSymbol, closeParenthesisSymbol) = definition.Parentheses;
                    string parameterSeparatorSymbol = definition.ParameterSeparator;
                    var opl = openParenthesisSymbol.Length;
                    var cpl = closeParenthesisSymbol.Length;
                    IParameterRegistry parametersTableReference = context.ParameterRegistry;

                    while (true)
                    {
                        op = source.InvariantCultureIndexOf(
                            openParenthesisSymbol,
                            op + opl);

                        if (op == -1)
                        {
                            // Not a function - this is a normal execution path
                            return null;
                        }

                        if (op == 0)
                        {
                            continue;
                        }

                        var functionHeaderCheck = source.Substring(
                            0,
                            op);

                        var allSymbols = InterpretationContext.Current.AllSymbols;
                        if (allSymbols.Any(
                            (
                                p,
                                check) => check.InvariantCultureEndsWith(p),
                            functionHeaderCheck))
                        {
                            continue;
                        }

                        var functionHeader = functionHeaderCheck.Split(
                            allSymbols,
                            StringSplitOptions.None).Last();

                        var oop = source.InvariantCultureIndexOf(
                            openParenthesisSymbol,
                            op + opl);
                        var cp = source.InvariantCultureIndexOf(
                            closeParenthesisSymbol,
                            op + cpl);

                        while (oop < cp && oop != -1 && cp != -1)
                        {
                            oop = source.InvariantCultureIndexOf(
                                openParenthesisSymbol,
                                oop + opl);
                            cp = source.InvariantCultureIndexOf(
                                closeParenthesisSymbol,
                                cp + cpl);
                        }

                        if (cp == -1)
                        {
                            continue;
                        }

                        var arguments = source.Substring(
                            op + opl,
                            cp - op - opl);
                        var originalArguments = arguments;

                        var q = arguments;
                        while (q != null)
                        {
                            arguments = q;
                            q = ReplaceFunctionsInternal(
                                q,
                                expressionSymbol);
                        }

                        var argPlaceholders = new List<string>();
                        foreach (var s in arguments.Split(
                            new[] { parameterSeparatorSymbol },
                            StringSplitOptions.RemoveEmptyEntries))
                        {
                            TablePopulationGenerator.PopulateTables(
                                s,
                                expressionSymbol);

                            // We check whether or not this is actually a constant
                            argPlaceholders.Add(
                                InterpretationContext.Current.CheckAndAddConstant(
                                    expressionSymbol,
                                    s) ?? (!parametersTableReference.Exists(s)
                                    ? SymbolExpressionGenerator.GenerateSymbolExpression(
                                        s,
                                        false)
                                    : s));
                        }

                        var functionCallBody =
                            $"{functionHeader}{openParenthesisSymbol}{string.Join(parameterSeparatorSymbol, argPlaceholders)}{closeParenthesisSymbol}";
                        var functionCallToReplace =
                            $"{functionHeader}{openParenthesisSymbol}{originalArguments}{closeParenthesisSymbol}";
                        var functionCallItem = SymbolExpressionGenerator.GenerateSymbolExpression(
                            functionCallBody,
                            true);

                        return source.Replace(
                            functionCallToReplace,
                            functionCallItem);
                    }
                }
            }
        }
    }
}