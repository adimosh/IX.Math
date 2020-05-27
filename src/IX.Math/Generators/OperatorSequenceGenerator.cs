// <copyright file="OperatorSequenceGenerator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using IX.StandardExtensions.Contracts;
using JetBrains.Annotations;

namespace IX.Math.Generators
{
    internal static class OperatorSequenceGenerator
    {
        [NotNull]
        internal static List<Tuple<int, int, string>> GetOperatorsInOrderInExpression(
            [NotNull] string expression,
            [NotNull] KeyValuePair<int, string[]>[] operators)
        {
            Requires.NotNull(
                expression,
                nameof(expression));
            Requires.NotNull(
                operators,
                nameof(operators));

            var indexes = new List<Tuple<int, int, string>>();

            foreach (KeyValuePair<int, string[]> level in operators)
            {
                foreach (var op in level.Value)
                {
                    var index = 0 - op.Length;

                    restartFindProcess:
                    index = expression.IndexOf(
                        op,
                        index + op.Length,
                        StringComparison.Ordinal);

                    if (index != -1)
                    {
                        indexes.Add(
                            new Tuple<int, int, string>(
                                level.Key,
                                index,
                                op));

                        goto restartFindProcess;
                    }
                }
            }

            return indexes;
        }
    }
}