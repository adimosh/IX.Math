// <copyright file="ComputedExpression.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using IX.Math.Exceptions;
using IX.Math.Extensibility;
using IX.Math.Formatters;
using IX.Math.Nodes;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;
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
    public sealed partial class ComputedExpression : DisposableBase, IDeepCloneable<ComputedExpression>
    {
#if NET452
        private static readonly ReadOnlyCollection<Type> EmptyTypeCollection = new ReadOnlyCollection<Type>(new Type[0]);
#else
        private static readonly ReadOnlyCollection<Type> EmptyTypeCollection = new ReadOnlyCollection<Type>(Array.Empty<Type>());
#endif

        private readonly IDictionary<string, ExternalParameterNode> parametersRegistry;
        private readonly List<IStringFormatter> stringFormatters;

        private readonly string initialExpression;
        private NodeBase body;

        internal ComputedExpression(
            string initialExpression,
            NodeBase body,
            bool isRecognized,
            IDictionary<string, ExternalParameterNode> parameterRegistry,
            List<IStringFormatter> stringFormatters)
        {
            this.parametersRegistry = parameterRegistry;
            this.stringFormatters = stringFormatters;

            this.initialExpression = initialExpression;
            this.body = body;
            this.RecognizedCorrectly = isRecognized;
            this.IsConstant = body?.IsConstant ?? false;
            this.IsTolerant = body?.IsTolerant ?? false;
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
        /// Gets a value indicating whether this computed expression can have tolerance.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this expression is tolerant; otherwise, <c>false</c>.
        /// </value>
        public bool IsTolerant { get; }

        /// <summary>
        /// Gets a value indicating whether this computed expression is compiled.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this expression is compiled; otherwise, <c>false</c>.
        /// </value>
        public bool IsCompiled { get; private set; }

        /// <summary>
        /// Gets a value indicating whether or not the expression has undefined parameters.
        /// </summary>
        /// <value><see langword="true"/> if the expression has undefined parameters, <see langword="false"/> otherwise.</value>
        public bool HasUndefinedParameters =>
            this.parametersRegistry
                .Any(p => p.Value.ParameterType == SupportedValueType.Unknown);

        /// <summary>
        /// Gets the names of the parameters this expression requires, if any.
        /// </summary>
        /// <returns>An array of required parameter names.</returns>
        public string[] GetParameterNames() =>
            this.parametersRegistry
                .Select(p => p.Key)
                .ToArray();

        internal (bool, bool, Delegate, object) CompileDelegate(in ComparisonTolerance tolerance) =>
            this.CompileDelegate(
                in tolerance,
                EmptyTypeCollection);

        internal (bool, bool, Delegate, object) CompileDelegate(in ComparisonTolerance tolerance, ReadOnlyCollection<Type> parameterTypes)
        {
            this.RequiresNotDisposed();

            if (!this.RecognizedCorrectly)
            {
                // Expression was not recognized correctly.
                return (false, default, default, default);
            }

            var parameterContexts = this.parametersRegistry.Values.ToArray();

            if (parameterTypes.Count != parameterContexts.Length)
            {
                // Invalid number of parameters
                return (false, default, default, default);
            }

            if (this.IsConstant)
            {
                return (true, true, default, ((ConstantNodeBase)this.body).DistillValue());
            }

            Expression localBody = tolerance.IsEmpty ? this.body.GenerateExpression() : this.body.GenerateExpression(in tolerance);

            Delegate del;
            try
            {
                del = Expression.Lambda(
                        localBody,
                        this.parametersRegistry.Dump()
                            .Select(p => p.ParameterExpression))
                    .Compile();
            }
            catch (MathematicsEngineException)
            {
                throw;
            }
            catch
            {
                // Delegate could not be compiled with the given arguments.
                return (false, default, default, default);
            }

            return (true, false, del, default);
        }

        /// <summary>
        /// Creates a deep clone of the source object.
        /// </summary>
        /// <returns>A deep clone.</returns>
        public ComputedExpression DeepClone()
        {
            var registry = new StandardParameterRegistry(this.stringFormatters);
            var context = new NodeCloningContext { ParameterRegistry = registry, SpecialRequestFunction = this.specialObjectRequestFunc };

            return new ComputedExpression(this.initialExpression, this.body.DeepClone(context), this.RecognizedCorrectly, registry, this.stringFormatters, this.specialObjectRequestFunc);
        }

        /// <summary>
        /// Disposes in the general (managed and unmanaged) context.
        /// </summary>
        protected override void DisposeGeneralContext()
        {
            base.DisposeGeneralContext();

            Interlocked.Exchange(ref this.body, null);
        }

        private object[] FormatArgumentsAccordingToParameters(
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
                paramValue ??= parameterValues[i];

                // Type filtration
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
                                    StringFormatter.FormatIntoString(convertedParam, this.stringFormatters));

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
                                    StringFormatter.FormatIntoString(convertedParam, this.stringFormatters));
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
                                paramValue = CreateValue(
                                    paraContext,
                                    StringFormatter.FormatIntoString(convertedParam, this.stringFormatters));
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
                                paramValue = CreateValue(
                                    paraContext,
                                    StringFormatter.FormatIntoString(convertedParam, this.stringFormatters));
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
                                paramValue = CreateValueFromFunc(paraContext, () => StringFormatter.FormatIntoString(convertedParam(), this.stringFormatters));
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
                                paramValue = CreateValueFromFunc(paraContext, () => StringFormatter.FormatIntoString(convertedParam(), this.stringFormatters));

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
                                paramValue = CreateValueFromFunc(paraContext, () => StringFormatter.FormatIntoString(convertedParam(), this.stringFormatters));
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
                                            ? Convert.ToDouble(numericResult, CultureInfo.CurrentCulture)
                                            : throw new ArgumentInvalidTypeException(paraContext.Name));
                                        break;
                                    case false:
                                        paramValue = CreateValueFromFunc(paraContext, () => ParsingFormatter.ParseNumeric(
                                            convertedParam(),
                                            out var numericResult)
                                            ? Convert.ToInt64(numericResult, CultureInfo.CurrentCulture)
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
                                paramValue = CreateValueFromFunc(paraContext, () => StringFormatter.FormatIntoString(convertedParam(), this.stringFormatters));
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
    }
}