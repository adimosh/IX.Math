// <copyright file="FunctionsExtractor.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using IX.Math.ExpressionState;
using IX.Math.Extensibility;
using IX.Math.Generators;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;
using IX.StandardExtensions.Contracts;
using IX.StandardExtensions.Extensions;
using IX.StandardExtensions.Globalization;
using IX.System.Collections.Generic;
using JetBrains.Annotations;

namespace IX.Math.Extraction
{
    /// <summary>
    ///     A class to handle function extraction.
    /// </summary>
    internal static class FunctionsExtractor
    {
        /// <summary>
        /// Replaces functions calls with expression placeholders.
        /// </summary>
        /// <param name="openParenthesis">The symbol of an open parenthesis.</param>
        /// <param name="closeParenthesis">The symbol of a closed parenthesis.</param>
        /// <param name="parameterSeparator">The symbol of a parameter separator.</param>
        /// <param name="constantsTable">The constants table.</param>
        /// <param name="reverseConstantsTable">The reverse-lookup constants table.</param>
        /// <param name="symbolTable">The symbols table.</param>
        /// <param name="reverseSymbolTable">The reverse-lookup symbols table.</param>
        /// <param name="interpreters">The constant interpreters.</param>
        /// <param name="parametersTable">The parameters table.</param>
        /// <param name="expression">The expression before processing.</param>
        /// <param name="allSymbols">All symbols.</param>
        /// <param name="stringFormatters">The string formatters.</param>
        internal static void ReplaceFunctions(
            [NotNull] string openParenthesis,
            [NotNull] string closeParenthesis,
            [NotNull] string parameterSeparator,
            [NotNull] Dictionary<string, ConstantNodeBase> constantsTable,
            [NotNull] Dictionary<string, string> reverseConstantsTable,
            [NotNull] Dictionary<string, ExpressionSymbol> symbolTable,
            [NotNull] Dictionary<string, string> reverseSymbolTable,
            [NotNull] LevelDictionary<Type, IConstantInterpreter> interpreters,
            [NotNull] IDictionary<string, ExternalParameterNode> parametersTable,
            [NotNull] string expression,
            [NotNull] string[] allSymbols,
            [NotNull] List<IStringFormatter> stringFormatters)
        {
            // Validate parameters
            Requires.NotNullOrWhiteSpace(
                openParenthesis,
                nameof(openParenthesis));
            Requires.NotNullOrWhiteSpace(
                closeParenthesis,
                nameof(closeParenthesis));
            Requires.NotNullOrWhiteSpace(
                parameterSeparator,
                nameof(parameterSeparator));
            Requires.NotNull(
                constantsTable,
                nameof(constantsTable));
            Requires.NotNull(
                reverseConstantsTable,
                nameof(reverseConstantsTable));
            Requires.NotNull(
                symbolTable,
                nameof(symbolTable));
            Requires.NotNull(
                reverseSymbolTable,
                nameof(reverseSymbolTable));
            Requires.NotNull(
                interpreters,
                nameof(interpreters));
            Requires.NotNull(
                parametersTable,
                nameof(parametersTable));
            Requires.NotNullOrWhiteSpace(
                expression,
                nameof(expression));
            Requires.NotNull(
                allSymbols,
                nameof(allSymbols));
            Requires.NotNull(
                allSymbols,
                nameof(allSymbols));

            // Replace the main expression
            ReplaceOneFunction(
                string.Empty,
                openParenthesis,
                closeParenthesis,
                parameterSeparator,
                constantsTable,
                reverseConstantsTable,
                symbolTable,
                reverseSymbolTable,
                interpreters,
                parametersTable,
                expression,
                allSymbols,
                stringFormatters);

            for (var i = 1; i < symbolTable.Count; i++)
            {
                // Replace sub-expressions
                ReplaceOneFunction(
                    $"item{i.ToString(CultureInfo.InvariantCulture).PadLeft(4, '0')}",
                    openParenthesis,
                    closeParenthesis,
                    parameterSeparator,
                    constantsTable,
                    reverseConstantsTable,
                    symbolTable,
                    reverseSymbolTable,
                    interpreters,
                    parametersTable,
                    expression,
                    allSymbols,
                    stringFormatters);
            }

            static void ReplaceOneFunction(
                string key,
                string outerOpenParanthesisSymbol,
                string outerCloseParanthesisSymbol,
                string outerParameterSeparatorSymbol,
                Dictionary<string, ConstantNodeBase> outerConstantsTableReference,
                Dictionary<string, string> outerReverseConstantsTableReference,
                Dictionary<string, ExpressionSymbol> outerSymbolTableReference,
                Dictionary<string, string> outerReverseSymbolTableRefeerence,
                LevelDictionary<Type, IConstantInterpreter> interpreters,
                IDictionary<string, ExternalParameterNode> outerParametersTableReference,
                string outerExpressionSymbol,
                string[] outerAllSymbolsSymbols,
                List<IStringFormatter> outerStringFormatters)
            {
                ExpressionSymbol symbol = outerSymbolTableReference[key];
                if (symbol.IsFunctionCall)
                {
                    return;
                }

                var replaced = symbol.Expression;
                while (replaced != null)
                {
                    outerSymbolTableReference[key].Expression = replaced;
                    replaced = ReplaceFunctions(
                        replaced,
                        outerOpenParanthesisSymbol,
                        outerCloseParanthesisSymbol,
                        outerParameterSeparatorSymbol,
                        outerConstantsTableReference,
                        outerReverseConstantsTableReference,
                        outerSymbolTableReference,
                        outerReverseSymbolTableRefeerence,
                        interpreters,
                        outerParametersTableReference,
                        outerExpressionSymbol,
                        outerAllSymbolsSymbols,
                        outerStringFormatters);
                }

                static string ReplaceFunctions(
                    string source,
                    string openParanthesisSymbol,
                    string closeParanthesisSymbol,
                    string parameterSeparatorSymbol,
                    Dictionary<string, ConstantNodeBase> constantsTableReference,
                    Dictionary<string, string> reverseConstantsTableReference,
                    Dictionary<string, ExpressionSymbol> symbolTableReference,
                    Dictionary<string, string> reverseSymbolTableReference,
                    LevelDictionary<Type, IConstantInterpreter> interpretersReference,
                    IDictionary<string, ExternalParameterNode> parametersTableReference,
                    string expressionSymbol,
                    string[] allSymbolsSymbols,
                    List<IStringFormatter> innerStringFormatters)
                {
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

                        if (allSymbolsSymbols.Any(
                            (
                                p,
                                check) => check.InvariantCultureEndsWith(p), functionHeaderCheck))
                        {
                            continue;
                        }

                        var functionHeader = functionHeaderCheck.Split(
                            allSymbolsSymbols,
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
                            q = ReplaceFunctions(
                                q,
                                openParanthesisSymbol,
                                closeParanthesisSymbol,
                                parameterSeparatorSymbol,
                                constantsTableReference,
                                reverseConstantsTableReference,
                                symbolTableReference,
                                reverseSymbolTableReference,
                                interpretersReference,
                                parametersTableReference,
                                expressionSymbol,
                                allSymbolsSymbols,
                                innerStringFormatters);
                        }

                        var argPlaceholders = new List<string>();
                        foreach (var s in arguments.Split(
                            new[] { parameterSeparatorSymbol },
                            StringSplitOptions.RemoveEmptyEntries))
                        {
                            TablePopulationGenerator.PopulateTables(
                                s,
                                constantsTableReference,
                                reverseConstantsTableReference,
                                symbolTableReference,
                                reverseSymbolTableReference,
                                parametersTableReference,
                                interpretersReference,
                                expressionSymbol,
                                openParanthesisSymbol,
                                allSymbolsSymbols,
                                innerStringFormatters);

                            // We check whether or not this is actually a constant
                            argPlaceholders.Add(
                                ConstantsGenerator.CheckAndAdd(
                                    constantsTableReference,
                                    reverseConstantsTableReference,
                                    interpretersReference,
                                    expressionSymbol,
                                    s,
                                    innerStringFormatters) ?? (!parametersTableReference.ContainsKey(s)
                                    ? SymbolExpressionGenerator.GenerateSymbolExpression(
                                        symbolTableReference,
                                        reverseSymbolTableReference,
                                        s,
                                        false)
                                    : s));
                        }

                        var functionCallBody =
                            $"{functionHeader}{openParanthesisSymbol}{string.Join(parameterSeparatorSymbol, argPlaceholders)}{closeParanthesisSymbol}";
                        var functionCallToReplace =
                            $"{functionHeader}{openParanthesisSymbol}{originalArguments}{closeParanthesisSymbol}";
                        var functionCallItem = SymbolExpressionGenerator.GenerateSymbolExpression(
                            symbolTableReference,
                            reverseSymbolTableReference,
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