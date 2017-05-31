// <copyright file="SymbolExpressionGenerator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;

namespace IX.Math.Generators
{
    internal static class SymbolExpressionGenerator
    {
        internal static string GenerateSymbolExpression(
            Dictionary<string, Tuple<RawExpressionContainer, SymbolOptimizationData>> symbolTable,
            Dictionary<string, string> reverseSymbolTable,
            string expression,
            bool isFunction = false)
        {
            var expressionContainer = new RawExpressionContainer(expression, isFunction);
            if (!reverseSymbolTable.TryGetValue(expression, out string itemName))
            {
                itemName = $"item{symbolTable.Count}";
                symbolTable.Add(itemName, new Tuple<RawExpressionContainer, SymbolOptimizationData>(expressionContainer, new SymbolOptimizationData()));
                reverseSymbolTable.Add(expressionContainer.Expression, itemName);
            }

            return itemName;
        }
    }
}