// <copyright file="ExpressionParsingService.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using IX.Math.Extensibility;
using IX.Math.Extraction;
using IX.Math.Generators;
using IX.Math.Nodes;
using IX.Math.Nodes.Constants;
using IX.Math.Registration;
using IX.StandardExtensions.ComponentModel;
using IX.StandardExtensions.Contracts;
using IX.StandardExtensions.Extensions;
using IX.System.Collections.Generic;
using JetBrains.Annotations;

namespace IX.Math
{
    /// <summary>
    ///     A service that is able to parse strings containing mathematical expressions and solve them.
    /// </summary>
    [PublicAPI]
    public sealed class ExpressionParsingService : DisposableBase, IExpressionParsingService
    {
        private List<Assembly> assembliesToRegister;
        private Dictionary<string, Type> binaryFunctions;
        private Dictionary<string, Type> nonaryFunctions;
        private Dictionary<string, Type> ternaryFunctions;
        private Dictionary<string, Type> unaryFunctions;
        private MathDefinition workingDefinition;
#pragma warning disable IDISP002 // Dispose member. - It is
#pragma warning disable IDISP006 // Implement IDisposable. - It is
        private LevelDictionary<Type, IConstantsExtractor> constantExtractors;
        private LevelDictionary<Type, IConstantPassThroughExtractor> constantPassThroughExtractors;
#pragma warning restore IDISP006 // Implement IDisposable.
#pragma warning restore IDISP002 // Dispose member.

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExpressionParsingService" /> class with a standard math definition
        ///     object.
        /// </summary>
        public ExpressionParsingService()
            : this(MathDefinition.Default)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ExpressionParsingService" /> class with a specified math definition
        ///     object.
        /// </summary>
        /// <param name="definition">The math definition to use.</param>
        public ExpressionParsingService(MathDefinition definition)
        {
            this.workingDefinition = definition;

            this.assembliesToRegister = new List<Assembly>
            {
                typeof(ExpressionParsingService).GetTypeInfo().Assembly
            };
        }

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
        public ComputedExpression Interpret(
            string expression,
            CancellationToken cancellationToken = default)
        {
            Contract.RequiresNotNullOrWhitespace(
                expression,
                nameof(expression));

            this.ThrowIfCurrentObjectDisposed();

            if (this.constantPassThroughExtractors == null)
            {
                this.InitializePassThroughExtractorsDictionary();
            }

#pragma warning disable HAA0401 // Possible allocation of reference type enumerator - Nothing to be done about it
            foreach (Type cpteKey in this.constantPassThroughExtractors.KeysByLevel.SelectMany(p => p.Value))
#pragma warning restore HAA0401 // Possible allocation of reference type enumerator
            {
                if (this.constantPassThroughExtractors[cpteKey].Evaluate(expression))
                {
                    return new ComputedExpression(
                        expression,
                        new StringNode(expression),
                        true,
                        new StandardParameterRegistry(),
                        this.workingDefinition.AutoConvertStringFormatSpecifier);
                }
            }

            if (this.nonaryFunctions == null || this.unaryFunctions == null || this.binaryFunctions == null ||
                this.ternaryFunctions == null)
            {
                this.InitializeFunctionsDictionary();
            }

            if (this.constantExtractors == null)
            {
                this.InitializeExtractorsDictionary();
            }

            ComputedExpression result;
            using (var workingSet = new WorkingExpressionSet(
                expression,
                this.workingDefinition.DeepClone(),
                this.nonaryFunctions,
                this.unaryFunctions,
                this.binaryFunctions,
                this.ternaryFunctions,
                this.constantExtractors,
                this.constantPassThroughExtractors,
                cancellationToken))
            {
                (NodeBase node, IParameterRegistry parameterRegistry) = ExpressionGenerator.CreateBody(workingSet);

                result = !workingSet.Success
                    ? new ComputedExpression(
                        expression,
                        null,
                        false,
                        null,
                        null)
                    : new ComputedExpression(
                        expression,
                        node,
                        true,
                        parameterRegistry,
                        this.workingDefinition.AutoConvertStringFormatSpecifier);

                Interlocked.MemoryBarrier();
            }

            return result;
        }

        /// <summary>
        ///     Returns the prototypes of all registered functions.
        /// </summary>
        /// <returns>All function names, with all possible combinations of input and output data.</returns>
        public string[] GetRegisteredFunctions()
        {
            this.ThrowIfCurrentObjectDisposed();

            if (this.nonaryFunctions == null || this.unaryFunctions == null || this.binaryFunctions == null ||
                this.ternaryFunctions == null)
            {
                this.InitializeFunctionsDictionary();
            }

            // Capacity is sum of all, times 3; the "3" number was chosen as a good-enough average of how many overloads are defined, on average
            var bldr = new List<string>((this.nonaryFunctions.Count + this.unaryFunctions.Count + this.binaryFunctions.Count + this.ternaryFunctions.Count) * 3);

            foreach (KeyValuePair<string, Type> function in this.nonaryFunctions)
            {
                bldr.Add($"{function.Key}()");
            }

            (from KeyValuePair<string, Type> function in this.unaryFunctions
                from ConstructorInfo constructor in function.Value.GetTypeInfo().DeclaredConstructors
                let parameters = constructor.GetParameters()
                where parameters.Length == 1
                let parameterName = parameters[0].Name
                where parameterName != null
                let functionName = function.Key
                select (functionName, parameterName)).ForEach(
                (
                    parameter,
                    bldrL1) => bldrL1.Add($"{parameter.functionName}({parameter.parameterName})"), bldr);

            (from KeyValuePair<string, Type> function in this.binaryFunctions
                from ConstructorInfo constructor in function.Value.GetTypeInfo().DeclaredConstructors
                let parameters = constructor.GetParameters()
                where parameters.Length == 2
                let parameterNameLeft = parameters[0].Name
                let parameterNameRight = parameters[1].Name
                where parameterNameLeft != null && parameterNameRight != null
                let functionName = function.Key
                select (functionName, parameterNameLeft, parameterNameRight)).ForEach(
                (
                    parameter,
                    bldrL1) => bldrL1.Add($"{parameter.functionName}({parameter.parameterNameLeft}, {parameter.parameterNameRight})"), bldr);

            (from KeyValuePair<string, Type> function in this.ternaryFunctions
                from ConstructorInfo constructor in function.Value.GetTypeInfo().DeclaredConstructors
                let parameters = constructor.GetParameters()
                where parameters.Length == 3
                let parameterNameLeft = parameters[0].Name
                let parameterNameMiddle = parameters[1].Name
                let parameterNameRight = parameters[2].Name
                where parameterNameLeft != null && parameterNameMiddle != null && parameterNameRight != null
                let functionName = function.Key
                select (functionName, parameterNameLeft, parameterNameMiddle, parameterNameRight)).ForEach(
                (
                    parameter,
                    bldrL1) => bldrL1.Add($"{parameter.functionName}({parameter.parameterNameLeft}, {parameter.parameterNameMiddle}, {parameter.parameterNameRight})"), bldr);

            return bldr.ToArray();
        }

        /// <summary>
        ///     Registers an assembly to extract compatible functions from.
        /// </summary>
        /// <param name="assembly">The assembly to register.</param>
        public void RegisterFunctionsAssembly(Assembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            this.ThrowIfCurrentObjectDisposed();

            if (this.assembliesToRegister.Contains(assembly))
            {
                return;
            }

            this.assembliesToRegister.Add(assembly);

            this.nonaryFunctions?.Clear();
            this.unaryFunctions?.Clear();
            this.binaryFunctions?.Clear();
            this.ternaryFunctions?.Clear();
        }

        /// <summary>
        ///     Disposes in the managed context.
        /// </summary>
        protected override void DisposeManagedContext()
        {
            this.nonaryFunctions?.Clear();
            this.unaryFunctions?.Clear();
            this.binaryFunctions?.Clear();
            this.ternaryFunctions?.Clear();
            this.assembliesToRegister?.Clear();

            base.DisposeManagedContext();
        }

        /// <summary>
        ///     Disposes in the general (managed and unmanaged) context.
        /// </summary>
        protected override void DisposeGeneralContext()
        {
            base.DisposeGeneralContext();

            Interlocked.Exchange(
                ref this.nonaryFunctions,
                null);
            Interlocked.Exchange(
                ref this.unaryFunctions,
                null);
            Interlocked.Exchange(
                ref this.binaryFunctions,
                null);
            Interlocked.Exchange(
                ref this.ternaryFunctions,
                null);
            Interlocked.Exchange(
                ref this.assembliesToRegister,
                null);

            Interlocked.Exchange(
                ref this.workingDefinition,
                null);
        }

        private void InitializeFunctionsDictionary()
        {
            Interlocked.Exchange(
                ref this.nonaryFunctions,
                FunctionsDictionaryGenerator.GenerateInternalNonaryFunctionsDictionary(this.assembliesToRegister))
            ?.Clear();

            Interlocked.Exchange(
                ref this.unaryFunctions,
                FunctionsDictionaryGenerator.GenerateInternalUnaryFunctionsDictionary(this.assembliesToRegister))
            ?.Clear();

            Interlocked.Exchange(
                ref this.binaryFunctions,
                FunctionsDictionaryGenerator.GenerateInternalBinaryFunctionsDictionary(this.assembliesToRegister))
            ?.Clear();

            Interlocked.Exchange(
                    ref this.ternaryFunctions,
                    FunctionsDictionaryGenerator.GenerateInternalTernaryFunctionsDictionary(this.assembliesToRegister))
                ?.Clear();
        }

        private void InitializeExtractorsDictionary()
        {
            Interlocked.Exchange(
                ref this.constantExtractors,
                new LevelDictionary<Type, IConstantsExtractor>
                {
                    { typeof(StringExtractor), new StringExtractor(), 1000 },
                    { typeof(ScientificFormatNumberExtractor), new ScientificFormatNumberExtractor(), 2000 }
                })?.Dispose();

            var incrementer = 2001;
            this.assembliesToRegister.GetTypesAssignableFrom<IConstantsExtractor>()
                .Where(
                    p => p.IsClass && !p.IsAbstract && !p.IsGenericTypeDefinition &&
                         p.HasPublicParameterlessConstructor()).Select(p => p.AsType()).Where(
                    (
                        p,
                        thisL1) => !thisL1.constantExtractors.ContainsKey(p), this).ForEach(
                    (
                        Type p,
                        ref int i,
                        ExpressionParsingService thisL1) =>
                    {
#pragma warning disable SA1118 // Parameter should not span multiple lines - It should, when it's this complex
                        thisL1.constantExtractors.Add(
                            p,
                            (IConstantsExtractor)p.Instantiate(),
                            p.GetAttributeDataByTypeWithoutVersionBinding<ConstantsExtractorAttribute, int>(
                                out var explicitLevel)
                                ? explicitLevel
                                : Interlocked.Increment(ref i));
#pragma warning restore SA1118 // Parameter should not span multiple lines
                    }, ref incrementer,
                    this);
        }

        private void InitializePassThroughExtractorsDictionary()
        {
            Interlocked.Exchange(
                ref this.constantPassThroughExtractors,
                new LevelDictionary<Type, IConstantPassThroughExtractor>())?.Dispose();

            var incrementer = 2001;
            this.assembliesToRegister.GetTypesAssignableFrom<IConstantPassThroughExtractor>()
                .Where(
                    p => p.IsClass && !p.IsAbstract && !p.IsGenericTypeDefinition &&
                         p.HasPublicParameterlessConstructor()).Select(p => p.AsType()).Where(
                    (
                        p,
                        thisL1) => !thisL1.constantPassThroughExtractors.ContainsKey(p), this).ForEach(
                    (
                        Type p,
                        ref int i,
                        ExpressionParsingService thisL1) =>
                    {
#pragma warning disable SA1118 // Parameter should not span multiple lines - It should, when it's this complex
                        thisL1.constantPassThroughExtractors.Add(
                            p,
                            (IConstantPassThroughExtractor)p.Instantiate(),
                            p.GetAttributeDataByTypeWithoutVersionBinding<ConstantsPassThroughExtractorAttribute, int>(
                                out var explicitLevel)
                                ? explicitLevel
                                : Interlocked.Increment(ref i));
#pragma warning restore SA1118 // Parameter should not span multiple lines
                    }, ref incrementer,
                    this);
        }
    }
}