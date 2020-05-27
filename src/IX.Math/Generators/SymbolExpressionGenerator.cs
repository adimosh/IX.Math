// <copyright file="SymbolExpressionGenerator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Collections.Generic;
using System.Globalization;
using IX.Math.ExpressionState;
using IX.StandardExtensions.Contracts;
using JetBrains.Annotations;

namespace IX.Math.Generators
{
    internal static class SymbolExpressionGenerator
    {
        internal static string GenerateSymbolExpression(
            [NotNull] Dictionary<string, ExpressionSymbol> symbolTable,
            [NotNull] Dictionary<string, string> reverseSymbolTable,
            [NotNull] string expression,
            bool isFunction)
        {
            Requires.NotNull(
                symbolTable,
                nameof(symbolTable));
            Requires.NotNull(
                reverseSymbolTable,
                nameof(reverseSymbolTable));
            Requires.NotNull(
                expression,
                nameof(expression));

            if (reverseSymbolTable.TryGetValue(
                expression,
                out var itemName))
            {
                return itemName;
            }

            itemName = $"item{symbolTable.Count.ToString(CultureInfo.InvariantCulture).PadLeft(4, '0')}";
            ExpressionSymbol symb = isFunction
                ? ExpressionSymbol.GenerateFunctionCall(
                    itemName,
                    expression)
                : ExpressionSymbol.GenerateSymbol(
                    itemName,
                    expression);

            symbolTable.Add(
                itemName,
                symb);
            reverseSymbolTable.Add(
                symb.Expression,
                itemName);

            return itemName;
        }
    }
}