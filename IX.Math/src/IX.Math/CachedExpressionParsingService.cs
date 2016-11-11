#if !NETSTANDARD10

using IX.Math.BuiltIn;
using IX.Math.SimplificationAide;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

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
    public class CachedExpressionParsingService : IExpressionParsingService, IDisposable
    {
        private ExpressionParsingService eps;
        private ConcurrentDictionary<string, Tuple<Delegate, Type, IEnumerable<ExpressionTreeNodeParameter>>> cachedDelegates;
        private ConcurrentDictionary<string, ComputedExpression> cachedComputedExpressions;
        private bool disposedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedExpressionParsingService"/> class.
        /// </summary>
        public CachedExpressionParsingService()
        {
            eps = new ExpressionParsingService();
            cachedDelegates = new ConcurrentDictionary<string, Tuple<Delegate, Type, IEnumerable<ExpressionTreeNodeParameter>>>();
            cachedComputedExpressions = new ConcurrentDictionary<string, ComputedExpression>();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CachedExpressionParsingService"/> class.
        /// </summary>
        /// <param name="definition">The math definition to use.</param>
        public CachedExpressionParsingService(MathDefinition definition)
        {
            eps = new ExpressionParsingService(definition);
            cachedDelegates = new ConcurrentDictionary<string, Tuple<Delegate, Type, IEnumerable<ExpressionTreeNodeParameter>>>();
            cachedComputedExpressions = new ConcurrentDictionary<string, ComputedExpression>();
        }

        /// <summary>
        /// Disposes of an instance of the <see cref="CachedExpressionParsingService"/> class.
        /// </summary>
        ~CachedExpressionParsingService()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        /// <inheritDoc />
        public ComputedExpression Interpret(string expression, CancellationToken cancellationToken = default(CancellationToken))
        {
            return cachedComputedExpressions.GetOrAdd(expression, expr => eps.Interpret(expr, cancellationToken));
        }

        /// <summary>
        /// Interprets a mathematical expression and executes it, returning the result.
        /// </summary>
        /// <param name="expressionToParse">The mathematical expression to parse.</param>
        /// <param name="cancellationToken">The cancellation token to use for this operation.</param>
        /// <returns>The result of the expression, if calculable, whatever it might be.</returns>
        [Obsolete("This method will be removed in 0.4.0, please use Interpret instead.")]
        public object ExecuteExpression(string expressionToParse, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(nameof(CachedExpressionParsingService));
            }

            var del = GetDelegateForExpression(expressionToParse, WorkingConstants.defaultNumericType, cancellationToken);
            
            if (del == null)
            {
                return expressionToParse;
            }

            try
            {
                return del.Item1.DynamicInvoke();
            }
            catch (Exception)
            {
                return expressionToParse;
            }
        }

        /// <summary>
        /// Interprets a mathematical expression and executes it, returning the result.
        /// </summary>
        /// <param name="expressionToParse">The mathematical expression to parse.</param>
        /// <param name="dataFinder">A service instance that is used to find the data that the expression requires in order to execute.</param>
        /// <param name="cancellationToken">The cancellation token to use for this operation.</param>
        /// <returns>The result of the expression, if calculable, whatever it might be.</returns>
        [Obsolete("This method will be removed in 0.4.0, please use Interpret instead.")]
        public object ExecuteExpression(string expressionToParse, IDataFinder dataFinder, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(nameof(CachedExpressionParsingService));
            }

            Type numericType = WorkingConstants.defaultNumericTypeWithFinder;

            var del = GetDelegateForExpression(expressionToParse, numericType, cancellationToken);

            try
            {
                object[] convertedArguments = NumericTypeAide.GetValuesFromFinder(del.Item3, dataFinder);

                if (convertedArguments.Any(p => p == null))
                {
                    return expressionToParse;
                }

                return del.Item1.DynamicInvoke(convertedArguments);
            }
            catch (Exception)
            {
                return expressionToParse;
            }
        }

        /// <summary>
        /// Interprets a mathematical expression and executes it, returning the result.
        /// </summary>
        /// <param name="expressionToParse">The mathematical expression to parse.</param>
        /// <param name="arguments">The arguments to pass to the expression.</param>
        /// <param name="cancellationToken">The cancellation token to use for this operation.</param>
        /// <returns>The result of the expression, if calculable, whatever it might be.</returns>
        [Obsolete("This method will be removed in 0.4.0, please use Interpret instead.")]
        public object ExecuteExpression(string expressionToParse, object[] arguments, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(nameof(CachedExpressionParsingService));
            }

            Type numericType = WorkingConstants.defaultNumericType;

            NumericTypeAide.GetProperRequestedNumericalType(arguments, ref numericType);

            var del = GetDelegateForExpression(expressionToParse, numericType, cancellationToken);

            object[] convertedArguments = NumericTypeAide.GetProperNumericTypeValues(arguments, del.Item2);

            return del.Item1.DynamicInvoke(convertedArguments);
        }

        /// <summary>
        /// Generates a delegate from a mathematical expression.
        /// </summary>
        /// <param name="expressionToParse">The mathematical expression to parse.</param>
        /// <param name="cancellationToken">The cancellation token to use for this operation.</param>
        /// <returns>A <see cref="Delegate"/> that can be used to calculate the result of the given expression.</returns>
        [Obsolete("This method will be removed in 0.4.0, please use Interpret instead.")]
        public Delegate GenerateDelegate(string expressionToParse, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(nameof(CachedExpressionParsingService));
            }

            return GetDelegateForExpression(expressionToParse, WorkingConstants.defaultNumericType, cancellationToken)?.Item1;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <inheritdoc />
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    cachedDelegates.Clear();
                    cachedComputedExpressions.Clear();
                }

                cachedDelegates = null;
                cachedComputedExpressions = null;
                eps = null;

                disposedValue = true;
            }
        }

        private Tuple<Delegate, Type, IEnumerable<ExpressionTreeNodeParameter>> GetDelegateForExpression(
            string expressionToParse,
            Type numericalType,
            CancellationToken cancellationToken)
        {
            var del = cachedDelegates.GetOrAdd(expressionToParse, (expr) =>
            {
                var d = eps.GenerateDelegateInternal(expr, numericalType, cancellationToken);
                if (d == null)
                {
                    return new Tuple<Delegate, Type, IEnumerable<ExpressionTreeNodeParameter>>(new Func<object>(() => expr), numericalType, new ExpressionTreeNodeParameter[0]);
                }
                return new Tuple<Delegate, Type, IEnumerable<ExpressionTreeNodeParameter>>(d.Item1, numericalType, d.Item2);
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
                var d = eps.GenerateDelegateInternal(expressionToParse, numericalType, cancellationToken);
                var newDel = new Tuple<Delegate, Type, IEnumerable<ExpressionTreeNodeParameter>>(d?.Item1 == null ? (new Func<object>(() => expressionToParse)) : d.Item1, numericalType, d.Item2);
                cachedDelegates.TryUpdate(expressionToParse, newDel, del);

                return newDel;
            }

            return del;
        }
    }
}
#endif