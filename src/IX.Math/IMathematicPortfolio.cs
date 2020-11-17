// <copyright file="IMathematicPortfolio.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using JetBrains.Annotations;

namespace IX.Math
{
    /// <summary>
    /// A service contract for the mathematic portfolio.
    /// </summary>
    /// <seealso cref="IDisposable" />
    [PublicAPI]
    public interface IMathematicPortfolio : IDisposable
    {
        /// <summary>
        ///     Returns the prototypes of all registered functions.
        /// </summary>
        /// <returns>All function names, with all possible combinations of input and output data.</returns>
        string[] GetRegisteredFunctions();

        /// <summary>
        /// Loads the expressions into context.
        /// </summary>
        /// <param name="expressions">The expressions to load.</param>
        void LoadIntoContext(params string[] expressions);

        /// <summary>
        /// Gets the required parameters of an expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>An array with the required parameter names and order.</returns>
        string[]? GetRequiredParameters(string expression);

        /// <summary>
        /// Solves the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="dataFinder">The data finder.</param>
        /// <returns>A computed object.</returns>
        [NotNull]
        object Solve(
            [NotNull] string expression,
            [NotNull] IDataFinder dataFinder);

        /// <summary>
        /// Solves the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <param name="dataFinder">The data finder.</param>
        /// <returns>A computed object.</returns>
        [NotNull]
        object Solve(
            [NotNull] string expression,
            in ComparisonTolerance tolerance,
            [NotNull] IDataFinder dataFinder);

        /// <summary>
        /// Solves the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A computed object.</returns>
        [NotNull]
        object Solve(
            [NotNull] string expression,
            params object[] parameters);

        /// <summary>
        /// Solves the specified expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <param name="parameters">The parameters.</param>
        /// <returns>A computed object.</returns>
        [NotNull]
        object Solve(
            [NotNull] string expression,
            in ComparisonTolerance tolerance,
            params object[] parameters);
    }
}