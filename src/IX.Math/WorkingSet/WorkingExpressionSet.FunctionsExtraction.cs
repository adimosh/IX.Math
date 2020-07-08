// <copyright file="WorkingExpressionSet.FunctionsExtraction.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using IX.Math.ExpressionState;
using IX.Math.Extensibility;
using IX.Math.Generators;
using IX.Math.Nodes.Parameters;
using IX.StandardExtensions.Contracts;
using IX.StandardExtensions.Extensions;
using IX.StandardExtensions.Globalization;
using IX.System.Collections.Generic;
using JetBrains.Annotations;

namespace IX.Math.WorkingSet
{
    internal partial class WorkingExpressionSet
    {
        /// <summary>
        /// Replaces functions calls with expression placeholders.
        /// </summary>
        /// <param name="openParenthesis">The symbol of an open parenthesis.</param>
        /// <param name="closeParenthesis">The symbol of a closed parenthesis.</param>
        /// <param name="parameterSeparator">The symbol of a parameter separator.</param>
        /// <param name="interpreters">The constant interpreters.</param>
        /// <param name="parametersTable">The parameters table.</param>
        /// <param name="expression">The expression before processing.</param>
        /// <param name="allSymbols">All symbols.</param>
        /// <param name="stringFormatters">The string formatters.</param>
        /// <param name="mathDefinition">The math definition.</param>
        private void ReplaceFunctions(
            [NotNull] string openParenthesis,
            [NotNull] string closeParenthesis,
            [NotNull] string parameterSeparator,
            [NotNull] LevelDictionary<Type, IConstantInterpreter> interpreters,
            [NotNull] IDictionary<string, ExternalParameterNode> parametersTable,
            [NotNull] string expression,
            [NotNull] string[] allSymbols,
            [NotNull] List<IStringFormatter> stringFormatters,
            [NotNull] MathDefinition mathDefinition)
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
                mathDefinition,
                nameof(mathDefinition));

            // Replace the main expression
            ReplaceOneFunction(
                string.Empty,
                openParenthesis,
                closeParenthesis,
                parameterSeparator,
                interpreters,
                parametersTable,
                expression,
                allSymbols,
                stringFormatters,
                mathDefinition);

            for (var i = 1; i < this.symbolTable.Count; i++)
            {
                // Replace sub-expressions
                ReplaceOneFunction(
                    $"item{i.ToString(CultureInfo.InvariantCulture).PadLeft(4, '0')}",
                    openParenthesis,
                    closeParenthesis,
                    parameterSeparator,
                    interpreters,
                    parametersTable,
                    expression,
                    allSymbols,
                    stringFormatters,
                    mathDefinition);
            }

            void ReplaceOneFunction(
                string key,
                string outerOpenParanthesisSymbol,
                string outerCloseParanthesisSymbol,
                string outerParameterSeparatorSymbol,
                LevelDictionary<Type, IConstantInterpreter> interpreters,
                IDictionary<string, ExternalParameterNode> outerParametersTableReference,
                string outerExpressionSymbol,
                string[] outerAllSymbolsSymbols,
                List<IStringFormatter> outerStringFormatters,
                MathDefinition outerMathDefinition)
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
                    replaced = ReplaceFunctions(
                        replaced,
                        outerOpenParanthesisSymbol,
                        outerCloseParanthesisSymbol,
                        outerParameterSeparatorSymbol,
                        interpreters,
                        outerParametersTableReference,
                        outerExpressionSymbol,
                        outerAllSymbolsSymbols,
                        outerStringFormatters,
                        outerMathDefinition);
                }

                string ReplaceFunctions(
                    string source,
                    string openParanthesisSymbol,
                    string closeParanthesisSymbol,
                    string parameterSeparatorSymbol,
                    LevelDictionary<Type, IConstantInterpreter> interpretersReference,
                    IDictionary<string, ExternalParameterNode> parametersTableReference,
                    string expressionSymbol,
                    string[] allSymbolsSymbols,
                    List<IStringFormatter> innerStringFormatters,
                    MathDefinition innerMathDefinition)
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
                                interpretersReference,
                                parametersTableReference,
                                expressionSymbol,
                                allSymbolsSymbols,
                                innerStringFormatters,
                                innerMathDefinition);
                        }

                        var argPlaceholders = new List<string>();
                        foreach (var s in arguments.Split(
                            new[] { parameterSeparatorSymbol },
                            StringSplitOptions.RemoveEmptyEntries))
                        {
                            this.PopulateTables(
                                s,
                                parametersTableReference,
                                interpretersReference,
                                expressionSymbol,
                                openParanthesisSymbol);

                            // We check whether or not this is actually a constant
                            argPlaceholders.Add(
                                ConstantsGenerator.CheckAndAdd(
                                    this.constantsTable,
                                    this.reverseConstantsTable,
                                    interpretersReference,
                                    expressionSymbol,
                                    s,
                                    innerStringFormatters,
                                    innerMathDefinition) ?? (!parametersTableReference.ContainsKey(s)
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
