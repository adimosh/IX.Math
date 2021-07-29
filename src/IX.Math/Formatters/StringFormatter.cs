// <copyright file="StringFormatter.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Globalization;
using System.Linq.Expressions;
using IX.Abstractions.Logging;
using IX.StandardExtensions;
using IX.StandardExtensions.Contracts;

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
        public static string FormatIntoString<T>(T value)
        {
            Log.Debug($"Formatting {value} into string.");

            if (typeof(T) == typeof(string))
            {
                return (value as string)!;
            }

            var (success, result) = PluginCollection.Current.InterpretAsString(value);

            return success ? (result ?? ToStringRegular(value)) : ToStringRegular(value);

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
        public static Expression CreateStringConversionExpression(
                    Expression expression)
        {
            Requires.NotNull(
                expression,
                nameof(expression));

            if (expression.Type == typeof(string))
            {
                return expression;
            }

            if (expression.Type == typeof(long))
            {
                Expression<Func<long, string>> innerLambda = value => FormatIntoString(value);
                return Expression.Invoke(
                    innerLambda,
                    expression);
            }

            if (expression.Type == typeof(int))
            {
                Expression<Func<int, string>> innerLambda = value => FormatIntoString(value);
                return Expression.Invoke(
                    innerLambda,
                    expression);
            }

            if (expression.Type == typeof(bool))
            {
                Expression<Func<bool, string>> innerLambda = value => FormatIntoString(value);
                return Expression.Invoke(
                    innerLambda,
                    expression);
            }

            if (expression.Type == typeof(double))
            {
                Expression<Func<double, string>> innerLambda = value => FormatIntoString(value);
                return Expression.Invoke(
                    innerLambda,
                    expression);
            }

            if (expression.Type == typeof(byte[]))
            {
                Expression<Func<byte[], string>> innerLambda = value => FormatIntoString(value);
                return Expression.Invoke(
                    innerLambda,
                    expression);
            }

            throw new ExpressionNotValidLogicallyException();
        }
    }
}