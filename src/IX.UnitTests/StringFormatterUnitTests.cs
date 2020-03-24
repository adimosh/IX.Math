// <copyright file="StringFormatterUnitTests.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Globalization;
using IX.Math;
using IX.Math.Extensibility;
using IX.StandardExtensions.TestUtils;
using Xunit;

namespace IX.UnitTests
{
    /// <summary>
    /// Tests with the string formatter.
    /// </summary>
    public class StringFormatterUnitTests
    {
        /// <summary>
        /// Tests the string formatter according to the <see cref="FactAttribute"/> of this method.
        /// </summary>
        [Fact(DisplayName = "String formatter test with simple expression")]
        public void Test1()
        {
            // Arrange
            using ExpressionParsingService eps = new ExpressionParsingService();
            eps.RegisterTypeFormatter(new SillyStringFormatter());
            int comparisonValue = DataGenerator.RandomNonNegativeInteger();
            string expression = $"\"The number is \" + {comparisonValue}";
            string expectedResult = $"The number is 0x{comparisonValue:x8}";

            // Act
            using var computedExpression = eps.Interpret(expression);
            var result = computedExpression.Compute();

            // Assert
            Assert.Equal(expectedResult, Assert.IsType<string>(result));
        }

        /// <summary>
        /// Tests the string formatter according to the <see cref="FactAttribute"/> of this method.
        /// </summary>
        [Fact(DisplayName = "String formatter test with parameter expression")]
        public void Test2()
        {
            // Arrange
            using ExpressionParsingService eps = new ExpressionParsingService();
            eps.RegisterTypeFormatter(new SillyStringFormatter());
            int comparisonValue = DataGenerator.RandomNonNegativeInteger();
            string expression = $"\"The number is \" + x";
            string expectedResult = $"The number is 0x{comparisonValue:x8}";

            // Act
            using var computedExpression = eps.Interpret(expression);
            var result = computedExpression.Compute(comparisonValue);

            // Assert
            Assert.Equal(expectedResult, Assert.IsType<string>(result));
        }

        /// <summary>
        /// Tests the string formatter according to the <see cref="FactAttribute"/> of this method.
        /// </summary>
        [Fact(DisplayName = "String formatter test with complex expression")]
        public void Test3()
        {
            // Arrange
            using ExpressionParsingService eps = new ExpressionParsingService();
            eps.RegisterTypeFormatter(new SillyStringFormatter());
            int comparisonValue1 = DataGenerator.RandomNonNegativeInteger();
            int comparisonValue2 = DataGenerator.RandomNonNegativeInteger();
            string expression = $"\"The number is \" + ({comparisonValue1} + {comparisonValue2})";
            string expectedResult = $"The number is 0x{comparisonValue1 + comparisonValue2:x8}";

            // Act
            using var computedExpression = eps.Interpret(expression);
            var result = computedExpression.Compute();

            // Assert
            Assert.Equal(expectedResult, Assert.IsType<string>(result));
        }

        /// <summary>
        /// Tests the string formatter according to the <see cref="FactAttribute"/> of this method.
        /// </summary>
        [Fact(DisplayName = "String formatter test with complex parameter expression")]
        public void Test4()
        {
            // Arrange
            using ExpressionParsingService eps = new ExpressionParsingService();
            eps.RegisterTypeFormatter(new SillyStringFormatter());
            long comparisonValue1 = DataGenerator.RandomNonNegativeInteger();
            long comparisonValue2 = DataGenerator.RandomNonNegativeInteger();
            string expression = $"\"The number is \" + (x + y)";
            string expectedResult = $"The number is 0x{comparisonValue1 + comparisonValue2:x8}";

            // Act
            using var computedExpression = eps.Interpret(expression);
            var result = computedExpression.Compute(comparisonValue1, comparisonValue2);

            // Assert
            Assert.Equal(expectedResult, Assert.IsType<string>(result));
        }

        /// <summary>
        /// Tests the string formatter according to the <see cref="FactAttribute"/> of this method.
        /// </summary>
        [Fact(DisplayName = "String formatter test with coercion expression")]
        public void Test5()
        {
            // Arrange
            using ExpressionParsingService eps = new ExpressionParsingService();
            eps.RegisterTypeFormatter(new SillyStringFormatter());
            int comparisonValue = DataGenerator.RandomNonNegativeInteger();
            string expression = $"strlen(\"The number is \" + {comparisonValue})";
            const long expectedResult = 24;

            // Act
            using var computedExpression = eps.Interpret(expression);
            var result = computedExpression.Compute();

            // Assert
            Assert.Equal(expectedResult, Assert.IsType<long>(result));
        }

        private class SillyStringFormatter : IStringFormatter
        {
            public (bool Success, string ParsedData) ParseIntoString<T>(T data) => data switch
            {
                long integralNumber => (true, "0x" + integralNumber.ToString("x8", CultureInfo.CurrentCulture)),
                _ => (false, default),
            };
        }
    }
}