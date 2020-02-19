// <copyright file="ComputedExpression.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using IX.Math.Formatters;
using IX.Math.Nodes;
using IX.Math.Registration;
using IX.StandardExtensions;
using IX.StandardExtensions.ComponentModel;
using IX.StandardExtensions.Contracts;
using JetBrains.Annotations;

namespace IX.Math
{
    /// <summary>
    /// A representation of a computed expression, resulting from a string expression.
    /// </summary>
    [PublicAPI]
    public sealed class ComputedExpression : DisposableBase, IDeepCloneable<ComputedExpression>
    {
        private readonly IParameterRegistry parametersRegistry;

        private readonly string defaultStringFormat;
        private readonly string initialExpression;
        private NodeBase body;

        internal ComputedExpression(string initialExpression, NodeBase body, bool isRecognized, IParameterRegistry parameterRegistry, string defaultStringFormat)
        {
            this.parametersRegistry = parameterRegistry;

            this.initialExpression = initialExpression;
            this.body = body;
            this.RecognizedCorrectly = isRecognized;
            this.IsConstant = body?.IsConstant ?? false;
            this.defaultStringFormat = string.IsNullOrWhiteSpace(defaultStringFormat) ? null : defaultStringFormat;
        }

        /// <summary>
        /// Gets a value indicating whether or not the expression was actually recognized. <see langword="true"/> can possibly return an actual expression or a static value.
        /// </summary>
        /// <value><see langword="true"/> if the expression is recognized correctly, <see langword="false"/> otherwise.</value>
        public bool RecognizedCorrectly { get; }

        /// <summary>
        /// Gets a value indicating whether or not the expression is constant.
        /// </summary>
        /// <value><see langword="true"/> if the expression is constant, <see langword="false"/> otherwise.</value>
        public bool IsConstant { get; }

        /// <summary>
        /// Gets a value indicating whether or not the expression has undefined parameters.
        /// </summary>
        /// <value><see langword="true"/> if the expression has undefined parameters, <see langword="false"/> otherwise.</value>
        public bool HasUndefinedParameters => this.parametersRegistry.Dump().Any(p => p.ReturnType == SupportedValueType.Unknown);

        /// <summary>
        /// Gets the names of the parameters this expression requires, if any. This property is obsolete and should not be used anymore.
        /// </summary>
        /// <remarks>
        /// <para>This property is obsolete and will be removed in a future version of the library.</para>
        /// <para>The method <see cref="GetParameterNames"/> instead.</para>
        /// </remarks>
        [Obsolete("Please use the GetParameterNames method instead.")]
        public string[] ParameterNames => this.GetParameterNames();

        /// <summary>
        /// Gets the names of the parameters this expression requires, if any.
        /// </summary>
        /// <returns>An array of required parameter names.</returns>
        public string[] GetParameterNames() => this.parametersRegistry.Dump().Select(p => p.Name).ToArray();

        /// <summary>
        /// Computes the expression and returns a result.
        /// </summary>
        /// <param name="arguments">The arguments with which to invoke the execution of the expression.</param>
        /// <returns>The computed result, or, if the expression is not recognized correctly, the expression as a <see cref="string"/>.</returns>
        public object Compute(params object[] arguments) =>
            this.Compute(
                null,
                arguments);

        /// <summary>
        /// Computes the expression and returns a result, with tolerance.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <param name="arguments">The arguments with which to invoke the execution of the expression.</param>
        /// <returns>
        /// The computed result, or, if the expression is not recognized correctly, the expression as a <see cref="string" />.
        /// </returns>
        [SuppressMessage("CodeSmell", "ERP022:Unobserved exception in generic exception handler", Justification = "We want this in this case.")]
        [SuppressMessage("Performance", "HAA0302:Display class allocation to capture closure", Justification = "Unavoidable for now.")]
        [SuppressMessage("Performance", "HAA0301:Closure Allocation Source", Justification = "Unavoidable for now.")]
        public object Compute(Tolerance tolerance, params object[] arguments)
        {
            this.ThrowIfCurrentObjectDisposed();

            if (!this.RecognizedCorrectly)
            {
                // Expression was not recognized correctly.
                return this.initialExpression;
            }

            var convertedArguments = FormatArgumentsAccordingToParameters(arguments, this.parametersRegistry.Dump());

            object[] FormatArgumentsAccordingToParameters(
                object[] parameterValues,
                ParameterContext[] parameters)
            {
                if (parameterValues.Length != parameters.Length)
                {
                    return null;
                }

                var finalValues = new object[parameterValues.Length];

                var i = 0;

                object paramValue = null;

                while (i < finalValues.Length)
                {
                    ParameterContext paraContext = parameters[i];

                    // If there was no continuation, initialize parameter with value.
                    if (paramValue == null)
                    {
                        paramValue = parameterValues[i];
                    }

                    // Initial filtration
                    switch (paramValue)
                    {
                        case byte convertedParam:
                            paramValue = Convert.ToInt64(convertedParam);
                            continue;

                        case sbyte convertedParam:
                            paramValue = Convert.ToInt64(convertedParam);
                            continue;

                        case short convertedParam:
                            paramValue = Convert.ToInt64(convertedParam);
                            continue;

                        case ushort convertedParam:
                            paramValue = Convert.ToInt64(convertedParam);
                            continue;

                        case int convertedParam:
                            paramValue = Convert.ToInt64(convertedParam);
                            continue;

                        case uint convertedParam:
                            paramValue = Convert.ToInt64(convertedParam);
                            continue;

                        case long convertedParam:
                            paramValue = convertedParam;
                            break;

                        case ulong convertedParam:
                            paramValue = Convert.ToDouble(convertedParam);
                            continue;

                        case float convertedParam:
                            paramValue = Convert.ToDouble(convertedParam);
                            continue;

                        case double convertedParam:
                            paramValue = convertedParam;
                            break;

                        case string convertedParam:
                            paramValue = convertedParam;
                            break;

                        case bool convertedParam:
                            paramValue = convertedParam;
                            break;

                        case byte[] convertedParam:
                            paramValue = convertedParam;
                            break;

                        case Func<byte> convertedParam:
                            paramValue = new Func<long>(() => Convert.ToInt64(convertedParam()));
                            continue;

                        case Func<sbyte> convertedParam:
                            paramValue = new Func<long>(() => Convert.ToInt64(convertedParam()));
                            continue;

                        case Func<short> convertedParam:
                            paramValue = new Func<long>(() => Convert.ToInt64(convertedParam()));
                            continue;

                        case Func<ushort> convertedParam:
                            paramValue = new Func<long>(() => Convert.ToInt64(convertedParam()));
                            continue;

                        case Func<int> convertedParam:
                            paramValue = new Func<long>(() => Convert.ToInt64(convertedParam()));
                            continue;

                        case Func<uint> convertedParam:
                            paramValue = new Func<long>(() => Convert.ToInt64(convertedParam()));
                            continue;

                        case Func<long> convertedParam:
                            paramValue = convertedParam;
                            break;

                        case Func<ulong> convertedParam:
                            paramValue = new Func<double>(() => Convert.ToDouble(convertedParam()));
                            continue;

                        case Func<float> convertedParam:
                            paramValue = new Func<double>(() => Convert.ToDouble(convertedParam()));
                            continue;

                        case Func<double> convertedParam:
                            paramValue = convertedParam;
                            break;

                        case Func<string> convertedParam:
                            paramValue = convertedParam;
                            break;

                        case Func<bool> convertedParam:
                            paramValue = convertedParam;
                            break;

                        case Func<byte[]> convertedParam:
                            paramValue = convertedParam;
                            break;

                        default:
                            // Argument type is not (yet) supported
                            return null;
                    }

                    // Secondary filtration
                    switch (paramValue)
                    {
                        case long convertedParam:
                            switch (paraContext.ReturnType)
                            {
                                case SupportedValueType.Boolean:
                                    paramValue = CreateValue(paraContext, convertedParam != 0);
                                    break;

                                case SupportedValueType.ByteArray:
                                    paramValue = CreateValue(paraContext, BitConverter.GetBytes(convertedParam));
                                    break;

                                case SupportedValueType.Numeric:
                                    switch (paraContext.IsFloat)
                                    {
                                        case true:
                                            paramValue = CreateValue(paraContext, Convert.ToDouble(convertedParam));
                                            break;
                                        case false:
                                            paramValue = CreateValue(paraContext, convertedParam);
                                            break;
                                        default:
                                            paraContext.DetermineInteger();
                                            continue;
                                    }

                                    break;

                                case SupportedValueType.String:
                                    paramValue = CreateValue(
                                        paraContext,
                                        this.defaultStringFormat == null ? convertedParam.ToString() : convertedParam.ToString(this.defaultStringFormat));

                                    break;

                                case SupportedValueType.Unknown:
                                    paraContext.DetermineType(SupportedValueType.Numeric);
                                    continue;
                            }

                            break;

                        case double convertedParam:
                            switch (paraContext.ReturnType)
                            {
                                case SupportedValueType.Boolean:
                                    // ReSharper disable once CompareOfFloatsByEqualityOperator - no better idea for now
                                    paramValue = CreateValue(paraContext, convertedParam != 0D);
                                    break;

                                case SupportedValueType.ByteArray:
                                    paramValue = CreateValue(paraContext, BitConverter.GetBytes(convertedParam));
                                    break;

                                case SupportedValueType.Numeric:
                                    switch (paraContext.IsFloat)
                                    {
                                        case true:
                                            paramValue = CreateValue(paraContext, convertedParam);
                                            break;
                                        case false:
                                            paramValue = CreateValue(paraContext, Convert.ToInt64(convertedParam));
                                            break;
                                        default:
                                            paraContext.DetermineFloat();
                                            continue;
                                    }

                                    break;

                                case SupportedValueType.String:
                                    paramValue = CreateValue(
                                        paraContext,
                                        this.defaultStringFormat == null ?
                                            convertedParam.ToString(CultureInfo.CurrentCulture) :
                                            convertedParam.ToString(this.defaultStringFormat));
                                    break;

                                case SupportedValueType.Unknown:
                                    paraContext.DetermineType(SupportedValueType.Numeric);
                                    continue;
                            }

                            break;

                        case bool convertedParam:
                            switch (paraContext.ReturnType)
                            {
                                case SupportedValueType.Boolean:
                                    paramValue = CreateValue(paraContext, convertedParam);
                                    break;

                                case SupportedValueType.ByteArray:
                                    paramValue = CreateValue(paraContext, BitConverter.GetBytes(convertedParam));
                                    break;

                                case SupportedValueType.Numeric:
                                    return null;

                                case SupportedValueType.String:
                                    paramValue = CreateValue(paraContext, convertedParam.ToString());
                                    break;

                                case SupportedValueType.Unknown:
                                    paraContext.DetermineType(SupportedValueType.Boolean);
                                    continue;
                            }

                            break;

                        case string convertedParam:
                            switch (paraContext.ReturnType)
                            {
                                case SupportedValueType.Boolean:
                                    paramValue = CreateValue(paraContext, bool.Parse(convertedParam));
                                    break;

                                case SupportedValueType.ByteArray:
                                    {
                                        if (ParsingFormatter.ParseByteArray(convertedParam, out var byteArrayResult))
                                        {
                                            paramValue = CreateValue(paraContext, byteArrayResult);
                                        }
                                        else
                                        {
                                            // Cannot parse byte array.
                                            return null;
                                        }
                                    }

                                    break;

                                case SupportedValueType.Numeric:
                                    {
                                        if (ParsingFormatter.ParseNumeric(convertedParam, out var numericResult))
                                        {
                                            switch (numericResult)
                                            {
                                                case long integerResult:
                                                    paramValue = CreateValue(paraContext, integerResult);
                                                    break;
                                                case double floatResult:
                                                    paramValue = CreateValue(paraContext, floatResult);
                                                    break;
                                                default:
                                                    // Numeric type unknown.
                                                    return null;
                                            }
                                        }
                                        else
                                        {
                                            // Cannot parse numeric type.
                                            return null;
                                        }
                                    }

                                    break;

                                case SupportedValueType.String:
                                    paramValue = CreateValue(paraContext, convertedParam);
                                    break;

                                case SupportedValueType.Unknown:
                                    paraContext.DetermineType(SupportedValueType.String);
                                    continue;
                            }

                            break;

                        case byte[] convertedParam:
                            switch (paraContext.ReturnType)
                            {
                                case SupportedValueType.Boolean:
                                    paramValue = CreateValue(paraContext, BitConverter.ToBoolean(convertedParam, 0));
                                    break;

                                case SupportedValueType.ByteArray:
                                    paramValue = CreateValue(paraContext, convertedParam);
                                    break;

                                case SupportedValueType.Numeric:
                                    switch (paraContext.IsFloat)
                                    {
                                        case true:
                                            paramValue = CreateValue(paraContext, BitConverter.ToDouble(convertedParam, 0));
                                            break;
                                        case false:
                                            paramValue = CreateValue(paraContext, BitConverter.ToInt64(convertedParam, 0));
                                            break;
                                        default:
                                            paraContext.DetermineFloat();
                                            continue;
                                    }

                                    break;

                                case SupportedValueType.String:
                                    paramValue = CreateValue(paraContext, BitConverter.ToString(convertedParam));
                                    break;

                                case SupportedValueType.Unknown:
                                    paraContext.DetermineType(SupportedValueType.ByteArray);
                                    continue;
                            }

                            break;

                        case Func<long> convertedParam:
                            paraContext.DetermineFunc();

                            switch (paraContext.ReturnType)
                            {
                                case SupportedValueType.Boolean:
                                    paramValue = CreateValueFromFunc(paraContext, () => convertedParam() == 0);
                                    break;

                                case SupportedValueType.ByteArray:
                                    paramValue = CreateValueFromFunc(paraContext, () => BitConverter.GetBytes(convertedParam()));
                                    break;

                                case SupportedValueType.Numeric:
                                    switch (paraContext.IsFloat)
                                    {
                                        case true:
                                            paramValue = CreateValueFromFunc(paraContext, () => Convert.ToDouble(convertedParam()));
                                            break;
                                        case false:
                                            paramValue = CreateValueFromFunc(paraContext, convertedParam);
                                            break;
                                        default:
                                            paraContext.DetermineInteger();
                                            continue;
                                    }

                                    break;

                                case SupportedValueType.String:
                                    paramValue = this.defaultStringFormat == null ? CreateValueFromFunc(paraContext, () => convertedParam().ToString()) : CreateValueFromFunc(paraContext, () => convertedParam().ToString(this.defaultStringFormat));
                                    break;

                                case SupportedValueType.Unknown:
                                    paraContext.DetermineType(SupportedValueType.Numeric);
                                    continue;
                            }

                            break;

                        case Func<double> convertedParam:
                            paraContext.DetermineFunc();

                            switch (paraContext.ReturnType)
                            {
                                case SupportedValueType.Boolean:
                                    // ReSharper disable once CompareOfFloatsByEqualityOperator - no better idea for now
                                    paramValue = CreateValueFromFunc(paraContext, () => convertedParam() == 0D);
                                    break;

                                case SupportedValueType.ByteArray:
                                    paramValue = CreateValueFromFunc(paraContext, () => BitConverter.GetBytes(convertedParam()));
                                    break;

                                case SupportedValueType.Numeric:
                                    switch (paraContext.IsFloat)
                                    {
                                        case true:
                                            paramValue = CreateValueFromFunc(paraContext, convertedParam);
                                            break;
                                        case false:
                                            paramValue = CreateValueFromFunc(paraContext, () => Convert.ToInt64(convertedParam()));
                                            break;
                                        default:
                                            paraContext.DetermineFloat();
                                            continue;
                                    }

                                    break;

                                case SupportedValueType.String:
                                    paramValue = this.defaultStringFormat == null ?
                                        CreateValueFromFunc(paraContext, () => convertedParam().ToString(CultureInfo.CurrentCulture)) :
                                        CreateValueFromFunc(paraContext, () => convertedParam().ToString(this.defaultStringFormat));

                                    break;

                                case SupportedValueType.Unknown:
                                    paraContext.DetermineType(SupportedValueType.Numeric);
                                    continue;
                            }

                            break;

                        case Func<bool> convertedParam:
                            paraContext.DetermineFunc();

                            switch (paraContext.ReturnType)
                            {
                                case SupportedValueType.Boolean:
                                    paramValue = CreateValueFromFunc(paraContext, convertedParam);
                                    break;

                                case SupportedValueType.ByteArray:
                                    paramValue = CreateValueFromFunc(paraContext, () => BitConverter.GetBytes(convertedParam()));
                                    break;

                                case SupportedValueType.Numeric:
                                    return null;

                                case SupportedValueType.String:
                                    paramValue = CreateValueFromFunc(paraContext, () => convertedParam().ToString());
                                    break;

                                case SupportedValueType.Unknown:
                                    paraContext.DetermineType(SupportedValueType.Boolean);
                                    continue;
                            }

                            break;

                        case Func<string> convertedParam:
                            paraContext.DetermineFunc();

                            switch (paraContext.ReturnType)
                            {
                                case SupportedValueType.Boolean:
                                    paramValue = CreateValueFromFunc(paraContext, () => bool.Parse(convertedParam()));
                                    break;

                                case SupportedValueType.ByteArray:
                                    paramValue = CreateValueFromFunc(paraContext, () => ParsingFormatter.ParseByteArray(convertedParam(), out var baResult) ? baResult : null);
                                    break;

                                case SupportedValueType.Numeric:
                                    switch (paraContext.IsFloat)
                                    {
                                        case true:
                                            paramValue = CreateValueFromFunc(paraContext, () => ParsingFormatter.ParseNumeric(
                                                convertedParam(),
                                                out var numericResult)
                                                ? Convert.ToDouble(numericResult)
                                                : throw new ArgumentInvalidTypeException(paraContext.Name));
                                            break;
                                        case false:
                                            paramValue = CreateValueFromFunc(paraContext, () => ParsingFormatter.ParseNumeric(
                                                convertedParam(),
                                                out var numericResult)
                                                ? Convert.ToInt64(numericResult)
                                                : throw new ArgumentInvalidTypeException(paraContext.Name));
                                            break;
                                        default:
                                            paraContext.DetermineFloat();
                                            continue;
                                    }

                                    break;

                                case SupportedValueType.String:
                                    paramValue = CreateValueFromFunc(paraContext, convertedParam);
                                    break;

                                case SupportedValueType.Unknown:
                                    paraContext.DetermineType(SupportedValueType.String);
                                    continue;
                            }

                            break;

                        case Func<byte[]> convertedParam:
                            paraContext.DetermineFunc();

                            switch (paraContext.ReturnType)
                            {
                                case SupportedValueType.Boolean:
                                    paramValue = CreateValueFromFunc(paraContext, () => BitConverter.ToBoolean(convertedParam(), 0));
                                    break;

                                case SupportedValueType.ByteArray:
                                    paramValue = CreateValueFromFunc(paraContext, convertedParam);
                                    break;

                                case SupportedValueType.Numeric:
                                    switch (paraContext.IsFloat)
                                    {
                                        case true:
                                            paramValue = CreateValueFromFunc(paraContext, () => BitConverter.ToDouble(convertedParam(), 0));
                                            break;
                                        case false:
                                            paramValue = CreateValueFromFunc(paraContext, () => BitConverter.ToInt64(convertedParam(), 0));
                                            break;
                                        default:
                                            paraContext.DetermineFloat();
                                            continue;
                                    }

                                    break;

                                case SupportedValueType.String:
                                    paramValue = CreateValueFromFunc(paraContext, () => BitConverter.ToString(convertedParam()));
                                    break;

                                case SupportedValueType.Unknown:
                                    paraContext.DetermineType(SupportedValueType.ByteArray);
                                    continue;
                            }

                            break;

                        default:
                            // Argument type is not (yet) supported
                            return null;
                    }

#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation - This is unavoidable now
#pragma warning disable HAA0303 // Considering moving this out of the generic method - Not really possible
                    object CreateValue<T>(ParameterContext parameterContext, T value) => parameterContext.FuncParameter ? new Func<T>(() => value) : (object)value;

                    object CreateValueFromFunc<T>(
                        ParameterContext parameterContext,
                        Func<T> value) => parameterContext.FuncParameter ? (object)value : value();
#pragma warning restore HAA0303 // Considering moving this out of the generic method
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation

                    if (paramValue == null)
                    {
                        return null;
                    }

                    finalValues[i] = paramValue;

                    paramValue = null;

                    i++;
                }

                return finalValues;
            }

            if (convertedArguments == null)
            {
                // The arguments could not be correctly converted.
                return this.initialExpression;
            }

            Delegate del;
            try
            {
                del = Expression.Lambda(
                    tolerance == null ? this.body.GenerateExpression() : this.body.GenerateExpression(tolerance),
                    this.parametersRegistry.Dump().Select(p => p.ParameterExpression)).Compile();
            }
            catch
            {
                // Expression is somehow not valid
                del = null;
            }

            if (del == null)
            {
                // Delegate could not be compiled with the given arguments.
                return this.initialExpression;
            }

            try
            {
                return del.DynamicInvoke(convertedArguments);
            }
            catch (OutOfMemoryException)
            {
                throw;
            }
            catch (DivideByZeroException)
            {
                throw;
            }
            catch
            {
                // Dynamic invocation of generated expression failed.
                return this.initialExpression;
            }
        }

        /// <summary>
        /// Computes the expression and returns a result.
        /// </summary>
        /// <param name="dataFinder">The data finder for the arguments with which to invoke execution of the expression.</param>
        /// <returns>The computed result, or, if the expression is not recognized correctly, the expression as a <see cref="string"/>.</returns>
        public object Compute(IDataFinder dataFinder) =>
            this.Compute(
                null,
                dataFinder);

        /// <summary>
        /// Computes the expression and returns a result.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <param name="dataFinder">The data finder for the arguments with which to invoke execution of the expression.</param>
        /// <returns>
        /// The computed result, or, if the expression is not recognized correctly, the expression as a <see cref="string" />.
        /// </returns>
        public object Compute([CanBeNull] Tolerance tolerance, IDataFinder dataFinder)
        {
            this.ThrowIfCurrentObjectDisposed();

            if (!this.RecognizedCorrectly)
            {
                return this.initialExpression;
            }

            var pars = new List<object>();

            Contract.RequiresNotNull(
                in dataFinder,
                nameof(dataFinder));

            foreach (ParameterContext p in this.parametersRegistry.Dump())
            {
                if (!dataFinder.TryGetData(p.Name, out var data))
                {
                    data = null;
                }

                pars.Add(data);
            }

            return pars.Any(p => p == null) ? this.initialExpression : this.Compute(tolerance, pars.ToArray());
        }

        /// <summary>
        /// Creates a deep clone of the source object.
        /// </summary>
        /// <returns>A deep clone.</returns>
        public ComputedExpression DeepClone()
        {
            var registry = new StandardParameterRegistry();
            var context = new NodeCloningContext { ParameterRegistry = registry };

            return new ComputedExpression(this.initialExpression, this.body.DeepClone(context), this.RecognizedCorrectly, registry, this.defaultStringFormat);
        }

        /// <summary>
        /// Disposes in the general (managed and unmanaged) context.
        /// </summary>
        protected override void DisposeGeneralContext()
        {
            base.DisposeGeneralContext();

            Interlocked.Exchange(ref this.body, null);
        }
    }
}