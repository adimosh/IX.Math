// <copyright file="TablePopulationGenerator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using IX.Math.Interpretation;

namespace IX.Math.Generators
{
    /// <summary>
    ///     A class to handle table population.
    /// </summary>
    internal static class TablePopulationGenerator
    {
        /// <summary>
        /// Populates tables according to the currently-processed expression.
        /// </summary>
        /// <param name="processedExpression">The expression that is being processed.</param>
        /// <param name="originalExpression">The expression before processing.</param>
        internal static void PopulateTables(
             string processedExpression,
             string originalExpression)
        {
            // Split expression by all symbols
            var context = InterpretationContext.Current;

            string[] expressions = processedExpression.Split(
                context.AllSymbols,
                StringSplitOptions.RemoveEmptyEntries);

            foreach (var exp in expressions)
            {
                if (context.ConstantsTable.ContainsKey(exp))
                {
                    // We have a constant
                    continue;
                }

                if (context.ReverseConstantsTable.ContainsKey(exp))
                {
                    // We have a constant that has bee evaluated before
                    continue;
                }

                if (context.ParameterRegistry.Exists(exp))
                {
                    // We have a parameter
                    continue;
                }

                if (context.SymbolTable.ContainsKey(exp))
                {
                    // We have an already-existing symbol
                    continue;
                }

                if (context.ReverseSymbolTable.ContainsKey(exp))
                {
                    // We have a symbol value that has been evaluated before
                    continue;
                }

                if (exp.Contains(context.Definition.Parentheses.Left))
                {
                    // We have a part of a function
                    continue;
                }

                // Let's check whether it is a constant
                if (context.CheckAndAddConstant(
                        originalExpression,
                        exp) != null)
                {
                    continue;
                }

                // It's not a constant, nor something ever encountered before
                // Therefore it should be a parameter
                context.ParameterRegistry.AdvertiseParameter(exp);
            }
        }
    }
}