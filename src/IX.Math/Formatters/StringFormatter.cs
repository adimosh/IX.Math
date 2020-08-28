// <copyright file="StringFormatter.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Globalization;
using System.Linq.Expressions;
using IX.StandardExtensions;
using IX.StandardExtensions.Contracts;
using JetBrains.Annotations;
using DiagCA = System.Diagnostics.CodeAnalysis;

namespace IX.Math.Formatters
{
    /// <summary>
    /// Contains helper methods to format strings.
    /// </summary>
    public static class StringFormatter
    {
        /// <summary>
        /// Formats a value into string using formatters, if any.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="value">The value.</param>
        /// <returns>A formatted string, if the input type is supported.</returns>
        public static string FormatIntoString<T>(
            T value)
        {
            var sf = MathematicPortfolio.CurrentContext?.Value?.stringFormatters;
            if (sf == null)
            {
                return ToStringRegular(value);
            }

            foreach (var formatter in sf)
            {
                var (success, result) = formatter.ParseIntoString(value);
                if (success)
                {
                    return result;
                }
            }

            return ToStringRegular(value);

            static string ToStringRegular(T value) =>
                value switch
                {
                    int i => i.ToString(CultureInfo.CurrentCulture),
                    long l => l.ToString(CultureInfo.CurrentCulture),
                    bool b => b.ToString(CultureInfo.CurrentCulture),
                    double d => d.ToString(CultureInfo.CurrentCulture),
                    byte[] ba => BitConverter.ToString(ba),
                    string s => s,
                    _ => throw new ArgumentInvalidTypeException(nameof(value)),
                };
        }

        /// <summary>
        /// Creates the string conversion expression.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <returns>An expression representing the string transformation.</returns>
        [DiagCA.SuppressMessage(
            "Performance",
            "HAA0301:Closure Allocation Source",
            Justification = "We're actively looking for closures in this method.")]
        [DiagCA.SuppressMessage(
            "Performance",
            "HAA0302:Display class allocation to capture closure",
            Justification = "We're actively looking for closures in this method.")]
        [DiagCA.SuppressMessage(
            "ReSharper",
            "AssignNullToNotNullAttribute",
            Justification = "We've checked manually.")]
        [DiagCA.SuppressMessage(
            "ReSharper",
            "InvertIf",
            Justification = "We don't care about inverting ifs here.")]
        [NotNull]
        public static Expression CreateStringConversionExpression(Expression expression)
        {
            Requires.NotNull(
                expression,
                nameof(expression));

            if (expression.Type == typeof(string))
            {
                return expression;
            }

            bool areFormatters = (MathematicPortfolio.CurrentContext.Value.StringFormatters?.Count ?? 0) > 0;

            if (expression.Type == typeof(long))
            {
                if (areFormatters)
                {
                    Expression<Func<long, string>> innerLambda = value => FormatIntoString(value);
                    return Expression.Invoke(
                        innerLambda,
                        expression);
                }

                return Expression.Call(
                    expression,
                    typeof(long).GetMethod(
                        nameof(long.ToString),
                        new[] { typeof(IFormatProvider) }),
                    Expression.Property(
                        null,
                        typeof(CultureInfo),
                        nameof(CultureInfo.CurrentCulture)));
            }

            if (expression.Type == typeof(int))
            {
                if (areFormatters)
                {
                    Expression<Func<int, string>> innerLambda = value => FormatIntoString(value);
                    return Expression.Invoke(
                        innerLambda,
                        expression);
                }

                return Expression.Call(
                    expression,
                    typeof(int).GetMethod(
                        nameof(int.ToString),
                        new[] { typeof(IFormatProvider) }),
                    Expression.Property(
                        null,
                        typeof(CultureInfo),
                        nameof(CultureInfo.CurrentCulture)));
            }

            if (expression.Type == typeof(bool))
            {
                if (areFormatters)
                {
                    Expression<Func<bool, string>> innerLambda = value => FormatIntoString(value);
                    return Expression.Invoke(
                        innerLambda,
                        expression);
                }

                return Expression.Call(
                    expression,
                    typeof(bool).GetMethod(
                        nameof(bool.ToString),
                        new[] { typeof(IFormatProvider) }),
                    Expression.Property(
                        null,
                        typeof(CultureInfo),
                        nameof(CultureInfo.CurrentCulture)));
            }

            if (expression.Type == typeof(double))
            {
                if (areFormatters)
                {
                    Expression<Func<double, string>> innerLambda = value => FormatIntoString(value);
                    return Expression.Invoke(
                        innerLambda,
                        expression);
                }

                return Expression.Call(
                    expression,
                    typeof(double).GetMethod(
                        nameof(double.ToString),
                        new[] { typeof(IFormatProvider) }),
                    Expression.Property(
                        null,
                        typeof(CultureInfo),
                        nameof(CultureInfo.CurrentCulture)));
            }

            if (expression.Type == typeof(byte[]))
            {
                if (areFormatters)
                {
                    Expression<Func<byte[], string>> innerLambda = value => FormatIntoString(value);
                    return Expression.Invoke(
                        innerLambda,
                        expression);
                }

                return Expression.Call(
                    null,
                    typeof(BitConverter).GetMethod(
                        nameof(BitConverter.ToString),
                        new[] { typeof(byte[]) }),
                    expression);
            }

            throw new Exceptions.ExpressionNotValidLogicallyException();
        }
    }
}