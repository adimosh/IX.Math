// <copyright file="BinaryFunctionNodeBase.Obsolete.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;

// ReSharper disable once CheckNamespace
namespace IX.Math.Nodes
{
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1601:Partial elements should be documented",
        Justification = "This obsolete part of the class will be removed, we don't really care.")]
    public partial class BinaryFunctionNodeBase
    {
        /// <summary>
        /// Generates a static binary function call expression.
        /// </summary>
        /// <param name="t">The type to call on.</param>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="tolerance">The tolerance, should there be any. This argument can be <c>null</c> (<c>Nothing</c> in Visual Basic).</param>
        /// <returns>
        /// Expression.
        /// </returns>
        /// <exception cref="ArgumentException">The function name is invalid.</exception>
        [Obsolete("Please use the overload with a ComparisonTolerance parameter.")]
        protected Expression GenerateBinaryFunctionCallFirstParameterInstance(
            Type t,
            string functionName,
            Tolerance tolerance)
        {
            ComparisonTolerance ct = tolerance;
            return this.GenerateBinaryFunctionCallFirstParameterInstance(
                t,
                functionName,
                in ct);
        }

        /// <summary>
        /// Generates a static binary function call with explicit parameter types.
        /// </summary>
        /// <typeparam name="TParam1">The type of the first parameter.</typeparam>
        /// <typeparam name="TParam2">The type of the second parameter.</typeparam>
        /// <param name="t">The type to call on.</param>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="tolerance">The tolerance, should there be any. This argument can be <c>null</c> (<c>Nothing</c> in Visual Basic).</param>
        /// <returns>
        /// The generated binary method call expression.
        /// </returns>
        /// <exception cref="ArgumentException">The function name is invalid.</exception>
        [Obsolete("Please use the overload with a ComparisonTolerance parameter.")]
        protected Expression GenerateStaticBinaryFunctionCall<TParam1, TParam2>(
            Type t,
            string functionName,
            Tolerance tolerance)
        {
            ComparisonTolerance ct = tolerance;
            return this.GenerateStaticBinaryFunctionCall<TParam1, TParam2>(
                t,
                functionName,
                in ct);
        }

        /// <summary>
        /// Generates a static binary function call expression.
        /// </summary>
        /// <param name="t">The type to call on.</param>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="tolerance">The tolerance, should there be any. This argument can be <c>null</c> (<c>Nothing</c> in Visual Basic).</param>
        /// <returns>Expression.</returns>
        /// <exception cref="ArgumentException">The function name is invalid.</exception>
        [Obsolete("Please use the overload with a ComparisonTolerance parameter.")]
        protected Expression GenerateStaticBinaryFunctionCall(
            Type t,
            string functionName,
            Tolerance tolerance)
        {
            ComparisonTolerance ct = tolerance;
            return this.GenerateStaticBinaryFunctionCall(
                t,
                functionName,
                in ct);
        }

        /// <summary>
        /// Generates a static binary function call expression.
        /// </summary>
        /// <typeparam name="T">The type to call on.</typeparam>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="tolerance">The tolerance, should there be any. This argument can be <c>null</c> (<c>Nothing</c> in Visual Basic).</param>
        /// <returns>An expression representing the call.</returns>
        [Obsolete("Please use the overload with a ComparisonTolerance parameter.")]
        protected Expression GenerateStaticBinaryFunctionCall<T>(
            string functionName,
            Tolerance tolerance)
        {
            ComparisonTolerance ct = tolerance;
            return this.GenerateStaticBinaryFunctionCall<T>(
                functionName,
                in ct);
        }
    }
}