// <copyright file="WorkingExpressionSet.TablePopulationGeneration.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using IX.Math.Nodes.Parameters;
using IX.StandardExtensions.Contracts;
using IX.StandardExtensions.Globalization;
using JetBrains.Annotations;

namespace IX.Math.WorkingSet
{
    internal partial class WorkingExpressionSet
    {
        /// <summary>
        /// Populates tables according to the currently-processed expression.
        /// </summary>
        /// <param name="processedExpression">The expression that is being processed.</param>
        private void PopulateTables(
            [NotNull] string processedExpression)
        {
            // Validate parameters
            Requires.NotNullOrWhiteSpace(
                processedExpression,
                nameof(processedExpression));

            string openParenthesis = this.definition.Parentheses.Open;

            // Split expression by all symbols
            string[] expressions = processedExpression.Split(
                this.allSymbols,
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var exp in expressions)
            {
                if (this.constantsTable.ContainsKey(exp))
                {
                    // We have a constant
                    continue;
                }

                if (this.reverseConstantsTable.ContainsKey(exp))
                {
                    // We have a constant that has bee evaluated before
                    continue;
                }

                if (this.parameterRegistry.ContainsKey(exp))
                {
                    // We have a parameter
                    continue;
                }

                if (this.symbolTable.ContainsKey(exp))
                {
                    // We have an already-existing symbol
                    continue;
                }

                if (this.reverseSymbolTable.ContainsKey(exp))
                {
                    // We have a symbol value that has been evaluated before
                    continue;
                }

                if (exp.Contains(openParenthesis))
                {
                    // We have a part of a function
                    continue;
                }

                // Let's check whether it is a constant
                if (this.CheckAndAdd(exp) != null)
                {
                    continue;
                }

                // It's not a constant, nor something ever encountered before
                // Therefore it should be a parameter
                var exp2 = exp;

                // We check whether or not we have an indexer in the constant name
                if (exp2.CurrentCultureEndsWith(this.definition.IndexerIndicators.Close))
                {
                    var openIndex = exp2.IndexOf(this.definition.IndexerIndicators.Open);

                    if (openIndex != -1)
                    {
                        var constantKey = exp2.Substring(
                            openIndex + 1,
                            exp2.Length - openIndex - 2);

                        if (this.constantsTable.TryGetValue(
                            constantKey,
                            out var constantValue))
                        {
                            // We first replace back the constants in the parameter registry
                            exp2 =
                                $"{exp2.Substring(0, openIndex)}{this.definition.IndexerIndicators.Open}{constantValue.OriginalStringValue ?? constantValue.ValueAsString}{this.definition.IndexerIndicators.Close}";

                            if (this.parameterRegistry.ContainsKey(exp2))
                            {
                                // We have a parameter
                                continue;
                            }
                        }
                    }
                }

                this.parameterRegistry.Add(exp, new ExternalParameterNode(exp2, this.stringFormatters));
            }
        }
    }
}