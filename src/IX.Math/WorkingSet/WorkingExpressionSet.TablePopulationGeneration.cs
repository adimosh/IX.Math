// <copyright file="WorkingExpressionSet.TablePopulationGeneration.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using IX.Math.Extensibility;
using IX.Math.Generators;
using IX.Math.Nodes.Parameters;
using IX.StandardExtensions.Contracts;
using IX.System.Collections.Generic;
using JetBrains.Annotations;

namespace IX.Math.WorkingSet
{
    internal partial class WorkingExpressionSet
    {
        /// <summary>
        /// Populates tables according to the currently-processed expression.
        /// </summary>
        /// <param name="processedExpression">The expression that is being processed.</param>
        /// <param name="parameterRegistry">The parameters registry.</param>
        /// <param name="interpreters">The constant interpreters.</param>
        /// <param name="originalExpression">The expression before processing.</param>
        /// <param name="openParenthesis">The symbol of an open parenthesis.</param>
        private void PopulateTables(
            [NotNull] string processedExpression,
            [NotNull] IDictionary<string, ExternalParameterNode> parameterRegistry,
            [NotNull] LevelDictionary<Type, IConstantInterpreter> interpreters,
            [NotNull] string originalExpression,
            [NotNull] string openParenthesis)
        {
            // Validate parameters
            Requires.NotNullOrWhiteSpace(
                processedExpression,
                nameof(processedExpression));
            Requires.NotNull(
                parameterRegistry,
                nameof(parameterRegistry));
            Requires.NotNull(
                interpreters,
                nameof(interpreters));
            Requires.NotNullOrWhiteSpace(
                originalExpression,
                nameof(originalExpression));
            Requires.NotNullOrWhiteSpace(
                openParenthesis,
                nameof(openParenthesis));

            // Split expression by all symbols
            string[] expressions = processedExpression.Split(
                this.AllSymbols,
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

                if (parameterRegistry.ContainsKey(exp))
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
                if (ConstantsGenerator.CheckAndAdd(
                        this.constantsTable,
                        this.reverseConstantsTable,
                        interpreters,
                        originalExpression,
                        exp,
                        this.StringFormatters,
                        this.definition) != null)
                {
                    continue;
                }

                // It's not a constant, nor something ever encountered before
                // Therefore it should be a parameter
                parameterRegistry.Add(exp, new ExternalParameterNode(exp, this.StringFormatters));
            }
        }
    }
}