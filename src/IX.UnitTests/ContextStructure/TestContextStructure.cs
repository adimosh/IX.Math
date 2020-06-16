// <copyright file="TestContextStructure.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using IX.Math;
using IX.UnitTests.Data;
using JetBrains.Annotations;
using Moq;
using Xunit;

namespace IX.UnitTests.ContextStructure
{
    /// <summary>
    /// The test context structure.
    /// </summary>
    [PublicAPI]
    public static class TestContextStructure
    {
        private static MathematicPortfolio staticPortfolio = new MathematicPortfolio();

        /// <summary>
        /// Generates the test data.
        /// </summary>
        /// <returns>The test data.</returns>
        public static object[][] GenerateTestData() =>
            (from p in TestData.GenerateDataObjects()
            from q in GenerateStructure()
            select Stitch(
                p,
                q)).ToArray();

        /// <summary>
        /// Generates the structure.
        /// </summary>
        /// <returns>The objects relating to test structure.</returns>
        public static object[][] GenerateStructure() =>
            (from p in GenerateCreatorsAndDisposers()
                from q in GenerateSolverExpressions()
                select Stitch(
                    p,
                    q)).ToArray();

        private static object[][] GenerateCreatorsAndDisposers() =>
            new[]
            {
                new object[]
                {
                    new Func<MathematicPortfolio>(() => new MathematicPortfolio()),
                    new Action<MathematicPortfolio>((p) => p.Dispose())
                },
                new object[]
                {
                    new Func<MathematicPortfolio>(() => staticPortfolio),
                    new Action<MathematicPortfolio>((p) => { })
                },
            };

        private static object[][] GenerateSolverExpressions() =>
            new[]
            {
                new object[]
                {
                    new Func<MathematicPortfolio, string, Dictionary<string, object>, object>(SolveWithData),
                },
                new object[]
                {
                    new Func<MathematicPortfolio, string, Dictionary<string, object>, object>(SolveWithFinder),
                },
                new object[]
                {
                    new Func<MathematicPortfolio, string, Dictionary<string, object>, object>(SolveWithDataFunctions),
                },
                new object[]
                {
                    new Func<MathematicPortfolio, string, Dictionary<string, object>, object>(SolveWithFinderFunctions),
                },
                new object[]
                {
                    new Func<MathematicPortfolio, string, Dictionary<string, object>, object>(SolveWithDataThenFinder),
                },
                new object[]
                {
                    new Func<MathematicPortfolio, string, Dictionary<string, object>, object>(SolveWithDataFunctionsThenFinder),
                },
                new object[]
                {
                    new Func<MathematicPortfolio, string, Dictionary<string, object>, object>(SolveWithDataThenFinderFunctions),
                },
                new object[]
                {
                    new Func<MathematicPortfolio, string, Dictionary<string, object>, object>(SolveWithDataFunctionsThenFinderFunctions),
                },
                new object[]
                {
                    new Func<MathematicPortfolio, string, Dictionary<string, object>, object>(SolveWithFinderThenData),
                },
                new object[]
                {
                    new Func<MathematicPortfolio, string, Dictionary<string, object>, object>(SolveWithFinderFunctionsThenData),
                },
                new object[]
                {
                    new Func<MathematicPortfolio, string, Dictionary<string, object>, object>(SolveWithFinderThenDataFunctions),
                },
                new object[]
                {
                    new Func<MathematicPortfolio, string, Dictionary<string, object>, object>(SolveWithFinderFunctionsThenDataFunctions),
                },
            };

        private static object[] Stitch(
            object[] left,
            object[] right)
        {
            object[] result = new object[left.Length + right.Length];
            Array.Copy(left, result, left.Length);
            Array.Copy(right, 0, result, left.Length, right.Length);
            return result;
        }

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "StyleCop.CSharp.ReadabilityRules",
            "SA1118:Parameter should not span multiple lines",
            Justification = "We don't really care about this at this point.")]
        private static object SolveWithData(
            MathematicPortfolio portfolio,
            string expression,
            Dictionary<string, object> parameters) => portfolio.Solve(
                expression,
                in ComparisonTolerance.Empty,
                parameters?.OrderBy(
                        q => expression.IndexOf(
                            q.Key,
                            StringComparison.Ordinal))
                    .Select(p => p.Value)
                    .ToArray() ??
                new object[0]);

        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "StyleCop.CSharp.ReadabilityRules",
            "SA1118:Parameter should not span multiple lines",
            Justification = "We don't really care about this at this point.")]
        private static object SolveWithDataFunctions(
            MathematicPortfolio portfolio,
            string expression,
            Dictionary<string, object> parameters) => portfolio.Solve(
            expression,
            in ComparisonTolerance.Empty,
            parameters?.OrderBy(
                    q => expression.IndexOf(
                        q.Key,
                        StringComparison.Ordinal))
                .Select(p => GenerateFuncOutOfParameterValue(p.Value))
                .ToArray() ??
            new object[0]);

        private static object SolveWithFinder(
            MathematicPortfolio portfolio,
            string expression,
            Dictionary<string, object> parameters)
        {
            var finder = new Mock<IDataFinder>(MockBehavior.Loose);

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

            return portfolio.Solve(
                expression,
                in ComparisonTolerance.Empty,
                finder);
        }

        private static object SolveWithFinderFunctions(
            MathematicPortfolio portfolio,
            string expression,
            Dictionary<string, object> parameters)
        {
            var finder = new Mock<IDataFinder>(MockBehavior.Loose);

            if (parameters != null)
            {
#if NETCOREAPP3_1
                foreach ((var key, object val) in parameters)
                {
                    object value = GenerateFuncOutOfParameterValue(val);
#else
                foreach (var kvp in parameters)
                {
                    var key = kvp.Key;
                    object value = GenerateFuncOutOfParameterValue(kvp.Value);
#endif
                    finder.Setup(
                        p => p.TryGetData(
                            key,
                            out value)).Returns(true);
                }
            }

            return portfolio.Solve(
                expression,
                in ComparisonTolerance.Empty,
                finder);
        }

        private static object SolveWithDataThenFinder(
            MathematicPortfolio portfolio,
            string expression,
            Dictionary<string, object> parameters)
        {
            var obj1 = SolveWithData(
                portfolio,
                expression,
                parameters);
            var obj2 = SolveWithFinder(
                portfolio,
                expression,
                parameters);

            Assert.Equal(obj1, obj2);

            return obj1;
        }

        private static object SolveWithDataFunctionsThenFinder(
            MathematicPortfolio portfolio,
            string expression,
            Dictionary<string, object> parameters)
        {
            var obj1 = SolveWithDataFunctions(
                portfolio,
                expression,
                parameters);
            var obj2 = SolveWithFinder(
                portfolio,
                expression,
                parameters);

            Assert.Equal(obj1, obj2);

            return obj1;
        }

        private static object SolveWithDataThenFinderFunctions(
            MathematicPortfolio portfolio,
            string expression,
            Dictionary<string, object> parameters)
        {
            var obj1 = SolveWithData(
                portfolio,
                expression,
                parameters);
            var obj2 = SolveWithFinderFunctions(
                portfolio,
                expression,
                parameters);

            Assert.Equal(obj1, obj2);

            return obj1;
        }

        private static object SolveWithDataFunctionsThenFinderFunctions(
            MathematicPortfolio portfolio,
            string expression,
            Dictionary<string, object> parameters)
        {
            var obj1 = SolveWithDataFunctions(
                portfolio,
                expression,
                parameters);
            var obj2 = SolveWithFinderFunctions(
                portfolio,
                expression,
                parameters);

            Assert.Equal(obj1, obj2);

            return obj1;
        }

        private static object SolveWithFinderThenData(
            MathematicPortfolio portfolio,
            string expression,
            Dictionary<string, object> parameters)
        {
            var obj1 = SolveWithFinder(
                portfolio,
                expression,
                parameters);
            var obj2 = SolveWithData(
                portfolio,
                expression,
                parameters);

            Assert.Equal(obj1, obj2);

            return obj1;
        }

        private static object SolveWithFinderFunctionsThenData(
            MathematicPortfolio portfolio,
            string expression,
            Dictionary<string, object> parameters)
        {
            var obj1 = SolveWithFinderFunctions(
                portfolio,
                expression,
                parameters);
            var obj2 = SolveWithData(
                portfolio,
                expression,
                parameters);

            Assert.Equal(obj1, obj2);

            return obj1;
        }

        private static object SolveWithFinderThenDataFunctions(
            MathematicPortfolio portfolio,
            string expression,
            Dictionary<string, object> parameters)
        {
            var obj1 = SolveWithFinder(
                portfolio,
                expression,
                parameters);
            var obj2 = SolveWithDataFunctions(
                portfolio,
                expression,
                parameters);

            Assert.Equal(obj1, obj2);

            return obj1;
        }

        private static object SolveWithFinderFunctionsThenDataFunctions(
            MathematicPortfolio portfolio,
            string expression,
            Dictionary<string, object> parameters)
        {
            var obj1 = SolveWithFinderFunctions(
                portfolio,
                expression,
                parameters);
            var obj2 = SolveWithDataFunctions(
                portfolio,
                expression,
                parameters);

            Assert.Equal(obj1, obj2);

            return obj1;
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
    }
}