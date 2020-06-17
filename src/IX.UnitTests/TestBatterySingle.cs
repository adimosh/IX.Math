// <copyright file="TestBatterySingle.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using IX.Math;
using IX.UnitTests.ContextStructure;
using Xunit;

namespace IX.UnitTests
{
    /// <summary>
    ///     Tests computed expressions.
    /// </summary>
    public class TestBatterySingle : TestBatteryBase
    {
        /// <summary>
        /// Runs a test in the battery once for a set of data.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="expectedResult">The expected result.</param>
        /// <param name="creator">The creator.</param>
        /// <param name="disposer">The disposer.</param>
        /// <param name="solver">The solver.</param>
        [Theory(DisplayName = "Single test battery")]
        [MemberData(nameof(TestContextStructure.GenerateTestData), MemberType = typeof(TestContextStructure))]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
                    "IDisposableAnalyzers.Correctness",
                    "IDISP001:Dispose created.",
                    Justification = "It's being disposed, but the analyzer cannot tell.")]
        public void SingleTest(
                    string expression,
                    Dictionary<string, object> parameters,
                    object expectedResult,
                    Func<MathematicPortfolio> creator,
                    Action<MathematicPortfolio> disposer,
                    Func<MathematicPortfolio, string, Dictionary<string, object>, object> solver)
        {
            MathematicPortfolio portfolio = null;
            object result = null;
            try
            {
                portfolio = creator?.Invoke();

                result = solver?.Invoke(
                    portfolio,
                    expression,
                    parameters);
            }
            catch (DivideByZeroException)
            {
                // We don't do anything - this is entirely possible in random data, and is acceptable
            }
            finally
            {
                disposer?.Invoke(portfolio);
            }

            this.AssertResults(
                in expectedResult,
                in result);
        }
    }
}