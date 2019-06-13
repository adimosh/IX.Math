// <copyright file="TablePopulationGenerator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using IX.Math.ExpressionState;
using IX.Math.Nodes;
using IX.Math.Registration;
using IX.StandardExtensions.Contracts;
using JetBrains.Annotations;

namespace IX.Math.Generators
{
    /// <summary>
    ///     A class to handle table population.
    /// </summary>
    internal static class TablePopulationGenerator
    {
        /// <summary>
        ///     Populates tables according to the currently-processed expression.
        /// </summary>
        /// <param name="processedExpression">The expression that is being processed.</param>
        /// <param name="constantsTable">The constants table.</param>
        /// <param name="reverseConstantsTable">The reverse-lookup constants table.</param>
        /// <param name="symbolTable">The symbols table.</param>
        /// <param name="reverseSymbolTable">The reverse-lookup symbols table.</param>
        /// <param name="parameterRegistry">The parameters registry.</param>
        /// <param name="expression">The expression before processing.</param>
        /// <param name="openParenthesis">The symbol of an open parenthesis.</param>
        /// <param name="allSymbols">All symbols on which to split, in order.</param>
        internal static void PopulateTables(
            [NotNull] string processedExpression,
            [NotNull] Dictionary<string, ConstantNodeBase> constantsTable,
            [NotNull] Dictionary<string, string> reverseConstantsTable,
            [NotNull] Dictionary<string, ExpressionSymbol> symbolTable,
            [NotNull] Dictionary<string, string> reverseSymbolTable,
            [NotNull] IParameterRegistry parameterRegistry,
            [NotNull] string expression,
            [NotNull] string openParenthesis,
            [NotNull] string[] allSymbols)
        {
            Contract.RequiresNotNullOrWhitespacePrivate(
                processedExpression,
                nameof(processedExpression));
            Contract.RequiresNotNullPrivate(
                in constantsTable,
                nameof(constantsTable));
            Contract.RequiresNotNullPrivate(
                in reverseConstantsTable,
                nameof(reverseConstantsTable));
            Contract.RequiresNotNullPrivate(
                in symbolTable,
                nameof(symbolTable));
            Contract.RequiresNotNullPrivate(
                in reverseSymbolTable,
                nameof(reverseSymbolTable));
            Contract.RequiresNotNullPrivate(
                in parameterRegistry,
                nameof(parameterRegistry));
            Contract.RequiresNotNullOrWhitespacePrivate(
                expression,
                nameof(expression));
            Contract.RequiresNotNullOrWhitespacePrivate(
                openParenthesis,
                nameof(openParenthesis));
            Contract.RequiresNotNullPrivate(
                in allSymbols,
                nameof(allSymbols));

            string[] expressions = processedExpression.Split(
                allSymbols,
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var exp in expressions)
            {
                if (constantsTable.ContainsKey(exp))
                {
                    continue;
                }

                if (reverseConstantsTable.ContainsKey(exp))
                {
                    continue;
                }

                if (parameterRegistry.Exists(exp))
                {
                    continue;
                }

                if (symbolTable.ContainsKey(exp))
                {
                    continue;
                }

                if (reverseSymbolTable.ContainsKey(exp))
                {
                    continue;
                }

                if (exp.Contains(openParenthesis))
                {
                    continue;
                }

                if (ConstantsGenerator.CheckAndAdd(
                        constantsTable,
                        reverseConstantsTable,
                        expression,
                        exp) != null)
                {
                    continue;
                }

                parameterRegistry.AdvertiseParameter(exp);
            }
        }
    }
}