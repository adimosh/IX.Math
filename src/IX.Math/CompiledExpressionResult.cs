// <copyright file="CompiledExpressionResult.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using JetBrains.Annotations;

namespace IX.Math
{
    /// <summary>
    /// The result of a computed expression.
    /// </summary>
    [PublicAPI]
    public readonly struct CompiledExpressionResult
    {
        /// <summary>
        /// The uncomputable result.
        /// </summary>
        public static readonly CompiledExpressionResult UncomputableResult = new CompiledExpressionResult(true);

        /// <summary>
        /// Initializes a new instance of the <see cref="CompiledExpressionResult"/> struct.
        /// </summary>
        /// <param name="compiledExpression">The compiled expression.</param>
        public CompiledExpressionResult(Delegate compiledExpression)
        {
            this.CompiledExpression = compiledExpression;
            this.IsConstant = false;
            this.ConstantValue = null;
            this.Uncomputable = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CompiledExpressionResult"/> struct.
        /// </summary>
        /// <param name="value">The value.</param>
        public CompiledExpressionResult(object value)
        {
            this.CompiledExpression = null;
            this.IsConstant = true;
            this.ConstantValue = value;
            this.Uncomputable = false;
        }

        private CompiledExpressionResult(bool uncomputable)
        {
            this.CompiledExpression = null;
            this.IsConstant = false;
            this.ConstantValue = null;
            this.Uncomputable = uncomputable;
        }

        /// <summary>
        /// Gets the compiled expression, if one exists.
        /// </summary>
        /// <value>
        /// The compiled expression, or <see langword="null" /> (<see langword="Nothing" /> in Visual Basic).
        /// </value>
        public Delegate CompiledExpression { get; }

        /// <summary>
        /// Gets a value indicating whether this instance is constant.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is constant; otherwise, <c>false</c>.
        /// </value>
        public bool IsConstant { get; }

        /// <summary>
        /// Gets the constant value, if one exists.
        /// </summary>
        /// <value>
        /// The constant value.
        /// </value>
        public object ConstantValue { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="CompiledExpressionResult"/> is uncomputable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if uncomputable; otherwise, <c>false</c>.
        /// </value>
        public bool Uncomputable { get; }
    }
}