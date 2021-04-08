// <copyright file="TestData.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq;
using JetBrains.Annotations;

namespace IX.UnitTests
{
    /// <summary>
    ///     Test data for IX.Math tests.
    /// </summary>
    [UsedImplicitly]
    public static partial class TestData
    {
        /// <summary>
        ///     Provides templated text data.
        /// </summary>
        /// <returns>Test data.</returns>
        public static object[][] GenerateDataObjects() => BasicOperatorsWithRandomNumbers().Union(SpecialCases()).ToArray();
    }
}