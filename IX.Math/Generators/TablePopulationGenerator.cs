// <copyright file="TablePopulationGenerator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;

namespace IX.Math.Generators
{
    internal static class TablePopulationGenerator
    {
        internal static void PopulateTables(
            string p,
            WorkingExpressionSet workingSet)
        {
            var expressions = p.Split(workingSet.AllOperatorsInOrder, StringSplitOptions.RemoveEmptyEntries);

            foreach (var exp in expressions)
            {
                if (workingSet.ConstantsTable.ContainsKey(exp))
                {
                    continue;
                }

                if (workingSet.ReverseConstantsTable.ContainsKey(exp))
                {
                    continue;
                }

                if (workingSet.ParametersTable.ContainsKey(exp.ToLower()))
                {
                    continue;
                }

                if (workingSet.SymbolTable.ContainsKey(exp))
                {
                    continue;
                }

                if (workingSet.ReverseSymbolTable.ContainsKey(exp))
                {
                    continue;
                }

                if (exp.Contains(workingSet.Definition.Parantheses.Item1))
                {
                    continue;
                }

                if (ConstantsGenerator.CheckAndAdd(workingSet.ConstantsTable, workingSet.ReverseConstantsTable, workingSet.Expression, exp) != null)
                {
                    continue;
                }

                ParametersGenerator.GenerateParameter(workingSet.ParametersTable, exp);
            }
        }
    }
}