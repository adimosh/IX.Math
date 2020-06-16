// <copyright file="ExternalExtractorUnitTests.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Reflection;
using IX.Math;
using Xunit;

namespace IX.UnitTests
{
    /// <summary>
    ///     A class containing tests for external library support in IX.Math.
    /// </summary>
    public class ExternalExtractorUnitTests
    {
        /// <summary>
        ///     Tests extractors from external libraries.
        /// </summary>
        [Fact(DisplayName = "Test extractors from external libraries")]
        public void Test1()
        {
            using var eps = new MathematicPortfolio(typeof(ExternalExtractorUnitTests).GetTypeInfo().Assembly);

            object interpreted = eps.Solve("1+silly+3");

            Assert.NotNull(interpreted);
            Assert.Equal("1stupid3", interpreted);
        }

        /// <summary>
        ///     Tests pass-through extractors from external libraries.
        /// </summary>
        [Fact(DisplayName = "Test pass-through extractors from external libraries")]
        public void Test3()
        {
            using var eps = new MathematicPortfolio(typeof(ExternalExtractorUnitTests).GetTypeInfo().Assembly);

            var interpreted = eps.Solve("1+2");

            Assert.NotNull(interpreted);
            Assert.Equal(
                "1+2",
                interpreted);
        }

        /// <summary>
        ///     Tests interpreters from external libraries.
        /// </summary>
        [Fact(DisplayName = "Test interpreters from external libraries")]
        public void Test4()
        {
            using var eps = new MathematicPortfolio(typeof(ExternalExtractorUnitTests).GetTypeInfo().Assembly);

            var interpreted = eps.Solve("substring(\"alabalaportocala\",bumbly dumb)");

            Assert.NotNull(interpreted);
            Assert.Equal(
                "abalaportocala",
                interpreted);
        }
    }
}