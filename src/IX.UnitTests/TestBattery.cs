// <copyright file="TestBattery.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using IX.Math;
using IX.StandardExtensions.TestUtils;
using IX.UnitTests.Helpers;
using IX.UnitTests.IX.Math;
using Moq;
using Xunit;

namespace IX.UnitTests
{
    /// <summary>
    ///     Tests computed expressions.
    /// </summary>
    public class TestBattery : IClassFixture<CachedExpressionProviderFixture>
    {
        private readonly CachedExpressionProviderFixture fixture;
        private readonly ReturnValueEqualityComparer comparer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="TestBattery" /> class.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        public TestBattery(CachedExpressionProviderFixture fixture)
        {
            this.fixture = fixture;
            this.comparer = new ReturnValueEqualityComparer();
        }

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

        private static Type FixNumericType(in object source) => source switch
        {
            byte _ => typeof(double),
            sbyte _ => typeof(double),
            int _ => typeof(double),
            uint _ => typeof(double),
            short _ => typeof(double),
            ushort _ => typeof(double),
            long _ => typeof(double),
            ulong _ => typeof(double),
            float _ => typeof(double),
            double _ => typeof(double),
            _ => source.GetType()
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
        [Theory(DisplayName = "EPS")]
        [MemberData(nameof(TestData.GenerateDataObjects), MemberType = typeof(TestData))]
        public void ComputedExpressionWithParameters(
            string expression,
            Dictionary<string, object> parameters,
            object expectedResult)
        {
            try
            {
                object result;

                using (var service = new ExpressionParsingService())
                {
                    using (ComputedExpression del = service.Interpret(expression))
                    {
                        result = del.Compute(
                            parameters?.OrderBy(
                                q => expression.IndexOf(
                                    q.Key,
                                    StringComparison.Ordinal)).Select(p => p.Value).ToArray() ??
                            new object[0]);
                    }
                }

                this.AssertResults(
                    in expectedResult,
                    in result);
            }
            catch (DivideByZeroException)
            {
            }
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
        [Theory(DisplayName = "EPSF")]
        [MemberData(nameof(TestData.GenerateDataObjects), MemberType = typeof(TestData))]
        public void ComputedExpressionWithFinder(
            string expression,
            Dictionary<string, object> parameters,
            object expectedResult)
        {
            try
            {
                object result;

                using (var service = new ExpressionParsingService())
                {
                    var finder = new Mock<IDataFinder>(MockBehavior.Loose);

                    using (ComputedExpression del = service.Interpret(expression))
                    {
                        if (parameters != null)
                        {
#if NETCOREAPP3_1
                            foreach ((var key, object val) in parameters)
                            {
                                object value = val;
#else
                            foreach (var kvp in parameters)
                            {
                                var key = kvp.Key;
                                object value = kvp.Value;
#endif
                                finder.Setup(
                                    p => p.TryGetData(
                                        key,
                                        out value)).Returns(true);
                            }
                        }

                        result = del.Compute(finder.Object);
                    }
                }

                this.AssertResults(
                    in expectedResult,
                    in result);
            }
            catch (DivideByZeroException)
            {
            }
        }

#pragma warning disable IDISP001 // Dispose created. - We specifically do not want these to be disposed

        /// <summary>
        ///     Tests the cached computed expression with parameters.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="expectedResult">The expected result.</param>
        /// <exception cref="InvalidOperationException">
        ///     No computed expression was generated.
        /// </exception>
        [Theory(DisplayName = "CEPS")]
        [MemberData(nameof(TestData.GenerateDataObjects), MemberType = typeof(TestData))]
        public void CachedComputedExpressionWithParameters(
            string expression,
            Dictionary<string, object> parameters,
            object expectedResult)
        {
            try
            {
                ComputedExpression del = this.fixture.CachedService.Interpret(expression);

                object result = del.Compute(parameters?.OrderBy(q => expression.IndexOf(q.Key, StringComparison.Ordinal)).Select(p => p.Value).ToArray() ?? new object[0]);

                this.AssertResults(in expectedResult, in result);
            }
            catch (DivideByZeroException)
            {
            }
        }

        /// <summary>
        ///     Tests a cached computed expression with finder.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="expectedResult">The expected result.</param>
        /// <exception cref="InvalidOperationException">
        ///     No computed expression was generated.
        /// </exception>
        [Theory(DisplayName = "CEPSF")]
        [MemberData(nameof(TestData.GenerateDataObjects), MemberType = typeof(TestData))]
        public void CachedComputedExpressionWithFinder(
            string expression,
            Dictionary<string, object> parameters,
            object expectedResult)
        {
            try
            {
                var finder = new Mock<IDataFinder>(MockBehavior.Loose);

                ComputedExpression del = this.fixture.CachedService.Interpret(expression);

                if (parameters != null)
                {
#if NETCOREAPP3_1
                    foreach ((var key, object val) in parameters)
                    {
                        object value = val;
#else
                    foreach (var kvp in parameters)
                    {
                        var key = kvp.Key;
                        object value = kvp.Value;
#endif
                        finder.Setup(
                            p => p.TryGetData(
                                key,
                                out value)).Returns(true);
                    }
                }

                object result = del.Compute(finder.Object);

                this.AssertResults(in expectedResult, in result);
            }
            catch (DivideByZeroException)
            {
            }
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
        [Theory(DisplayName = "EPSFF")]
        [MemberData(nameof(TestData.GenerateDataObjects), MemberType = typeof(TestData))]
        public void ComputedExpressionWithFunctionFinder(
            string expression,
            Dictionary<string, object> parameters,
            object expectedResult)
        {
            try
            {
                object result;

                using (var service = new ExpressionParsingService())
                {
                    var finder = new Mock<IDataFinder>(MockBehavior.Loose);

                    using (ComputedExpression del = service.Interpret(expression))
                    {
                        if (parameters != null)
                        {
#if NETCOREAPP3_1
                            foreach ((var key, object val) in parameters)
                            {
#else
                            foreach (var kvp in parameters)
                            {
                                var key = kvp.Key;
                                object val = kvp.Value;
#endif
                                object value = GenerateFuncOutOfParameterValue(val);
                                finder.Setup(
                                    p => p.TryGetData(
                                        key,
                                        out value)).Returns(true);
                            }
                        }

                        result = del.Compute(finder.Object);
                    }
                }

                this.AssertResults(in expectedResult, in result);
            }
            catch (DivideByZeroException)
            {
            }
        }

        /// <summary>
        ///     Tests a cached computed expression with finder.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="expectedResult">The expected result.</param>
        /// <exception cref="InvalidOperationException">
        ///     No computed expression was generated!.
        /// </exception>
        [Theory(DisplayName = "CEPSFF")]
        [MemberData(nameof(TestData.GenerateDataObjects), MemberType = typeof(TestData))]
        public void CachedComputedExpressionWithFunctionFinder(
            string expression,
            Dictionary<string, object> parameters,
            object expectedResult)
        {
            try
            {
                var finder = new Mock<IDataFinder>(MockBehavior.Loose);

                ComputedExpression del = this.fixture.CachedService.Interpret(expression);

                if (parameters != null)
                {
#if NETCOREAPP3_1
                    foreach ((var key, object val) in parameters)
                    {
#else
                    foreach (var kvp in parameters)
                    {
                        var key = kvp.Key;
                        object val = kvp.Value;
#endif
                        object value = GenerateFuncOutOfParameterValue(val);
                        finder.Setup(
                            p => p.TryGetData(
                                key,
                                out value)).Returns(true);
                    }
                }

                object result = del.Compute(finder.Object);

                this.AssertResults(in expectedResult, in result);
            }
            catch (DivideByZeroException)
            {
            }
        }

        /// <summary>
        ///     Tests a cached computed expression with finder returning functions repeatedly.
        /// </summary>
        /// <param name="expression">The expression.</param>
        /// <param name="parameters">The parameters.</param>
        /// <param name="expectedResult">The expected result.</param>
        /// <exception cref="InvalidOperationException">
        ///     No computed expression was generated.
        /// </exception>
        [Theory(DisplayName = "CEPSFFR")]
        [MemberData(nameof(TestData.GenerateDataObjects), MemberType = typeof(TestData))]
        public void CachedComputedExpressionWithFunctionFinderRepeated(
            string expression,
            Dictionary<string, object> parameters,
            object expectedResult)
        {
            try
            {
                var indexLimit = DataGenerator.RandomInteger(
                    3,
                    5);
                for (var index = 0; index < indexLimit; index++)
                {
                    var finder = new Mock<IDataFinder>(MockBehavior.Loose);

                    ComputedExpression del = this.fixture.CachedService.Interpret(expression);

                    if (parameters != null)
                    {
#if NETCOREAPP3_1
                        foreach ((var key, object val) in parameters)
                        {
#else
                        foreach (var kvp in parameters)
                        {
                            var key = kvp.Key;
                            object val = kvp.Value;
#endif
                            object value = GenerateFuncOutOfParameterValue(val);
                            finder.Setup(
                                p => p.TryGetData(
                                    key,
                                    out value)).Returns(true);
                        }
                    }

                    object result = del.Compute(finder.Object);

                    this.AssertResults(in expectedResult, in result);
                }
            }
            catch (DivideByZeroException)
            {
            }
        }
#pragma warning restore IDISP001 // Dispose created.

        private void AssertResults(
            in object expectedResult,
            in object result)
        {
            Assert.NotNull(result);

            var resultType = FixNumericType(in result);
            var eresType = FixNumericType(in expectedResult);

            Assert.Equal(eresType, resultType);

            Assert.Equal(
                expectedResult,
                result,
                this.comparer);
        }
    }
}