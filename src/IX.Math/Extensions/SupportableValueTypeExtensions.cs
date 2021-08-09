// <copyright file="SupportableValueTypeExtensions.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

namespace IX.Math.Extensions
{
    /// <summary>
    /// Extensions for <see cref="SupportableValueType"/>
    /// </summary>
    public static class SupportableValueTypeExtensions
    {
        /// <summary>
        /// Distills the determination of possible return values into a single one, if possible, throwing an exception otherwise.
        /// </summary>
        /// <param name="supportableValues">The supportable value types.</param>
        /// <returns>A single supported value type.</returns>
        public static SupportedValueType DistillDetermination(this SupportableValueType supportableValues) =>
            supportableValues switch
            {
                SupportableValueType.Boolean => SupportedValueType.Boolean,
                SupportableValueType.ByteArray => SupportedValueType.ByteArray,
                SupportableValueType.Integer => SupportedValueType.Integer,
                SupportableValueType.Numeric => SupportedValueType.Numeric,
                SupportableValueType.String => SupportedValueType.String,
                _ => throw new ExpressionNotValidLogicallyException()
            };
    }
}