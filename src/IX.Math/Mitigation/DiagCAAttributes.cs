// <copyright file="DiagCAAttributes.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#if !NETSTANDARD2_1 && !NET5_0
// ReSharper disable CheckNamespace
namespace System.Diagnostics.CodeAnalysis
{
    /// <summary>
    /// Attribute to mitigate System.Diagnostics.CodeAnalysis.MaybeNullWhenAttribute on platforms lower than .NET standard 2.1.
    /// </summary>
    /// <seealso cref="System.Attribute" />
    [AttributeUsage(AttributeTargets.Parameter)]
    [SuppressMessage(
        "StyleCop.CSharp.DocumentationRules",
        "SA1649:File name should match first type name",
        Justification = "This file is mitigating for unavailable items.")]
    internal class MaybeNullWhenAttribute : Attribute
    {
        public MaybeNullWhenAttribute(bool returnValue)
        {
            this.ReturnValue = returnValue;
        }

        /// <summary>
        /// Gets the return value condition.
        /// </summary>
        /// <value>
        ///   The return value condition.
        /// </value>
        [SuppressMessage(
            "StyleCop.CSharp.DocumentationRules",
            "SA1623:Property summary documentation should match accessors",
            Justification = "This is not a boolean switch.")]
        public bool ReturnValue { get; }
    }
}
#endif