// <copyright file="TestData.BasicOperatorsWithRandomNumbers.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Collections.Generic;
using IX.StandardExtensions.TestUtils;

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
            const int limit = 1000;
            var tests = new List<object[]>();
            int operand1, operand2, operand3;

            // Tests
            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} + {operand2}",
                null,
                operand1 + operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} + ({operand1} + {operand2})",
                null,
                operand3 + (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} + {operand2}) + {operand3}",
                null,
                (operand1 + operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z + ({operand1} + {operand2})",
                new Dictionary<string, object> { ["z"] = operand3 },
                operand3 + (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} + {operand2}) + z",
                new Dictionary<string, object> { ["z"] = operand3 },
                (operand1 + operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x + {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 + operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} + (x + {operand2})",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand3 + (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x + {operand2}) + {operand3}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (operand1 + operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z + (x + {operand2})",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                operand3 + (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x + {operand2}) + z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (operand1 + operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} + y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 + operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} + ({operand1} + y)",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand3 + (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} + y) + {operand3}",
                new Dictionary<string, object> { ["y"] = operand2 },
                (operand1 + operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z + ({operand1} + y)",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                operand3 + (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} + y) + z",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (operand1 + operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x + y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 + operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} + (x + y)",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand3 + (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x + y) + {operand3}",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (operand1 + operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z + (x + y)",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                operand3 + (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x + y) + z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (operand1 + operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} - {operand2}",
                null,
                operand1 - operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} + ({operand1} - {operand2})",
                null,
                operand3 + (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} - {operand2}) + {operand3}",
                null,
                (operand1 - operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z + ({operand1} - {operand2})",
                new Dictionary<string, object> { ["z"] = operand3 },
                operand3 + (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} - {operand2}) + z",
                new Dictionary<string, object> { ["z"] = operand3 },
                (operand1 - operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x - {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 - operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} + (x - {operand2})",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand3 + (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x - {operand2}) + {operand3}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (operand1 - operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z + (x - {operand2})",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                operand3 + (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x - {operand2}) + z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (operand1 - operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} - y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 - operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} + ({operand1} - y)",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand3 + (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} - y) + {operand3}",
                new Dictionary<string, object> { ["y"] = operand2 },
                (operand1 - operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z + ({operand1} - y)",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                operand3 + (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} - y) + z",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (operand1 - operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x - y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 - operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} + (x - y)",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand3 + (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x - y) + {operand3}",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (operand1 - operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z + (x - y)",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                operand3 + (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x - y) + z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (operand1 - operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} * {operand2}",
                null,
                operand1 * operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} + ({operand1} * {operand2})",
                null,
                operand3 + (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} * {operand2}) + {operand3}",
                null,
                (operand1 * operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z + ({operand1} * {operand2})",
                new Dictionary<string, object> { ["z"] = operand3 },
                operand3 + (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} * {operand2}) + z",
                new Dictionary<string, object> { ["z"] = operand3 },
                (operand1 * operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x * {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 * operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} + (x * {operand2})",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand3 + (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x * {operand2}) + {operand3}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (operand1 * operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z + (x * {operand2})",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                operand3 + (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x * {operand2}) + z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (operand1 * operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} * y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 * operand2
            });
/*
            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} + ({operand1} * y)",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand3 + (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} * y) + {operand3}",
                new Dictionary<string, object> { ["y"] = operand2 },
                (operand1 * operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z + ({operand1} * y)",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                operand3 + (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} * y) + z",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (operand1 * operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x * y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 * operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} + (x * y)",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand3 + (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x * y) + {operand3}",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (operand1 * operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z + (x * y)",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                operand3 + (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x * y) + z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (operand1 * operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} / {operand2}",
                null,
                (double)operand1 / (double)operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x / {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (double)operand1 / (double)operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} / y",
                new Dictionary<string, object> { ["y"] = operand2 },
                (double)operand1 / (double)operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x / y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (double)operand1 / (double)operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} & {operand2}",
                null,
                operand1 & operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} + ({operand1} & {operand2})",
                null,
                operand3 + (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} & {operand2}) + {operand3}",
                null,
                (operand1 & operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z + ({operand1} & {operand2})",
                new Dictionary<string, object> { ["z"] = operand3 },
                operand3 + (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} & {operand2}) + z",
                new Dictionary<string, object> { ["z"] = operand3 },
                (operand1 & operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x & {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 & operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} + (x & {operand2})",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand3 + (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x & {operand2}) + {operand3}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (operand1 & operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z + (x & {operand2})",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                operand3 + (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x & {operand2}) + z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (operand1 & operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} & y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 & operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} + ({operand1} & y)",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand3 + (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} & y) + {operand3}",
                new Dictionary<string, object> { ["y"] = operand2 },
                (operand1 & operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z + ({operand1} & y)",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                operand3 + (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} & y) + z",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (operand1 & operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x & y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 & operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} + (x & y)",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand3 + (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x & y) + {operand3}",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (operand1 & operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z + (x & y)",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                operand3 + (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x & y) + z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (operand1 & operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} | {operand2}",
                null,
                operand1 | operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} + ({operand1} | {operand2})",
                null,
                operand3 + (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} | {operand2}) + {operand3}",
                null,
                (operand1 | operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z + ({operand1} | {operand2})",
                new Dictionary<string, object> { ["z"] = operand3 },
                operand3 + (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} | {operand2}) + z",
                new Dictionary<string, object> { ["z"] = operand3 },
                (operand1 | operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x | {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 | operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} + (x | {operand2})",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand3 + (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x | {operand2}) + {operand3}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (operand1 | operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z + (x | {operand2})",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                operand3 + (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x | {operand2}) + z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (operand1 | operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} | y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 | operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} + ({operand1} | y)",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand3 + (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} | y) + {operand3}",
                new Dictionary<string, object> { ["y"] = operand2 },
                (operand1 | operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z + ({operand1} | y)",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                operand3 + (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} | y) + z",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (operand1 | operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x | y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 | operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} + (x | y)",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand3 + (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x | y) + {operand3}",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (operand1 | operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z + (x | y)",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                operand3 + (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x | y) + z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (operand1 | operand2) + operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} + {operand2}",
                null,
                operand1 + operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} - ({operand1} + {operand2})",
                null,
                operand3 - (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} + {operand2}) - {operand3}",
                null,
                (operand1 + operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z - ({operand1} + {operand2})",
                new Dictionary<string, object> { ["z"] = operand3 },
                operand3 - (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} + {operand2}) - z",
                new Dictionary<string, object> { ["z"] = operand3 },
                (operand1 + operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x + {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 + operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} - (x + {operand2})",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand3 - (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x + {operand2}) - {operand3}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (operand1 + operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z - (x + {operand2})",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                operand3 - (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x + {operand2}) - z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (operand1 + operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} + y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 + operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} - ({operand1} + y)",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand3 - (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} + y) - {operand3}",
                new Dictionary<string, object> { ["y"] = operand2 },
                (operand1 + operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z - ({operand1} + y)",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                operand3 - (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} + y) - z",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (operand1 + operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x + y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 + operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} - (x + y)",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand3 - (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x + y) - {operand3}",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (operand1 + operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z - (x + y)",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                operand3 - (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x + y) - z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (operand1 + operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} - {operand2}",
                null,
                operand1 - operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} - ({operand1} - {operand2})",
                null,
                operand3 - (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} - {operand2}) - {operand3}",
                null,
                (operand1 - operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z - ({operand1} - {operand2})",
                new Dictionary<string, object> { ["z"] = operand3 },
                operand3 - (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} - {operand2}) - z",
                new Dictionary<string, object> { ["z"] = operand3 },
                (operand1 - operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x - {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 - operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} - (x - {operand2})",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand3 - (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x - {operand2}) - {operand3}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (operand1 - operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z - (x - {operand2})",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                operand3 - (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x - {operand2}) - z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (operand1 - operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} - y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 - operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} - ({operand1} - y)",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand3 - (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} - y) - {operand3}",
                new Dictionary<string, object> { ["y"] = operand2 },
                (operand1 - operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z - ({operand1} - y)",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                operand3 - (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} - y) - z",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (operand1 - operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x - y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 - operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} - (x - y)",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand3 - (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x - y) - {operand3}",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (operand1 - operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z - (x - y)",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                operand3 - (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x - y) - z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (operand1 - operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} * {operand2}",
                null,
                operand1 * operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} - ({operand1} * {operand2})",
                null,
                operand3 - (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} * {operand2}) - {operand3}",
                null,
                (operand1 * operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z - ({operand1} * {operand2})",
                new Dictionary<string, object> { ["z"] = operand3 },
                operand3 - (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} * {operand2}) - z",
                new Dictionary<string, object> { ["z"] = operand3 },
                (operand1 * operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x * {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 * operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} - (x * {operand2})",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand3 - (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x * {operand2}) - {operand3}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (operand1 * operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z - (x * {operand2})",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                operand3 - (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x * {operand2}) - z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (operand1 * operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} * y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 * operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} - ({operand1} * y)",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand3 - (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} * y) - {operand3}",
                new Dictionary<string, object> { ["y"] = operand2 },
                (operand1 * operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z - ({operand1} * y)",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                operand3 - (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} * y) - z",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (operand1 * operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x * y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 * operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} - (x * y)",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand3 - (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x * y) - {operand3}",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (operand1 * operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z - (x * y)",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                operand3 - (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x * y) - z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (operand1 * operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} / {operand2}",
                null,
                (double)operand1 / (double)operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x / {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (double)operand1 / (double)operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} / y",
                new Dictionary<string, object> { ["y"] = operand2 },
                (double)operand1 / (double)operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x / y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (double)operand1 / (double)operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} & {operand2}",
                null,
                operand1 & operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} - ({operand1} & {operand2})",
                null,
                operand3 - (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} & {operand2}) - {operand3}",
                null,
                (operand1 & operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z - ({operand1} & {operand2})",
                new Dictionary<string, object> { ["z"] = operand3 },
                operand3 - (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} & {operand2}) - z",
                new Dictionary<string, object> { ["z"] = operand3 },
                (operand1 & operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x & {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 & operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} - (x & {operand2})",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand3 - (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x & {operand2}) - {operand3}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (operand1 & operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z - (x & {operand2})",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                operand3 - (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x & {operand2}) - z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (operand1 & operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} & y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 & operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} - ({operand1} & y)",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand3 - (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} & y) - {operand3}",
                new Dictionary<string, object> { ["y"] = operand2 },
                (operand1 & operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z - ({operand1} & y)",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                operand3 - (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} & y) - z",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (operand1 & operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x & y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 & operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} - (x & y)",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand3 - (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x & y) - {operand3}",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (operand1 & operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z - (x & y)",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                operand3 - (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x & y) - z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (operand1 & operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} | {operand2}",
                null,
                operand1 | operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} - ({operand1} | {operand2})",
                null,
                operand3 - (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} | {operand2}) - {operand3}",
                null,
                (operand1 | operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z - ({operand1} | {operand2})",
                new Dictionary<string, object> { ["z"] = operand3 },
                operand3 - (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} | {operand2}) - z",
                new Dictionary<string, object> { ["z"] = operand3 },
                (operand1 | operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x | {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 | operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} - (x | {operand2})",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand3 - (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x | {operand2}) - {operand3}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (operand1 | operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z - (x | {operand2})",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                operand3 - (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x | {operand2}) - z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (operand1 | operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} | y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 | operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} - ({operand1} | y)",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand3 - (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} | y) - {operand3}",
                new Dictionary<string, object> { ["y"] = operand2 },
                (operand1 | operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z - ({operand1} | y)",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                operand3 - (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} | y) - z",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (operand1 | operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x | y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 | operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} - (x | y)",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand3 - (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x | y) - {operand3}",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (operand1 | operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z - (x | y)",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                operand3 - (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x | y) - z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (operand1 | operand2) - operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} + {operand2}",
                null,
                operand1 + operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} * ({operand1} + {operand2})",
                null,
                operand3 * (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} + {operand2}) * {operand3}",
                null,
                (operand1 + operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z * ({operand1} + {operand2})",
                new Dictionary<string, object> { ["z"] = operand3 },
                operand3 * (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} + {operand2}) * z",
                new Dictionary<string, object> { ["z"] = operand3 },
                (operand1 + operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x + {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 + operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} * (x + {operand2})",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand3 * (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x + {operand2}) * {operand3}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (operand1 + operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z * (x + {operand2})",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                operand3 * (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x + {operand2}) * z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (operand1 + operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} + y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 + operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} * ({operand1} + y)",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand3 * (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} + y) * {operand3}",
                new Dictionary<string, object> { ["y"] = operand2 },
                (operand1 + operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z * ({operand1} + y)",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                operand3 * (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} + y) * z",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (operand1 + operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x + y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 + operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} * (x + y)",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand3 * (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x + y) * {operand3}",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (operand1 + operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z * (x + y)",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                operand3 * (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x + y) * z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (operand1 + operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} - {operand2}",
                null,
                operand1 - operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} * ({operand1} - {operand2})",
                null,
                operand3 * (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} - {operand2}) * {operand3}",
                null,
                (operand1 - operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z * ({operand1} - {operand2})",
                new Dictionary<string, object> { ["z"] = operand3 },
                operand3 * (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} - {operand2}) * z",
                new Dictionary<string, object> { ["z"] = operand3 },
                (operand1 - operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x - {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 - operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} * (x - {operand2})",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand3 * (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x - {operand2}) * {operand3}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (operand1 - operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z * (x - {operand2})",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                operand3 * (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x - {operand2}) * z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (operand1 - operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} - y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 - operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} * ({operand1} - y)",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand3 * (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} - y) * {operand3}",
                new Dictionary<string, object> { ["y"] = operand2 },
                (operand1 - operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z * ({operand1} - y)",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                operand3 * (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} - y) * z",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (operand1 - operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x - y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 - operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} * (x - y)",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand3 * (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x - y) * {operand3}",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (operand1 - operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z * (x - y)",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                operand3 * (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x - y) * z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (operand1 - operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} * {operand2}",
                null,
                operand1 * operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} * ({operand1} * {operand2})",
                null,
                operand3 * (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} * {operand2}) * {operand3}",
                null,
                (operand1 * operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z * ({operand1} * {operand2})",
                new Dictionary<string, object> { ["z"] = operand3 },
                operand3 * (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} * {operand2}) * z",
                new Dictionary<string, object> { ["z"] = operand3 },
                (operand1 * operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x * {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 * operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} * (x * {operand2})",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand3 * (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x * {operand2}) * {operand3}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (operand1 * operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z * (x * {operand2})",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                operand3 * (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x * {operand2}) * z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (operand1 * operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} * y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 * operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} * ({operand1} * y)",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand3 * (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} * y) * {operand3}",
                new Dictionary<string, object> { ["y"] = operand2 },
                (operand1 * operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z * ({operand1} * y)",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                operand3 * (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} * y) * z",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (operand1 * operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x * y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 * operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} * (x * y)",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand3 * (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x * y) * {operand3}",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (operand1 * operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z * (x * y)",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                operand3 * (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x * y) * z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (operand1 * operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} / {operand2}",
                null,
                (double)operand1 / (double)operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x / {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (double)operand1 / (double)operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} / y",
                new Dictionary<string, object> { ["y"] = operand2 },
                (double)operand1 / (double)operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x / y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (double)operand1 / (double)operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} & {operand2}",
                null,
                operand1 & operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} * ({operand1} & {operand2})",
                null,
                operand3 * (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} & {operand2}) * {operand3}",
                null,
                (operand1 & operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z * ({operand1} & {operand2})",
                new Dictionary<string, object> { ["z"] = operand3 },
                operand3 * (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} & {operand2}) * z",
                new Dictionary<string, object> { ["z"] = operand3 },
                (operand1 & operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x & {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 & operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} * (x & {operand2})",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand3 * (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x & {operand2}) * {operand3}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (operand1 & operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z * (x & {operand2})",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                operand3 * (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x & {operand2}) * z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (operand1 & operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} & y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 & operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} * ({operand1} & y)",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand3 * (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} & y) * {operand3}",
                new Dictionary<string, object> { ["y"] = operand2 },
                (operand1 & operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z * ({operand1} & y)",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                operand3 * (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} & y) * z",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (operand1 & operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x & y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 & operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} * (x & y)",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand3 * (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x & y) * {operand3}",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (operand1 & operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z * (x & y)",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                operand3 * (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x & y) * z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (operand1 & operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} | {operand2}",
                null,
                operand1 | operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} * ({operand1} | {operand2})",
                null,
                operand3 * (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} | {operand2}) * {operand3}",
                null,
                (operand1 | operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z * ({operand1} | {operand2})",
                new Dictionary<string, object> { ["z"] = operand3 },
                operand3 * (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} | {operand2}) * z",
                new Dictionary<string, object> { ["z"] = operand3 },
                (operand1 | operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x | {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 | operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} * (x | {operand2})",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand3 * (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x | {operand2}) * {operand3}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (operand1 | operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z * (x | {operand2})",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                operand3 * (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x | {operand2}) * z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (operand1 | operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} | y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 | operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} * ({operand1} | y)",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand3 * (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} | y) * {operand3}",
                new Dictionary<string, object> { ["y"] = operand2 },
                (operand1 | operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z * ({operand1} | y)",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                operand3 * (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} | y) * z",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (operand1 | operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x | y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 | operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} * (x | y)",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand3 * (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x | y) * {operand3}",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (operand1 | operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z * (x | y)",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                operand3 * (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x | y) * z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (operand1 | operand2) * operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} + {operand2}",
                null,
                operand1 + operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x + {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 + operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} + y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 + operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x + y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 + operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} - {operand2}",
                null,
                operand1 - operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x - {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 - operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} - y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 - operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x - y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 - operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} * {operand2}",
                null,
                operand1 * operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x * {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 * operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} * y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 * operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x * y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 * operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} / {operand2}",
                null,
                (double)operand1 / (double)operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} / ({operand1} / {operand2})",
                null,
                (double)operand3 / (double)((double)operand1 / (double)operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} / {operand2}) / {operand3}",
                null,
                (double)((double)operand1 / (double)operand2) / (double)operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z / ({operand1} / {operand2})",
                new Dictionary<string, object> { ["z"] = operand3 },
                (double)operand3 / (double)((double)operand1 / (double)operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} / {operand2}) / z",
                new Dictionary<string, object> { ["z"] = operand3 },
                (double)((double)operand1 / (double)operand2) / (double)operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x / {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (double)operand1 / (double)operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} / (x / {operand2})",
                new Dictionary<string, object> { ["x"] = operand1 },
                (double)operand3 / (double)((double)operand1 / (double)operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x / {operand2}) / {operand3}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (double)((double)operand1 / (double)operand2) / (double)operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z / (x / {operand2})",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (double)operand3 / (double)((double)operand1 / (double)operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x / {operand2}) / z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (double)((double)operand1 / (double)operand2) / (double)operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} / y",
                new Dictionary<string, object> { ["y"] = operand2 },
                (double)operand1 / (double)operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} / ({operand1} / y)",
                new Dictionary<string, object> { ["y"] = operand2 },
                (double)operand3 / (double)((double)operand1 / (double)operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} / y) / {operand3}",
                new Dictionary<string, object> { ["y"] = operand2 },
                (double)((double)operand1 / (double)operand2) / (double)operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z / ({operand1} / y)",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (double)operand3 / (double)((double)operand1 / (double)operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} / y) / z",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (double)((double)operand1 / (double)operand2) / (double)operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x / y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (double)operand1 / (double)operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} / (x / y)",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (double)operand3 / (double)((double)operand1 / (double)operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x / y) / {operand3}",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (double)((double)operand1 / (double)operand2) / (double)operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z / (x / y)",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (double)operand3 / (double)((double)operand1 / (double)operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x / y) / z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (double)((double)operand1 / (double)operand2) / (double)operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} & {operand2}",
                null,
                operand1 & operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x & {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 & operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} & y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 & operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x & y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 & operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} | {operand2}",
                null,
                operand1 | operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x | {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 | operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} | y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 | operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x | y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 | operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} + {operand2}",
                null,
                operand1 + operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} & ({operand1} + {operand2})",
                null,
                operand3 & (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} + {operand2}) & {operand3}",
                null,
                (operand1 + operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z & ({operand1} + {operand2})",
                new Dictionary<string, object> { ["z"] = operand3 },
                operand3 & (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} + {operand2}) & z",
                new Dictionary<string, object> { ["z"] = operand3 },
                (operand1 + operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x + {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 + operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} & (x + {operand2})",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand3 & (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x + {operand2}) & {operand3}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (operand1 + operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z & (x + {operand2})",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                operand3 & (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x + {operand2}) & z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (operand1 + operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} + y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 + operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} & ({operand1} + y)",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand3 & (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} + y) & {operand3}",
                new Dictionary<string, object> { ["y"] = operand2 },
                (operand1 + operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z & ({operand1} + y)",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                operand3 & (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} + y) & z",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (operand1 + operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x + y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 + operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} & (x + y)",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand3 & (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x + y) & {operand3}",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (operand1 + operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z & (x + y)",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                operand3 & (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x + y) & z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (operand1 + operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} - {operand2}",
                null,
                operand1 - operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} & ({operand1} - {operand2})",
                null,
                operand3 & (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} - {operand2}) & {operand3}",
                null,
                (operand1 - operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z & ({operand1} - {operand2})",
                new Dictionary<string, object> { ["z"] = operand3 },
                operand3 & (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} - {operand2}) & z",
                new Dictionary<string, object> { ["z"] = operand3 },
                (operand1 - operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x - {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 - operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} & (x - {operand2})",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand3 & (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x - {operand2}) & {operand3}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (operand1 - operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z & (x - {operand2})",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                operand3 & (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x - {operand2}) & z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (operand1 - operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} - y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 - operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} & ({operand1} - y)",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand3 & (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} - y) & {operand3}",
                new Dictionary<string, object> { ["y"] = operand2 },
                (operand1 - operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z & ({operand1} - y)",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                operand3 & (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} - y) & z",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (operand1 - operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x - y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 - operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} & (x - y)",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand3 & (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x - y) & {operand3}",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (operand1 - operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z & (x - y)",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                operand3 & (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x - y) & z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (operand1 - operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} * {operand2}",
                null,
                operand1 * operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} & ({operand1} * {operand2})",
                null,
                operand3 & (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} * {operand2}) & {operand3}",
                null,
                (operand1 * operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z & ({operand1} * {operand2})",
                new Dictionary<string, object> { ["z"] = operand3 },
                operand3 & (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} * {operand2}) & z",
                new Dictionary<string, object> { ["z"] = operand3 },
                (operand1 * operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x * {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 * operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} & (x * {operand2})",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand3 & (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x * {operand2}) & {operand3}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (operand1 * operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z & (x * {operand2})",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                operand3 & (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x * {operand2}) & z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (operand1 * operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} * y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 * operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} & ({operand1} * y)",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand3 & (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} * y) & {operand3}",
                new Dictionary<string, object> { ["y"] = operand2 },
                (operand1 * operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z & ({operand1} * y)",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                operand3 & (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} * y) & z",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (operand1 * operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x * y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 * operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} & (x * y)",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand3 & (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x * y) & {operand3}",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (operand1 * operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z & (x * y)",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                operand3 & (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x * y) & z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (operand1 * operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} / {operand2}",
                null,
                (double)operand1 / (double)operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x / {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (double)operand1 / (double)operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} / y",
                new Dictionary<string, object> { ["y"] = operand2 },
                (double)operand1 / (double)operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x / y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (double)operand1 / (double)operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} & {operand2}",
                null,
                operand1 & operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} & ({operand1} & {operand2})",
                null,
                operand3 & (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} & {operand2}) & {operand3}",
                null,
                (operand1 & operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z & ({operand1} & {operand2})",
                new Dictionary<string, object> { ["z"] = operand3 },
                operand3 & (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} & {operand2}) & z",
                new Dictionary<string, object> { ["z"] = operand3 },
                (operand1 & operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x & {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 & operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} & (x & {operand2})",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand3 & (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x & {operand2}) & {operand3}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (operand1 & operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z & (x & {operand2})",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                operand3 & (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x & {operand2}) & z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (operand1 & operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} & y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 & operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} & ({operand1} & y)",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand3 & (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} & y) & {operand3}",
                new Dictionary<string, object> { ["y"] = operand2 },
                (operand1 & operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z & ({operand1} & y)",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                operand3 & (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} & y) & z",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (operand1 & operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x & y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 & operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} & (x & y)",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand3 & (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x & y) & {operand3}",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (operand1 & operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z & (x & y)",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                operand3 & (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x & y) & z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (operand1 & operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} | {operand2}",
                null,
                operand1 | operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} & ({operand1} | {operand2})",
                null,
                operand3 & (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} | {operand2}) & {operand3}",
                null,
                (operand1 | operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z & ({operand1} | {operand2})",
                new Dictionary<string, object> { ["z"] = operand3 },
                operand3 & (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} | {operand2}) & z",
                new Dictionary<string, object> { ["z"] = operand3 },
                (operand1 | operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x | {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 | operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} & (x | {operand2})",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand3 & (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x | {operand2}) & {operand3}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (operand1 | operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z & (x | {operand2})",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                operand3 & (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x | {operand2}) & z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (operand1 | operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} | y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 | operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} & ({operand1} | y)",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand3 & (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} | y) & {operand3}",
                new Dictionary<string, object> { ["y"] = operand2 },
                (operand1 | operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z & ({operand1} | y)",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                operand3 & (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} | y) & z",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (operand1 | operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x | y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 | operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} & (x | y)",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand3 & (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x | y) & {operand3}",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (operand1 | operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z & (x | y)",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                operand3 & (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x | y) & z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (operand1 | operand2) & operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} + {operand2}",
                null,
                operand1 + operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} | ({operand1} + {operand2})",
                null,
                operand3 | (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} + {operand2}) | {operand3}",
                null,
                (operand1 + operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z | ({operand1} + {operand2})",
                new Dictionary<string, object> { ["z"] = operand3 },
                operand3 | (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} + {operand2}) | z",
                new Dictionary<string, object> { ["z"] = operand3 },
                (operand1 + operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x + {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 + operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} | (x + {operand2})",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand3 | (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x + {operand2}) | {operand3}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (operand1 + operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z | (x + {operand2})",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                operand3 | (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x + {operand2}) | z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (operand1 + operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} + y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 + operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} | ({operand1} + y)",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand3 | (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} + y) | {operand3}",
                new Dictionary<string, object> { ["y"] = operand2 },
                (operand1 + operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z | ({operand1} + y)",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                operand3 | (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} + y) | z",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (operand1 + operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x + y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 + operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} | (x + y)",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand3 | (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x + y) | {operand3}",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (operand1 + operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z | (x + y)",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                operand3 | (operand1 + operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x + y) | z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (operand1 + operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} - {operand2}",
                null,
                operand1 - operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} | ({operand1} - {operand2})",
                null,
                operand3 | (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} - {operand2}) | {operand3}",
                null,
                (operand1 - operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z | ({operand1} - {operand2})",
                new Dictionary<string, object> { ["z"] = operand3 },
                operand3 | (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} - {operand2}) | z",
                new Dictionary<string, object> { ["z"] = operand3 },
                (operand1 - operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x - {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 - operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} | (x - {operand2})",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand3 | (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x - {operand2}) | {operand3}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (operand1 - operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z | (x - {operand2})",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                operand3 | (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x - {operand2}) | z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (operand1 - operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} - y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 - operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} | ({operand1} - y)",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand3 | (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} - y) | {operand3}",
                new Dictionary<string, object> { ["y"] = operand2 },
                (operand1 - operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z | ({operand1} - y)",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                operand3 | (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} - y) | z",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (operand1 - operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x - y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 - operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} | (x - y)",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand3 | (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x - y) | {operand3}",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (operand1 - operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z | (x - y)",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                operand3 | (operand1 - operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x - y) | z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (operand1 - operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} * {operand2}",
                null,
                operand1 * operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} | ({operand1} * {operand2})",
                null,
                operand3 | (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} * {operand2}) | {operand3}",
                null,
                (operand1 * operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z | ({operand1} * {operand2})",
                new Dictionary<string, object> { ["z"] = operand3 },
                operand3 | (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} * {operand2}) | z",
                new Dictionary<string, object> { ["z"] = operand3 },
                (operand1 * operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x * {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 * operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} | (x * {operand2})",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand3 | (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x * {operand2}) | {operand3}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (operand1 * operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z | (x * {operand2})",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                operand3 | (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x * {operand2}) | z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (operand1 * operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} * y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 * operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} | ({operand1} * y)",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand3 | (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} * y) | {operand3}",
                new Dictionary<string, object> { ["y"] = operand2 },
                (operand1 * operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z | ({operand1} * y)",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                operand3 | (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} * y) | z",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (operand1 * operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x * y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 * operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} | (x * y)",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand3 | (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x * y) | {operand3}",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (operand1 * operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z | (x * y)",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                operand3 | (operand1 * operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x * y) | z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (operand1 * operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} / {operand2}",
                null,
                (double)operand1 / (double)operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x / {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (double)operand1 / (double)operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} / y",
                new Dictionary<string, object> { ["y"] = operand2 },
                (double)operand1 / (double)operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x / y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (double)operand1 / (double)operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} & {operand2}",
                null,
                operand1 & operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} | ({operand1} & {operand2})",
                null,
                operand3 | (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} & {operand2}) | {operand3}",
                null,
                (operand1 & operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z | ({operand1} & {operand2})",
                new Dictionary<string, object> { ["z"] = operand3 },
                operand3 | (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} & {operand2}) | z",
                new Dictionary<string, object> { ["z"] = operand3 },
                (operand1 & operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x & {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 & operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} | (x & {operand2})",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand3 | (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x & {operand2}) | {operand3}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (operand1 & operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z | (x & {operand2})",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                operand3 | (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x & {operand2}) | z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (operand1 & operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} & y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 & operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} | ({operand1} & y)",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand3 | (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} & y) | {operand3}",
                new Dictionary<string, object> { ["y"] = operand2 },
                (operand1 & operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z | ({operand1} & y)",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                operand3 | (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} & y) | z",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (operand1 & operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x & y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 & operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} | (x & y)",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand3 | (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x & y) | {operand3}",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (operand1 & operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z | (x & y)",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                operand3 | (operand1 & operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x & y) | z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (operand1 & operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} | {operand2}",
                null,
                operand1 | operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} | ({operand1} | {operand2})",
                null,
                operand3 | (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} | {operand2}) | {operand3}",
                null,
                (operand1 | operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z | ({operand1} | {operand2})",
                new Dictionary<string, object> { ["z"] = operand3 },
                operand3 | (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} | {operand2}) | z",
                new Dictionary<string, object> { ["z"] = operand3 },
                (operand1 | operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x | {operand2}",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand1 | operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} | (x | {operand2})",
                new Dictionary<string, object> { ["x"] = operand1 },
                operand3 | (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x | {operand2}) | {operand3}",
                new Dictionary<string, object> { ["x"] = operand1 },
                (operand1 | operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z | (x | {operand2})",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                operand3 | (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x | {operand2}) | z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1 },
                (operand1 | operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand1} | y",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand1 | operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} | ({operand1} | y)",
                new Dictionary<string, object> { ["y"] = operand2 },
                operand3 | (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} | y) | {operand3}",
                new Dictionary<string, object> { ["y"] = operand2 },
                (operand1 | operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z | ({operand1} | y)",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                operand3 | (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"({operand1} | y) | z",
                new Dictionary<string, object> { ["z"] = operand3, ["y"] = operand2 },
                (operand1 | operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"x | y",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand1 | operand2
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"{operand3} | (x | y)",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                operand3 | (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x | y) | {operand3}",
                new Dictionary<string, object> { ["x"] = operand1, ["y"] = operand2 },
                (operand1 | operand2) | operand3
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"z | (x | y)",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                operand3 | (operand1 | operand2)
            });

            operand1 = DataGenerator.RandomNonNegativeInteger(limit);
            operand2 = DataGenerator.RandomNonNegativeInteger(limit);
            operand3 = DataGenerator.RandomNonNegativeInteger(limit);

            tests.Add(new object[]
            {
                $"(x | y) | z",
                new Dictionary<string, object> { ["z"] = operand3, ["x"] = operand1, ["y"] = operand2 },
                (operand1 | operand2) | operand3
            });*/

            // Return
            return tests;
        }
    }
}