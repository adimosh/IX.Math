// <copyright file="IConstantsExtractor.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using JetBrains.Annotations;

namespace IX.Math.Extraction
{
    /// <summary>
    ///     A service contract for extractors of constant values from the expression.
    /// </summary>
    [PublicAPI]
    [Obsolete("This has been moved to the IX.Math.Extensibility namespace, please use the interface there.")]
    public interface IConstantsExtractor : Extensibility.IConstantsExtractor
    {
    }
}