﻿#if !NETSTANDARD10
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
    public class CachedExpressionParsingService : IExpressionParsingService, IDisposable
    {
        private ExpressionParsingService eps;
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

        /// <summary>
        /// Interprets a mathematical expression and executes it, returning the result.
        /// </summary>
        /// <param name="expressionToParse">The mathematical expression to parse.</param>
        /// <param name="cancellationToken">The cancellation token to use for this operation.</param>
        /// <returns>The result of the expression, if calculable, whatever it might be.</returns>
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

            return del.Item1.DynamicInvoke();
        }

        /// <summary>
        /// Interprets a mathematical expression and executes it, returning the result.
        /// </summary>
        /// <param name="expressionToParse">The mathematical expression to parse.</param>
        /// <param name="dataFinder">A service instance that is used to find the data that the expression requires in order to execute.</param>
        /// <param name="cancellationToken">The cancellation token to use for this operation.</param>
        /// <returns>The result of the expression, if calculable, whatever it might be.</returns>
        public object ExecuteExpression(string expressionToParse, IDataFinder dataFinder, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(nameof(CachedExpressionParsingService));
            }

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

        /// <summary>
        /// Interprets a mathematical expression and executes it, returning the result.
        /// </summary>
        /// <param name="expressionToParse">The mathematical expression to parse.</param>
        /// <param name="arguments">The arguments to pass to the expression.</param>
        /// <param name="cancellationToken">The cancellation token to use for this operation.</param>
        /// <returns>The result of the expression, if calculable, whatever it might be.</returns>
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
        public Delegate GenerateDelegate(string expressionToParse, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(nameof(CachedExpressionParsingService));
            }

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

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        /// <inheritdoc />
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    cachedDelegates.Clear();
                }

                cachedDelegates = null;
                eps = null;

                disposedValue = true;
            }
        }

        /// <summary>
        /// Disposes of an instance of the <see cref="CachedExpressionParsingService"/> class.
        /// </summary>
        ~CachedExpressionParsingService()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
#endif