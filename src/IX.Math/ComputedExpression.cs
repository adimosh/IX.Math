// <copyright file="ComputedExpression.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using IX.Math.Extensibility;
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
    public sealed partial class ComputedExpression : DisposableBase, IDeepCloneable<ComputedExpression>
    {
        private readonly IParameterRegistry parametersRegistry;
        private readonly List<IStringFormatter> stringFormatters;
        private readonly Func<Type, object> specialObjectRequestFunc;

        private readonly string initialExpression;
        private NodeBase body;

        internal ComputedExpression(
            string initialExpression,
            NodeBase body,
            bool isRecognized,
            IParameterRegistry parameterRegistry,
            List<IStringFormatter> stringFormatters,
            Func<Type, object> specialObjectRequestFunc)
        {
            this.parametersRegistry = parameterRegistry;
            this.stringFormatters = stringFormatters;
            this.specialObjectRequestFunc = specialObjectRequestFunc;

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
            this.parametersRegistry.Dump()
                .Any(p => p.ReturnType == SupportedValueType.Unknown);

        /// <summary>
        /// Gets the names of the parameters this expression requires, if any.
        /// </summary>
        /// <returns>An array of required parameter names.</returns>
        public string[] GetParameterNames() =>
            this.parametersRegistry.Dump()
                .Select(p => p.Name)
                .ToArray();

        internal (bool, bool, Delegate, object) CompileDelegate(in ComparisonTolerance tolerance, ReadOnlyCollection<Type> parameterTypes)
        {
            this.RequiresNotDisposed();

            if (!this.RecognizedCorrectly)
            {
                // Expression was not recognized correctly.
                return (false, default, default, default);
            }

            var parameterContexts = this.parametersRegistry.Dump();

            if (parameterTypes.Count != parameterContexts.Length)
            {
                // Invalid number of parameters
                return (false, default, default, default);
            }

            if (this.IsConstant)
            {
                return (true, true, default, ((ConstantNodeBase)this.body).DistillValue());
            }

            var body = tolerance.IsEmpty ? this.body.GenerateExpression() : this.body.GenerateExpression(in tolerance);

            Delegate del;
            try
            {
                del = Expression.Lambda(
                    body,
                    this.parametersRegistry.Dump().Select(p => p.ParameterExpression)).Compile();
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
    }
}