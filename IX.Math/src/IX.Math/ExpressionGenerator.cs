using IX.Math.PlatformMitigation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace IX.Math
{
    internal static class ExpressionGenerator
    {
        private static Regex functionSupport = new Regex(@"(?'functionName'.*?)\((?'expression'.*?)\)");

        internal static Expression CreateBody(
            WorkingExpressionSet workingSet,
            WorkingDefinition definition)
        {
            workingSet.cancellationToken.ThrowIfCancellationRequested();

            // Break by parantheses

            // Algorithm determines multiple imbricated parantheses
            int i = 0;
            string expression;
            try
            {
                expression = BreakOneLevel(workingSet.initialExpression, workingSet, definition, ref i);
            }
            catch
            {
                return null;
            }

            workingSet.symbolTable.Add(string.Empty, new RawExpressionContainer { Expression = expression });

            // Generating constants and external parameters
            if (expression.Contains(definition.Definition.PowerSymbol))
            {
                workingSet.numericType = WorkingConstants.defaultNumericTypeWithFinder;
            }

            workingSet.cancellationToken.ThrowIfCancellationRequested();

            workingSet.symbolTable.Select(p => p.Value.Expression).ToList().ForEach(p =>
                PopulateTables(p, workingSet, definition, ref workingSet.numericType));

            Type numericType = workingSet.numericType;
            Dictionary<string, ConstantExpression> typeChangers = new Dictionary<string, ConstantExpression>();
            foreach (var c in workingSet.constants.Where(p => p.Value.Type != numericType))
            {
                typeChangers[c.Key] = Expression.Constant(Convert.ChangeType(c.Value.Value, numericType), numericType);
            }

            foreach (var c in typeChangers)
            {
                workingSet.constants[c.Key] = c.Value;
            }

            foreach (var c in workingSet.externalParams.Where(p => p.Value.Type != numericType))
            {
                workingSet.externalParams[c.Key] = Expression.Parameter(numericType, c.Value.Name);
            }

            workingSet.cancellationToken.ThrowIfCancellationRequested();

            // Generate expressions
            Expression body;
            try
            {
                body = GenerateExpression(workingSet.symbolTable[string.Empty].Expression, workingSet, definition);
            }
            catch
            {
                return null;
            }

            if (body == null)
            {
                return null;
            }

            int reduceAttempt = 0;
            while (body.CanReduce && reduceAttempt < 30)
            {
                body = body.Reduce();
                reduceAttempt++;
            }

            return body;
        }

        private static string BreakOneLevel(
            string source,
            WorkingExpressionSet workingSet,
            WorkingDefinition definition,
            ref int i)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return string.Empty;
            }

            int op = source.IndexOf(definition.Definition.Parantheses.Item1);
            int cp = source.IndexOf(definition.Definition.Parantheses.Item2);

            beginning:
            if (op != -1)
            {
                if (cp != -1)
                {
                    if (op < cp)
                    {
                        string expr3 = BreakOneLevel(source.Substring(op + definition.Definition.Parantheses.Item1.Length), workingSet, definition, ref i);

                        if (op == 0)
                        {
                            source = expr3;
                        }
                        else
                        {
                            string expr4 = op == 0 ? string.Empty : source.Substring(0, op);

                            if (!definition.AllOperatorsInOrder.Any(p => expr4.EndsWith(p)))
                            {
                                // We have a function call

                                int inx = definition.AllOperatorsInOrder.Max(p => expr4.LastIndexOf(p));
                                var expr5 = inx == -1 ? expr4 : expr4.Substring(inx);
                                string op1 = definition.AllOperatorsInOrder.OrderByDescending(p => p.Length).FirstOrDefault(p => expr5.StartsWith(p));
                                var expr6 = op1 == null ? expr5 : expr5.Substring(op1.Length);

                                i++;
                                string expr2 = $"item{i}";
                                var rec = new RawExpressionContainer { Expression = $"{expr6}(item{i - 1})" };
                                workingSet.symbolTable.Add(expr2, rec);
                                workingSet.reverseSymbolTable.Add(rec.Expression, expr2);

                                if (expr6 == expr4)
                                {
                                    expr4 = string.Empty;
                                }
                                else
                                {
                                    expr4 = expr4.Substring(0, expr4.Length - expr6.Length);
                                }

                                expr3 = expr3.Replace($"item{i - 1}", $"item{i}");
                            }

                            source = $"{expr4}{expr3}";
                        }

                        op = source.IndexOf(definition.Definition.Parantheses.Item1);
                        cp = source.IndexOf(definition.Definition.Parantheses.Item2);

                        goto beginning;
                    }

                    return ProcessSubExpression(source, cp, workingSet, definition, ref i);
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                if (cp == -1)
                {
                    return source;
                }
                else
                {
                    return ProcessSubExpression(source, cp, workingSet, definition, ref i);
                }
            }
        }

        private static string ProcessSubExpression(
            string source,
            int cp,
            WorkingExpressionSet workingSet,
            WorkingDefinition definition,
            ref int i)
        {
            string expr1 = source.Substring(0, cp);

            RawExpressionContainer rec = new RawExpressionContainer { Expression = expr1 };
            string expr2;

            int k = cp + definition.Definition.Parantheses.Item2.Length;
            if (!workingSet.reverseSymbolTable.TryGetValue(rec.Expression, out expr2))
            {
                i++;
                expr2 = $"item{i}";
                workingSet.symbolTable.Add(expr2, rec);
                workingSet.reverseSymbolTable.Add(rec.Expression, expr2);
            }

            return $"{expr2}{(source.Length == k ? string.Empty : source.Substring(k))}";
        }

        private static void PopulateTables(string p,
            WorkingExpressionSet workingSet,
            WorkingDefinition definition,
            ref Type numericType)
        {
            var expressions = p.Split(definition.AllOperatorsInOrder, StringSplitOptions.RemoveEmptyEntries);

            foreach (var exp in expressions)
            {
                if (workingSet.constants.ContainsKey(exp))
                {
                    continue;
                }

                if (workingSet.externalParams.ContainsKey(exp))
                {
                    continue;
                }

                if (workingSet.symbolTable.ContainsKey(exp))
                {
                    continue;
                }

                if (exp.Contains(definition.Definition.Parantheses.Item1))
                {
                    numericType = WorkingConstants.defaultNumericTypeWithFinder;
                    continue;
                }

                object value;
                if (TryGetNumericValue(exp, ref numericType, out value))
                {
                    workingSet.constants.Add(exp, Expression.Constant(value, numericType));
                    continue;
                }

                workingSet.externalParams.Add(exp, Expression.Parameter(numericType, exp));
            }
        }

        private static bool TryGetNumericValue(string s, ref Type numericType, out object value)
        {
            Type workingNumericType = numericType;
            object result = null;

            try
            {
                if (workingNumericType == typeof(short))
                {
                    short shortVal;
                    if (short.TryParse(s, out shortVal))
                    {
                        result = shortVal;
                        return true;
                    }
                    else
                    {
                        workingNumericType = typeof(int);
                    }
                }

                if (workingNumericType == typeof(int))
                {
                    int intVal;
                    if (int.TryParse(s, out intVal))
                    {
                        result = intVal;
                        return true;
                    }
                    else
                    {
                        workingNumericType = typeof(long);
                    }
                }

                if (workingNumericType == typeof(long))
                {
                    long longVal;
                    if (long.TryParse(s, out longVal))
                    {
                        result = longVal;
                        return true;
                    }
                    else
                    {
                        workingNumericType = typeof(float);
                    }
                }

                if (workingNumericType == typeof(float))
                {
                    float floatVal;
                    if (float.TryParse(s, out floatVal))
                    {
                        result = floatVal;
                        return true;
                    }
                    else
                    {
                        workingNumericType = typeof(double);
                    }
                }

                if (workingNumericType == typeof(double))
                {
                    double doubleVal;
                    if (double.TryParse(s, out doubleVal))
                    {
                        result = doubleVal;
                        return true;
                    }
                }

                value = null;
                return false;
            }
            finally
            {
                if (workingNumericType != numericType && result != null)
                {
                    numericType = workingNumericType;
                }

                value = result;
            }
        }

        private static Expression GenerateExpression(
            string s,
            WorkingExpressionSet workingSet,
            WorkingDefinition definition)
        {
            // Check whether expression is constant
            ConstantExpression constantResult;
            if (workingSet.constants.TryGetValue(s, out constantResult))
            {
                return constantResult;
            }

            // Check whether expression is an external parameter
            ParameterExpression parameterResult;
            if (workingSet.externalParams.TryGetValue(s, out parameterResult))
            {
                return parameterResult;
            }

            // Check whether the expression already exists in the symbols table
            RawExpressionContainer expression;
            if (workingSet.symbolTable.TryGetValue(s, out expression))
            {
                return GenerateExpression(expression.Expression, workingSet, definition);
            }

            // Check whether the expression is a function call
            if (s.Contains(definition.Definition.Parantheses.Item1))
            {
                return GenerateFunctionCallExpression(
                    new RawExpressionContainer { Expression = s }.Expression,
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

        private static Expression GenerateFunctionCallExpression(
            string expression,
            WorkingExpressionSet workingSet,
            WorkingDefinition definition)
        {
            Match match = functionSupport.Match(expression);

            if (match.Success)
            {
                string functionName = match.Groups["functionName"].Value;
                string expr = match.Groups["expression"].Value;

                var body = GenerateExpression(expr, workingSet, definition);

                if (body == null)
                {
                    return null;
                }

                return SupportedFunctionsLocator.LoadUnaryFunction(functionName, body);
            }

            return null;
        }

        private static Expression ExpressionByBinaryOperator(
            string s, string op,
            WorkingExpressionSet workingSet,
            WorkingDefinition definition)
        {
            workingSet.cancellationToken.ThrowIfCancellationRequested();

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
                        Expression left = GenerateExpression(split[0], workingSet, definition);
                        if (left == null)
                        {
                            return null;
                        }

                        Expression right = GenerateExpression(string.Join(op, split.Skip(1).ToArray()), workingSet, definition);
                        if (right == null)
                        {
                            return null;
                        }

                        return (definition.BinaryExpressionGenerators[op](
                                left,
                                right))
                            .ReduceIfConstantOperation();
                    }
                    catch
                    {
                        return null;
                    }
                }
            }

            return null;
        }

        private static Expression ExpressionByUnaryOperator(
            string s, string op,
            WorkingExpressionSet workingSet,
            WorkingDefinition definition)
        {
            workingSet.cancellationToken.ThrowIfCancellationRequested();

            if (s.StartsWith(op))
            {
                try
                {
                    Expression expr = GenerateExpression(s.Substring(op.Length), workingSet, definition);
                    if (expr == null)
                    {
                        return null;
                    }

                    return (definition.UnaryExpressionGenerators[op](
                        workingSet.numericType,
                        expr))
                        .ReduceIfConstantOperation();
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
