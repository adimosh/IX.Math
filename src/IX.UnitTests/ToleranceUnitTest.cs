// <copyright file="ToleranceUnitTest.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using IX.Math;
using Moq;
using Xunit;

namespace IX.UnitTests
{
    /// <summary>
    ///     Tests computed expressions.
    /// </summary>
    public class ToleranceUnitTest
    {
        /// <summary>
        ///     Provides the data for theory.
        /// </summary>
        /// <returns>Theory data.</returns>
        public static object[][] ProvideDataForTheory() => new[]
        {
            // Equation
            new object[]
            {
                "x=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1.7D,
                    ["y"] = 1.9D,
                },
                true,
                new ComparisonTolerance(toleranceRangeLowerBound: 0.2, toleranceRangeUpperBound: 0.2),
            },
            new object[]
            {
                "1.7=1.9",
                null,
                true,
                new ComparisonTolerance(toleranceRangeLowerBound: 0.2, toleranceRangeUpperBound: 0.2),
            },
            new object[]
            {
                "x=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1.7D,
                    ["y"] = 1.9D,
                },
                false,
                new ComparisonTolerance(toleranceRangeLowerBound: 0.1, toleranceRangeUpperBound: 0.1),
            },
            new object[]
            {
                "1.7=1.9",
                null,
                false,
                new ComparisonTolerance(toleranceRangeLowerBound: 0.1, toleranceRangeUpperBound: 0.1),
            },
            new object[]
            {
                "x=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1.7D,
                    ["y"] = 1.9D,
                },
                true,
                new ComparisonTolerance(toleranceRangeLowerBound: 0.2),
            },
            new object[]
            {
                "1.7=1.9",
                null,
                true,
                new ComparisonTolerance(toleranceRangeLowerBound: 0.2),
            },
            new object[]
            {
                "x=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1.7D,
                    ["y"] = 1.9D,
                },
                false,
                new ComparisonTolerance(toleranceRangeUpperBound: 0.2),
            },
            new object[]
            {
                "1.7=1.9",
                null,
                false,
                new ComparisonTolerance(toleranceRangeUpperBound: 0.2),
            },
            new object[]
            {
                "x=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1.7D,
                    ["y"] = 1.9D,
                },
                true,
                new ComparisonTolerance(integerToleranceRangeLowerBound: 1, integerToleranceRangeUpperBound: 1),
            },
            new object[]
            {
                "1.7=1.9",
                null,
                true,
                new ComparisonTolerance(integerToleranceRangeLowerBound: 1, integerToleranceRangeUpperBound: 1),
            },
            new object[]
            {
                "x=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1.7D,
                    ["y"] = 1.9D,
                },
                true,
                new ComparisonTolerance(integerToleranceRangeLowerBound: 1),
            },
            new object[]
            {
                "1.7=1.9",
                null,
                true,
                new ComparisonTolerance(integerToleranceRangeLowerBound: 1),
            },
            new object[]
            {
                "x=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1.7D,
                    ["y"] = 1.9D,
                },
                false,
                new ComparisonTolerance(integerToleranceRangeUpperBound: 1),
            },
            new object[]
            {
                "1.7=1.9",
                null,
                false,
                new ComparisonTolerance(integerToleranceRangeUpperBound: 1),
            },
            new object[]
            {
                "x=y",
                new Dictionary<string, object>
                {
                    ["x"] = 0.7D,
                    ["y"] = 1.9D,
                },
                false,
                new ComparisonTolerance(integerToleranceRangeLowerBound: 1, integerToleranceRangeUpperBound: 1),
            },
            new object[]
            {
                "0.7=1.9",
                null,
                false,
                new ComparisonTolerance(integerToleranceRangeLowerBound: 1, integerToleranceRangeUpperBound: 1),
            },
            new object[]
            {
                "x=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1.7D,
                    ["y"] = 2.9D,
                },
                false,
                new ComparisonTolerance(integerToleranceRangeLowerBound: 1, integerToleranceRangeUpperBound: 1),
            },
            new object[]
            {
                "1.7=2.9",
                null,
                false,
                new ComparisonTolerance(integerToleranceRangeLowerBound: 1, integerToleranceRangeUpperBound: 1),
            },
            new object[]
            {
                "x=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1D,
                    ["y"] = 1.5D,
                },
                false,
                new ComparisonTolerance(proportionalTolerance: 0.1),
            },
            new object[]
            {
                "1=1.5",
                null,
                false,
                new ComparisonTolerance(proportionalTolerance: 0.1),
            },
            new object[]
            {
                "x=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1D,
                    ["y"] = 1.5D,
                },
                true,
                new ComparisonTolerance(proportionalTolerance: 0.5),
            },
            new object[]
            {
                "1=1.5",
                null,
                true,
                new ComparisonTolerance(proportionalTolerance: 0.5),
            },
            new object[]
            {
                "x=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1D,
                    ["y"] = 1.5D,
                },
                false,
                new ComparisonTolerance(proportionalTolerance: 1.1),
            },
            new object[]
            {
                "1=1.5",
                null,
                false,
                new ComparisonTolerance(proportionalTolerance: 1.1),
            },
            new object[]
            {
                "x=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1D,
                    ["y"] = 1.5D,
                },
                true,
                new ComparisonTolerance(proportionalTolerance: 2),
            },
            new object[]
            {
                "1=1.5",
                null,
                true,
                new ComparisonTolerance(proportionalTolerance: 2),
            },

            // Negative equation
            new object[]
            {
                "x!=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1.7D,
                    ["y"] = 1.9D,
                },
                false,
                new ComparisonTolerance(toleranceRangeLowerBound: 0.2, toleranceRangeUpperBound: 0.2),
            },
            new object[]
            {
                "1.7!=1.9",
                null,
                false,
                new ComparisonTolerance(toleranceRangeLowerBound: 0.2, toleranceRangeUpperBound: 0.2),
            },
            new object[]
            {
                "x!=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1.7D,
                    ["y"] = 1.9D,
                },
                true,
                new ComparisonTolerance(toleranceRangeLowerBound: 0.1, toleranceRangeUpperBound: 0.1),
            },
            new object[]
            {
                "1.7!=1.9",
                null,
                true,
                new ComparisonTolerance(toleranceRangeLowerBound: 0.1, toleranceRangeUpperBound: 0.1),
            },
            new object[]
            {
                "x!=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1.7D,
                    ["y"] = 1.9D,
                },
                false,
                new ComparisonTolerance(toleranceRangeLowerBound: 0.2),
            },
            new object[]
            {
                "1.7!=1.9",
                null,
                false,
                new ComparisonTolerance(toleranceRangeLowerBound: 0.2),
            },
            new object[]
            {
                "x!=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1.7D,
                    ["y"] = 1.9D,
                },
                true,
                new ComparisonTolerance(toleranceRangeUpperBound: 0.2),
            },
            new object[]
            {
                "1.7!=1.9",
                null,
                true,
                new ComparisonTolerance(toleranceRangeUpperBound: 0.2),
            },
            new object[]
            {
                "x!=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1.7D,
                    ["y"] = 1.9D,
                },
                false,
                new ComparisonTolerance(integerToleranceRangeLowerBound: 1, integerToleranceRangeUpperBound: 1),
            },
            new object[]
            {
                "1.7!=1.9",
                null,
                false,
                new ComparisonTolerance(integerToleranceRangeLowerBound: 1, integerToleranceRangeUpperBound: 1),
            },
            new object[]
            {
                "x!=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1.7D,
                    ["y"] = 1.9D,
                },
                false,
                new ComparisonTolerance(integerToleranceRangeLowerBound: 1),
            },
            new object[]
            {
                "1.7!=1.9",
                null,
                false,
                new ComparisonTolerance(integerToleranceRangeLowerBound: 1),
            },
            new object[]
            {
                "x!=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1.7D,
                    ["y"] = 1.9D,
                },
                true,
                new ComparisonTolerance(integerToleranceRangeUpperBound: 1),
            },
            new object[]
            {
                "1.7!=1.9",
                null,
                true,
                new ComparisonTolerance(integerToleranceRangeUpperBound: 1),
            },
            new object[]
            {
                "x!=y",
                new Dictionary<string, object>
                {
                    ["x"] = 0.7D,
                    ["y"] = 1.9D,
                },
                true,
                new ComparisonTolerance(integerToleranceRangeLowerBound: 1, integerToleranceRangeUpperBound: 1),
            },
            new object[]
            {
                "0.7!=1.9",
                null,
                true,
                new ComparisonTolerance(integerToleranceRangeLowerBound: 1, integerToleranceRangeUpperBound: 1),
            },
            new object[]
            {
                "x!=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1.7D,
                    ["y"] = 2.9D,
                },
                true,
                new ComparisonTolerance(integerToleranceRangeLowerBound: 1, integerToleranceRangeUpperBound: 1),
            },
            new object[]
            {
                "1.7!=2.9",
                null,
                true,
                new ComparisonTolerance(integerToleranceRangeLowerBound: 1, integerToleranceRangeUpperBound: 1),
            },
            new object[]
            {
                "x!=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1D,
                    ["y"] = 1.5D,
                },
                true,
                new ComparisonTolerance(proportionalTolerance: 0.1),
            },
            new object[]
            {
                "1!=1.5",
                null,
                true,
                new ComparisonTolerance(proportionalTolerance: 0.1),
            },
            new object[]
            {
                "x!=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1D,
                    ["y"] = 1.5D,
                },
                false,
                new ComparisonTolerance(proportionalTolerance: 0.5),
            },
            new object[]
            {
                "1!=1.5",
                null,
                false,
                new ComparisonTolerance(proportionalTolerance: 0.5),
            },
            new object[]
            {
                "x!=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1D,
                    ["y"] = 1.5D,
                },
                true,
                new ComparisonTolerance(proportionalTolerance: 1.1),
            },
            new object[]
            {
                "1!=1.5",
                null,
                true,
                new ComparisonTolerance(proportionalTolerance: 1.1),
            },
            new object[]
            {
                "x!=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1D,
                    ["y"] = 1.5D,
                },
                false,
                new ComparisonTolerance(proportionalTolerance: 2),
            },
            new object[]
            {
                "1!=1.5",
                null,
                false,
                new ComparisonTolerance(proportionalTolerance: 2),
            },

            // Greater than
            new object[]
            {
                "x>y",
                new Dictionary<string, object>
                {
                    ["x"] = 1D,
                    ["y"] = 1.5D,
                },
                true,
                new ComparisonTolerance(integerToleranceRangeLowerBound: 1L),
            },
            new object[]
            {
                "1>1.5",
                null,
                true,
                new ComparisonTolerance(integerToleranceRangeLowerBound: 1L),
            },
            new object[]
            {
                "x>y",
                new Dictionary<string, object>
                {
                    ["x"] = 1D,
                    ["y"] = 1.5D,
                },
                true,
                new ComparisonTolerance(toleranceRangeLowerBound: 1D),
            },
            new object[]
            {
                "1>1.5",
                null,
                true,
                new ComparisonTolerance(toleranceRangeLowerBound: 1D),
            },
            new object[]
            {
                "x>y",
                new Dictionary<string, object>
                {
                    ["x"] = 1D,
                    ["y"] = 1.5D,
                },
                false,
                new ComparisonTolerance(toleranceRangeLowerBound: 0.5D),
            },
            new object[]
            {
                "1>1.5",
                null,
                false,
                new ComparisonTolerance(toleranceRangeLowerBound: 0.5D),
            },
            new object[]
            {
                "x>y",
                new Dictionary<string, object>
                {
                    ["x"] = 1D,
                    ["y"] = 1.5D,
                },
                false,
                new ComparisonTolerance(proportionalTolerance: 0.2D),
            },
            new object[]
            {
                "1>1.5",
                null,
                false,
                new ComparisonTolerance(proportionalTolerance: 0.2D),
            },
            new object[]
            {
                "x>y",
                new Dictionary<string, object>
                {
                    ["x"] = 1D,
                    ["y"] = 1.5D,
                },
                true,
                new ComparisonTolerance(proportionalTolerance: 0.5D),
            },
            new object[]
            {
                "1>1.5",
                null,
                true,
                new ComparisonTolerance(proportionalTolerance: 0.5D),
            },
            new object[]
            {
                "x>y",
                new Dictionary<string, object>
                {
                    ["x"] = 1D,
                    ["y"] = 1.5D,
                },
                false,
                new ComparisonTolerance(proportionalTolerance: 1.1D),
            },
            new object[]
            {
                "1>1.5",
                null,
                false,
                new ComparisonTolerance(proportionalTolerance: 1.1D),
            },
            new object[]
            {
                "x>y",
                new Dictionary<string, object>
                {
                    ["x"] = 1D,
                    ["y"] = 1.5D,
                },
                true,
                new ComparisonTolerance(proportionalTolerance: 2D),
            },
            new object[]
            {
                "1>1.5",
                null,
                true,
                new ComparisonTolerance(proportionalTolerance: 2D),
            },

            // Greater than or equal
            new object[]
            {
                "x>=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1D,
                    ["y"] = 1.5D,
                },
                true,
                new ComparisonTolerance(integerToleranceRangeLowerBound: 1L),
            },
            new object[]
            {
                "1>=1.5",
                null,
                true,
                new ComparisonTolerance(integerToleranceRangeLowerBound: 1L),
            },
            new object[]
            {
                "x>=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1D,
                    ["y"] = 1.5D,
                },
                true,
                new ComparisonTolerance(toleranceRangeLowerBound: 1D),
            },
            new object[]
            {
                "1>=1.5",
                null,
                true,
                new ComparisonTolerance(toleranceRangeLowerBound: 1D),
            },
            new object[]
            {
                "x>=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1D,
                    ["y"] = 1.5D,
                },
                true,
                new ComparisonTolerance(toleranceRangeLowerBound: 0.5D),
            },
            new object[]
            {
                "1>=1.5",
                null,
                true,
                new ComparisonTolerance(toleranceRangeLowerBound: 0.5D),
            },
            new object[]
            {
                "x>=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1D,
                    ["y"] = 1.5D,
                },
                false,
                new ComparisonTolerance(proportionalTolerance: 0.2D),
            },
            new object[]
            {
                "1>=1.5",
                null,
                false,
                new ComparisonTolerance(proportionalTolerance: 0.2D),
            },
            new object[]
            {
                "x>=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1D,
                    ["y"] = 1.5D,
                },
                true,
                new ComparisonTolerance(proportionalTolerance: 0.5D),
            },
            new object[]
            {
                "1>=1.5",
                null,
                true,
                new ComparisonTolerance(proportionalTolerance: 0.5D),
            },
            new object[]
            {
                "x>=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1D,
                    ["y"] = 1.5D,
                },
                false,
                new ComparisonTolerance(proportionalTolerance: 1.1D),
            },
            new object[]
            {
                "1>=1.5",
                null,
                false,
                new ComparisonTolerance(proportionalTolerance: 1.1D),
            },
            new object[]
            {
                "x>=y",
                new Dictionary<string, object>
                {
                    ["x"] = 1D,
                    ["y"] = 1.5D,
                },
                true,
                new ComparisonTolerance(proportionalTolerance: 2D),
            },
            new object[]
            {
                "1>=1.5",
                null,
                true,
                new ComparisonTolerance(proportionalTolerance: 2D),
            },

            // Less than
            new object[]
            {
                "y<x",
                new Dictionary<string, object>
                {
                    ["y"] = 1.5D,
                    ["x"] = 1D,
                },
                true,
                new ComparisonTolerance(integerToleranceRangeUpperBound: 1L),
            },
            new object[]
            {
                "1.5<1",
                null,
                true,
                new ComparisonTolerance(integerToleranceRangeUpperBound: 1L),
            },
            new object[]
            {
                "y<x",
                new Dictionary<string, object>
                {
                    ["y"] = 1.5D,
                    ["x"] = 1D,
                },
                true,
                new ComparisonTolerance(toleranceRangeUpperBound: 1D),
            },
            new object[]
            {
                "1.5<1",
                null,
                true,
                new ComparisonTolerance(toleranceRangeUpperBound: 1D),
            },
            new object[]
            {
                "y<x",
                new Dictionary<string, object>
                {
                    ["y"] = 1.5D,
                    ["x"] = 1D,
                },
                false,
                new ComparisonTolerance(toleranceRangeUpperBound: 0.5D),
            },
            new object[]
            {
                "1.5<1",
                null,
                false,
                new ComparisonTolerance(toleranceRangeUpperBound: 0.5D),
            },
            new object[]
            {
                "y<x",
                new Dictionary<string, object>
                {
                    ["y"] = 1.5D,
                    ["x"] = 1D,
                },
                false,
                new ComparisonTolerance(proportionalTolerance: 0.2D),
            },
            new object[]
            {
                "1.5<1",
                null,
                false,
                new ComparisonTolerance(proportionalTolerance: 0.2D),
            },
            new object[]
            {
                "y<x",
                new Dictionary<string, object>
                {
                    ["y"] = 1.5D,
                    ["x"] = 1D,
                },
                false,
                new ComparisonTolerance(proportionalTolerance: 0.5D),
            },
            new object[]
            {
                "1.5<1",
                null,
                false,
                new ComparisonTolerance(proportionalTolerance: 0.5D),
            },
            new object[]
            {
                "y<x",
                new Dictionary<string, object>
                {
                    ["y"] = 1.5D,
                    ["x"] = 1D,
                },
                false,
                new ComparisonTolerance(proportionalTolerance: 1.1D),
            },
            new object[]
            {
                "1.5<1",
                null,
                false,
                new ComparisonTolerance(proportionalTolerance: 1.1D),
            },
            new object[]
            {
                "y<x",
                new Dictionary<string, object>
                {
                    ["y"] = 1.5D,
                    ["x"] = 1D,
                },
                true,
                new ComparisonTolerance(proportionalTolerance: 2D),
            },
            new object[]
            {
                "1.5<1",
                null,
                true,
                new ComparisonTolerance(proportionalTolerance: 2D),
            },

            // Less than or equal
            new object[]
            {
                "y<=x",
                new Dictionary<string, object>
                {
                    ["y"] = 1.5D,
                    ["x"] = 1D,
                },
                true,
                new ComparisonTolerance(integerToleranceRangeUpperBound: 1L),
            },
            new object[]
            {
                "1.5<=1",
                null,
                true,
                new ComparisonTolerance(integerToleranceRangeUpperBound: 1L),
            },
            new object[]
            {
                "y<=x",
                new Dictionary<string, object>
                {
                    ["y"] = 1.5D,
                    ["x"] = 1D,
                },
                true,
                new ComparisonTolerance(toleranceRangeUpperBound: 1D),
            },
            new object[]
            {
                "1.5<=1",
                null,
                true,
                new ComparisonTolerance(toleranceRangeUpperBound: 1D),
            },
            new object[]
            {
                "y<=x",
                new Dictionary<string, object>
                {
                    ["y"] = 1.5D,
                    ["x"] = 1D,
                },
                true,
                new ComparisonTolerance(toleranceRangeUpperBound: 0.5D),
            },
            new object[]
            {
                "1.5<=1",
                null,
                true,
                new ComparisonTolerance(toleranceRangeUpperBound: 0.5D),
            },
            new object[]
            {
                "y<=x",
                new Dictionary<string, object>
                {
                    ["y"] = 1.5D,
                    ["x"] = 1D,
                },
                false,
                new ComparisonTolerance(proportionalTolerance: 0.2D),
            },
            new object[]
            {
                "1.5<=1",
                null,
                false,
                new ComparisonTolerance(proportionalTolerance: 0.2D),
            },
            new object[]
            {
                "y<=x",
                new Dictionary<string, object>
                {
                    ["y"] = 1.5D,
                    ["x"] = 1D,
                },
                true,
                new ComparisonTolerance(proportionalTolerance: 0.5D),
            },
            new object[]
            {
                "1.5<=1",
                null,
                true,
                new ComparisonTolerance(proportionalTolerance: 0.5D),
            },
            new object[]
            {
                "y<=x",
                new Dictionary<string, object>
                {
                    ["y"] = 1.5D,
                    ["x"] = 1D,
                },
                false,
                new ComparisonTolerance(proportionalTolerance: 1.1D),
            },
            new object[]
            {
                "1.5<=1",
                null,
                false,
                new ComparisonTolerance(proportionalTolerance: 1.1D),
            },
            new object[]
            {
                "y<=x",
                new Dictionary<string, object>
                {
                    ["y"] = 1.5D,
                    ["x"] = 1D,
                },
                true,
                new ComparisonTolerance(proportionalTolerance: 2D),
            },
            new object[]
            {
                "1.5<=1",
                null,
                true,
                new ComparisonTolerance(proportionalTolerance: 2D),
            },
            new object[]
            {
                "x=y",
                new Dictionary<string, object>
                {
                    ["x"] = "1",
                    ["y"] = 2,
                },
                true,
                new ComparisonTolerance(integerToleranceRangeLowerBound: 1, integerToleranceRangeUpperBound: 1),
            },
        };

        /// <summary>
        /// Tests the computed expression with parameters.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="expectedResult">The expected result.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <exception cref="InvalidOperationException">No computed expression was generated.</exception>
        [Theory(DisplayName = "TEPSPara")]
        [MemberData(nameof(ProvideDataForTheory))]
        public void ComputedExpressionWithParameters(
            string expression,
            Dictionary<string, object> parameters,
            object expectedResult,
            ComparisonTolerance tolerance)
        {
            using var service = new MathematicPortfolio();

            object result = service.Solve(expression, in tolerance, parameters?.Select(p => p.Value).ToArray() ?? new object[0]);

            Assert.Equal(
                expectedResult,
                result);
        }

        /// <summary>
        /// Tests a computed expression with finder.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="expectedResult">The expected result.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <exception cref="InvalidOperationException">No computed expression was generated.</exception>
        [Theory(DisplayName = "TEPSFindr")]
        [MemberData(nameof(ProvideDataForTheory))]
        public void ComputedExpressionWithFinder(
            string expression,
            Dictionary<string, object> parameters,
            object expectedResult,
            ComparisonTolerance tolerance)
        {
            using var service = new MathematicPortfolio();

            var finder = new Mock<IDataFinder>(MockBehavior.Loose);

            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> parameter in parameters)
                {
                    var key = parameter.Key;
                    object value = parameter.Value;
                    finder.Setup(
                        p => p.TryGetData(
                            key,
                            out value)).Returns(true);
                }
            }

            object result = service.Solve(expression, in tolerance, finder.Object);

            Assert.Equal(
                expectedResult,
                result);
        }

        /// <summary>
        /// Tests a computed expression with finder.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="expectedResult">The expected result.</param>
        /// <param name="tolerance">The tolerance.</param>
        /// <exception cref="InvalidOperationException">No computed expression was generated.</exception>
        [Theory(DisplayName = "TEPSFindrFunc")]
        [MemberData(nameof(ProvideDataForTheory))]
        public void ComputedExpressionWithFunctionFinder(
            string expression,
            Dictionary<string, object> parameters,
            object expectedResult,
            ComparisonTolerance tolerance)
        {
            using var service = new MathematicPortfolio();

            var finder = new Mock<IDataFinder>(MockBehavior.Loose);

            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> parameter in parameters)
                {
                    var key = parameter.Key;
                    object value = this.GenerateFuncOutOfParameterValue(parameter.Value);
                    finder.Setup(
                        p => p.TryGetData(
                            key,
                            out value)).Returns(true);
                }
            }

            object result = service.Solve(expression, in tolerance, finder.Object);

            Assert.Equal(
                expectedResult,
                result);
        }

        private object GenerateFuncOutOfParameterValue(object tempParameter)
        {
            switch (tempParameter)
            {
                case byte convertedValue:
                    return new Func<byte>(() => convertedValue);
                case sbyte convertedValue:
                    return new Func<sbyte>(() => convertedValue);
                case short convertedValue:
                    return new Func<short>(() => convertedValue);
                case ushort convertedValue:
                    return new Func<ushort>(() => convertedValue);
                case int convertedValue:
                    return new Func<int>(() => convertedValue);
                case uint convertedValue:
                    return new Func<uint>(() => convertedValue);
                case long convertedValue:
                    return new Func<long>(() => convertedValue);
                case ulong convertedValue:
                    return new Func<ulong>(() => convertedValue);
                case float convertedValue:
                    return new Func<float>(() => convertedValue);
                case double convertedValue:
                    return new Func<double>(() => convertedValue);
                case byte[] convertedValue:
                    return new Func<byte[]>(() => convertedValue);
                case string convertedValue:
                    return new Func<string>(() => convertedValue);
                case bool convertedValue:
                    return new Func<bool>(() => convertedValue);
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}