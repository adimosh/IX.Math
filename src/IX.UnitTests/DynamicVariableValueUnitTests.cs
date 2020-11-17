// <copyright file="DynamicVariableValueUnitTests.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IX.Math;
using Xunit;

namespace IX.UnitTests
{
    public class DynamicVariableValueUnitTests
    {
        [Fact(DisplayName = "DynamicVariableValue initialization forward")]
        public void Test1()
        {
            MathematicPortfolio p = new MathematicPortfolio();

            p.RegisterConversion<long, string>(source => (true, source.ToString()));

            DynamicVariableValue dv = DataGeneration.DataGenerator.RandomInteger();

            Assert.True(dv.TryGetInteger(out var l));
            Assert.True(dv.TryGetString(out var s));
            Assert.False(dv.TryGetBinary(out _));
            Assert.False(dv.TryGetBoolean(out _));
            Assert.False(dv.TryGetNumeric(out _));

            Assert.Equal(l, long.Parse(s));
        }

        [Fact(DisplayName = "DynamicVariableValue initialization reverse")]
        public void Test2()
        {
            MathematicPortfolio p = new MathematicPortfolio();

            p.RegisterConversion<string, long>(source => (true, long.Parse(source)));

            DynamicVariableValue dv = DataGeneration.DataGenerator.RandomInteger().ToString();

            Assert.True(dv.TryGetInteger(out var l));
            Assert.True(dv.TryGetString(out var s));
            Assert.False(dv.TryGetBinary(out _));
            Assert.False(dv.TryGetBoolean(out _));
            Assert.False(dv.TryGetNumeric(out _));

            Assert.Equal(s, l.ToString());
        }
    }
}