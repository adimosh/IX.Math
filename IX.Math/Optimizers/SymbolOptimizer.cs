﻿// <copyright file="SymbolOptimizer.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using IX.Math.Nodes;

namespace IX.Math.Optimizers
{
    internal class SymbolOptimizer// : ISymbolOptimizer
    {
#pragma warning disable SA1008 // Opening parenthesis must be spaced correctly
#pragma warning disable SA1009 // Closing parenthesis must be spaced correctly
        public (List<NodeBase> BodyExpressions, Dictionary<string, ParameterNodeBase> VariableExpressions) OptimizeSymbols(
            Dictionary<string, RawExpressionContainer> symbols,
            Dictionary<string, string> reverseSymbols,
            Dictionary<string, ConstantNodeBase> constantsTable,
            Dictionary<string, string> reverseConstants,
            Func<RawExpressionContainer, NodeBase> expressionGenerator,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            var bodyExpressions = new List<NodeBase>();
            var variableExpressions = new List<ParameterNodeBase>();

            while (symbols.Count > 1)
            {
                KeyValuePair<string, (RawExpressionContainer Container, SymbolOptimizationData OptimizationData)>[] countedSymbols
                    = CreateSymbolOptimizationCounters(symbols).Where(q => q.Value.OptimizationData.Contains == 0).ToArray();

                if (countedSymbols.Length == 0)
                {
                    break;
                }

                foreach (KeyValuePair<string, (RawExpressionContainer Container, SymbolOptimizationData OptimizationData)> p in countedSymbols)
                {
                    NodeBase expression = expressionGenerator(p.Value.Container)?.Simplify();

                    if (expression == null)
                    {
                        throw new ExpressionNotValidLogicallyException();
                    }

                    if (expression is ConstantNodeBase constantExpression)
                    {
                        symbols.Remove(p.Key);
                        reverseSymbols.Remove(p.Value.Container.Expression);

                        constantsTable.Add(p.Key, constantExpression);
                        reverseConstants.Add(p.Value.Container.Expression, p.Key);
                    }
                    else
                    {

                    }
                }
            }

            return (bodyExpressions, null);

            Dictionary<string, (RawExpressionContainer Container, SymbolOptimizationData OptimizationData)>
                CreateSymbolOptimizationCounters(Dictionary<string, RawExpressionContainer> symb)
            {
                Dictionary<string, (RawExpressionContainer Container, SymbolOptimizationData OptimizationData)> futureOptimalSymbols =
                    symb.ToDictionary(p => p.Key, p => (p.Value, new SymbolOptimizationData()));

                foreach (var p in futureOptimalSymbols)
                {
                    foreach (var q in futureOptimalSymbols.Where(z => z.Key != p.Key))
                    {
                        if (q.Value.Item1.Expression.Contains(p.Key))
                        {
                            p.Value.Item2.ContainedIn++;
                            q.Value.Item2.Contains++;
                        }
                    }
                }

                return futureOptimalSymbols;
            }
        }
#pragma warning restore SA1008 // Opening parenthesis must be spaced correctly
#pragma warning restore SA1009 // Closing parenthesis must be spaced correctly
    }
}