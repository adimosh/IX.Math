using IX.Math.BuiltIn;
using IX.Math.PlatformMitigation;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace IX.Math
{
    internal static class ExpressionGenerator
    {
        internal static void CreateBody(
            WorkingExpressionSet workingSet,
            WorkingDefinition definition)
        {
            workingSet.CancellationToken.ThrowIfCancellationRequested();

            // Strings
            StringExpressionGenerator.ReplaceStrings(workingSet, definition);

            workingSet.CancellationToken.ThrowIfCancellationRequested();

            // Prepares expression and takes care of operators to ensure that they are all OK and usable
            PrepareMathDefinitionAndExpression(workingSet, definition);

            definition.Initialize();

            workingSet.SymbolTable.Add(string.Empty, new RawExpressionContainer(workingSet.InitialExpression));

            workingSet.CancellationToken.ThrowIfCancellationRequested();

            // Break expression based on function calls
            FunctionExpressionGenerator.ReplaceFunctions(workingSet, definition);

            workingSet.CancellationToken.ThrowIfCancellationRequested();

            // Break by parantheses
            ParanthesesExpressionGenerator.FormatParantheses(workingSet, definition);

            workingSet.CancellationToken.ThrowIfCancellationRequested();

            // Populating symbol tables
            workingSet.SymbolTable.Where(p => !p.Value.IsFunctionCall && !p.Value.IsString).Select(p => p.Value.Expression).ToList().ForEach(p =>
                PopulateTables(p, workingSet, definition));

            workingSet.CancellationToken.ThrowIfCancellationRequested();

            // Generate expressions
            try
            {
                workingSet.Body = GenerateExpression(workingSet.SymbolTable[string.Empty], workingSet, definition);
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

        private static void PrepareMathDefinitionAndExpression(WorkingExpressionSet workingSet, WorkingDefinition definition)
        {
            var operators = definition.AllOperatorsInOrder.OrderByDescending(p => p.Length).Where(p => definition.AllOperatorsInOrder.Any(q => q.Length < p.Length && p.Contains(q)));

            int i = 1;
            foreach (var op in operators.OrderByDescending(p => p.Length))
            {
                var s = $"@op{i}@";

                workingSet.InitialExpression = workingSet.InitialExpression.Replace(op, s);

                var allIndex = Array.IndexOf(definition.AllOperatorsInOrder, op);
                if (allIndex != -1)
                {
                    definition.AllOperatorsInOrder[allIndex] = s;
                }

                allIndex = Array.IndexOf(definition.BinaryOperatorsInOrder, op);
                if (allIndex != -1)
                {
                    definition.BinaryOperatorsInOrder[allIndex] = s;
                }

                allIndex = Array.IndexOf(definition.UnaryOperatorsInOrder, op);
                if (allIndex != -1)
                {
                    definition.UnaryOperatorsInOrder[allIndex] = s;
                }

                allIndex = Array.IndexOf(definition.AllSymbols, op);
                if (allIndex != -1)
                {
                    definition.AllSymbols[allIndex] = s;
                }

                if (definition.Definition.AddSymbol == op)
                {
                    definition.Definition.AddSymbol = s;
                }
                if (definition.Definition.AndSymbol == op)
                {
                    definition.Definition.AndSymbol = s;
                }
                if (definition.Definition.DivideSymbol == op)
                {
                    definition.Definition.DivideSymbol = s;
                }
                if (definition.Definition.DoesNotEqualSymbol == op)
                {
                    definition.Definition.DoesNotEqualSymbol = s;
                }
                if (definition.Definition.EqualsSymbol == op)
                {
                    definition.Definition.EqualsSymbol = s;
                }
                if (definition.Definition.GreaterThanOrEqualSymbol == op)
                {
                    definition.Definition.GreaterThanOrEqualSymbol = s;
                }
                if (definition.Definition.GreaterThanSymbol == op)
                {
                    definition.Definition.GreaterThanSymbol = s;
                }
                if (definition.Definition.LessThanOrEqualSymbol == op)
                {
                    definition.Definition.LessThanOrEqualSymbol = s;
                }
                if (definition.Definition.LessThanSymbol == op)
                {
                    definition.Definition.LessThanSymbol = s;
                }
                if (definition.Definition.MultiplySymbol == op)
                {
                    definition.Definition.MultiplySymbol = s;
                }
                if (definition.Definition.NotSymbol == op)
                {
                    definition.Definition.NotSymbol = s;
                }
                if (definition.Definition.OrSymbol == op)
                {
                    definition.Definition.OrSymbol = s;
                }
                if (definition.Definition.PowerSymbol == op)
                {
                    definition.Definition.PowerSymbol = s;
                }
                if (definition.Definition.ShiftLeftSymbol == op)
                {
                    definition.Definition.ShiftLeftSymbol = s;
                }
                if (definition.Definition.ShiftRightSymbol == op)
                {
                    definition.Definition.ShiftRightSymbol = s;
                }
                if (definition.Definition.SubtractSymbol == op)
                {
                    definition.Definition.SubtractSymbol = s;
                }
                if (definition.Definition.XorSymbol == op)
                {
                    definition.Definition.XorSymbol = s;
                }

                i++;
            }
        }

        private static void PopulateTables(string p,
            WorkingExpressionSet workingSet,
            WorkingDefinition definition)
        {
            var expressions = p.Split(definition.AllOperatorsInOrder, StringSplitOptions.RemoveEmptyEntries);

            foreach (var exp in expressions)
            {
                if (exp.StartsWith(definition.Definition.SpecialSymbolIndicators.Item1) && exp.EndsWith(definition.Definition.SpecialSymbolIndicators.Item2))
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

                if (workingSet.Constants.ContainsKey(exp))
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

                if (exp.Contains(definition.Definition.Parantheses.Item1))
                {
                    continue;
                }

                if (workingSet.Constants.ParseNumeric(exp) != null)
                {
                    continue;
                }

                workingSet.ExternalParameters.Add(exp, new ExpressionTreeNodeParameter(exp));
            }
        }

        private static ExpressionTreeNodeBase[] GenerateExpression(
            RawExpressionContainer[] s,
            WorkingExpressionSet workingSet,
            WorkingDefinition definition)
        {
            ExpressionTreeNodeBase[] nodes = new ExpressionTreeNodeBase[s.Length];

            for (int i = 0; i < s.Length; i++)
            {
                var res = GenerateExpression(s[i], workingSet, definition);
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
            WorkingExpressionSet workingSet,
            WorkingDefinition definition)
        {
            if (expression.IsString)
            {
                return new ExpressionTreeNodeStringConstant(expression.Expression);
            }

            if (expression.IsFunctionCall)
            {
                return GenerateFunctionCallExpression(
                    expression.Expression,
                    workingSet,
                    definition);
            }

            string s = expression.Expression;

            // Check whether expression is special symbol
            ExpressionTreeNodeMathematicSpecialSymbol ss;
            if (s.StartsWith(definition.Definition.SpecialSymbolIndicators.Item1) && s.EndsWith(definition.Definition.SpecialSymbolIndicators.Item2))
            {
                string actualSymbol;
                if (SpecialSymbolsLocator.BuiltInSpecialSymbolsAlternateWriting.TryGetValue(s.Substring(1, s.Length-2), out actualSymbol))
                {
                    return SpecialSymbolsLocator.BuiltInSpecialSymbols[actualSymbol];
                }
            }
            
            if (SpecialSymbolsLocator.BuiltInSpecialSymbols.TryGetValue(s, out ss))
            {
                return ss;
            }

            // Check whether expression is constant
            ExpressionTreeNodeBase constantResult;
            if (workingSet.Constants.TryGetValue(s, out constantResult))
            {
                return constantResult;
            }

            // Check whether expression is an external parameter
            ExpressionTreeNodeParameter parameterResult;
            if (workingSet.ExternalParameters.TryGetValue(s, out parameterResult))
            {
                return parameterResult;
            }

            // Check whether the expression already exists in the symbols table
            if (workingSet.SymbolTable.TryGetValue(s, out expression))
            {
                return GenerateExpression(expression, workingSet, definition);
            }

            // Check whether the expression is a function call
            if (s.Contains(definition.Definition.Parantheses.Item1))
            {
                return GenerateFunctionCallExpression(
                    new RawExpressionContainer(s).Expression,
                    workingSet,
                    definition);
            }

            // Check whether the expression is a binary operator
            foreach (string op in definition.BinaryOperatorsInOrder)
            {
                var exp = ExpressionByBinaryOperator(s, op, workingSet, definition);
                if (exp != null)
                {
                    return exp;
                }
            }

            // Check whether the expression is a unary operator
            foreach (string op in definition.UnaryOperatorsInOrder)
            {
                var exp = ExpressionByUnaryOperator(s, op, workingSet, definition);
                if (exp != null)
                {
                    return exp;
                }
            }

            return null;
        }

        private static ExpressionTreeNodeBase GenerateFunctionCallExpression(
            string expression,
            WorkingExpressionSet workingSet,
            WorkingDefinition definition)
        {
            Match match = definition.FunctionRegex.Match(expression);

            try
            {
                if (match.Success)
                {
                    string functionName = match.Groups["functionName"].Value;
                    var expr = match.Groups["expression"].Value
                        .Split(new[] { definition.Definition.ParameterSeparator }, StringSplitOptions.None)
                        .Select(p => new RawExpressionContainer(p))
                        .ToArray();

                    var body = GenerateExpression(expr, workingSet, definition);

                    if (body != null)
                    {
                        Func<ExpressionTreeNodeBase> n;
                        if (SupportedFunctionsLocator.BuiltInFunctions.TryGetValue(functionName, out n))
                        {
                            return n().SetOperands(body);
                        }
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
            string s, string op,
            WorkingExpressionSet workingSet,
            WorkingDefinition definition)
        {
            workingSet.CancellationToken.ThrowIfCancellationRequested();

            var split = s.Split(new[] { op }, StringSplitOptions.None);
            if (split.Length > 1)
            {
                if (string.IsNullOrWhiteSpace(split[0]))
                {
                    // We are having a unary operator - until this is treated differently, let's simply return null
                }
                else
                {
                    // We are having a binary operator
                    try
                    {
                        ExpressionTreeNodeBase left = GenerateExpression(new RawExpressionContainer(string.Join(op, split.Take(split.Length - 1).ToArray())), workingSet, definition);
                        if (left == null)
                        {
                            return null;
                        }

                        ExpressionTreeNodeBase right = GenerateExpression(new RawExpressionContainer(split.Last()), workingSet, definition);
                        if (right == null)
                        {
                            return null;
                        }

                        if ((left.ReturnType == SupportedValueType.Numeric && right.ReturnType == SupportedValueType.Numeric) ||
                            (left.ReturnType == SupportedValueType.Unknown && right.ReturnType == SupportedValueType.Unknown) ||
                            (left.ReturnType == SupportedValueType.Numeric && right.ReturnType == SupportedValueType.Unknown) ||
                            (left.ReturnType == SupportedValueType.Unknown && right.ReturnType == SupportedValueType.Numeric))
                        {
                            return definition.NumericBinaryOperators[op]().SetOperands(left, right);
                        }
                        else if ((left.ReturnType == SupportedValueType.Boolean && right.ReturnType == SupportedValueType.Boolean) ||
                            (left.ReturnType == SupportedValueType.Boolean && right.ReturnType == SupportedValueType.Unknown) ||
                            (left.ReturnType == SupportedValueType.Unknown && right.ReturnType == SupportedValueType.Boolean))
                        {
                            return definition.NumericBinaryOperators[op]().SetOperands(left, right);
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
            string s, string op,
            WorkingExpressionSet workingSet,
            WorkingDefinition definition)
        {
            workingSet.CancellationToken.ThrowIfCancellationRequested();

            if (s.StartsWith(op))
            {
                try
                {
                    ExpressionTreeNodeBase expr = GenerateExpression(new RawExpressionContainer(s.Substring(op.Length)), workingSet, definition);
                    if (expr == null)
                    {
                        return null;
                    }

                    switch (expr.ReturnType)
                    {
                        case SupportedValueType.Numeric:
                        case SupportedValueType.Unknown:
                            return definition.NumericUnaryOperators[op]().SetOperands(expr);
                        case SupportedValueType.Boolean:
                            return definition.BooleanUnaryOperators[op]().SetOperands(expr);
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