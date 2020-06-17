// <copyright file="NonaryFunctionNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Collections.Generic;
using IX.Math.Extensibility;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Functions.Nonary
{
    /// <summary>
    /// A base class for functions that take no parameters.
    /// </summary>
    /// <seealso cref="FunctionNodeBase" />
    [PublicAPI]
    public abstract class NonaryFunctionNodeBase : FunctionNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="NonaryFunctionNodeBase"/> class.
        /// </summary>
        /// <param name="stringFormatters">The string formatters.</param>
        protected NonaryFunctionNodeBase(List<IStringFormatter> stringFormatters)
            : base(stringFormatters)
        {
        }

        /// <summary>
        ///     Gets a value indicating whether this node supports tolerance.
        /// </summary>
        /// <value>
        ///     Always <see langword="false" />.
        /// </value>
        public sealed override bool IsTolerant => false;

        /// <summary>
        ///     Gets a value indicating whether this node requires preservation of the original expression.
        /// </summary>
        /// <value>
        ///     Always <see langword="false" />.
        /// </value>
        public sealed override bool RequiresPreservedExpression => false;

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>Always this instance.</returns>
        public sealed override NodeBase Simplify() => this;
    }
}