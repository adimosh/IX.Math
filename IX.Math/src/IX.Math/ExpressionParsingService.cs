using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using IX.Math.SimplificationAide;
using IX.Math.BuiltIn;

namespace IX.Math
{
    /// <summary>
    /// A service that is able to parse strings containing mathematical expressions and solve them.
    /// </summary>
    public sealed class ExpressionParsingService : IExpressionParsingService
    {
        private readonly WorkingDefinition workingDefinition;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionParsingService"/> class with a standard math definition object.
        /// </summary>
        public ExpressionParsingService()
            : this(new MathDefinition
            {
                Parantheses = new Tuple<string, string>("(", ")"),
                SpecialSymbolIndicators = new Tuple<string, string>("[", "]"),
                StringIndicator = "\"",
                ParameterSeparator= ",",
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
            workingDefinition = new WorkingDefinition(definition);
        }

        /// <summary>
        /// Generates a delegate from a mathematical expression.
        /// </summary>
        /// <param name="expressionToParse">The mathematical expression to parse.</param>
        /// <param name="cancellationToken">The cancellation token to use for this operation.</param>
        /// <returns>A <see cref="Delegate"/> that can be used to calculate the result of the given expression, or <c>null</c> (<c>Nothing</c> in Visual Basic).</returns>
        public Delegate GenerateDelegate(string expressionToParse, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GenerateDelegateInternal(expressionToParse, WorkingConstants.defaultNumericType, cancellationToken)?.Item1;
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
            return GenerateDelegateInternal(expressionToParse, numericalType, cancellationToken)?.Item1;
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
            WorkingExpressionSet workingSet = new WorkingExpressionSet(expressionToParse, cancellationToken);

            ExpressionGenerator.CreateBody(workingSet, workingDefinition);

            if (!workingSet.Success)
            {
                return expressionToParse;
            }

            if (workingSet.Constant)
            {
                return workingSet.ValueIfConstant;
            }

            if (dataFinder == null)
            {
                throw new ArgumentNullException(nameof(dataFinder));
            }

            int numericTypeValue = NumericTypeAide.NumericTypesConversionDictionary[WorkingConstants.defaultNumericTypeWithFinder];

            try
            {
                object[] parameterValues = NumericTypeAide.GetValuesFromFinder(workingSet.ExternalParameters.Values, dataFinder);

                if (parameterValues.Any(p => p == null))
                {
                    return expressionToParse;
                }

                var body = workingSet.Body.GenerateExpression();

                if (body == null)
                {
                    return expressionToParse;
                }
                if (body is ConstantExpression)
                {
                    return ((ConstantExpression)body).Value;
                }

                return Expression.Lambda(body, workingSet.ExternalParameters.Values.Select(p => p.GeneratedExpression))
                    .Compile()?.DynamicInvoke(parameterValues.ToArray()) ?? expressionToParse;
            }
            catch (Exception ex)
            {
                throw new ExpressionNotValidLogicallyException(ex);
            }
        }

        internal Tuple<Delegate, IEnumerable<ExpressionTreeNodeParameter>> GenerateDelegateInternal(string expressionToParse, Type numericType, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(expressionToParse))
            {
                return null;
            }

            int numericTypeValue;
            if (!NumericTypeAide.NumericTypesConversionDictionary.TryGetValue(numericType, out numericTypeValue))
            {
                throw new InvalidOperationException(Resources.NumericTypeInvalid);
            }

            WorkingExpressionSet workingSet = new WorkingExpressionSet(expressionToParse, cancellationToken);

            ExpressionGenerator.CreateBody(workingSet, workingDefinition);

            if (!workingSet.Success)
            {
                return null;
            }

            try
            {
                var body = workingSet.Body.GenerateExpression(numericTypeValue);

                if (body == null)
                {
                    return null;
                }

                return new Tuple<Delegate, IEnumerable<ExpressionTreeNodeParameter>>(
                    Expression.Lambda(body, workingSet.ExternalParameters.Values.Select(p => p.GeneratedExpression))
                        .Compile(), workingSet.ExternalParameters.Values);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private object ExecuteExpression(string expressionToParse, Type requestedNumericType, object[] arguments, CancellationToken cancellationToken)
        {
            WorkingExpressionSet workingSet = new WorkingExpressionSet(expressionToParse, cancellationToken);

            ExpressionGenerator.CreateBody(workingSet, workingDefinition);

            if (!workingSet.Success)
            {
                return expressionToParse;
            }

            if (workingSet.Constant)
            {
                return workingSet.ValueIfConstant;
            }

            int bodyTypeValue = workingSet.Body.ComputeResultingNumericTypeValue();
            int reqTypeValue = NumericTypeAide.NumericTypesConversionDictionary[requestedNumericType];

            if (bodyTypeValue > reqTypeValue)
            {
                reqTypeValue = bodyTypeValue;
            }

            object[] convertedArguments = NumericTypeAide.GetProperNumericTypeValues(arguments, NumericTypeAide.InverseNumericTypesConversionDictionary[reqTypeValue]);

            if (workingSet.ExternalParameters.Count != convertedArguments.Length)
            {
                return expressionToParse;
            }

            try
            {
                var body = workingSet.Body.GenerateExpression(reqTypeValue);

                if (body == null)
                {
                    return null;
                }
                if (body is ConstantExpression)
                {
                    return ((ConstantExpression)body).Value;
                }

                return Expression.Lambda(body, workingSet.ExternalParameters.Values.Select(p => p.GeneratedExpression))
                    .Compile()?.DynamicInvoke(convertedArguments) ?? expressionToParse;
            }
            catch (Exception ex)
            {
                throw new ExpressionNotValidLogicallyException(ex);
            }
        }
    }
}