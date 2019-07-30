// <copyright file="SupportableValueType.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using JetBrains.Annotations;

namespace IX.Math
{
    /// <summary>
    ///     An enumeration of supported value types.
    /// </summary>
    [PublicAPI]
    [Flags]
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
        ///     Byte array (pass as <see cref="T:byte[]" />).
        /// </summary>
        ByteArray = 8,

        /// <summary>
        ///     All possible types.
        /// </summary>
        All = 15
    }
}