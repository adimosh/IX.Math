// <copyright file="UnaryFunctionNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using IX.Math.Extensibility;
using IX.StandardExtensions.Contracts;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Functions.Unary
{
    /// <summary>
    ///     A base class for a unary function (a function with only one parameter).
    /// </summary>
    /// <seealso cref="FunctionNodeBase" />
    [PublicAPI]
    public abstract class UnaryFunctionNodeBase : FunctionNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UnaryFunctionNodeBase" /> class.
        /// </summary>
        /// <param name="stringFormatters">The string formatters.</param>
        /// <param name="parameter">The parameter.</param>
        /// <exception cref="ArgumentNullException">parameter is <c>null</c> (<c>Nothing</c> in ).</exception>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Usage",
            "CA2214:Do not call overridable methods in constructors",
            Justification = "This is OK and expected at this point.")]
        protected UnaryFunctionNodeBase(
            List<IStringFormatter> stringFormatters,
            [NotNull] NodeBase parameter)
        : base(stringFormatters)
        {
            NodeBase parameterTemp = Requires.NotNull(parameter, nameof(parameter));

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
        public sealed override bool IsTolerant => this.Parameter.IsTolerant;

        /// <summary>
        ///     Gets a value indicating whether this node requires preservation of the original expression.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the node requires original expression preservation, or <see langword="false" />
        ///     if it can be polynomially-reduced.
        /// </value>
        public sealed override bool RequiresPreservedExpression => this.Parameter.RequiresPreservedExpression;

        /// <summary>
        /// Verifies this node and all nodes above it for logical validity.
        /// </summary>
        /// <remarks>
        /// <para>This method is expected to be overridden, and is a good place to do type restriction verification.</para>
        /// </remarks>
        public sealed override void Verify()
        {
            this.CalculatedCosts.Clear();

            this.Parameter.Verify();

            this.EnsureCompatibleParameter(this.Parameter);
        }

        /// <summary>
        ///     Ensures that the parameter that is received is compatible with the function, optionally allowing the parameter
        ///     reference to change.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected abstract void EnsureCompatibleParameter([NotNull] NodeBase parameter);
    }
}