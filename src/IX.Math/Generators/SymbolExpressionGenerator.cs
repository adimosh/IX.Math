// <copyright file="SymbolExpressionGenerator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Globalization;
using IX.Math.ExpressionState;
using IX.Math.Interpretation;

namespace IX.Math.Generators
{
    internal static class SymbolExpressionGenerator
    {
        internal static string GenerateSymbolExpression(
            string expression,
            bool isFunction)
        {
            var reverseSymbolTable = InterpretationContext.Current.ReverseSymbolTable;
            if (reverseSymbolTable.TryGetValue(
                expression,
                out var itemName))
            {
                return itemName;
            }

            var symbolTable = InterpretationContext.Current.SymbolTable;
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