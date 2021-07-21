// <copyright file="ExternalExtractorUnitTests.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Reflection;
using IX.Math;
using Xunit;

namespace IX.UnitTests.IX.Math
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
            using var eps = new ExpressionParsingService();

            eps.RegisterFunctionsAssembly(typeof(ExternalExtractorUnitTests).GetTypeInfo().Assembly);

            using ComputedExpression interpreted = eps.Interpret("1+silly+3");

            Assert.NotNull(interpreted);
            Assert.True(interpreted.RecognizedCorrectly);
            Assert.Contains(
                interpreted.GetParameterNames(),
                p => p == "stupid");
            Assert.DoesNotContain(
                interpreted.GetParameterNames(),
                p => p == "silly");
        }

        /// <summary>
        ///     Tests extractors from external libraries with ordering.
        /// </summary>
        [Fact(DisplayName = "Test extractors from external libraries with ordering")]
        public void Test2()
        {
            using var eps = new ExpressionParsingService();

            eps.RegisterFunctionsAssembly(typeof(ExternalExtractorUnitTests).GetTypeInfo().Assembly);

            using ComputedExpression interpreted = eps.Interpret("\"I am silly very much\"");

            Assert.NotNull(interpreted);
            Assert.True(interpreted.RecognizedCorrectly);
            Assert.True(interpreted.IsConstant);
            Assert.Equal(
                "I am stupid very much",
                interpreted.Compute());
        }

        /// <summary>
        ///     Tests pass-through extractors from external libraries.
        /// </summary>
        [Fact(DisplayName = "Test pass-through extractors from external libraries")]
        public void Test3()
        {
            using var eps = new ExpressionParsingService();

            eps.RegisterFunctionsAssembly(typeof(ExternalExtractorUnitTests).GetTypeInfo().Assembly);

            using ComputedExpression interpreted = eps.Interpret("1+2");

            Assert.NotNull(interpreted);
            Assert.True(interpreted.RecognizedCorrectly);
            Assert.True(interpreted.IsConstant);
            Assert.Equal(
                "1+2",
                interpreted.Compute());
        }

        /// <summary>
        ///     Tests interpreters from external libraries.
        /// </summary>
        [Fact(DisplayName = "Test interpreters from external libraries")]
        public void Test4()
        {
            using var eps = new ExpressionParsingService();

            eps.RegisterFunctionsAssembly(typeof(ExternalExtractorUnitTests).GetTypeInfo().Assembly);

            using ComputedExpression interpreted = eps.Interpret("substring(\"alabalaportocala\",bumbly dumb)");

            Assert.NotNull(interpreted);
            Assert.True(interpreted.RecognizedCorrectly);
            Assert.True(interpreted.IsConstant);
            Assert.Equal(
                "abalaportocala",
                interpreted.Compute());
        }
    }
}