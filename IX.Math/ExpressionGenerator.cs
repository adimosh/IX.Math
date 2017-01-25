// <copyright file="ExpressionGenerator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq;
using System.Text.RegularExpressions;
using IX.Math.BuiltIn;
using IX.Math.BuiltIn.Constants;
using IX.Math.Extraction;
using IX.Math.Generators;
using IX.Math.PlatformMitigation;
using IX.Math.Formatters;

namespace IX.Math
{
    internal static class ExpressionGenerator
    {
        internal static void CreateBody(
            WorkingExpressionSet workingSet)
        {
            workingSet.CancellationToken.ThrowIfCancellationRequested();

            // Strings
            workingSet.Expression = StringExtractor.ExtractStringConstants(
                workingSet.ConstantsTable,
                workingSet.ReverseConstantsTable,
                workingSet.Expression,
                workingSet.Definition.StringIndicator);

            workingSet.CancellationToken.ThrowIfCancellationRequested();

            workingSet.Expression = SubExpressionFormatter.Cleanup(workingSet.Expression);

            workingSet.SymbolTable.Add(string.Empty, new RawExpressionContainer(workingSet.Expression));

            // Prepares expression and takes care of operators to ensure that they are all OK and usable
            workingSet.Initialize();

            workingSet.SymbolTable[string.Empty] = new RawExpressionContainer(workingSet.Expression);

            workingSet.CancellationToken.ThrowIfCancellationRequested();

            // Break expression based on function calls
            FunctionExpressionGenerator.ReplaceFunctions(workingSet);

            workingSet.CancellationToken.ThrowIfCancellationRequested();

            // Break by parantheses
            ParanthesesExpressionGenerator.FormatParantheses(workingSet);

            workingSet.CancellationToken.ThrowIfCancellationRequested();

            // Populating symbol tables
            workingSet.SymbolTable.Where(p => !p.Value.IsFunctionCall && !p.Value.IsString).Select(p => p.Value.Expression).ToList().ForEach(p =>
                PopulateTables(p, workingSet));

            workingSet.CancellationToken.ThrowIfCancellationRequested();

            // Generate expressions
            try
            {
                workingSet.Body = GenerateExpression(workingSet.SymbolTable[string.Empty], workingSet);
            }
            catch
            {
                workingSet.Body = null;
            }

            if (workingSet.Body == null)
            {
                return;
            }

            workingSet.CancellationToken.ThrowIfCancellationRequested();

            // Set success values and possibly constant values
            if (workingSet.Body is ExpressionTreeNodeConstant)
            {
                if (workingSet.ExternalParameters.Count > 0)
                {
                    // Cannot have external parameters if the expression is itself constant; something somewhere doesn't make sense
                    return;
                }
                else
                {
                    workingSet.ValueIfConstant = ((ExpressionTreeNodeConstant)workingSet.Body).Value;
                    workingSet.Constant = true;
                }
            }
            else if (workingSet.Body is ExpressionTreeNodeParameter)
            {
                workingSet.PossibleString = true;
            }

            workingSet.InternallyValid = true;
            workingSet.Success = true;
        }

        private static void PopulateTables(
            string p,
            WorkingExpressionSet workingSet)
        {
            var expressions = p.Split(workingSet.AllOperatorsInOrder, StringSplitOptions.RemoveEmptyEntries);

            foreach (var exp in expressions)
            {
                if (exp.StartsWith(workingSet.Definition.SpecialSymbolIndicators.Item1) && exp.EndsWith(workingSet.Definition.SpecialSymbolIndicators.Item2))
                {
                    if (SpecialSymbolsLocator.BuiltInSpecialSymbolsAlternateWriting.ContainsKey(exp.Substring(1, exp.Length - 2)))
                    {
                        continue;
                    }
                }

                if (SpecialSymbolsLocator.BuiltInSpecialSymbols.ContainsKey(exp))
                {
                    continue;
                }

                if (workingSet.ConstantsTable.ContainsKey(exp))
                {
                    continue;
                }

                if (workingSet.ExternalParameters.ContainsKey(exp))
                {
                    continue;
                }

                if (workingSet.SymbolTable.ContainsKey(exp))
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

                workingSet.ExternalParameters.Add(exp, new ExpressionTreeNodeParameter(exp));
            }
        }

        private static ExpressionTreeNodeBase[] GenerateExpression(
            RawExpressionContainer[] s,
            WorkingExpressionSet workingSet)
        {
            ExpressionTreeNodeBase[] nodes = new ExpressionTreeNodeBase[s.Length];

            for (int i = 0; i < s.Length; i++)
            {
                var res = GenerateExpression(s[i], workingSet);
                if (res == null)
                {
                    return null;
                }

                nodes[i] = res;
            }

            return nodes;
        }

        private static ExpressionTreeNodeBase GenerateExpression(
            RawExpressionContainer expression,
            WorkingExpressionSet workingSet)
        {
            if (expression.IsString)
            {
                return new ExpressionTreeNodeStringConstant(expression.Expression);
            }

            if (expression.IsFunctionCall)
            {
                return GenerateFunctionCallExpression(
                    expression.Expression,
                    workingSet);
            }

            string s = expression.Expression;
            if (s.StartsWith(workingSet.Definition.SpecialSymbolIndicators.Item1)
                && s.EndsWith(workingSet.Definition.SpecialSymbolIndicators.Item2)
                && SpecialSymbolsLocator.BuiltInSpecialSymbolsAlternateWriting.TryGetValue(s.Substring(1, s.Length - 2), out string actualSymbol))
            {
                return SpecialSymbolsLocator.BuiltInSpecialSymbols[actualSymbol];
            }

            // Check whether expression is special symbol
            if (SpecialSymbolsLocator.BuiltInSpecialSymbols.TryGetValue(s, out var ss))
            {
                return ss;
            }

            // Check whether expression is constant
            if (workingSet.ConstantsTable.TryGetValue(s, out var constantResult))
            {
                return constantResult;
            }

            // Check whether expression is an external parameter
            if (workingSet.ExternalParameters.TryGetValue(s, out var parameterResult))
            {
                return parameterResult;
            }

            // Check whether the expression already exists in the symbols table
            if (workingSet.SymbolTable.TryGetValue(s, out expression))
            {
                return GenerateExpression(expression, workingSet);
            }

            // Check whether the expression is a function call
            if (s.Contains(workingSet.Definition.Parantheses.Item1))
            {
                return GenerateFunctionCallExpression(
                    new RawExpressionContainer(s).Expression,
                    workingSet);
            }

            // Check whether the expression is a binary operator
            foreach (string op in workingSet.BinaryOperatorsInOrder)
            {
                var exp = ExpressionByBinaryOperator(s, op, workingSet);
                if (exp != null)
                {
                    return exp;
                }
            }

            // Check whether the expression is a unary operator
            foreach (string op in workingSet.UnaryOperatorsInOrder)
            {
                var exp = ExpressionByUnaryOperator(s, op, workingSet);
                if (exp != null)
                {
                    return exp;
                }
            }

            return null;
        }

        private static ExpressionTreeNodeBase GenerateFunctionCallExpression(
            string expression,
            WorkingExpressionSet workingSet)
        {
            Match match = workingSet.FunctionRegex.Match(expression);

            try
            {
                if (match.Success)
                {
                    string functionName = match.Groups["functionName"].Value;
                    var expr = match.Groups["expression"].Value
                        .Split(new[] { workingSet.Definition.ParameterSeparator }, StringSplitOptions.None)
                        .Select(p => new RawExpressionContainer(p))
                        .ToArray();

                    var body = GenerateExpression(expr, workingSet);

                    if (body != null
                        && SupportedFunctionsLocator.BuiltInFunctions.TryGetValue(functionName, out var n))
                    {
                        return n().SetOperands(body);
                    }
                }
            }
            catch (Exception)
            {
                return null;
            }

            return null;
        }

        private static ExpressionTreeNodeBase ExpressionByBinaryOperator(
            string s,
            string op,
            WorkingExpressionSet workingSet)
        {
            workingSet.CancellationToken.ThrowIfCancellationRequested();

            var split = s.Split(new[] { op }, StringSplitOptions.None);
            if (split.Length > 1)
            {
                if (string.IsNullOrWhiteSpace(split[0]))
                {
                    // We are having a unary operator - until this is treated differently, let's simply return null
                    return null;
                }
                else
                {
                    // We are having a binary operator
                    try
                    {
                        ExpressionTreeNodeBase left;
                        ExpressionTreeNodeBase right;

                        if (split.Length >= 3 && string.IsNullOrWhiteSpace(split.Take(split.Length - 1).Last()) && !string.IsNullOrWhiteSpace(split.Last()))
                        {
                            // We have a doubling of an operator, probably because a unary operator (like -) is used in conjunction with a binary operator of the same kind
                            left = GenerateExpression(new RawExpressionContainer(string.Join(op, split.Take(split.Length - 2).ToArray())), workingSet);
                            if (left == null)
                            {
                                return null;
                            }

                            right = GenerateExpression(new RawExpressionContainer($"{op}{split.Last()}"), workingSet);
                            if (right == null)
                            {
                                return null;
                            }
                        }
                        else
                        {
                            // We have a normal, regular binary
                            left = GenerateExpression(new RawExpressionContainer(string.Join(op, split.Take(split.Length - 1).ToArray())), workingSet);
                            if (left == null)
                            {
                                return null;
                            }

                            right = GenerateExpression(new RawExpressionContainer(split.Last()), workingSet);
                            if (right == null)
                            {
                                return null;
                            }
                        }

                        if ((left.ReturnType == SupportedValueType.Numeric && right.ReturnType == SupportedValueType.Numeric) ||
                            (left.ReturnType == SupportedValueType.Unknown && right.ReturnType == SupportedValueType.Unknown) ||
                            (left.ReturnType == SupportedValueType.Numeric && right.ReturnType == SupportedValueType.Unknown) ||
                            (left.ReturnType == SupportedValueType.Unknown && right.ReturnType == SupportedValueType.Numeric))
                        {
                            return workingSet.NumericBinaryOperators[op]().SetOperands(left, right);
                        }
                        else if ((left.ReturnType == SupportedValueType.Boolean && right.ReturnType == SupportedValueType.Boolean) ||
                            (left.ReturnType == SupportedValueType.Boolean && right.ReturnType == SupportedValueType.Unknown) ||
                            (left.ReturnType == SupportedValueType.Unknown && right.ReturnType == SupportedValueType.Boolean))
                        {
                            return workingSet.NumericBinaryOperators[op]().SetOperands(left, right);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    catch
                    {
                        return null;
                    }
                }
            }

            return null;
        }

        private static ExpressionTreeNodeBase ExpressionByUnaryOperator(
            string s,
            string op,
            WorkingExpressionSet workingSet)
        {
            workingSet.CancellationToken.ThrowIfCancellationRequested();

            if (s.StartsWith(op))
            {
                try
                {
                    ExpressionTreeNodeBase expr = GenerateExpression(new RawExpressionContainer(s.Substring(op.Length)), workingSet);
                    if (expr == null)
                    {
                        return null;
                    }

                    switch (expr.ReturnType)
                    {
                        case SupportedValueType.Numeric:
                        case SupportedValueType.Unknown:
                            return workingSet.NumericUnaryOperators[op]().SetOperands(expr);
                        case SupportedValueType.Boolean:
                            return workingSet.BooleanUnaryOperators[op]().SetOperands(expr);
                        default:
                            return null;
                    }
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }
    }
}