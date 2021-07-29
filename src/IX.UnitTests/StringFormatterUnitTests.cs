// <copyright file="StringFormatterUnitTests.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Globalization;
using System.Reflection;
using IX.Abstractions.Logging;
using IX.DataGeneration;
using IX.Math;
using IX.Math.Extensibility;
using IX.UnitTests.Helpers;
using IX.UnitTests.Output;
using JetBrains.Annotations;
using Xunit;
using Xunit.Abstractions;

namespace IX.UnitTests
{
    /// <summary>
    /// Tests with the string formatter.
    /// </summary>
    [CollectionDefinition("SpecificFormatter", DisableParallelization = true)]
    public class StringFormatterUnitTests : IClassFixture<CachedExpressionProviderFixture>, IDisposable
    {
        private readonly CachedExpressionProviderFixture fixture;
        private readonly IDisposable logFixture;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StringFormatterUnitTests" /> class.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="outputHelper">The output helper.</param>
        public StringFormatterUnitTests(CachedExpressionProviderFixture fixture, ITestOutputHelper outputHelper)
        {
            lock (PluginCollection.Current)
            {
                PluginCollection.Current.RegisterSpecificPlugin<SillyStringFormatter>(true);
                this.logFixture = Log.UseSpecialLogger(new OutputLoggingShim(outputHelper));
                this.fixture = fixture;
            }
        }

        /// <summary>
        /// Tests the string formatter according to the <see cref="FactAttribute" /> of this method.
        /// </summary>
        /// <param name="create">The create method.</param>
        /// <param name="dispose">The dispose method.</param>
        [Theory(DisplayName = "String formatter test with simple expression")]
        [MemberData(nameof(DataExpressions.GetFixturePatternObjects), MemberType = typeof(DataExpressions))]
        public void Test1(
            Func<CachedExpressionProviderFixture, IExpressionParsingService> create,
            Action<IExpressionParsingService> dispose)
        {
            // Arrange
            using var eps = new FixtureCreateDisposePatternHelper(this.fixture, create, dispose);

            Log.Info("This is a logging test.");

            int comparisonValue = DataGenerator.RandomNonNegativeInteger();
            string expression = $"\"The number is \" + {comparisonValue}";
            string expectedResult = $"The number is 0x{comparisonValue:x8}";

            // Act
            using var computedExpression = eps.Service.Interpret(expression);
            var result = computedExpression.Compute();

            // Assert
            Assert.Equal(
                expectedResult,
                Assert.IsType<string>(result));
        }

        /// <summary>
        /// Tests the string formatter according to the <see cref="FactAttribute"/> of this method.
        /// </summary>
        /// <param name="create">The create method.</param>
        /// <param name="dispose">The dispose method.</param>
        [Theory(DisplayName = "String formatter test with parameter expression")]
        [MemberData(nameof(DataExpressions.GetFixturePatternObjects), MemberType = typeof(DataExpressions))]
        public void Test2(
            Func<CachedExpressionProviderFixture, IExpressionParsingService> create,
            Action<IExpressionParsingService> dispose)
        {
            // Arrange
            using var eps = new FixtureCreateDisposePatternHelper(this.fixture, create, dispose);

            int comparisonValue = DataGenerator.RandomNonNegativeInteger();
            string expression = $"\"The number is \" + x";
            string expectedResult = $"The number is 0x{comparisonValue:x8}";

            // Act
            using var computedExpression = eps.Service.Interpret(expression);
            var result = computedExpression.Compute(comparisonValue);

            // Assert
            Assert.Equal(expectedResult, Assert.IsType<string>(result));
        }

        /// <summary>
        /// Tests the string formatter according to the <see cref="FactAttribute"/> of this method.
        /// </summary>
        /// <param name="create">The create method.</param>
        /// <param name="dispose">The dispose method.</param>
        [Theory(DisplayName = "String formatter test with complex expression")]
        [MemberData(nameof(DataExpressions.GetFixturePatternObjects), MemberType = typeof(DataExpressions))]
        public void Test3(
            Func<CachedExpressionProviderFixture, IExpressionParsingService> create,
            Action<IExpressionParsingService> dispose)
        {
            // Arrange
            using var eps = new FixtureCreateDisposePatternHelper(this.fixture, create, dispose);

            int comparisonValue1 = DataGenerator.RandomNonNegativeInteger();
            int comparisonValue2 = DataGenerator.RandomNonNegativeInteger();
            string expression = $"\"The number is \" + ({comparisonValue1} + {comparisonValue2})";
            string expectedResult = $"The number is 0x{comparisonValue1 + comparisonValue2:x8}";

            // Act
            using var computedExpression = eps.Service.Interpret(expression);
            var result = computedExpression.Compute();

            // Assert
            Assert.Equal(expectedResult, Assert.IsType<string>(result));
        }

        /// <summary>
        /// Tests the string formatter according to the <see cref="FactAttribute"/> of this method.
        /// </summary>
        /// <param name="create">The create method.</param>
        /// <param name="dispose">The dispose method.</param>
        [Theory(DisplayName = "String formatter test with complex parameter expression")]
        [MemberData(nameof(DataExpressions.GetFixturePatternObjects), MemberType = typeof(DataExpressions))]
        public void Test4(
            Func<CachedExpressionProviderFixture, IExpressionParsingService> create,
            Action<IExpressionParsingService> dispose)
        {
            // Arrange
            using var eps = new FixtureCreateDisposePatternHelper(this.fixture, create, dispose);

            long comparisonValue1 = DataGenerator.RandomNonNegativeInteger();
            long comparisonValue2 = DataGenerator.RandomNonNegativeInteger();
            string expression = "\"The number is \" + (x + y)";
            string expectedResult = $"The number is 0x{comparisonValue1 + comparisonValue2:x8}";

            // Act
            using var computedExpression = eps.Service.Interpret(expression);
            var result = computedExpression.Compute(comparisonValue1, comparisonValue2);

            // Assert
            Assert.Equal(expectedResult, Assert.IsType<string>(result));
        }

        /// <summary>
        /// Tests the string formatter according to the <see cref="FactAttribute"/> of this method.
        /// </summary>
        /// <param name="create">The create method.</param>
        /// <param name="dispose">The dispose method.</param>
        [Theory(DisplayName = "String formatter test with coercion expression")]
        [MemberData(nameof(DataExpressions.GetFixturePatternObjects), MemberType = typeof(DataExpressions))]
        public void Test5(
            Func<CachedExpressionProviderFixture, IExpressionParsingService> create,
            Action<IExpressionParsingService> dispose)
        {
            // Arrange
            using var eps = new FixtureCreateDisposePatternHelper(this.fixture, create, dispose);

            int comparisonValue = DataGenerator.RandomNonNegativeInteger();
            string expression = $"strlen(\"The number is \" + {comparisonValue})";
            long expectedResult = 24;

            // Act
            using var computedExpression = eps.Service.Interpret(expression);
            var result = computedExpression.Compute();

            // Assert
            Assert.Equal(expectedResult, Assert.IsType<long>(result));
        }

        /// <summary>
        /// Tests the string formatter according to the <see cref="FactAttribute"/> of this method.
        /// </summary>
        /// <param name="create">The create method.</param>
        /// <param name="dispose">The dispose method.</param>
        [Theory(DisplayName = "String formatter test with complex expression and escape")]
        [MemberData(nameof(DataExpressions.GetFixturePatternObjects), MemberType = typeof(DataExpressions))]
        public void Test6(
            Func<CachedExpressionProviderFixture, IExpressionParsingService> create,
            Action<IExpressionParsingService> dispose)
        {
            // Arrange
            using var eps = new FixtureCreateDisposePatternHelper(this.fixture, create, dispose);
            eps.Service.RegisterFunctionsAssembly(Assembly.GetExecutingAssembly());

            int comparisonValue1 = DataGenerator.RandomNonNegativeInteger();
            int comparisonValue2 = DataGenerator.RandomNonNegativeInteger();
            string expression = $"\"The \\\"alabalaportocala\\\" number is \" + ({comparisonValue1} + {comparisonValue2})";
            string expectedResult = $"The \\\"alabalaportocala\\\" number is 0x{comparisonValue1 + comparisonValue2:x8}";

            // Act
            using var computedExpression = eps.Service.Interpret(expression);
            var result = computedExpression.Compute();

            // Assert
            Assert.Equal(expectedResult, Assert.IsType<string>(result));
        }

        /// <summary>Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.</summary>
        public void Dispose()
        {
            lock (PluginCollection.Current)
            {
                PluginCollection.Current.Reset();
                PluginCollection.Current.RegisterCurrentAssembly();
            }

            this.logFixture?.Dispose();
        }

        /// <summary>
        /// The test formatter.
        /// </summary>
        [UsedImplicitly]
        public class SillyStringFormatter : IStringFormatter
        {
            /// <summary>
            /// Implements the parser.
            /// </summary>
            /// <typeparam name="T">The type of data to parse into.</typeparam>
            /// <param name="data">The data to parse.</param>
            /// <returns>A success state, as well as the parse data.</returns>
            public (bool Success, string ParsedData) ParseIntoString<T>(T data) => data switch
            {
                long integralNumber => (true, "0x" + integralNumber.ToString("x8", CultureInfo.CurrentCulture)),
                _ => (false, default),
            };
        }
    }
}