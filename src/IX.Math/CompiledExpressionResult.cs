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
    public readonly struct CompiledExpressionResult : IEquatable<CompiledExpressionResult>
    {
        /// <summary>
        /// The uncomputable result.
        /// </summary>
        public static readonly CompiledExpressionResult UncomputableResult = new(true);

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
        public Delegate? CompiledExpression { get; }

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
        public object? ConstantValue { get; }

        /// <summary>
        /// Gets a value indicating whether this <see cref="CompiledExpressionResult"/> is uncomputable.
        /// </summary>
        /// <value>
        ///   <c>true</c> if uncomputable; otherwise, <c>false</c>.
        /// </value>
        public bool Uncomputable { get; }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(CompiledExpressionResult left, CompiledExpressionResult right) => left.Equals(right);

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(CompiledExpressionResult left, CompiledExpressionResult right) => !left.Equals(right);

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is not CompiledExpressionResult other)
            {
                return false;
            }

            return this.IsConstant == other.IsConstant &&
                   this.Uncomputable == other.Uncomputable &&
                   this.CompiledExpression == other.CompiledExpression &&
                   this.ConstantValue == other.ConstantValue;
        }

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///   <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.
        /// </returns>
        public bool Equals(CompiledExpressionResult other) =>
            this.IsConstant == other.IsConstant &&
            this.Uncomputable == other.Uncomputable &&
            this.CompiledExpression == other.CompiledExpression &&
            this.ConstantValue == other.ConstantValue;

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode() =>
            (this.IsConstant, this.Uncomputable, this.CompiledExpression, this.ConstantValue).GetHashCode();
    }
}