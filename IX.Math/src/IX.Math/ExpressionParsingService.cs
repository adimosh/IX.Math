using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;
using IX.Math.PlatformMitigation;
using IX.Math.SimplificationAide;

namespace IX.Math
{
    /// <summary>
    /// A service that is able to parse strings containing mathematical expressions and solve them.
    /// </summary>
    public sealed class ExpressionParsingService : IExpressionParsingService
    {
        private static Regex functionSupport = new Regex(@"(?'functionName'.*?)\((?'expression'.*?)\)");

        private readonly MathDefinition definition;
        private readonly Regex paranthesesMatcher;
        private readonly string operatorsForRegex;
        private readonly string[] binaryOperatorsInOrder;
        private readonly string[] unaryOperatorsInOrder;
        private readonly string[] allOperatorsInOrder;
        private readonly Dictionary<string, Func<Expression, Expression, Expression>> binaryExpressionGenerators;
        private readonly Dictionary<string, Func<Type, Expression, Expression>> unaryExpressionGenerators;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionParsingService"/> class with a standard math definition object.
        /// </summary>
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
                LessThanSymbol = "<",
                ShiftRightSymbol = ">>",
                ShiftLeftSymbol = "<<",
            })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionParsingService"/> class with a specified math definition object.
        /// </summary>
        /// <param name="definition">The math definition to use.</param>
        public ExpressionParsingService(MathDefinition definition)
        {
            this.definition = definition;

            paranthesesMatcher =
                new Regex($"(?'before'.*)(?'toreplace'{Regex.Escape(definition.Parantheses.Item1)}(?'body'.*){Regex.Escape(definition.Parantheses.Item2)})(?'after'.*)");

            operatorsForRegex = $"(?:{Regex.Escape(definition.AddSymbol)}|{Regex.Escape(definition.AndSymbol)}|{Regex.Escape(definition.DivideSymbol)}|{Regex.Escape(definition.DoesNotEqualSymbol)}|{Regex.Escape(definition.EqualsSymbol)}|{Regex.Escape(definition.MultiplySymbol)}|{Regex.Escape(definition.NotSymbol)}|{Regex.Escape(definition.OrSymbol)}|{Regex.Escape(definition.PowerSymbol)}|{Regex.Escape(definition.SubtractSymbol)}|{Regex.Escape(definition.XorSymbol)}|{Regex.Escape(definition.ShiftLeftSymbol)}|{Regex.Escape(definition.ShiftRightSymbol)})";

            binaryOperatorsInOrder = new[]
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
                definition.ShiftLeftSymbol,
                definition.ShiftRightSymbol,
            };

            unaryOperatorsInOrder = new[]
            {
                definition.SubtractSymbol,
                definition.NotSymbol
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
                definition.ShiftLeftSymbol,
                definition.ShiftRightSymbol,
                definition.NotSymbol
            };

            binaryExpressionGenerators = new Dictionary<string, Func<Expression, Expression, Expression>>
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
                [definition.ShiftLeftSymbol] = (left, right) => Expression.LeftShift(left, right),
                [definition.ShiftRightSymbol] = (left, right) => Expression.RightShift(left, right),
            };

            unaryExpressionGenerators = new Dictionary<string, Func<Type, Expression, Expression>>
            {
                [definition.SubtractSymbol] = (type, expr) => Expression.Subtract(Expression.Constant(Convert.ChangeType(0, type), type), expr),
                [definition.NotSymbol] = (type, expr) => Expression.Negate(expr)
            };
        }

        /// <summary>
        /// Generates a delegate from a mathematical expression.
        /// </summary>
        /// <param name="expressionToParse">The mathematical expression to parse.</param>
        /// <param name="cancellationToken">The cancellation token to use for this operation.</param>
        /// <returns>A <see cref="Delegate"/> that can be used to calculate the result of the given expression, or <c>null</c> (<c>Nothing</c> in Visual Basic).</returns>
        public Delegate GenerateDelegate(string expressionToParse, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GenerateDelegate(expressionToParse, WorkingConstants.defaultNumericType, cancellationToken);
        }

        /// <summary>
        /// Generates a delegate from a mathematical expression.
        /// </summary>
        /// <param name="expressionToParse">The mathematical expression to parse.</param>
        /// <param name="numericalType">The numerical type to use.</param>
        /// <param name="cancellationToken">The cancellation token to use for this operation.</param>
        /// <returns>A <see cref="Delegate"/> that can be used to calculate the result of the given expression, or <c>null</c> (<c>Nothing</c> in Visual Basic).</returns>
        /// <remarks>
        /// <para>The numerical type is not guaranteed to be the type requested. It will, however, be at least the requested type as size.</para>
        /// <para>For instance, if type <see cref="int"/> is requested, but a <see cref="double"/> value is found anywhere in the expression, the resulting type will be
        /// <see cref="double"/>.</para>
        /// </remarks>
        public Delegate GenerateDelegate(string expressionToParse, Type numericalType, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (!NumericTypeAide.NumericTypesConversionDictionary.ContainsKey(numericalType))
            {
                throw new InvalidOperationException(Resources.NumericTypeInvalid);
            }

            IEnumerable<ParameterExpression> externalParameters;
            Type resultingNumericType;
            Expression body = CreateBody(expressionToParse, numericalType, out externalParameters, out resultingNumericType, cancellationToken);

            if (body == null)
            {
                return null;
            }

            return Expression.Lambda(body, externalParameters).Compile();
        }

        /// <summary>
        /// Interprets a mathematical expression and executes it, returning the result.
        /// </summary>
        /// <param name="expressionToParse">The mathematical expression to parse.</param>
        /// <param name="cancellationToken">The cancellation token to use for this operation.</param>
        /// <returns>The result of the expression, if calculable, whatever it might be.</returns>
        public object ExecuteExpression(string expressionToParse, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ExecuteExpression(expressionToParse, WorkingConstants.defaultNumericType, null, cancellationToken);
        }

        /// <summary>
        /// Interprets a mathematical expression and executes it, returning the result.
        /// </summary>
        /// <param name="expressionToParse">The mathematical expression to parse.</param>
        /// <param name="numericalType">The numerical type to use.</param>
        /// <param name="cancellationToken">The cancellation token to use for this operation.</param>
        /// <returns>The result of the expression, if calculable, whatever it might be.</returns>
        /// <remarks>
        /// <para>The numerical type is not guaranteed to be the type requested. It will, however, be at least the requested type as size.</para>
        /// <para>For instance, if type <see cref="int"/> is requested, but a <see cref="double"/> value is found anywhere in the expression, the resulting type will be
        /// <see cref="double"/>.</para>
        /// </remarks>
        public object ExecuteExpression(string expressionToParse, Type numericalType, CancellationToken cancellationToken = default(CancellationToken))
        {
            return ExecuteExpression(expressionToParse, numericalType, null, cancellationToken);
        }

        /// <summary>
        /// Interprets a mathematical expression and executes it, returning the result.
        /// </summary>
        /// <param name="expressionToParse">The mathematical expression to parse.</param>
        /// <param name="arguments">The arguments to pass to the expression.</param>
        /// <param name="cancellationToken">The cancellation token to use for this operation.</param>
        /// <returns>The result of the expression, if calculable, whatever it might be.</returns>
        /// <remarks>
        /// <para>The numerical type that will be used will be the type with the highest capacity between the used types.</para>
        /// <para>All numerical arguments will have been converted to the numerical type before being used as parameters.</para>
        /// </remarks>
        public object ExecuteExpression(string expressionToParse, object[] arguments, CancellationToken cancellationToken = default(CancellationToken))
        {
            Type numericType = WorkingConstants.defaultNumericType;

            NumericTypeAide.GetProperRequestedNumericalType(arguments, ref numericType);

            return ExecuteExpression(expressionToParse, numericType, arguments, cancellationToken);
        }

        /// <summary>
        /// Interprets a mathematical expression, finds its data and executes it, returning the result.
        /// </summary>
        /// <param name="expressionToParse">The mathematical expression to parse.</param>
        /// <param name="dataFinder">A service instance that is used to find the data that the expression requires in order to execute.</param>
        /// <param name="cancellationToken">The cancellation token to use for this operation.</param>
        /// <returns>The result of the expression, if calculable, whatever it might be.</returns>
        /// <remarks>
        /// <para>Due to the non-deterministic nature of the <see cref="IDataFinder"/>, it is impossible to properly evaluate the numeric type in concert with
        /// all the values that might come from the injected finder, as well as the expression.</para>
        /// <para>It is therefore implied that the numeric type is always <see cref="double"/>, since this data type has the biggest capacity to support
        /// all possible input values.</para>
        /// <para>Calling this method will attempt to automatically convert values returned by the finder to <see cref="double"/>.</para>
        /// <para>Any other rule still applies, so, if you pass a <see cref="string"/> where a numeric value is required, the method will still fail.</para>
        /// </remarks>
        public object ExecuteExpression(string expressionToParse, IDataFinder dataFinder, CancellationToken cancellationToken = default(CancellationToken))
        {
            IEnumerable<ParameterExpression> externalParameters;
            Type resultingNumericType;
            Expression body = CreateBody(expressionToParse, WorkingConstants.defaultNumericTypeWithFinder, out externalParameters, out resultingNumericType, cancellationToken);

            if (body == null)
            {
                return expressionToParse;
            }

            if (body is ConstantExpression && !externalParameters.Any())
            {
                return ((ConstantExpression)body).Value;
            }

            if (dataFinder == null)
            {
                throw new ArgumentNullException(nameof(dataFinder));
            }

            object[] parameterValues = NumericTypeAide.GetValuesFromFinder(externalParameters.Select(p => new Tuple<string, Type>(p.Name, p.Type)), dataFinder);

            try
            {
                return Expression.Lambda(body, externalParameters).Compile()?.DynamicInvoke(parameterValues.ToArray()) ?? expressionToParse;
            }
            catch (Exception ex)
            {
                throw new ExpressionNotValidLogicallyException(ex);
            }
        }

        private object ExecuteExpression(string expressionToParse, Type requestedNumericType, object[] arguments, CancellationToken cancellationToken)
        {
            Type resultingNumericType;
            IEnumerable<ParameterExpression> externalParameters;
            Expression body = CreateBody(expressionToParse, requestedNumericType, out externalParameters, out resultingNumericType, cancellationToken);

            if (body == null)
            {
                return expressionToParse;
            }

            if (body is ConstantExpression && !externalParameters.Any())
            {
                return ((ConstantExpression)body).Value;
            }

            object[] convertedArguments = NumericTypeAide.GetProperNumericTypeValues(arguments, resultingNumericType);

            if (externalParameters.Count() != convertedArguments.Length)
            {
                return expressionToParse;
            }

            try
            {
                return Expression.Lambda(body, externalParameters).Compile()?.DynamicInvoke(convertedArguments) ?? expressionToParse;
            }
            catch (Exception ex)
            {
                throw new ExpressionNotValidLogicallyException(ex);
            }
        }

        private Expression CreateBody(
            string expressionToParse,
            Type numericTypeToStart,
            out IEnumerable<ParameterExpression> externalParams,
            out Type resultingNumericType,
            CancellationToken cancellationToken)
        {
            Dictionary<string, RawExpressionContainer> symbolTable = new Dictionary<string, RawExpressionContainer>();
            Dictionary<string, string> reverseSymbolTable = new Dictionary<string, string>();

            // Break by parantheses

            // Algorithm determines multiple imbricated parantheses
            int i = 0;
            string expression;
            try
            {
                expression = BreakOneLevel(expressionToParse, symbolTable, reverseSymbolTable, ref i);
            }
            catch
            {
                externalParams = null;
                resultingNumericType = null;
                return null;
            }

            symbolTable.Add(string.Empty, new RawExpressionContainer { Expression = expression });

            // Generating constants and external parameters
            Type numericType;
            if (expression.Contains(definition.PowerSymbol))
            {
                numericType = WorkingConstants.defaultNumericTypeWithFinder;
            }
            else
            {
                numericType = numericTypeToStart;
            }

            Dictionary<string, ParameterExpression> externalParameters = new Dictionary<string, ParameterExpression>();
            Dictionary<string, ConstantExpression> constants = new Dictionary<string, ConstantExpression>();

            symbolTable.Select(p => p.Value.Expression).ToList().ForEach(p => PopulateTables(p, externalParameters, constants, symbolTable, ref numericType));

            resultingNumericType = numericType;

            Dictionary<string, ConstantExpression> typeChangers = new Dictionary<string, ConstantExpression>();
            foreach (var c in constants.Where(p => p.Value.Type != numericType))
            {
                typeChangers[c.Key] = Expression.Constant(Convert.ChangeType(c.Value.Value, numericType), numericType);
            }

            foreach (var c in typeChangers)
            {
                constants[c.Key] = c.Value;
            }

            foreach (var c in externalParameters.Where(p => p.Value.Type != numericType))
            {
                externalParameters[c.Key] = Expression.Parameter(numericType, c.Value.Name);
            }

            // Generate expressions
            Expression body;
            try
            {
                body = GenerateExpression(symbolTable[string.Empty].Expression, numericType, symbolTable, constants, externalParameters, cancellationToken);
            }
            catch
            {
                externalParams = null;
                resultingNumericType = null;
                return null;
            }

            if (body == null)
            {
                externalParams = new ParameterExpression[0];
                return null;
            }

            int reduceAttempt = 0;
            while (body.CanReduce && reduceAttempt < 30)
            {
                body = body.Reduce();
                reduceAttempt++;
            }

            externalParams = externalParameters.Values;
            return body;
        }

        private string BreakOneLevel(string source, Dictionary<string, RawExpressionContainer> symbolTable, Dictionary<string, string> reverseSymbolTable, ref int i)
        {
            if (string.IsNullOrWhiteSpace(source))
            {
                return string.Empty;
            }

            int op = source.IndexOf(definition.Parantheses.Item1);
            int cp = source.IndexOf(definition.Parantheses.Item2);

            beginning:
            if (op != -1)
            {
                if (cp != -1)
                {
                    if (op < cp)
                    {
                        string expr3 = BreakOneLevel(source.Substring(op + definition.Parantheses.Item1.Length), symbolTable, reverseSymbolTable, ref i);

                        if (op == 0)
                        {
                            source = expr3;
                        }
                        else
                        {
                            string expr4 = op == 0 ? string.Empty : source.Substring(0, op);

                            if (!allOperatorsInOrder.Any(p => expr4.EndsWith(p)))
                            {
                                // We have a function call

                                int inx = allOperatorsInOrder.Max(p => expr4.LastIndexOf(p));
                                var expr5 = inx == -1 ? expr4 : expr4.Substring(inx);
                                string op1 = allOperatorsInOrder.OrderByDescending(p => p.Length).FirstOrDefault(p => expr5.StartsWith(p));
                                var expr6 = op1 == null ? expr5 : expr5.Substring(op1.Length);

                                i++;
                                string expr2 = $"item{i}";
                                var rec = new RawExpressionContainer { Expression = $"{expr6}(item{i - 1})" };
                                symbolTable.Add(expr2, rec);
                                reverseSymbolTable.Add(rec.Expression, expr2);

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

                        op = source.IndexOf(definition.Parantheses.Item1);
                        cp = source.IndexOf(definition.Parantheses.Item2);

                        goto beginning;
                    }

                    return ProcessSubExpression(source, cp, symbolTable, reverseSymbolTable, ref i);
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
                    return ProcessSubExpression(source, cp, symbolTable, reverseSymbolTable, ref i);
                }
            }
        }

        private string ProcessSubExpression(
            string source,
            int cp,
            Dictionary<string, RawExpressionContainer> symbolTable,
            Dictionary<string, string> reverseSymbolTable,
            ref int i)
        {
            string expr1 = source.Substring(0, cp);

            RawExpressionContainer rec = new RawExpressionContainer { Expression = expr1 };
            string expr2;

            int k = cp + definition.Parantheses.Item2.Length;
            if (!reverseSymbolTable.TryGetValue(rec.Expression, out expr2))
            {
                i++;
                expr2 = $"item{i}";
                symbolTable.Add(expr2, rec);
                reverseSymbolTable.Add(rec.Expression, expr2);
            }

            return $"{expr2}{(source.Length == k ? string.Empty : source.Substring(k))}";
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
                {
                    continue;
                }

                if (externalParameters.ContainsKey(exp))
                {
                    continue;
                }

                if (symbolTable.ContainsKey(exp))
                {
                    continue;
                }

                if (exp.Contains(definition.Parantheses.Item1))
                {
                    numericType = WorkingConstants.defaultNumericTypeWithFinder;
                    continue;
                }

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

        private Expression GenerateExpression(
            string s,
            Type numericType,
            Dictionary<string, RawExpressionContainer> symbolTable,
            Dictionary<string, ConstantExpression> constants,
            Dictionary<string, ParameterExpression> externalParameters,
            CancellationToken cancellationToken)
        {
            // Check whether expression is constant
            ConstantExpression constantResult;
            if (constants.TryGetValue(s, out constantResult))
            {
                return constantResult;
            }

            // Check whether expression is an external parameter
            ParameterExpression parameterResult;
            if (externalParameters.TryGetValue(s, out parameterResult))
            {
                return parameterResult;
            }

            // Check whether the expression already exists in the symbols table
            RawExpressionContainer expression;
            if (symbolTable.TryGetValue(s, out expression))
            {
                return GenerateExpression(expression.Expression, numericType, symbolTable, constants, externalParameters, cancellationToken);
            }

            // Check whether the expression is a function call
            if (s.Contains(definition.Parantheses.Item1))
            {
                return GenerateFunctionCallExpression(new RawExpressionContainer { Expression = s }.Expression, numericType, symbolTable, constants, externalParameters, cancellationToken);
            }

            // Check whether the expression is a binary operator
            foreach (string op in binaryOperatorsInOrder)
            {
                var exp = ExpressionByBinaryOperator(s, op, numericType, symbolTable, constants, externalParameters, cancellationToken);
                if (exp != null)
                {
                    return exp;
                }
            }

            // Check whether the expression is a unary operator
            foreach (string op in unaryOperatorsInOrder)
            {
                var exp = ExpressionByUnaryOperator(s, op, numericType, symbolTable, constants, externalParameters, cancellationToken);
                if (exp != null)
                {
                    return exp;
                }
            }

            return null;
        }

        private Expression GenerateFunctionCallExpression(
            string expression,
            Type numericType,
            Dictionary<string, RawExpressionContainer> symbolTable,
            Dictionary<string, ConstantExpression> constants,
            Dictionary<string, ParameterExpression> externalParameters,
            CancellationToken cancellationToken)
        {
            Match match = functionSupport.Match(expression);

            if (match.Success)
            {
                string functionName = match.Groups["functionName"].Value;
                string expr = match.Groups["expression"].Value;

                var body = GenerateExpression(expr, numericType, symbolTable, constants, externalParameters, cancellationToken);

                if (body == null)
                {
                    return null;
                }

                return GenerateInternalFunctionCallExpression(functionName, body, symbolTable, constants, externalParameters, cancellationToken);
            }

            return null;
        }

        private Expression GenerateInternalFunctionCallExpression(string functionName, Expression body, Dictionary<string, RawExpressionContainer> symbolTable, Dictionary<string, ConstantExpression> constants, Dictionary<string, ParameterExpression> externalParameters, CancellationToken cancellationToken)
        {
            return SupportedFunctionsLocator.LoadUnaryFunction(functionName, body);
        }

        private Expression ExpressionByBinaryOperator(
            string s, string op,
            Type numericType,
            Dictionary<string, RawExpressionContainer> symbolTable,
            Dictionary<string, ConstantExpression> constants,
            Dictionary<string, ParameterExpression> externalParameters,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

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
                        Expression left = GenerateExpression(split[0], numericType, symbolTable, constants, externalParameters, cancellationToken);
                        if (left == null)
                        {
                            return null;
                        }

                        Expression right = GenerateExpression(string.Join(op, split.Skip(1).ToArray()), numericType, symbolTable, constants, externalParameters, cancellationToken);
                        if (right == null)
                        {
                            return null;
                        }

                        return (binaryExpressionGenerators[op](
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

        private Expression ExpressionByUnaryOperator(
            string s, string op,
            Type numericType,
            Dictionary<string, RawExpressionContainer> symbolTable,
            Dictionary<string, ConstantExpression> constants,
            Dictionary<string, ParameterExpression> externalParameters,
            CancellationToken cancellationToken)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (s.StartsWith(op))
            {
                try
                {
                    Expression expr = GenerateExpression(s.Substring(op.Length), numericType, symbolTable, constants, externalParameters, cancellationToken);
                    if (expr == null)
                    {
                        return null;
                    }

                    return (unaryExpressionGenerators[op](
                        numericType,
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