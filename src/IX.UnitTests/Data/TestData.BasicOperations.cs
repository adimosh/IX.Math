// <copyright file="TestData.BasicOperations.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Collections.Generic;

namespace IX.UnitTests.Data
{
    /// <summary>
    ///     Test data for IX.Math tests.
    /// </summary>
    public static partial class TestData
    {
        /// <summary>
        ///     Provides templated random text data for basic operators and parantheses.
        /// </summary>
        /// <returns>Test data.</returns>
        public static List<object[]> BasicOperatorsWithRandomNumbers()
        {
            // Define and initialize
            var tests = new List<object[]>();

            // Tests
            foreach (var item in GenerateThreeFoldTestData(Operators))
            {
                tests.Add(
                    new[]
                    {
                        item.Item2,
                        item.Item3,
                        item.Item1,
                    });
            }

            // Return
            return tests;
        }
    }
}