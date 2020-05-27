// <copyright file="ExpressionTypedKey.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;

namespace IX.Math
{
    /// <summary>
    /// A key based on a mathematical expression and the types it has as parameters.
    /// </summary>
    /// <seealso cref="IEquatable{T}" />
    [PublicAPI]
    public readonly struct ExpressionTypedKey : IEquatable<ExpressionTypedKey>
    {
#if NET452
        private static readonly Type[] emptyArray = new Type[0];
#endif

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTypedKey"/> struct.
        /// </summary>
        /// <param name="expression">The expression.</param>
        public ExpressionTypedKey(string expression)
            : this(
                expression,
                default,
#if NET452
                emptyArray)
#else
                Array.Empty<Type>())
#endif
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTypedKey"/> struct.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The parameters.</param>
        public ExpressionTypedKey(
            string expression,
            params Type[] parameters)
            : this(
                expression,
                default,
                parameters)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTypedKey"/> struct.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="tolerance">The tolerance.</param>
        public ExpressionTypedKey(
            string expression,
            ComparisonTolerance tolerance)
            : this(
                expression,
                tolerance,
#if NET452
                emptyArray)
#else
                Array.Empty<Type>())
#endif
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTypedKey" /> struct.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <param name="parameters">The parameters.</param>
        public ExpressionTypedKey(
            string expression,
            ComparisonTolerance tolerance,
            params Type[] parameters)
        {
            this.Expression = expression;
            this.Tolerance = tolerance;
            this.ParameterTypes = Array.AsReadOnly(parameters);
        }

        /// <summary>
        /// Gets the expression.
        /// </summary>
        /// <value>
        /// The expression.
        /// </value>
        public string Expression { get; }

        /// <summary>
        /// Gets the tolerance.
        /// </summary>
        /// <value>
        /// The tolerance.
        /// </value>
        public ComparisonTolerance Tolerance { get; }

        /// <summary>
        /// Gets the parameter types.
        /// </summary>
        /// <value>
        /// The parameter types.
        /// </value>
        public ReadOnlyCollection<Type> ParameterTypes { get; }

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="one">The one.</param>
        /// <param name="other">The other.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator ==(ExpressionTypedKey one, ExpressionTypedKey other) =>
            string.Equals(
                one.Expression,
                other.Expression,
                StringComparison.Ordinal) &&
            one.Tolerance == other.Tolerance &&
            one.ParameterTypes.SequenceEqual(other.ParameterTypes);

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="one">The one.</param>
        /// <param name="other">The other.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        public static bool operator !=(ExpressionTypedKey one, ExpressionTypedKey other) =>
            !string.Equals(
                one.Expression,
                other.Expression,
                StringComparison.Ordinal) ||
            one.Tolerance != other.Tolerance ||
            !one.ParameterTypes.SequenceEqual(other.ParameterTypes);

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object obj)
        {
            if (obj is ExpressionTypedKey other)
            {
                return this.Equals(other);
            }

            return false;
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        /// <remarks><para>This method optimizes hash code by fwardingthehash code oofthe expressioring.</para>
        /// <para>This optimization wlll put same expressions with different parmeters in the same bucket. Please use the <see cref="Equals(ExpressionTypedKey)"/> method to check for equality.</para></remarks>
        public override int GetHashCode() => this.Expression.GetHashCode();

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///   <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.
        /// </returns>
        public bool Equals(ExpressionTypedKey other) => this == other;
    }
}