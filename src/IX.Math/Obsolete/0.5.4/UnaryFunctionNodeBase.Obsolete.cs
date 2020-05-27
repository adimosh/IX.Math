// <copyright file="UnaryFunctionNodeBase.Obsolete.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace IX.Math.Nodes
{
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1601:Partial elements should be documented",
        Justification = "This obsolete part of the class will be removed, we don't really care.")]
    public partial class UnaryFunctionNodeBase
    {
        /// <summary>
        ///     Generates a static unary function call.
        /// </summary>
        /// <typeparam name="T">The type to call the method from.</typeparam>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="tolerance">The tolerance for this expression. Can be <c>null</c> (<c>Nothing</c> in Visual Basic).</param>
        /// <returns>An expression representing the static function call.</returns>
        /// <exception cref="ArgumentException"><paramref name="functionName" /> represents a function that cannot be found.</exception>
        [NotNull]
        [Obsolete("Please use the overload with a ComparisonTolerance parameter.")]
        protected Expression GenerateStaticUnaryFunctionCall<T>(
            [NotNull] string functionName,
            Tolerance tolerance)
        {
            ComparisonTolerance ct = tolerance;
            return this.GenerateStaticUnaryFunctionCall<T>(
                functionName,
                in ct);
        }

        /// <summary>
        ///     Generates a static unary function call.
        /// </summary>
        /// <param name="t">The type to call the method from.</param>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="tolerance">The tolerance for this expression. Can be <c>null</c> (<c>Nothing</c> in Visual Basic).</param>
        /// <returns>
        ///     An expression representing the static function call.
        /// </returns>
        /// <exception cref="ArgumentException"><paramref name="functionName" /> represents a function that cannot be found.</exception>
        [NotNull]
        [Obsolete("Please use the overload with a ComparisonTolerance parameter.")]
        protected Expression GenerateStaticUnaryFunctionCall(
            [NotNull] Type t,
            [NotNull] string functionName,
            [CanBeNull] Tolerance tolerance)
        {
            ComparisonTolerance ct = tolerance;
            return this.GenerateStaticUnaryFunctionCall(
                t,
                functionName,
                in ct);
        }

        /// <summary>
        ///     Generates a property call on the parameter.
        /// </summary>
        /// <typeparam name="T">The type to call the property from.</typeparam>
        /// <param name="propertyName">Name of the parameter.</param>
        /// <param name="tolerance">The tolerance for this expression. Can be <c>null</c> (<c>Nothing</c> in Visual Basic).</param>
        /// <returns>An expression representing a property call.</returns>
        /// <exception cref="ArgumentException"><paramref name="propertyName" /> represents a property that cannot be found.</exception>
        [NotNull]
        [Obsolete("Please use the overload with a ComparisonTolerance parameter.")]
        protected Expression GenerateParameterPropertyCall<T>(
            [NotNull] string propertyName,
            [CanBeNull] Tolerance tolerance)
        {
            ComparisonTolerance ct = tolerance;
            return this.GenerateParameterPropertyCall<T>(
                propertyName,
                in ct);
        }

        /// <summary>
        ///     Generates a property call on the parameter.
        /// </summary>
        /// <typeparam name="T">The type to call the property from.</typeparam>
        /// <param name="methodName">Name of the parameter.</param>
        /// <param name="tolerance">The tolerance for this expression. Can be <c>null</c> (<c>Nothing</c> in Visual Basic).</param>
        /// <returns>An expression representing a property call.</returns>
        /// <exception cref="ArgumentException"><paramref name="methodName" /> represents a property that cannot be found.</exception>
        [NotNull]
        [Obsolete("Please use the overload with a ComparisonTolerance parameter.")]
        protected Expression GenerateParameterMethodCall<T>(
            [NotNull] string methodName,
            [CanBeNull] Tolerance tolerance)
        {
            ComparisonTolerance ct = tolerance;
            return this.GenerateParameterMethodCall<T>(
                methodName,
                in ct);
        }
    }
}