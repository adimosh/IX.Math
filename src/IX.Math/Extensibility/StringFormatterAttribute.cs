// <copyright file="StringFormatterAttribute.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using JetBrains.Annotations;

namespace IX.Math.Extensibility
{
    /// <summary>
    ///     An attribute that will signal a specific class as containing a string formatter.
    /// </summary>
    /// <seealso cref="Attribute" />
    [PublicAPI]
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class StringFormatterAttribute : Attribute
    {
        /// <summary>
        ///     Gets or sets the level.
        /// </summary>
        /// <value>The level.</value>
        public int Level { get; set; }
    }
}