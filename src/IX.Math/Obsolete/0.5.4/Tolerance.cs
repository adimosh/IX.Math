// <copyright file="Tolerance.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Runtime.Serialization;

namespace IX.Math
{
    /// <summary>
    /// A data contract for a numeric tolerance.
    /// </summary>
    [DataContract]
    [Obsolete("Please use the ComparisonTolerance value type (implicitly convertible).")]
    public class Tolerance
    {
        /// <summary>
        /// Gets or sets the lower bound for a floating-point tolerance.
        /// </summary>
        /// <value>
        /// The tolerance range lower bound, or <see langword="null" /> for exact comparison or limit.
        /// </value>
        [DataMember]
        public double? ToleranceRangeLowerBound { get; set; }

        /// <summary>
        /// Gets or sets the upper bound for a floating-point tolerance.
        /// </summary>
        /// <value>
        /// The tolerance range upper bound, or <see langword="null" /> for exact comparison or limit.
        /// </value>
        [DataMember]
        public double? ToleranceRangeUpperBound { get; set; }

        /// <summary>
        /// Gets or sets the lower bound for an integer tolerance.
        /// </summary>
        /// <value>
        /// The tolerance range lower bound, or <see langword="null" /> for exact comparison or limit.
        /// </value>
        [DataMember]
        public long? IntegerToleranceRangeLowerBound { get; set; }

        /// <summary>
        /// Gets or sets the upper bound for an integer tolerance.
        /// </summary>
        /// <value>
        /// The tolerance range upper bound, or <see langword="null" /> for exact comparison or limit.
        /// </value>
        [DataMember]
        public long? IntegerToleranceRangeUpperBound { get; set; }

        /// <summary>
        /// Gets or sets the proportional tolerance.
        /// </summary>
        /// <value>
        /// The proportional tolerance, or <see langword="null" /> for exact comparison.
        /// </value>
        [DataMember]
        public double? ProportionalTolerance { get; set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Tolerance"/> to <see cref="ComparisonTolerance"/>.
        /// </summary>
        /// <param name="tolerance">The tolerance.</param>
        /// <returns>
        /// The result of the conversion.
        /// </returns>
        public static implicit operator ComparisonTolerance(Tolerance tolerance) =>
            tolerance == null
                ? new ComparisonTolerance(
                    null,
                    null,
                    null,
                    null,
                    null)
                : new ComparisonTolerance(
                    tolerance.ToleranceRangeLowerBound,
                    tolerance.ToleranceRangeUpperBound,
                    tolerance.IntegerToleranceRangeLowerBound,
                    tolerance.IntegerToleranceRangeLowerBound,
                    tolerance.ProportionalTolerance);

        /// <summary>
        /// Converts to <see cref="ComparisonTolerance"/>.
        /// </summary>
        /// <returns>An equivalent <see cref="ComparisonTolerance"/>.</returns>
        public ComparisonTolerance ToComparisonTolerance() => this;
    }
}