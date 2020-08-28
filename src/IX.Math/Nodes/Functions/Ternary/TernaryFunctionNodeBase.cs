// <copyright file="TernaryFunctionNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using IX.StandardExtensions.Contracts;

namespace IX.Math.Nodes.Functions.Ternary
{
    /// <summary>
    /// A base class for a function that takes three parameters.
    /// </summary>
    /// <seealso cref="FunctionNodeBase" />
    public abstract class TernaryFunctionNodeBase : FunctionNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TernaryFunctionNodeBase" /> class.
        /// </summary>
        /// <param name="firstParameter">The first parameter.</param>
        /// <param name="secondParameter">The second parameter.</param>
        /// <param name="thirdParameter">The third parameter.</param>
        /// <exception cref="ArgumentNullException"><paramref name="firstParameter" />
        /// or
        /// <paramref name="secondParameter" />
        /// or
        /// <paramref name="thirdParameter" />
        /// is <see langword="null" /> (<see langword="Nothing" /> in Visual Basic).</exception>
        [SuppressMessage(
            "Usage",
            "CA2214:Do not call overridable methods in constructors",
            Justification = "We specifically want this to happen.")]
        [SuppressMessage(
            "ReSharper",
            "VirtualMemberCallInConstructor",
            Justification = "We specifically want this to happen.")]
        protected TernaryFunctionNodeBase(
            NodeBase firstParameter,
            NodeBase secondParameter,
            NodeBase thirdParameter)
        {
            NodeBase firstParameterTemp = Requires.NotNull(firstParameter, nameof(firstParameter)).Simplify();
            NodeBase secondParameterTemp = Requires.NotNull(secondParameter, nameof(secondParameter)).Simplify();
            NodeBase thirdParameterTemp = Requires.NotNull(thirdParameter, nameof(thirdParameter)).Simplify();

            this.EnsureCompatibleParameters(firstParameterTemp, secondParameterTemp, thirdParameterTemp);

            this.FirstParameter = firstParameterTemp;
            this.SecondParameter = secondParameterTemp;
            this.ThirdParameter = thirdParameterTemp;
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
        /// Gets the third parameter.
        /// </summary>
        /// <value>The third parameter.</value>
        public NodeBase ThirdParameter { get; }

        /// <summary>
        ///     Gets a value indicating whether this node supports tolerance.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is tolerant; otherwise, <c>false</c>.
        /// </value>
        public sealed override bool IsTolerant =>
            this.FirstParameter.IsTolerant || this.SecondParameter.IsTolerant || this.ThirdParameter.IsTolerant;

        /// <summary>
        ///     Gets a value indicating whether this node requires preservation of the original expression.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the node requires original expression preservation, or <see langword="false" />
        ///     if it can be polynomially-reduced.
        /// </value>
        public sealed override bool RequiresPreservedExpression =>
            this.FirstParameter.RequiresPreservedExpression ||
            this.SecondParameter.RequiresPreservedExpression ||
            this.ThirdParameter.RequiresPreservedExpression;

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
            this.ThirdParameter.Verify();

            this.EnsureCompatibleParameters(
                this.FirstParameter,
                this.SecondParameter,
                this.ThirdParameter);
        }

        /// <summary>
        /// Ensures the parameters are compatible for this node.
        /// </summary>
        /// <param name="first">The first.</param>
        /// <param name="second">The second.</param>
        /// <param name="third">The third.</param>
        protected abstract void EnsureCompatibleParameters(
            NodeBase first,
            NodeBase second,
            NodeBase third);
    }
}