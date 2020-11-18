// <copyright file="StaticVariableValueUnitTests.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using IX.Math;
using Xunit;

namespace IX.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="StaticVariableValue"/>.
    /// </summary>
    public class StaticVariableValueUnitTests
    {
        /// <summary>
        /// DynamicVariableValue initialization forward.
        /// </summary>
        [Fact(DisplayName = "DynamicVariableValue initialization forward")]
        public void Test1()
        {
            // ARRANGE
            MathematicPortfolio p = new MathematicPortfolio();

            p.RegisterConversion<long, string>(source => (true, source.ToString()));

            // ACT
            StaticVariableValue dv = DataGeneration.DataGenerator.RandomInteger();

            // ASSERT
            Assert.True(dv.TryGetInteger(out var l));
            Assert.True(dv.TryGetString(out var s));
            Assert.False(dv.TryGetBinary(out _));
            Assert.False(dv.TryGetBoolean(out _));
            Assert.False(dv.TryGetNumeric(out _));

            Assert.Equal(l, long.Parse(s));
        }

        /// <summary>
        /// DynamicVariableValue initialization reverse.
        /// </summary>
        [Fact(DisplayName = "DynamicVariableValue initialization reverse")]
        public void Test2()
        {
            // ARRANGE
            MathematicPortfolio p = new MathematicPortfolio();

            p.RegisterConversion<string, long>(source => (true, long.Parse(source)));

            // ACT
            StaticVariableValue dv = DataGeneration.DataGenerator.RandomInteger().ToString();

            // ASSERT
            Assert.True(dv.TryGetInteger(out var l));
            Assert.True(dv.TryGetString(out var s));
            Assert.False(dv.TryGetBinary(out _));
            Assert.False(dv.TryGetBoolean(out _));
            Assert.False(dv.TryGetNumeric(out _));

            Assert.Equal(s, l.ToString());
        }

        /// <summary>
        /// DynamicVariableValue with simple operation.
        /// </summary>
        [Fact(DisplayName = "DynamicVariableValue with simple operation")]
        public void Test3()
        {
            // ARRANGE
            MathematicPortfolio p = new MathematicPortfolio();

            p.RegisterConversion<string, long>(source => (true, long.Parse(source)));

            long value1 = DataGeneration.DataGenerator.RandomInteger();
            long value2 = DataGeneration.DataGenerator.RandomInteger();
            StaticVariableValue dv = value1.ToString();

            // ACT
            var result = p.Solve(
                $"{value2} + x",
                dv);

            // ASSERT
            Assert.Equal(
                result,
                value1 + value2);
        }
    }
}