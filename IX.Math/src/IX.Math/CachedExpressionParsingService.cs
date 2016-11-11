#if !NETSTANDARD10

using IX.Math.BuiltIn;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace IX.Math
{
    /// <summary>
    /// A service that is able to parse strings containing mathematical expressions and solve them in a cached way.
    /// </summary>
    /// <remarks>
    /// <para>This service also caches expressions so that they can be garbage-collected after a specific time period with no use.</para>
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
    }
}
#endif