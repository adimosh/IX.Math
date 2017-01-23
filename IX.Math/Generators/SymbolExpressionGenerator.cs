// <copyright file="SymbolExpressionGenerator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IX.Math.Generators
{
    internal static class SymbolExpressionGenerator
    {
        internal static string GenerateSymbolExpression(WorkingExpressionSet workingSet, string expression, bool isFunction = false, bool isString = false)
        {
            var expressionContainer = new RawExpressionContainer(expression, isFunction, isString);
            if (!workingSet.ReverseSymbolTable.TryGetValue(expression, out string itemName))
            {
                itemName = $"item{workingSet.SymbolTable.Count}";
                workingSet.SymbolTable.Add(itemName, expressionContainer);
                workingSet.ReverseSymbolTable.Add(expressionContainer.Expression, itemName);
            }

            return itemName;
        }
    }
}