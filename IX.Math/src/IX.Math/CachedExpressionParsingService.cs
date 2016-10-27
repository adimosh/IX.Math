#if !NETSTANDARD10
using IX.Math.SimplificationAide;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace IX.Math
{
    /// <summary>
    /// A service that is able to parse strings containing mathematical expressions and solve them in a cached way.
    /// </summary>
    /// <remarks>
    /// <para>This service also caches expressions so that they can be garbage-collected after a specific time period with no use.</para>
    /// <para>Please note that directly accessing the <see cref="IExpressionParsingService.GenerateDelegate(string, CancellationToken)"/>
    /// explicitly-implemented method does not bypass the cache, but, due to the expression being unable to </para>
    /// </remarks>
    public class CachedExpressionParsingService : IExpressionParsingService
    {
        private readonly ExpressionParsingService eps;
        private ConcurrentDictionary<string, Tuple<Delegate, Type>> cachedDelegates;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedExpressionParsingService"/> class.
        /// </summary>
        public CachedExpressionParsingService()
        {
            eps = new ExpressionParsingService();
            cachedDelegates = new ConcurrentDictionary<string, Tuple<Delegate, Type>>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedExpressionParsingService"/> class.
        /// </summary>
        /// <param name="definition">The math definition to use.</param>
        public CachedExpressionParsingService(MathDefinition definition)
        {
            eps = new ExpressionParsingService(definition);
        }

        public object ExecuteExpression(string expressionToParse, CancellationToken cancellationToken = default(CancellationToken))
        {
            var del = GetDelegateForExpression(expressionToParse, WorkingConstants.defaultNumericType, cancellationToken);
            
            if (del == null)
            {
                return expressionToParse;
            }

            return del.Item1.DynamicInvoke();
        }

        public object ExecuteExpression(string expressionToParse, IDataFinder dataFinder, CancellationToken cancellationToken = default(CancellationToken))
        {
            Type numericType = WorkingConstants.defaultNumericTypeWithFinder;

            var del = GetDelegateForExpression(expressionToParse, numericType, cancellationToken);

            MethodInfo mi = del.Item1.GetType()
#if NETSTANDARD11 || NETSTANDARD12
                .GetRuntimeMethods()
#else
                .GetMethods()
#endif
                .Where(p => p.Name == "Invoke").FirstOrDefault();

            object[] convertedArguments = NumericTypeAide.GetValuesFromFinder(mi.GetParameters().Select(p => new Tuple<string, Type>(p.Name, p.ParameterType)), dataFinder);

            return del.Item1.DynamicInvoke(convertedArguments);
        }

        public object ExecuteExpression(string expressionToParse, object[] arguments, CancellationToken cancellationToken = default(CancellationToken))
        {
            Type numericType = WorkingConstants.defaultNumericType;

            NumericTypeAide.GetProperRequestedNumericalType(arguments, ref numericType);

            var del = GetDelegateForExpression(expressionToParse, numericType, cancellationToken);

            object[] convertedArguments = NumericTypeAide.GetProperNumericTypeValues(arguments, del.Item2);

            return del.Item1.DynamicInvoke(convertedArguments);
        }

        public Delegate GenerateDelegate(string expressionToParse, CancellationToken cancellationToken = default(CancellationToken))
        {
            return GetDelegateForExpression(expressionToParse, WorkingConstants.defaultNumericType, cancellationToken)?.Item1;
        }

        private Tuple<Delegate, Type> GetDelegateForExpression(string expressionToParse, Type numericalType, CancellationToken cancellationToken)
        {
            var del = cachedDelegates.GetOrAdd(expressionToParse, (expr) =>
            {
                Delegate d = eps.GenerateDelegate(expr, numericalType, cancellationToken);
                if (d == null)
                {
                    return new Tuple<Delegate, Type>(new Func<object>(() => expr), numericalType);
                }
                return new Tuple<Delegate, Type>(d, numericalType);
            });

            int numTypeValue;
            if (!NumericTypeAide.NumericTypesConversionDictionary.TryGetValue(numericalType, out numTypeValue))
            {
                throw new InvalidOperationException(Resources.NumericTypeInvalid);
            }

            int numReturnedTypeValue;
            if (!NumericTypeAide.NumericTypesConversionDictionary.TryGetValue(del.Item2, out numReturnedTypeValue))
            {
                throw new InvalidOperationException(Resources.NumericTypeInvalid);
            }

            if (numReturnedTypeValue < numTypeValue)
            {
                Delegate d = eps.GenerateDelegate(expressionToParse, numericalType, cancellationToken);
                Tuple<Delegate, Type> newDel = new Tuple<Delegate, Type>(d == null ? (new Func<object>(() => expressionToParse)) : d, numericalType);
                cachedDelegates.TryUpdate(expressionToParse, newDel, del);

                return newDel;
            }

            return del;
        }
    }
}
#endif