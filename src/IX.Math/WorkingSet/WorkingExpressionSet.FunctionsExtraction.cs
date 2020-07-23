// <copyright file="WorkingExpressionSet.FunctionsExtraction.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using IX.Math.ExpressionState;
using IX.Math.Generators;
using IX.StandardExtensions.Extensions;
using IX.StandardExtensions.Globalization;

namespace IX.Math.WorkingSet
{
    internal partial class WorkingExpressionSet
    {
        /// <summary>
        /// Replaces functions calls with expression placeholders.
        /// </summary>
        private void ReplaceFunctions()
        {
            // Replace the main expression
            ReplaceOneFunction(
                string.Empty);

            for (var i = 1; i < this.symbolTable.Count; i++)
            {
                // Replace sub-expressions
                ReplaceOneFunction(
                    $"item{i.ToString(CultureInfo.InvariantCulture).PadLeft(4, '0')}");
            }

            void ReplaceOneFunction(string key)
            {
                ExpressionSymbol symbol = this.symbolTable[key];
                if (symbol.IsFunctionCall)
                {
                    return;
                }

                var replaced = symbol.Expression;
                while (replaced != null)
                {
                    this.symbolTable[key].Expression = replaced;
                    replaced = ReplaceFunctions(replaced);
                }

                string ReplaceFunctions(string source)
                {
                    string openParanthesisSymbol = this.definition.Parentheses.Open;
                    string closeParanthesisSymbol = this.definition.Parentheses.Close;
                    string parameterSeparatorSymbol = this.definition.ParameterSeparator;

                    var op = -1;
                    var opl = openParanthesisSymbol.Length;
                    var cpl = closeParanthesisSymbol.Length;

                    while (true)
                    {
                        op = source.InvariantCultureIndexOf(
                            openParanthesisSymbol,
                            op + opl);

                        if (op == -1)
                        {
                            return null;
                        }

                        if (op == 0)
                        {
                            continue;
                        }

                        var functionHeaderCheck = source.Substring(
                            0,
                            op);

                        if (this.allSymbols.Any(
                            (
                                p,
                                check) => check.InvariantCultureEndsWith(p), functionHeaderCheck))
                        {
                            continue;
                        }

                        var functionHeader = functionHeaderCheck.Split(
                            this.allSymbols,
                            StringSplitOptions.None).Last();

                        var oop = source.InvariantCultureIndexOf(
                            openParanthesisSymbol,
                            op + opl);
                        var cp = source.InvariantCultureIndexOf(
                            closeParanthesisSymbol,
                            op + cpl);

                        while (oop < cp && oop != -1 && cp != -1)
                        {
                            oop = source.InvariantCultureIndexOf(
                                openParanthesisSymbol,
                                oop + opl);
                            cp = source.InvariantCultureIndexOf(
                                closeParanthesisSymbol,
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
                            q = ReplaceFunctions(q);
                        }

                        var argPlaceholders = new List<string>();
                        foreach (var s in arguments.Split(
                            new[] { parameterSeparatorSymbol },
                            StringSplitOptions.RemoveEmptyEntries))
                        {
                            this.PopulateTables(s);

                            // We check whether or not this is actually a constant
                            argPlaceholders.Add(
                                this.CheckAndAdd(s) ??
                                (!this.parameterRegistry.ContainsKey(s)
                                    ? SymbolExpressionGenerator.GenerateSymbolExpression(
                                        this.symbolTable,
                                        this.reverseSymbolTable,
                                        s,
                                        false)
                                    : s));
                        }

                        var functionCallBody =
                            $"{functionHeader}{openParanthesisSymbol}{string.Join(parameterSeparatorSymbol, argPlaceholders)}{closeParanthesisSymbol}";
                        var functionCallToReplace =
                            $"{functionHeader}{openParanthesisSymbol}{originalArguments}{closeParanthesisSymbol}";
                        var functionCallItem = SymbolExpressionGenerator.GenerateSymbolExpression(
                            this.symbolTable,
                            this.reverseSymbolTable,
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