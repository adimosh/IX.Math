// <copyright file="ExpressionParsingServiceBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Reflection;
using System.Threading;
using IX.Math.Generators;
using IX.Math.Interpretation;
using IX.Math.Nodes;
using IX.Math.Nodes.Constants;
using IX.Math.Registration;
using IX.StandardExtensions.ComponentModel;
using IX.StandardExtensions.Contracts;
using IX.StandardExtensions.Threading;
using JetBrains.Annotations;
using DiagCA = System.Diagnostics.CodeAnalysis;

namespace IX.Math
{
    /// <summary>
    ///     A base class for an expression parsing service.
    /// </summary>
    /// <seealso cref="DisposableBase" />
    /// <seealso cref="IExpressionParsingService" />
    [PublicAPI]
    public abstract class ExpressionParsingServiceBase : ReaderWriterSynchronizedBase, IExpressionParsingService
    {
        private MathDefinition workingDefinition;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExpressionParsingServiceBase" /> class with a specified math
        ///     definition
        ///     object.
        /// </summary>
        /// <param name="definition">The math definition to use.</param>
        protected private ExpressionParsingServiceBase(MathDefinition definition)
        {
            Requires.NotNull(out this.workingDefinition, definition, nameof(definition));

            PluginCollection.Current.RegisterFunctionsAssembly(typeof(ExpressionParsingService).GetTypeInfo()
                .Assembly);
        }

        /// <summary>
        ///     Interprets the mathematical expression and returns a container that can be invoked for solving using specific
        ///     mathematical types.
        /// </summary>
        /// <param name="expression">The expression to interpret.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A <see cref="ComputedExpression" /> that represents the interpreted expression.</returns>
        public abstract ComputedExpression Interpret(
            string expression,
            CancellationToken cancellationToken = default);

        /// <summary>
        ///     Interprets the mathematical expression and returns a container that can be invoked for solving using specific
        ///     mathematical types.
        /// </summary>
        /// <param name="expression">The expression to interpret.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>
        ///     A <see cref="ComputedExpression" /> that represents a compilable form of the original expression, if the
        ///     expression itself makes sense.
        /// </returns>
        /// <exception cref="ArgumentNullException"><paramref name="expression" /> is either null, empty or whitespace-only.</exception>
        [DiagCA.SuppressMessage(
            "Performance",
            "HAA0401:Possible allocation of reference type enumerator",
            Justification = "Unavoidable here.")]
        [DiagCA.SuppressMessage(
            "Performance",
            "HAA0603:Delegate allocation from a method group",
            Justification = "We're OK with this.")]
        protected ComputedExpression InterpretInternal(
            string expression,
            CancellationToken cancellationToken = default)
        {
            Requires.NotNullOrWhiteSpace(
                expression,
                nameof(expression));

            this.ThrowIfCurrentObjectDisposed();

            if (PluginCollection.Current.CheckExpressionPassThroughConstant(expression))
            {
                return new ComputedExpression(
                    expression,
                    new StringNode(expression),
                    true,
                    EmptyParametersRegistry.Empty);
            }

            InterpretationContext.Start(
                this.workingDefinition,
                expression,
                cancellationToken);

            ComputedExpression result;
            try
            {
                (NodeBase? node, IReadOnlyParameterRegistry parameterRegistry) = ExpressionGenerator.CreateBody();

                result = node == null
                    ? new ComputedExpression(
                        expression,
                        null,
                        false,
                        EmptyParametersRegistry.Empty)
                    : new ComputedExpression(
                        expression,
                        node,
                        true,
                        parameterRegistry);

                Interlocked.MemoryBarrier();
            }
            finally
            {
                InterpretationContext.Stop();
            }

            return result;
        }

        /// <summary>
        /// Registers an array of assemblies to extract compatible functions from.
        /// </summary>
        /// <param name="assemblies">The assemblies to register.</param>
        public void RegisterFunctionsAssemblies(params Assembly[] assemblies) =>
            PluginCollection.Current.RegisterFunctionsAssemblies(assemblies);

        /// <summary>
        ///     Returns the prototypes of all registered functions.
        /// </summary>
        /// <returns>All function names, with all possible combinations of input and output data.</returns>
        public string[] GetRegisteredFunctions() => PluginCollection.Current.GetRegisteredFunctions();

        /// <summary>
        /// Registers an assembly to extract compatible functions from.
        /// </summary>
        /// <param name="assembly">The assembly to register.</param>
        public void RegisterFunctionsAssembly(Assembly assembly) => PluginCollection.Current.RegisterFunctionsAssembly(assembly);
    }
}