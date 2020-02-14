// <copyright file="IConstantPassThroughExtractor.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using JetBrains.Annotations;

namespace IX.Math.Extraction
{
    /// <summary>
    ///     A service contract for extracting pass-through constants.
    /// </summary>
    [PublicAPI]
    [Obsolete("This has been moved to the IX.Math.Extensibility namespace, please use the interface there.")]
    public interface IConstantPassThroughExtractor : Extensibility.IConstantPassThroughExtractor
    {
    }
}