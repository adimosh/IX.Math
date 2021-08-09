// <copyright file="Tolerance.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Runtime.Serialization;

namespace IX.Math
{
    // TODO BREAKING: Change this into a readonly struct when the next breaking change occurs

    /// <summary>
    /// A data contract for a numeric tolerance.
    /// </summary>
    [DataContract]
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
    }
}