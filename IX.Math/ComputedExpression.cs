using IX.Math.BuiltIn;
using IX.Math.SimplificationAide;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace IX.Math
{
    /// <summary>
    /// A representation of a computed expression, resulting from a string expression.
    /// </summary>
    public class ComputedExpression : IDisposable
    {
        private readonly string initialExpression;
        private ExpressionTreeNodeBase body;
        private ExpressionTreeNodeParameter[] parameters;
        private object locker;
        private Dictionary<int, Delegate> computedBodies;
        private bool disposedValue;

        internal ComputedExpression(string initialExpression, ExpressionTreeNodeBase body, ExpressionTreeNodeParameter[] parameters, bool isRecognized)
        {
            this.initialExpression = initialExpression;
            this.body = body;
            RecognizedCorrectly = isRecognized;
            locker = new object();
            computedBodies = new Dictionary<int, Delegate>();
            this.parameters = parameters;
            ParameterNames = parameters.Select(p => p.Name).ToArray();
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="ComputedExpression"/> class.
        /// </summary>
        ~ComputedExpression()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing).
            Dispose(false);
        }

        /// <summary>
        /// Gets a value indicating whether or not the expression was actually recognized. <c>true</c> can possibly return an actual 
        /// </summary>
        public bool RecognizedCorrectly { get; private set; }

        /// <summary>
        /// Gets the names of the parameters this expression requires, if any.
        /// </summary>
        public string[] ParameterNames { get; private set; }

        /// <summary>
        /// Disposes an instance of the <see cref="ComputedExpression"/> class.
        /// </summary>
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing).
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Computes the expression and returns a result.
        /// </summary>
        /// <param name="arguments">The arguments with which to invoke the execution of the expression.</param>
        /// <returns>The computed result, or, if the expression is not recognized correctly, the expression as a <see cref="string"/>.</returns>
        public object Compute(params object[] arguments)
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(nameof(ComputedExpression));
            }

            if (!RecognizedCorrectly)
            {
                return initialExpression;
            }

            Type numericType = WorkingConstants.defaultNumericType;
            NumericTypeAide.GetProperRequestedNumericalType(arguments, ref numericType);

            int numericTypeValue = body.ComputeResultingNumericTypeValue(NumericTypeAide.NumericTypesConversionDictionary[numericType]);

            var convertedArguments = NumericTypeAide.GetProperNumericTypeValues(arguments, NumericTypeAide.InverseNumericTypesConversionDictionary[numericTypeValue]);

            Delegate del = GetDelegate(numericTypeValue);

            if (del == null)
            {
                return initialExpression;
            }

            try
            {
                return del.DynamicInvoke(convertedArguments);
            }
            catch
            {
                return initialExpression;
            }
        }

        /// <summary>
        /// Computes the expression and returns a result.
        /// </summary>
        /// <param name="dataFinder">The data finder for the arguments whith which to invoke execution of the expression.</param>
        /// <returns>The computed result, or, if the expression is not recognized correctly, the expression as a <see cref="string"/>.</returns>
        public object Compute(IDataFinder dataFinder)
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException(nameof(ComputedExpression));
            }

            if (!RecognizedCorrectly)
            {
                return initialExpression;
            }

            List<object> pars = new List<object>();

            foreach (var p in parameters)
            {
                object data;
                if (!dataFinder.TryGetData(p.Name, out data))
                {
                    data = null;
                }
                pars.Add(data);
            };

            if (pars.Any(p => p == null))
            {
                return initialExpression;
            }

            return Compute(pars.ToArray());
        }

        /// <summary>
        /// Disposes an instance of the <see cref="ComputedExpression"/> class.
        /// </summary>
        /// <param name="disposing">Indicates whether or not disposal is a result of a normal dispose usage.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    lock (locker)
                    {
                        computedBodies.Clear();
                    }
                }

                computedBodies = null;
                body = null;
                parameters = null;

                disposedValue = true;
            }
        }

        private Delegate GetDelegate(int numericTypeValue)
        {
            Delegate result;
            if (computedBodies.TryGetValue(numericTypeValue, out result))
            {
                return result;
            }

            lock (locker)
            {
                if (computedBodies.TryGetValue(numericTypeValue, out result))
                {
                    return result;
                }

                Expression bodyExpression;
                try
                {
                    bodyExpression = body.GenerateExpression(numericTypeValue);
                }
                catch
                {
                    return null;
                }

                result = Expression.Lambda(bodyExpression, parameters.Select(p => (ParameterExpression)p.GenerateExpression(numericTypeValue)))
                    ?.Compile();

                foreach (var parameterExpression in parameters)
                {
                    parameterExpression.Reset();
                }

                if (result == null)
                {
                    return null;
                }

                computedBodies.Add(numericTypeValue, result);
            }

            return result;
        }
    }
}