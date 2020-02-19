// <copyright file="SupportableValueType.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace IX.Math
{
    /// <summary>
    ///     An enumeration of supported value types.
    /// </summary>
    [PublicAPI]
    [Flags]
    [SuppressMessage(
        "Naming",
        "CA1720:Identifier contains type name",
        Justification = "This is OK, we're actually referring to string.")]
    [SuppressMessage(
        "Naming",
        "CA1714:Flags enums should have plural names",
        Justification = "This is OK, we're talking about types with pre-set names.")]
    public enum SupportableValueType
    {
        /// <summary>
        ///     No type supported.
        /// </summary>
        None = 0,

        /// <summary>
        ///     Numeric (depends on the numeric type).
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
        ///     All possible types.
        /// </summary>
        All = 15
    }
}