// <copyright file="OneDataSetComputedExpressionUnitTest.cs" company="Adrian Mos">
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
    public class OneDataSetComputedExpressionUnitTest
    {
        /// <summary>
        ///     Provides the data for theory.
        /// </summary>
        /// <returns>Theory data.</returns>
        // ReSharper disable once MemberCanBePrivate.Global - It really cannot
        public static object[][] ProvideDataForTheory() => new[]
        {
            new object[]
            {
                "leftOperand-rightOperand",
                new Dictionary<string, object>
                {
                    ["leftOperand"] = 5745786L,
                    ["rightOperand"] = 1986373029L,
                },
                -1980627243L,
            },
        };

        private static object GenerateFuncOutOfParameterValue(object tempParameter) => tempParameter switch
        {
            byte convertedValue => new Func<byte>(() => convertedValue),
            sbyte convertedValue => new Func<sbyte>(() => convertedValue),
            short convertedValue => new Func<short>(() => convertedValue),
            ushort convertedValue => new Func<ushort>(() => convertedValue),
            int convertedValue => new Func<int>(() => convertedValue),
            uint convertedValue => new Func<uint>(() => convertedValue),
            long convertedValue => new Func<long>(() => convertedValue),
            ulong convertedValue => new Func<ulong>(() => convertedValue),
            float convertedValue => new Func<float>(() => convertedValue),
            double convertedValue => new Func<double>(() => convertedValue),
            byte[] convertedValue => new Func<byte[]>(() => convertedValue),
            string convertedValue => new Func<string>(() => convertedValue),
            bool convertedValue => new Func<bool>(() => convertedValue),
            _ => throw new InvalidOperationException(),
        };

        /// <summary>
        ///     Tests the computed expression with parameters.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="expectedResult">The expected result.</param>
        /// <exception cref="InvalidOperationException">
        ///     No computed expression was generated.
        /// </exception>
        [Theory(DisplayName = "One data set data test")]
        [MemberData(nameof(ProvideDataForTheory))]
        public void ComputedExpressionWithParameters(
            string expression,
            Dictionary<string, object> parameters,
            object expectedResult)
        {
            using var service = new MathematicPortfolio();

            object result = service.Solve(expression, parameters?.Select(p => p.Value).ToArray() ?? new object[0]);

            Assert.Equal(
                expectedResult,
                result);
        }

        /// <summary>
        ///     Tests a computed expression with finder.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="expectedResult">The expected result.</param>
        /// <exception cref="InvalidOperationException">
        ///     No computed expression was generated.
        /// </exception>
        [Theory(DisplayName = "One data set finder test")]
        [MemberData(nameof(ProvideDataForTheory))]
        public void ComputedExpressionWithFinder(
            string expression,
            Dictionary<string, object> parameters,
            object expectedResult)
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

            object result = service.Solve(expression, finder.Object);

            Assert.Equal(
                expectedResult,
                result);
        }

        /// <summary>
        ///     Tests a computed expression with finder.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="expectedResult">The expected result.</param>
        /// <exception cref="InvalidOperationException">
        ///     No computed expression was generated.
        /// </exception>
        [Theory(DisplayName = "One data set finder test with functions")]
        [MemberData(nameof(ProvideDataForTheory))]
        public void ComputedExpressionWithFunctionFinder(
            string expression,
            Dictionary<string, object> parameters,
            object expectedResult)
        {
            using var service = new MathematicPortfolio();
            var finder = new Mock<IDataFinder>(MockBehavior.Loose);

            if (parameters != null)
            {
                foreach (KeyValuePair<string, object> parameter in parameters)
                {
                    var key = parameter.Key;
                    object value = GenerateFuncOutOfParameterValue(parameter.Value);
                    finder.Setup(
                        p => p.TryGetData(
                            key,
                            out value)).Returns(true);
                }
            }

            object result = service.Solve(expression, finder.Object);

            Assert.Equal(
                expectedResult,
                result);
        }
    }
}