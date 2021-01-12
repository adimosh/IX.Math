// <copyright file="ComparisonTolerance.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace IX.Math
{
    /// <summary>
    /// A data contract for a numeric tolerance.
    /// </summary>
    [DataContract]
    [PublicAPI]
    public readonly struct ComparisonTolerance : IEquatable<ComparisonTolerance>
    {
        /// <summary>
        /// An empty tolerance object.
        /// </summary>
        public static readonly ComparisonTolerance Empty;

        /// <summary>
        /// Initializes a new instance of the <see cref="ComparisonTolerance"/> struct.
        /// </summary>
        /// <param name="toleranceRangeLowerBound">The tolerance range lower bound.</param>
        /// <param name="toleranceRangeUpperBound">The tolerance range upper bound.</param>
        /// <param name="integerToleranceRangeLowerBound">The integer tolerance range lower bound.</param>
        /// <param name="integerToleranceRangeUpperBound">The integer tolerance range upper bound.</param>
        /// <param name="proportionalTolerance">The proportional tolerance.</param>
        public ComparisonTolerance(
            double? toleranceRangeLowerBound = null,
            double? toleranceRangeUpperBound = null,
            long? integerToleranceRangeLowerBound = null,
            long? integerToleranceRangeUpperBound = null,
            double? proportionalTolerance = null)
        {
            this.ToleranceRangeLowerBound = toleranceRangeLowerBound;
            this.ToleranceRangeUpperBound = toleranceRangeUpperBound;
            this.IntegerToleranceRangeLowerBound = integerToleranceRangeLowerBound;
            this.IntegerToleranceRangeUpperBound = integerToleranceRangeUpperBound;
            this.ProportionalTolerance = proportionalTolerance;
        }

        /// <summary>
        /// Gets the lower bound for a floating-point tolerance.
        /// </summary>
        /// <value>
        /// The tolerance range lower bound, or <see langword="null" /> for exact comparison or limit.
        /// </value>
        [field: DataMember]
        public double? ToleranceRangeLowerBound { get; }

        /// <summary>
        /// Gets the upper bound for a floating-point tolerance.
        /// </summary>
        /// <value>
        /// The tolerance range upper bound, or <see langword="null" /> for exact comparison or limit.
        /// </value>
        [field: DataMember]
        public double? ToleranceRangeUpperBound { get; }

        /// <summary>
        /// Gets the lower bound for an integer tolerance.
        /// </summary>
        /// <value>
        /// The tolerance range lower bound, or <see langword="null" /> for exact comparison or limit.
        /// </value>
        [field: DataMember]
        public long? IntegerToleranceRangeLowerBound { get; }

        /// <summary>
        /// Gets the upper bound for an integer tolerance.
        /// </summary>
        /// <value>
        /// The tolerance range upper bound, or <see langword="null" /> for exact comparison or limit.
        /// </value>
        [field: DataMember]
        public long? IntegerToleranceRangeUpperBound { get; }

        /// <summary>
        /// Gets the proportional tolerance.
        /// </summary>
        /// <value>
        /// The proportional tolerance, or <see langword="null" /> for exact comparison.
        /// </value>
        [field: DataMember]
        public double? ProportionalTolerance { get; }

        /// <summary>
        /// Gets a value indicating whether this tolerance is empty.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is empty; otherwise, <c>false</c>.
        /// </value>
        public bool IsEmpty =>
            !this.ToleranceRangeLowerBound.HasValue &&
            !this.ToleranceRangeUpperBound.HasValue &&
            !this.IntegerToleranceRangeLowerBound.HasValue &&
            !this.IntegerToleranceRangeUpperBound.HasValue &&
            !this.ProportionalTolerance.HasValue;

        /// <summary>
        /// Implements the operator ==.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [SuppressMessage(
            "ReSharper",
            "CompareOfFloatsByEqualityOperator",
            Justification = "We're not really interested here, since we're doing complete equality comparison.")]
        public static bool operator ==(
            ComparisonTolerance left,
            ComparisonTolerance right) => left.ToleranceRangeLowerBound == right.ToleranceRangeLowerBound &&
                   left.ToleranceRangeUpperBound == right.ToleranceRangeUpperBound &&
                   left.IntegerToleranceRangeLowerBound == right.IntegerToleranceRangeLowerBound &&
                   left.IntegerToleranceRangeUpperBound == right.IntegerToleranceRangeUpperBound &&
                   left.ProportionalTolerance == right.ProportionalTolerance;

        /// <summary>
        /// Implements the operator !=.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>
        /// The result of the operator.
        /// </returns>
        [SuppressMessage(
            "ReSharper",
            "CompareOfFloatsByEqualityOperator",
            Justification = "We're not really interested here, since we're doing complete equality comparison.")]
        public static bool operator !=(
            ComparisonTolerance left,
            ComparisonTolerance right) => left.ToleranceRangeLowerBound != right.ToleranceRangeLowerBound ||
                                          left.ToleranceRangeUpperBound != right.ToleranceRangeUpperBound ||
                                          left.IntegerToleranceRangeLowerBound != right.IntegerToleranceRangeLowerBound ||
                                          left.IntegerToleranceRangeUpperBound != right.IntegerToleranceRangeUpperBound ||
                                          left.ProportionalTolerance != right.ProportionalTolerance;

        /// <summary>
        /// Indicates whether the current object is equal to another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>
        ///   <see langword="true" /> if the current object is equal to the <paramref name="other" /> parameter; otherwise, <see langword="false" />.
        /// </returns>
        public bool Equals(ComparisonTolerance other) => this == other;

        /// <summary>Indicates whether this instance and a specified object are equal.</summary>
        /// <param name="obj">The object to compare with the current instance.</param>
        /// <returns>
        /// <see langword="true" /> if <paramref name="obj" /> and this instance are the same type and represent the same value; otherwise, <see langword="false" />.</returns>
        public override bool Equals(object? obj)
        {
            if (obj is ComparisonTolerance other)
            {
                return this == other;
            }

            return false;
        }

        /// <summary>Returns the hash code for this instance.</summary>
        /// <returns>A 32-bit signed integer that is the hash code for this instance.</returns>
        public override int GetHashCode() =>
            (this.ToleranceRangeLowerBound, this.ToleranceRangeUpperBound, this.IntegerToleranceRangeLowerBound,
                this.IntegerToleranceRangeUpperBound, this.ProportionalTolerance).GetHashCode();
    }
}