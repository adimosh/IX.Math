// <copyright file="UnaryFunctionNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using System.Reflection;
using IX.StandardExtensions.Extensions;
using JetBrains.Annotations;

namespace IX.Math.Nodes
{
    /// <summary>
    ///     A base class for a unary function (a function with only one parameter).
    /// </summary>
    /// <seealso cref="FunctionNodeBase" />
    [PublicAPI]
    public abstract class UnaryFunctionNodeBase : FunctionNodeBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UnaryFunctionNodeBase" /> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <exception cref="ArgumentNullException">parameter.</exception>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Usage",
            "CA2214:Do not call overridable methods in constructors",
            Justification = "This is OK and expected at this point.")]
        protected UnaryFunctionNodeBase([NotNull] NodeBase parameter)
        {
            NodeBase parameterTemp = parameter ?? throw new ArgumentNullException(nameof(parameter));

            // ReSharper disable once VirtualMemberCallInConstructor - We want this to happen
            this.EnsureCompatibleParameter(parameterTemp);

            this.Parameter = parameterTemp.Simplify();
        }

        /// <summary>
        ///     Gets the parameter.
        /// </summary>
        /// <value>The parameter.</value>
        [NotNull]
        public NodeBase Parameter { get; private set; }

        /// <summary>
        ///     Ensures that the parameter that is received is compatible with the function, optionally allowing the parameter
        ///     reference to change.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected abstract void EnsureCompatibleParameter([NotNull] NodeBase parameter);

        /// <summary>
        ///     Generates a static unary function call.
        /// </summary>
        /// <typeparam name="T">The type to call the method from.</typeparam>
        /// <param name="functionName">Name of the function.</param>
        /// <returns>An expression representing the static function call.</returns>
        /// <exception cref="ArgumentException"><paramref name="functionName" /> represents a function that cannot be found.</exception>
        [NotNull]
        protected Expression GenerateStaticUnaryFunctionCall<T>([NotNull] string functionName) =>
            this.GenerateStaticUnaryFunctionCall(
                typeof(T),
                functionName,
                null);

        /// <summary>
        ///     Generates a static unary function call.
        /// </summary>
        /// <typeparam name="T">The type to call the method from.</typeparam>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="tolerance">The tolerance for this expression. Can be <c>null</c> (<c>Nothing</c> in Visual Basic).</param>
        /// <returns>An expression representing the static function call.</returns>
        /// <exception cref="ArgumentException"><paramref name="functionName" /> represents a function that cannot be found.</exception>
        [NotNull]
        protected Expression GenerateStaticUnaryFunctionCall<T>(
            [NotNull] string functionName,
            Tolerance tolerance) =>
            this.GenerateStaticUnaryFunctionCall(
                typeof(T),
                functionName,
                tolerance);

        /// <summary>
        ///     Generates a static unary function call.
        /// </summary>
        /// <param name="t">The type to call the method from.</param>
        /// <param name="functionName">Name of the function.</param>
        /// <returns>An expression representing the static function call.</returns>
        /// <exception cref="ArgumentException"><paramref name="functionName" /> represents a function that cannot be found.</exception>
        [NotNull]
        protected Expression GenerateStaticUnaryFunctionCall(
            [NotNull] Type t,
            [NotNull] string functionName) =>
            this.GenerateStaticUnaryFunctionCall(
                t,
                functionName,
                null);

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
        protected Expression GenerateStaticUnaryFunctionCall(
            [NotNull] Type t,
            [NotNull] string functionName,
            [CanBeNull] Tolerance tolerance)
        {
            if (string.IsNullOrWhiteSpace(functionName))
            {
                throw new ArgumentException(
                    string.Format(
                        Resources.FunctionCouldNotBeFound,
                        functionName),
                    nameof(functionName));
            }

            Type parameterType = ParameterTypeFromParameter(this.Parameter);

            MethodInfo mi = t.GetMethodWithExactParameters(
                functionName,
                parameterType);

            if (mi == null)
            {
                parameterType = typeof(double);

                mi = t.GetMethodWithExactParameters(
                    functionName,
                    parameterType);

                if (mi == null)
                {
                    parameterType = typeof(long);

                    mi = t.GetMethodWithExactParameters(
                        functionName,
                        parameterType);

                    if (mi == null)
                    {
                        parameterType = typeof(int);

                        mi = t.GetMethodWithExactParameters(
                                 functionName,
                                 parameterType) ??
                             throw new ArgumentException(
                                 string.Format(
                                     Resources.FunctionCouldNotBeFound,
                                     functionName),
                                 nameof(functionName));
                    }
                }
            }

            Expression e = tolerance == null
                ? this.Parameter.GenerateExpression()
                : this.Parameter.GenerateExpression(tolerance);

            if (e.Type != parameterType)
            {
                e = Expression.Convert(
                    e,
                    parameterType);
            }

            return Expression.Call(
                mi,
                e);
        }

        /// <summary>
        ///     Generates a property call on the parameter.
        /// </summary>
        /// <typeparam name="T">The type to call the property from.</typeparam>
        /// <param name="propertyName">Name of the parameter.</param>
        /// <returns>An expression representing a property call.</returns>
        /// <exception cref="ArgumentException"><paramref name="propertyName" /> represents a property that cannot be found.</exception>
        [NotNull]
        protected Expression GenerateParameterPropertyCall<T>([NotNull] string propertyName) =>
            this.GenerateParameterPropertyCall<T>(
                propertyName,
                null);

        /// <summary>
        ///     Generates a property call on the parameter.
        /// </summary>
        /// <typeparam name="T">The type to call the property from.</typeparam>
        /// <param name="propertyName">Name of the parameter.</param>
        /// <param name="tolerance">The tolerance for this expression. Can be <c>null</c> (<c>Nothing</c> in Visual Basic).</param>
        /// <returns>An expression representing a property call.</returns>
        /// <exception cref="ArgumentException"><paramref name="propertyName" /> represents a property that cannot be found.</exception>
        [NotNull]
        protected Expression GenerateParameterPropertyCall<T>(
            [NotNull] string propertyName,
            [CanBeNull] Tolerance tolerance)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException(
                    string.Format(
                        Resources.FunctionCouldNotBeFound,
                        propertyName),
                    nameof(propertyName));
            }

            PropertyInfo pi = typeof(T).GetRuntimeProperty(propertyName) ??
                              throw new ArgumentException(
                                  string.Format(
                                      Resources.FunctionCouldNotBeFound,
                                      propertyName),
                                  nameof(propertyName));

            return Expression.Property(
                tolerance == null ? this.Parameter.GenerateExpression() : this.Parameter.GenerateExpression(tolerance),
                pi);
        }

        /// <summary>
        ///     Generates a property call on the parameter.
        /// </summary>
        /// <typeparam name="T">The type to call the property from.</typeparam>
        /// <param name="methodName">Name of the parameter.</param>
        /// <returns>An expression representing a property call.</returns>
        /// <exception cref="ArgumentException"><paramref name="methodName" /> represents a property that cannot be found.</exception>
        [NotNull]
        protected Expression GenerateParameterMethodCall<T>([NotNull] string methodName) =>
            this.GenerateParameterMethodCall<T>(
                methodName,
                null);

        /// <summary>
        ///     Generates a property call on the parameter.
        /// </summary>
        /// <typeparam name="T">The type to call the property from.</typeparam>
        /// <param name="methodName">Name of the parameter.</param>
        /// <param name="tolerance">The tolerance for this expression. Can be <c>null</c> (<c>Nothing</c> in Visual Basic).</param>
        /// <returns>An expression representing a property call.</returns>
        /// <exception cref="ArgumentException"><paramref name="methodName" /> represents a property that cannot be found.</exception>
        [NotNull]
        protected Expression GenerateParameterMethodCall<T>(
            [NotNull] string methodName,
            [CanBeNull] Tolerance tolerance)
        {
            if (string.IsNullOrWhiteSpace(methodName))
            {
                throw new ArgumentException(
                    string.Format(
                        Resources.FunctionCouldNotBeFound,
                        methodName),
                    nameof(methodName));
            }

#if NET452
            MethodInfo mi = typeof(T).GetRuntimeMethod(
                                methodName,
                                new Type[0]) ??
                            throw new ArgumentException(
                                string.Format(
                                    Resources.FunctionCouldNotBeFound,
                                    methodName),
                                nameof(methodName));
#else
            MethodInfo mi = typeof(T).GetRuntimeMethod(
                                methodName,
                                Array.Empty<Type>()) ??
                            throw new ArgumentException(
                                string.Format(
                                    Resources.FunctionCouldNotBeFound,
                                    methodName),
                                nameof(methodName));
#endif

            return tolerance == null
                ? Expression.Call(
                    this.Parameter.GenerateExpression(),
                    mi)
                : Expression.Call(
                    this.Parameter.GenerateExpression(tolerance),
                    mi);
        }
    }
}