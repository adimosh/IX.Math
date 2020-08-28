// <copyright file="BinaryFunctionNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using IX.StandardExtensions.Contracts;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Functions.Binary
{
    /// <summary>
    /// A base class for a function that takes two parameters.
    /// </summary>
    /// <seealso cref="FunctionNodeBase" />
    [PublicAPI]
    public abstract class BinaryFunctionNodeBase : FunctionNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BinaryFunctionNodeBase" /> class.
        /// </summary>
        /// <param name="firstParameter">The first parameter.</param>
        /// <param name="secondParameter">The second parameter.</param>
        /// <exception cref="ArgumentNullException"><paramref name="firstParameter" />
        /// or
        /// <paramref name="secondParameter" />
        /// is <see langword="null" /> (<see langword="Nothing" /> in Visual Basic).</exception>
        [SuppressMessage("ReSharper", "VirtualMemberCallInConstructor", Justification = "We specifically want this to happen.")]
        [SuppressMessage("Usage", "CA2214:Do not call overridable methods in constructors", Justification = "We specifically want this to happen.")]
        protected BinaryFunctionNodeBase(
            NodeBase firstParameter,
            NodeBase secondParameter)
        {
            NodeBase firstParameterTemp = Requires.NotNull(firstParameter, nameof(firstParameter)).Simplify();
            NodeBase secondParameterTemp = Requires.NotNull(secondParameter, nameof(secondParameter)).Simplify();

            this.EnsureCompatibleParameters(firstParameterTemp, secondParameterTemp);

            this.FirstParameter = firstParameterTemp;
            this.SecondParameter = secondParameterTemp;
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
        ///     Gets a value indicating whether this node requires preservation of the original expression.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the node requires original expression preservation, or <see langword="false" />
        ///     if it can be polynomially-reduced.
        /// </value>
        public override bool RequiresPreservedExpression =>
            this.FirstParameter.RequiresPreservedExpression || this.SecondParameter.RequiresPreservedExpression;

        /// <summary>
        /// Verifies this node and all nodes above it for logical validity.
        /// </summary>
        /// <remarks>
        /// <para>This method is expected to be overridden, and is a good place to do type restriction verification.</para>
        /// </remarks>
        public sealed override void Verify()
        {
            this.CalculatedCosts.Clear();

            this.FirstParameter.Verify();
            this.SecondParameter.Verify();

            this.EnsureCompatibleParameters(
                this.FirstParameter,
                this.SecondParameter);
        }

        /// <summary>
        /// Ensures that the parameters that are received are compatible with the function, optionally allowing the parameter references to change.
        /// </summary>
        /// <param name="firstParameter">The first parameter.</param>
        /// <param name="secondParameter">The second parameter.</param>
        protected abstract void EnsureCompatibleParameters(NodeBase firstParameter, NodeBase secondParameter);
    }
}