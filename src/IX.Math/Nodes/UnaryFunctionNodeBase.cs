// <copyright file="UnaryFunctionNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Extensibility;
using IX.StandardExtensions.Extensions;
using JetBrains.Annotations;

namespace IX.Math.Nodes
{
    /// <summary>
    ///     A base class for a unary function (a function with only one parameter).
    /// </summary>
    /// <seealso cref="FunctionNodeBase" />
    [PublicAPI]
    public abstract partial class UnaryFunctionNodeBase : FunctionNodeBase
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
        ///     Gets a value indicating whether this node supports tolerance.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is tolerant; otherwise, <c>false</c>.
        /// </value>
        public override bool IsTolerant => this.Parameter.IsTolerant;

        /// <summary>
        /// Sets the special object request function for sub objects.
        /// </summary>
        /// <param name="func">The function.</param>
        protected override void SetSpecialObjectRequestFunctionForSubObjects(Func<Type, object> func)
        {
            if (this.Parameter is ISpecialRequestNode srnl)
            {
                srnl.SetRequestSpecialObjectFunction(func);
            }
        }

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
                in ComparisonTolerance.Empty);

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
            in ComparisonTolerance tolerance) =>
            this.GenerateStaticUnaryFunctionCall(
                typeof(T),
                functionName,
                in tolerance);

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
                in ComparisonTolerance.Empty);

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
            in ComparisonTolerance tolerance)
        {
            if (string.IsNullOrWhiteSpace(functionName))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
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
                                     CultureInfo.CurrentCulture,
                                     Resources.FunctionCouldNotBeFound,
                                     functionName),
                                 nameof(functionName));
                    }
                }
            }

            Expression e = tolerance.IsEmpty
                ? this.Parameter.GenerateExpression()
                : this.Parameter.GenerateExpression(in tolerance);

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
                in ComparisonTolerance.Empty);

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
            in ComparisonTolerance tolerance)
        {
            if (string.IsNullOrWhiteSpace(propertyName))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
                        Resources.FunctionCouldNotBeFound,
                        propertyName),
                    nameof(propertyName));
            }

            PropertyInfo pi = typeof(T).GetRuntimeProperty(propertyName) ??
                              throw new ArgumentException(
                                  string.Format(
                                      CultureInfo.CurrentCulture,
                                      Resources.FunctionCouldNotBeFound,
                                      propertyName),
                                  nameof(propertyName));

            return Expression.Property(
                tolerance.IsEmpty ? this.Parameter.GenerateExpression() : this.Parameter.GenerateExpression(in tolerance),
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
                in ComparisonTolerance.Empty);

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
            in ComparisonTolerance tolerance)
        {
            if (string.IsNullOrWhiteSpace(methodName))
            {
                throw new ArgumentException(
                    string.Format(
                        CultureInfo.CurrentCulture,
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
                                    CultureInfo.CurrentCulture,
                                    Resources.FunctionCouldNotBeFound,
                                    methodName),
                                nameof(methodName));
#else
            MethodInfo mi = typeof(T).GetRuntimeMethod(
                                methodName,
                                Array.Empty<Type>()) ??
                            throw new ArgumentException(
                                string.Format(
                                    CultureInfo.CurrentCulture,
                                    Resources.FunctionCouldNotBeFound,
                                    methodName),
                                nameof(methodName));
#endif

            return tolerance.IsEmpty
                ? Expression.Call(
                    this.Parameter.GenerateExpression(),
                    mi)
                : Expression.Call(
                    this.Parameter.GenerateExpression(in tolerance),
                    mi);
        }
    }
}