// <copyright file="NodeBase.Obsolete.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using IX.StandardExtensions;

// ReSharper disable once CheckNamespace
namespace IX.Math.Nodes
{
#pragma warning disable SA1601 // Partial elements should be documented
    public partial class NodeBase
#pragma warning restore SA1601 // Partial elements should be documented
    {
        /// <summary>
        ///     Gets a value indicating whether or not this node is actually a constant.
        /// </summary>
        /// <value><see langword="true" /> if the node is a constant, <see langword="false" /> otherwise.</value>
        [Obsolete("This is not going to be used anymore.")]
        public bool IsConstant => throw new NotImplementedByDesignException();

        /// <summary>
        ///     Gets a value indicating whether this node supports tolerance.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is tolerant; otherwise, <c>false</c>.
        /// </value>
        [Obsolete("This is not going to be used anymore.")]
        public bool IsTolerant => throw new NotImplementedByDesignException();

        /// <summary>
        ///     Gets the return type of this node.
        /// </summary>
        /// <value>The node return type.</value>
        [Obsolete("This is not to be used anymore in determining the possible uses for a node.")]
        public SupportedValueType ReturnType => throw new NotImplementedByDesignException();

        /// <summary>
        ///     Generates the expression that will be compiled into code as a string expression.
        /// </summary>
        /// <returns>The generated <see cref="Expression" /> that gives the values as a string.</returns>
        [Obsolete("This is not going to be used anymore.")]
        public Expression GenerateStringExpression() => throw new NotImplementedByDesignException();

        /// <summary>
        ///     Generates the expression that will be compiled into code as a string expression.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>The generated <see cref="Expression" /> that gives the values as a string.</returns>
        [Obsolete("This is not going to be used anymore.")]
        public virtual Expression GenerateStringExpression(Tolerance tolerance) => throw new NotImplementedByDesignException();

        /// <summary>
        ///     Strongly determines the node's type, if possible.
        /// </summary>
        /// <param name="type">The type to determine to.</param>
        [Obsolete("This will no longer be used.")]
        public void DetermineStrongly(SupportedValueType type) => throw new NotImplementedByDesignException();

        /// <summary>
        ///     Weakly determines the node's type, if possible, and, optionally, strongly determines if there is only one possible
        ///     type left.
        /// </summary>
        /// <param name="type">The type or types to determine to.</param>
        [Obsolete("This will no longer be used.")]
        public void DetermineWeakly(SupportableValueType type) => throw new NotImplementedByDesignException();

        /// <summary>
        ///     Generates the expression that will be compiled into code.
        /// </summary>
        /// <param name="tolerance">The tolerance. If this parameter is <c>null</c> (<c>Nothing</c> in Visual Basic), then the operation is exact.</param>
        /// <returns>
        ///     The generated <see cref="Expression" />.
        /// </returns>
        [SuppressMessage(
            "ReSharper",
            "MethodOverloadWithOptionalParameter",
            Justification = "We've marked the overload as obsolete and will be removed in a future version")]
        [Obsolete("This will no longer be used.")]
        public Expression GenerateExpression(Tolerance? tolerance = null) => throw new NotImplementedByDesignException();

        /// <summary>
        ///     Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>The generated <see cref="Expression" />.</returns>
        [Obsolete("This will not be use anymore.")]
        public Expression GenerateExpression() => throw new NotImplementedByDesignException();
    }
}