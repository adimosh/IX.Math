// <copyright file="ConstantsGenerator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq;
using IX.Math.Nodes.Constants;

namespace IX.Math.Generators
{
    internal static class ConstantsGenerator
    {
        public static string GenerateStringConstant(WorkingExpressionSet set, string content)
        {
            if (set.ReverseConstantsTable.TryGetValue(content, out string key))
            {
                return key;
            }
            else
            {
                string name = GenerateName(set);
                set.ConstantsTable.Add(name, new StringNode(content.Substring(set.Definition.StringIndicator.Length, content.Length - set.Definition.StringIndicator.Length)));
                return name;
            }
        }

        private static string GenerateName(WorkingExpressionSet set)
        {
            int index = int.Parse(set.ConstantsTable.Keys.LastOrDefault()?.Substring(4) ?? "0");

            do
            {
                index++;
            }
            while (set.Expression.Contains($"Const{index}"));

            return $"Const{index}";
        }
    }
}