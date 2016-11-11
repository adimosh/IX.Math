using System;
using System.Threading;

namespace IX.Math
{
    /// <summary>
    /// A contract for a service that is able to parse strings containing mathematical expressions and solve them.
    /// </summary>
    public interface IExpressionParsingService
    {
        /// <summary>
        /// Interprets a mathematical expression and executes it, returning the result.
        /// </summary>
        /// <param name="expressionToParse">The mathematical expression to parse.</param>
        /// <param name="cancellationToken">The cancellation token to use for this operation.</param>
        /// <returns>The result of the expression, if calculable, whatever it might be.</returns>
        [Obsolete("This method will be removed in 0.4.0, please use Interpret instead.")]
        object ExecuteExpression(string expressionToParse, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Interprets a mathematical expression and executes it, returning the result.
        /// </summary>
        /// <param name="expressionToParse">The mathematical expression to parse.</param>
        /// <param name="arguments">The arguments to pass to the expression.</param>
        /// <param name="cancellationToken">The cancellation token to use for this operation.</param>
        /// <returns>The result of the expression, if calculable, whatever it might be.</returns>
        [Obsolete("This method will be removed in 0.4.0, please use Interpret instead.")]
        object ExecuteExpression(string expressionToParse, object[] arguments, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Interprets a mathematical expression and executes it, returning the result.
        /// </summary>
        /// <param name="expressionToParse">The mathematical expression to parse.</param>
        /// <param name="dataFinder">A service instance that is used to find the data that the expression requires in order to execute.</param>
        /// <param name="cancellationToken">The cancellation token to use for this operation.</param>
        /// <returns>The result of the expression, if calculable, whatever it might be.</returns>
        [Obsolete("This method will be removed in 0.4.0, please use Interpret instead.")]
        object ExecuteExpression(string expressionToParse, IDataFinder dataFinder, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Generates a delegate from a mathematical expression.
        /// </summary>
        /// <param name="expressionToParse">The mathematical expression to parse.</param>
        /// <param name="cancellationToken">The cancellation token to use for this operation.</param>
        /// <returns>A <see cref="Delegate"/> that can be used to calculate the result of the given expression.</returns>
        [Obsolete("This method will be removed in 0.4.0, please use Interpret instead.")]
        Delegate GenerateDelegate(string expressionToParse, CancellationToken cancellationToken = default(CancellationToken));

        /// <summary>
        /// Interprets the mathematical expression and returns a container that can be invoked for solving using specific mathematical types.
        /// </summary>
        /// <param name="expression">The expression to interpret.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A <see cref="ComputedExpression"/> that represent</returns>
        ComputedExpression Interpret(string expression, CancellationToken cancellationToken = default(CancellationToken));
    }
}