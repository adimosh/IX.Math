// <copyright file="BinaryFunctionNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Exceptions;
using IX.Math.Extensibility;
using IX.StandardExtensions.Extensions;
using JetBrains.Annotations;

namespace IX.Math.Nodes
{
    /// <summary>
    /// A base class for a function that takes two parameters.
    /// </summary>
    /// <seealso cref="FunctionNodeBase" />
    [PublicAPI]
    public abstract partial class BinaryFunctionNodeBase : FunctionNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFunctionNodeBase"/> class.
        /// </summary>
        /// <param name="firstParameter">The first parameter.</param>
        /// <param name="secondParameter">The second parameter.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="firstParameter"/>
        /// or
        /// <paramref name="secondParameter"/>
        /// is <see langword="null"/> (<see langword="Nothing"/> in Visual Basic).
        /// </exception>
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "We specifically want this to happen.")]
        [SuppressMessage("Usage", "CA2214:Do not call overridable methods in constructors", Justification = "We specifically want this to happen.")]
        protected BinaryFunctionNodeBase(NodeBase firstParameter, NodeBase secondParameter)
        {
            NodeBase firstParameterTemp = firstParameter ?? throw new ArgumentNullException(nameof(firstParameter));
            NodeBase secondParameterTemp = secondParameter ?? throw new ArgumentNullException(nameof(secondParameter));

            this.EnsureCompatibleParameters(firstParameter, secondParameter);

            this.FirstParameter = firstParameterTemp.Simplify();
            this.SecondParameter = secondParameterTemp.Simplify();
        }

        /// <summary>
        /// Gets the first parameter.
        /// </summary>
        /// <value>The first parameter.</value>
        public NodeBase FirstParameter { get; }

        /// <summary>
        /// Gets the second parameter.
        /// </summary>
        /// <value>The second parameter.</value>
        public NodeBase SecondParameter { get; }

        /// <summary>
        ///     Gets a value indicating whether this node supports tolerance.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is tolerant; otherwise, <c>false</c>.
        /// </value>
        public override bool IsTolerant => this.FirstParameter.IsTolerant || this.SecondParameter.IsTolerant;

        /// <summary>
        /// Sets the special object request function for sub objects.
        /// </summary>
        /// <param name="func">The function.</param>
        protected override void SetSpecialObjectRequestFunctionForSubObjects(Func<Type, object> func)
        {
            if (this.FirstParameter is ISpecialRequestNode srnl)
            {
                srnl.SetRequestSpecialObjectFunction(func);
            }

            if (this.SecondParameter is ISpecialRequestNode srnr)
            {
                srnr.SetRequestSpecialObjectFunction(func);
            }
        }

        /// <summary>
        /// Ensures that the parameters that are received are compatible with the function, optionally allowing the parameter references to change.
        /// </summary>
        /// <param name="firstParameter">The first parameter.</param>
        /// <param name="secondParameter">The second parameter.</param>
        protected abstract void EnsureCompatibleParameters(NodeBase firstParameter, NodeBase secondParameter);

        /// <summary>
        /// Generates a static binary function call expression.
        /// </summary>
        /// <typeparam name="T">The type to call on.</typeparam>
        /// <param name="functionName">Name of the function.</param>
        /// <returns>An expression representing the call.</returns>
        protected Expression GenerateStaticBinaryFunctionCall<T>(string functionName) =>
            this.GenerateStaticBinaryFunctionCall(
                typeof(T),
                functionName,
                in ComparisonTolerance.Empty);

        /// <summary>
        /// Generates a static binary function call expression.
        /// </summary>
        /// <typeparam name="T">The type to call on.</typeparam>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="tolerance">The tolerance, should there be any. This argument can be <c>null</c> (<c>Nothing</c> in Visual Basic).</param>
        /// <returns>An expression representing the call.</returns>
        protected Expression GenerateStaticBinaryFunctionCall<T>(
            string functionName,
            in ComparisonTolerance tolerance) =>
            this.GenerateStaticBinaryFunctionCall(
                typeof(T),
                functionName,
                in tolerance);

        /// <summary>
        /// Generates a static binary function call expression.
        /// </summary>
        /// <param name="t">The type to call on.</param>
        /// <param name="functionName">Name of the function.</param>
        /// <returns>Expression.</returns>
        /// <exception cref="ArgumentException">The function name is invalid.</exception>
        protected Expression GenerateStaticBinaryFunctionCall(Type t, string functionName)
            => this.GenerateStaticBinaryFunctionCall(t, functionName, in ComparisonTolerance.Empty);

        /// <summary>
        /// Generates a static binary function call expression.
        /// </summary>
        /// <param name="t">The type to call on.</param>
        /// <param name="functionName">Name of the function.</param>
        /// <param name="tolerance">The tolerance, should there be any. This argument can be <c>null</c> (<c>Nothing</c> in Visual Basic).</param>
        /// <returns>Expression.</returns>
        /// <exception cref="ArgumentException">The function name is invalid.</exception>
        protected Expression GenerateStaticBinaryFunctionCall(Type t, string functionName, in ComparisonTolerance tolerance)
        {
            if (string.IsNullOrWhiteSpace(functionName))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.FunctionCouldNotBeFound, functionName), nameof(functionName));
            }

            Type firstParameterType = ParameterTypeFromParameter(this.FirstParameter);
            Type secondParameterType = ParameterTypeFromParameter(this.SecondParameter);

            MethodInfo mi = t.GetMethodWithExactParameters(functionName, firstParameterType, secondParameterType);

            if (mi == null)
            {
                firstParameterType = typeof(double);
                secondParameterType = typeof(double);

                mi = t.GetMethodWithExactParameters(functionName, firstParameterType, secondParameterType);

                if (mi == null)
                {
                    firstParameterType = typeof(long);
                    secondParameterType = typeof(long);

                    mi = t.GetMethodWithExactParameters(functionName, firstParameterType, secondParameterType);

                    if (mi == null)
                    {
                        firstParameterType = typeof(int);
                        secondParameterType = typeof(int);

                        mi = t.GetMethodWithExactParameters(functionName, firstParameterType, secondParameterType) ??
                            throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.FunctionCouldNotBeFound, functionName), nameof(functionName));
                    }
                }
            }

            Expression e1, e2;
            if (tolerance.IsEmpty)
            {
                e1 = this.FirstParameter.GenerateExpression();
                e2 = this.SecondParameter.GenerateExpression();
            }
            else
            {
                e1 = this.FirstParameter.GenerateExpression(in tolerance);
                e2 = this.SecondParameter.GenerateExpression(in tolerance);
            }

            if (e1.Type != firstParameterType)
            {
                e1 = Expression.Convert(e1, firstParameterType);
            }

            if (e2.Type != secondParameterType)
            {
                e2 = Expression.Convert(e2, secondParameterType);
            }

            return Expression.Call(mi, e1, e2);
        }

        /// <summary>
        /// Generates a static binary function call with explicit parameter types.
        /// </summary>
        /// <typeparam name="TParam1">The type of the first parameter.</typeparam>
        /// <typeparam name="TParam2">The type of the second parameter.</typeparam>
        /// <param name="t">The type to call on.</param>
        /// <param name="functionName">Name of the function.</param>
        /// <returns>
        /// The generated binary method call expression.
        /// </returns>
        /// <exception cref="ArgumentException">The function name is invalid.</exception>
        protected Expression GenerateStaticBinaryFunctionCall<TParam1, TParam2>(Type t, string functionName)
            => this.GenerateStaticBinaryFunctionCall<TParam1, TParam2>(t, functionName, in ComparisonTolerance.Empty);

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
        protected Expression GenerateStaticBinaryFunctionCall<TParam1, TParam2>(Type t, string functionName, in ComparisonTolerance tolerance)
        {
            if (string.IsNullOrWhiteSpace(functionName))
            {
                throw new ArgumentException(string.Format(CultureInfo.CurrentCulture, Resources.FunctionCouldNotBeFound, functionName), nameof(functionName));
            }

            Type firstParameterType = ParameterTypeFromParameter(this.FirstParameter);
            Type secondParameterType = ParameterTypeFromParameter(this.SecondParameter);

            MethodInfo mi = t.GetMethodWithExactParameters(
                                functionName,
                                typeof(TParam1),
                                typeof(TParam2)) ??
                            throw new MathematicsEngineException();

            Expression e1, e2;
            if (tolerance.IsEmpty)
            {
                e1 = this.FirstParameter.GenerateExpression();
                e2 = this.SecondParameter.GenerateExpression();
            }
            else
            {
                e1 = this.FirstParameter.GenerateExpression(in tolerance);
                e2 = this.SecondParameter.GenerateExpression(in tolerance);
            }

            if (e1.Type != firstParameterType)
            {
                e1 = Expression.Convert(e1, firstParameterType);
            }

            if (e2.Type != secondParameterType)
            {
                e2 = Expression.Convert(e2, secondParameterType);
            }

            if (e1.Type != typeof(TParam1))
            {
                e1 = Expression.Convert(e1, typeof(TParam1));
            }

            if (e2.Type != typeof(TParam2))
            {
                e2 = Expression.Convert(e2, typeof(TParam2));
            }

            return Expression.Call(mi, e1, e2);
        }

        /// <summary>
        /// Generates a static binary function call expression.
        /// </summary>
        /// <typeparam name="T">The type to call on.</typeparam>
        /// <param name="functionName">Name of the function.</param>
        /// <returns>An expression representing the call.</returns>
        protected Expression GenerateBinaryFunctionCallFirstParameterInstance<T>(string functionName) =>
            this.GenerateBinaryFunctionCallFirstParameterInstance(
                typeof(T),
                functionName,
                in ComparisonTolerance.Empty);

        /// <summary>
        /// Generates a static binary function call expression.
        /// </summary>
        /// <param name="t">The type to call on.</param>
        /// <param name="functionName">Name of the function.</param>
        /// <returns>Expression.</returns>
        /// <exception cref="ArgumentException">The function name is invalid.</exception>
        protected Expression GenerateBinaryFunctionCallFirstParameterInstance(
            Type t,
            string functionName) =>
            this.GenerateBinaryFunctionCallFirstParameterInstance(
                t,
                functionName,
                in ComparisonTolerance.Empty);

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
        protected Expression GenerateBinaryFunctionCallFirstParameterInstance(
            Type t,
            string functionName,
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

            Type firstParameterType = ParameterTypeFromParameter(this.FirstParameter);
            Type secondParameterType = ParameterTypeFromParameter(this.SecondParameter);

            MethodInfo mi = t.GetMethodWithExactParameters(
                functionName,
                firstParameterType,
                secondParameterType);

            if (mi == null)
            {
                if ((firstParameterType == typeof(long) && secondParameterType == typeof(double)) ||
                    (firstParameterType == typeof(double) && secondParameterType == typeof(long)))
                {
                    firstParameterType = typeof(double);
                    secondParameterType = typeof(double);

                    mi = t.GetMethodWithExactParameters(
                        functionName,
                        firstParameterType,
                        secondParameterType);

                    if (mi == null)
                    {
                        firstParameterType = typeof(long);
                        secondParameterType = typeof(long);

                        mi = t.GetMethodWithExactParameters(
                            functionName,
                            firstParameterType,
                            secondParameterType);

                        if (mi == null)
                        {
                            firstParameterType = typeof(int);
                            secondParameterType = typeof(int);

                            mi = t.GetMethodWithExactParameters(
                                     functionName,
                                     firstParameterType,
                                     secondParameterType) ??
                                 throw new ArgumentException(
                                     string.Format(
                                         CultureInfo.CurrentCulture,
                                         Resources.FunctionCouldNotBeFound,
                                         functionName),
                                     nameof(functionName));
                        }
                    }
                }
                else
                {
                    throw new ArgumentException(
                        string.Format(
                            CultureInfo.CurrentCulture,
                            Resources.FunctionCouldNotBeFound,
                            functionName),
                        nameof(functionName));
                }
            }

            Expression e1, e2;
            if (tolerance.IsEmpty)
            {
                e1 = this.FirstParameter.GenerateExpression();
                e2 = this.SecondParameter.GenerateExpression();
            }
            else
            {
                e1 = this.FirstParameter.GenerateExpression(in tolerance);
                e2 = this.SecondParameter.GenerateExpression(in tolerance);
            }

            if (e1.Type != firstParameterType)
            {
                e1 = Expression.Convert(
                    e1,
                    firstParameterType);
            }

            if (e2.Type != secondParameterType)
            {
                e2 = Expression.Convert(
                    e2,
                    secondParameterType);
            }

            return Expression.Call(
                e1,
                mi,
                e2);
        }
    }
}