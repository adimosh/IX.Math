using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;

#if NETSTANDARD10 || NETSTANDARD11 || NETSTANDARD12
using IX.Math.PlatformMitigation;
#endif

namespace IX.Math
{
    public sealed class ExpressionParsingService
    {
        private readonly MathDefinition definition;
        private readonly Regex paranthesesMatcher;
        private readonly string operatorsForRegex;
        private readonly ReaderWriterLockSlim rwl;
        private readonly string[] separateOperatorsInOrder;
        private readonly string[] allOperatorsInOrder;
        private readonly Dictionary<string, Func<Expression, Expression, Expression>> expressionGenerators;

        public ExpressionParsingService()
            : this(new MathDefinition
            {
                Parantheses = new Tuple<string, string>("(", ")"),
                AddSymbol = "+",
                AndSymbol = "&",
                DivideSymbol = "/",
                DoesNotEqualSymbol = "!=",
                EqualsSymbol = "=",
                MultiplySymbol = "*",
                NotSymbol = "!",
                OrSymbol = "|",
                PowerSymbol = "^",
                SubtractSymbol = "-",
                XorSymbol = "#",
                GreaterThanOrEqualSymbol = ">=",
                GreaterThanSymbol = ">",
                LessThanOrEqualSymbol = "<=",
                LessThanSymbol = "<"
            })
        {
        }

        public ExpressionParsingService(MathDefinition definition)
        {
            this.definition = definition;

            rwl = new ReaderWriterLockSlim(LockRecursionPolicy.NoRecursion);

            paranthesesMatcher =
                new Regex($"(?'before'.*)(?'toreplace'{Regex.Escape(definition.Parantheses.Item1)}(?'body'.*?){Regex.Escape(definition.Parantheses.Item2)})(?'after'.*)");

            operatorsForRegex = $"(?:{Regex.Escape(definition.AddSymbol)}|{Regex.Escape(definition.AndSymbol)}|{Regex.Escape(definition.DivideSymbol)}|{Regex.Escape(definition.DoesNotEqualSymbol)}|{Regex.Escape(definition.EqualsSymbol)}|{Regex.Escape(definition.MultiplySymbol)}|{Regex.Escape(definition.NotSymbol)}|{Regex.Escape(definition.OrSymbol)}|{Regex.Escape(definition.PowerSymbol)}|{Regex.Escape(definition.SubtractSymbol)}|{Regex.Escape(definition.XorSymbol)})";

            separateOperatorsInOrder = new[]
            {
                definition.GreaterThanOrEqualSymbol,
                definition.LessThanOrEqualSymbol,
                definition.GreaterThanSymbol,
                definition.LessThanSymbol,
                definition.DoesNotEqualSymbol,
                definition.EqualsSymbol,
                definition.XorSymbol,
                definition.OrSymbol,
                definition.AndSymbol,
                definition.AddSymbol,
                definition.SubtractSymbol,
                definition.DivideSymbol,
                definition.MultiplySymbol,
                definition.PowerSymbol
            };

            allOperatorsInOrder = new[]
            {
                definition.GreaterThanOrEqualSymbol,
                definition.LessThanOrEqualSymbol,
                definition.GreaterThanSymbol,
                definition.LessThanSymbol,
                definition.DoesNotEqualSymbol,
                definition.EqualsSymbol,
                definition.XorSymbol,
                definition.OrSymbol,
                definition.AndSymbol,
                definition.AddSymbol,
                definition.SubtractSymbol,
                definition.DivideSymbol,
                definition.MultiplySymbol,
                definition.PowerSymbol,
                definition.NotSymbol
            };

            expressionGenerators = new Dictionary<string, Func<Expression, Expression, Expression>>
            {
                [definition.AddSymbol] = (left, right) => Expression.Add(left, right),
                [definition.AndSymbol] = (left, right) => Expression.And(left, right),
                [definition.DivideSymbol] = (left, right) => Expression.Divide(left, right),
                [definition.DoesNotEqualSymbol] = (left, right) => Expression.NotEqual(left, right),
                [definition.EqualsSymbol] = (left, right) => Expression.Equal(left, right),
                [definition.MultiplySymbol] = (left, right) => Expression.Multiply(left, right),
                [definition.OrSymbol] = (left, right) => Expression.Or(left, right),
                [definition.PowerSymbol] = (left, right) => Expression.Power(left, right),
                [definition.SubtractSymbol] = (left, right) => Expression.Subtract(left, right),
                [definition.XorSymbol] = (left, right) => Expression.ExclusiveOr(left, right),
                [definition.GreaterThanOrEqualSymbol] = (left, right) => Expression.GreaterThanOrEqual(left, right),
                [definition.GreaterThanSymbol] = (left, right) => Expression.GreaterThan(left, right),
                [definition.LessThanOrEqualSymbol] = (left, right) => Expression.LessThanOrEqual(left, right),
                [definition.LessThanSymbol] = (left, right) => Expression.LessThan(left, right),
            };
        }

        public Delegate GenerateDelegate(string expressionToParse, CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<ParameterExpression> externalParameters;
            Expression body = CreateBody(expressionToParse, out externalParameters, cancellationToken);

            return Expression.Lambda(body, externalParameters).Compile();
        }

        public object ExecuteExpression(string expressionToParse, object[] arguments = null, CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<ParameterExpression> externalParameters;
            Expression body = CreateBody(expressionToParse, out externalParameters, cancellationToken);

            if (body is ConstantExpression && !externalParameters.Any())
                return ((ConstantExpression)body).Value;

            return Expression.Lambda(body, externalParameters).Compile()?.DynamicInvoke(arguments ?? new object[0]);
        }

        private Expression CreateBody(string expressionToParse,
            out IEnumerable<ParameterExpression> externalParams,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            Dictionary<string, RawExpressionContainer> symbolTable = new Dictionary<string, RawExpressionContainer>();
            Dictionary<string, string> reverseSymbolTable = new Dictionary<string, string>();

            int index = 1;
            string expression = expressionToParse;

            // Break by parantheses
            var match = paranthesesMatcher.Match(expression);

            while (match.Success)
            {
                RawExpressionContainer capture = new RawExpressionContainer { Expression = match.Groups["body"].Value };

                string paramIndex; bool shouldAdd;
                if (reverseSymbolTable.TryGetValue(capture.Expression, out paramIndex))
                {
                    shouldAdd = false;
                }
                else
                {
                    while (expression.Contains(paramIndex = $"param{index}"))
                        index++;
                    shouldAdd = true;
                }

                expression = match.Result($"${{before}}param{index}${{after}}");

                if (shouldAdd)
                {
                    symbolTable.Add($"param{index}", capture);
                    reverseSymbolTable.Add(capture.Expression, $"param{index}");
                }

                match = paranthesesMatcher.Match(expression);
            }

            symbolTable.Add(string.Empty, new RawExpressionContainer { Expression = expression });

            // Generating constants and external parameters
            Type numericType;
            if (expression.Contains(definition.PowerSymbol))
                numericType = typeof(double);
            else
                numericType = typeof(int);

            Dictionary<string, ParameterExpression> externalParameters = new Dictionary<string, ParameterExpression>();
            Dictionary<string, ConstantExpression> constants = new Dictionary<string, ConstantExpression>();

            symbolTable.Select(p => p.Value.Expression).ToList().ForEach(p => PopulateTables(p, externalParameters, constants, symbolTable, ref numericType));

            foreach (var c in constants.Where(p => p.Value.Type != numericType))
            {
                constants[c.Key] = Expression.Constant(Convert.ChangeType(c.Value.Value, numericType), numericType);
            }

            foreach (var c in externalParameters.Where(p => p.Value.Type != numericType))
            {
                externalParameters[c.Key] = Expression.Parameter(numericType, c.Value.Name);
            }

            // Generate expressions
            Expression body = GenerateExpression(symbolTable[string.Empty].Expression, ref numericType, symbolTable, constants, externalParameters, cancellationToken);

            int reduceAttempt = 0;
            while (body.CanReduce && reduceAttempt < 30)
            {
                body = body.Reduce();
                reduceAttempt++;
            }

            externalParams = externalParameters.Values;
            return body;
        }

        private void PopulateTables(string p,
            Dictionary<string, ParameterExpression> externalParameters,
            Dictionary<string, ConstantExpression> constants,
            Dictionary<string, RawExpressionContainer> symbolTable,
            ref Type numericType)
        {
            var expressions = p.Split(allOperatorsInOrder, StringSplitOptions.RemoveEmptyEntries);

            foreach (var exp in expressions)
            {
                if (constants.ContainsKey(exp))
                    continue;

                if (externalParameters.ContainsKey(exp))
                    continue;

                if (symbolTable.ContainsKey(exp))
                    continue;

                object value;
                if (TryGetNumericValue(exp, ref numericType, out value))
                {
                    constants.Add(exp, Expression.Constant(value, numericType));
                    continue;
                }

                externalParameters.Add(exp, Expression.Parameter(numericType, exp));
            }
        }

        private bool TryGetNumericValue(string s, ref Type numericType, out object value)
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
                        workingNumericType = typeof(int);
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
                        workingNumericType = typeof(long);
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
                        workingNumericType = typeof(float);
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
                        workingNumericType = typeof(double);
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
                    numericType = workingNumericType;

                value = result;
            }
        }

        private Expression GenerateExpression(
            string s,
            ref Type numericType,
            Dictionary<string, RawExpressionContainer> symbolTable,
            Dictionary<string, ConstantExpression> constants,
            Dictionary<string, ParameterExpression> externalParameters,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (s.StartsWith(definition.NotSymbol))
            {
                return Expression.Negate(GenerateExpression(s.Substring(definition.NotSymbol.Length), ref numericType, symbolTable, constants, externalParameters,
                    cancellationToken));
            }

            ConstantExpression constantResult;
            if (constants.TryGetValue(s, out constantResult))
                return constantResult;

            ParameterExpression parameterResult;
            if (externalParameters.TryGetValue(s, out parameterResult))
                return parameterResult;

            RawExpressionContainer expression;
            if (symbolTable.TryGetValue(s, out expression))
                return GenerateExpression(expression.Expression, ref numericType, symbolTable, constants, externalParameters, cancellationToken);

            foreach (string op in separateOperatorsInOrder)
            {
                var exp = ExpressionByOneOperator(s, op, ref numericType, symbolTable, constants, externalParameters, cancellationToken);
                if (exp != null)
                    return exp;
            }

            throw new InvalidOperationException();
        }

        private Expression ExpressionByOneOperator(
            string s, string op,
            ref Type numericType,
            Dictionary<string, RawExpressionContainer> symbolTable,
            Dictionary<string, ConstantExpression> constants,
            Dictionary<string, ParameterExpression> externalParameters,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            cancellationToken.ThrowIfCancellationRequested();

            var split = s.Split(new[] { op }, StringSplitOptions.None);
            if (split.Length > 1)
                return ((BinaryExpression)expressionGenerators[op](GenerateExpression(split[0], ref numericType, symbolTable, constants, externalParameters, cancellationToken),
                    GenerateExpression(string.Join(op, split.Skip(1).ToArray()), ref numericType, symbolTable, constants, externalParameters, cancellationToken)))
#if NETSTANDARD10 || NETSTANDARD11
                    ;
#else
                    .ReduceIfConstantOperation();
#endif

            return null;
        }
    }
}