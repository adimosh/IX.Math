// <copyright file="TestBatteryBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using Xunit;

namespace IX.UnitTests
{
    /// <summary>
    /// A base class for test batteries.
    /// </summary>
    public class TestBatteryBase
    {
        private static readonly ReturnValueEqualityComparer Comparer = new ReturnValueEqualityComparer();

        /// <summary>
        /// Asserts the results.
        /// </summary>
        /// <param name="expectedResult">The expected result.</param>
        /// <param name="result">The result.</param>
        protected void AssertResults(
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
                Comparer);

            static Type FixNumericType(in object source) =>
                source switch
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
        }
    }
}