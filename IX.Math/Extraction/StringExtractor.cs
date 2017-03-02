// <copyright file="StringExtractor.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Collections.Generic;
using IX.Math.Generators;
using IX.Math.Nodes.Constants;

namespace IX.Math.Extraction
{
    internal static class StringExtractor
    {
        internal static string ExtractStringConstants(
            IDictionary<string, ConstantNodeBase> constantsTable,
            IDictionary<string, string> reverseConstantsTable,
            string originalExpression,
            string stringIndicator)
        {
            string process = originalExpression;

            while (true)
            {
                int op = process.IndexOf(stringIndicator);

                if (op == -1)
                {
                    break;
                }

                int cp = process.IndexOf(stringIndicator, op + stringIndicator.Length);

                escapeRoute:
                if (cp == -1 || (cp + stringIndicator.Length) > process.Length)
                {
                    break;
                }

                if (process.Substring(cp + stringIndicator.Length).StartsWith(stringIndicator))
                {
                    cp = process.IndexOf(stringIndicator, cp + (stringIndicator.Length * 2));
                    goto escapeRoute;
                }

                string itemName = ConstantsGenerator.GenerateStringConstant(
                    constantsTable,
                    reverseConstantsTable,
                    process,
                    stringIndicator,
                    process.Substring(op, cp - op));

                process = $"{process.Substring(0, op)}{itemName}{process.Substring(cp + stringIndicator.Length)}";
            }

            return process;
        }
    }
}