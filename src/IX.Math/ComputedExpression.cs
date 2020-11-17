// <copyright file="ComputedExpression.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using IX.Math.Exceptions;
using IX.Math.Extensibility;
using IX.Math.Nodes;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;
using IX.StandardExtensions;
using IX.StandardExtensions.ComponentModel;
using IX.StandardExtensions.Contracts;
using IX.StandardExtensions.Efficiency;
using JetBrains.Annotations;

namespace IX.Math
{
    /// <summary>
    /// A representation of a computed expression, resulting from a string expression.
    /// </summary>
    [PublicAPI]
    public sealed class ComputedExpression : DisposableBase, IDeepCloneable<ComputedExpression>
    {
        private static readonly ReadOnlyCollection<Type> EmptyTypeCollection = new ReadOnlyCollection<Type>(Array.Empty<Type>());

        private readonly ConcurrentDictionary<string, ExternalParameterNode> parametersRegistry;
        private readonly List<IStringFormatter> stringFormatters;

        private readonly string initialExpression;
        private NodeBase? body;

        internal ComputedExpression(
            string initialExpression,
            NodeBase? body,
            bool isRecognized,
            ConcurrentDictionary<string, ExternalParameterNode> parameterRegistry,
            List<IStringFormatter> stringFormatters)
        {
            this.parametersRegistry = parameterRegistry;
            this.stringFormatters = stringFormatters;

            this.initialExpression = initialExpression;
            this.body = body;
            this.RecognizedCorrectly = isRecognized;
            this.IsConstant = body?.IsConstant ?? false;
            this.IsTolerant = body?.IsTolerant ?? false;
            this.IsImmutable = (body?.IsConstant ?? false) ||
                               parameterRegistry.Count == 0 ||
                               parameterRegistry.Values.All(p => p.ParameterType != SupportedValueType.Unknown);
        }

        /// <summary>
        /// Gets a value indicating whether or not the expression was actually recognized. <see langword="true"/> can possibly return an actual expression or a static value.
        /// </summary>
        /// <value><see langword="true"/> if the expression is recognized correctly, <see langword="false"/> otherwise.</value>
        public bool RecognizedCorrectly { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is immutable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is immutable; otherwise, <c>false</c>.
        /// </value>
        public bool IsImmutable
        {
            get;
        }

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
                .OrderBy(p => p.Value.Order)
                .Select(p => p.Value.Name)
                .ToArray();

        /// <summary>
        /// Creates a deep clone of the source object.
        /// </summary>
        /// <returns>A deep clone.</returns>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance",
            "HAA0603:Delegate allocation from a method group",
            Justification = "This is how ForEach works.")]
        public ComputedExpression DeepClone()
        {
            var registry = new ConcurrentDictionary<string, ExternalParameterNode>();
            foreach (var p in this.parametersRegistry)
            {
                registry.TryAdd(
                    p.Key,
                    new ExternalParameterNode(p.Value.Name)
                    {
                        Order = p.Value.Order
                    });
            }

            var context = new NodeCloningContext(registry);

            return new ComputedExpression(
                this.initialExpression,
                this.body?.DeepClone(context),
                this.RecognizedCorrectly,
                registry,
                this.stringFormatters);
        }

        internal (bool Success, bool IsObject, Delegate? Function, object? ConstantValue) CompileDelegate(
            in ComparisonTolerance tolerance) =>
            this.CompileDelegate(
                in tolerance,
                EmptyTypeCollection);

        internal (bool Success, bool IsObject, Delegate? Function, object? ConstantValue) CompileDelegate(
            in ComparisonTolerance tolerance,
            ReadOnlyCollection<Type> parameterTypes)
        {
            this.RequiresNotDisposed();

            if (!this.RecognizedCorrectly)
            {
                // Expression was not recognized correctly.
                return (false, default, default, default);
            }

            if (this.body == null)
            {
                // Delegate body could not be generated.
                return (false, default, default, default);
            }

            var parameterContexts = this.parametersRegistry.Values.OrderBy(p => p.Order)
                .ToArray();

            if (parameterTypes.Count != parameterContexts.Length)
            {
                // Invalid number of parameters
                return (false, default, default, default);
            }

            if (this.IsConstant)
            {
                return (true, true, default, ((ConstantNodeBase)this.body).ValueAsObject);
            }

            try
            {
                for (int i = 0; i < parameterContexts.Length; i++)
                {
                    parameterContexts[i].DetermineParameterType(parameterTypes[i]);
                }

                this.body.Verify();

                Expression localBody = this.body.GenerateExpression(this.body.CalculateLeastCostlyStrategy(), in tolerance);

                Delegate del;
                try
                {
                    del = Expression.Lambda(
                            localBody,
                            this.parametersRegistry.Values.OrderBy(p => p.Order)
                                .Select(p => p.ParameterDefinitionExpression))
                        .Compile();
                }
                catch (MathematicsEngineException)
                {
                    throw;
                }
                catch (ExpressionNotValidLogicallyException)
                {
                    throw;
                }
                catch (Exception e)
                {
                    throw new ExpressionNotValidLogicallyException(e);
                }

                return (true, false, del, default);
            }
            catch (ExpressionNotValidLogicallyException)
            {
                // Delegate could not be compiled with the given arguments.
                return (false, default, default, default);
            }
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