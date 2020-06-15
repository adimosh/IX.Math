// <copyright file="SupportedValueType.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace IX.Math
{
    /// <summary>
    ///     An enumeration of supported value types.
    /// </summary>
    [PublicAPI]
    [SuppressMessage(
        "Naming",
        "CA1720:Identifier contains type name",
        Justification = "This is OK, we're actually referring to string.")]
    public enum SupportedValueType
    {
        /// <summary>
        ///     Not known (pass as <see cref="object" />).
        /// </summary>
        Unknown = 0,

        /// <summary>
        ///     Numeric (pass as <see cref="double"/>).
        /// </summary>
        Numeric = 1,

        /// <summary>
        ///     Boolean (pass as <see cref="bool" />).
        /// </summary>
        Boolean = 2,

        /// <summary>
        ///     String (pass as <see cref="string" />).
        /// </summary>
        String = 4,

        /// <summary>
        ///     Byte array (pass as array of <see cref="byte" />).
        /// </summary>
        ByteArray = 8,

        /// <summary>
        ///     Integer (pass as array of <see cref="long" />).
        /// </summary>
        Integer = 16,
    }
}