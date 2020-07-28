// <copyright file="TestData.Gathered.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Collections.Generic;

namespace IX.UnitTests.Data
{
    /// <summary>
    ///     Test data for IX.Math tests.
    /// </summary>
    public partial class TestData
    {
        /// <summary>
        ///     Provides templated random text data for basic operators and parantheses.
        /// </summary>
        /// <returns>Test data.</returns>
        private static List<object[]> Gathered() =>
            new List<object[]>
            {
                new object[]
                {
                    "ln(2*(8-3))",
                    null,
                    0L,
                },
                new object[]
                {
                    "!5=x",
                    new Dictionary<string, object>
                    {
                        ["x"] = -6,
                    },
                    true,
                },
            };
    }
}