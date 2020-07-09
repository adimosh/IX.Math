// <copyright file="WorkingExpressionSet.TablePopulationGeneration.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using IX.Math.Nodes.Parameters;
using IX.StandardExtensions.Contracts;
using JetBrains.Annotations;

namespace IX.Math.WorkingSet
{
    internal partial class WorkingExpressionSet
    {
        /// <summary>
        /// Populates tables according to the currently-processed expression.
        /// </summary>
        /// <param name="processedExpression">The expression that is being processed.</param>
        /// <param name="originalExpression">The expression before processing.</param>
        private void PopulateTables(
            [NotNull] string processedExpression,
            [NotNull] string originalExpression)
        {
            // Validate parameters
            Requires.NotNullOrWhiteSpace(
                processedExpression,
                nameof(processedExpression));
            Requires.NotNullOrWhiteSpace(
                originalExpression,
                nameof(originalExpression));

            string openParenthesis = this.definition.Parentheses.Item1;

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

                if (this.ParameterRegistry.ContainsKey(exp))
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
                if (this.CheckAndAdd(
                        originalExpression,
                        exp) != null)
                {
                    continue;
                }

                // It's not a constant, nor something ever encountered before
                // Therefore it should be a parameter
                this.ParameterRegistry.Add(exp, new ExternalParameterNode(exp, this.StringFormatters));
            }
        }
    }
}