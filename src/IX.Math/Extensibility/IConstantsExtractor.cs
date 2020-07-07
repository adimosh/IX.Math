// <copyright file="IConstantsExtractor.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using JetBrains.Annotations;

namespace IX.Math.Extensibility
{
    /// <summary>
    ///     A service contract for extractors of constant values from the expression.
    /// </summary>
    [PublicAPI]
    public interface IConstantsExtractor
    {
        /// <summary>
        ///     Extracts a constants, returning its value and placement.
        /// </summary>
        /// <param name="originalExpression">The original expression.</param>
        /// <param name="mathDefinition">The math definition.</param>
        /// <returns>
        ///     A tuple containing a switch indicating success or failure, the extracted value, if any, and the position at which
        ///     it is, as well as its length.
        /// </returns>
        /// <remarks>
        ///     Please note that the Position argument should be returned if a possible match is found, even if it does not
        ///     compute properly.
        /// </remarks>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance",
            "EPS02:Non-readonly struct used as in-parameter",
            Justification = "Special built-in type, the compiler can handle it.")]
        (bool Success, object Value, int Position, int Length) ExtractConstant(
            in ReadOnlySpan<char> originalExpression,
            MathDefinition mathDefinition);
    }
}