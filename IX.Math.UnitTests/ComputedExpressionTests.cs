// <copyright file="ComputedExpressionTests.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using Moq;
using Xunit;

namespace IX.Math.UnitTests
{
    public class ComputedExpressionTests
    {
        public static object[][] ProvideDataForTheory()
        {
            return new object[][]
            {
                new object[]
                {
                    "3+6",
                    new object[0],
                    9L,
                },
                new object[]
                {
                    "8-9",
                    new object[0],
                    -1L,
                },
                new object[]
                {
                    "6-2",
                    new object[0],
                    4L,
                },
                new object[]
                {
                    "3*6",
                    new object[0],
                    18L,
                },
                new object[]
                {
                    "3/6",
                    new object[0],
                    0.5,
                },
                new object[]
                {
                    "6/3",
                    new object[0],
                    2L,
                },
                new object[]
                {
                    "6^3",
                    new object[0],
                    216L,
                },
                new object[]
                {
                    @"""3""+6",
                    new object[0],
                    "36",
                },
                new object[]
                {
                    @"""3""+""6""",
                    new object[0],
                    "36",
                },
                new object[]
                {
                    @"""3+6""",
                    new object[0],
                    "3+6",
                },
                new object[]
                {
                    "3+6-2*4",
                    new object[0],
                    1L,
                },
                new object[]
                {
                    "3+(6-2)*2",
                    new object[0],
                    11L,
                },
                new object[]
                {
                    "3+(6-2*2)",
                    new object[0],
                    5L,
                },
                new object[]
                {
                    "1<<2",
                    new object[0],
                    4L,
                },
                new object[]
                {
                    "3-6+1<<2",
                    new object[0],
                    1L,
                },
                new object[]
                {
                    "x&y",
                    new object[2] { 5, 49 },
                    1L,
                },
                new object[]
                {
                    "x|y",
                    new object[2] { 5, 49 },
                    53L,
                },
                new object[]
                {
                    "x#y",
                    new object[2] { 5, 49 },
                    52L,
                },
                new object[]
                {
                    "x&y",
                    new object[2] { true, false },
                    false,
                },
                new object[]
                {
                    "x&y",
                    new object[2] { true, true },
                    true,
                },
                new object[]
                {
                    "x|y",
                    new object[2] { true, false },
                    true,
                },
                new object[]
                {
                    "x|(1>2)",
                    new object[1] { true },
                    true,
                },
                new object[]
                {
                    "x|y",
                    new object[2] { false, false },
                    false,
                },
                new object[]
                {
                    "x#y",
                    new object[2] { true, true },
                    false,
                },
                new object[]
                {
                    "x#y",
                    new object[2] { true, false },
                    true,
                },
                new object[]
                {
                    "x<<y",
                    new object[2] { 3, 2 },
                    12L,
                },
                new object[]
                {
                    "x>>y",
                    new object[2] { 3, 1 },
                    1L,
                },
                new object[]
                {
                    "2<<2+1<<2",
                    new object[0],
                    12L,
                },
                new object[]
                {
                    "1<<1<<2",
                    new object[0],
                    8L,
                },
                new object[]
                {
                    "1<<2>>2",
                    new object[0],
                    1L,
                },
                new object[]
                {
                    "((2+3)*2-1)*2",
                    new object[0],
                    18L,
                },
                new object[]
                {
                    "  3         +        6      ",
                    new object[0],
                    9L,
                },
                new object[]
                {
                    "3=6",
                    new object[0],
                    false,
                },
                new object[]
                {
                    "((2+3)*2-1)*2 - x",
                    new object[] { 12 },
                    6D,
                },
                new object[]
                {
                    "x^2",
                    new object[] { 2 },
                    4.0,
                },
                new object[]
                {
                    "x^3",
                    new object[] { 3 },
                    27.0,
                },
                new object[]
                {
                    "x",
                    new object[] { 12 },
                    12D,
                },
                new object[]
                {
                    "2*x-7*y",
                    new object[] { 12, 2 },
                    10D,
                },
                new object[]
                {
                    "x-y",
                    new object[] { 12, 2 },
                    10D,
                },
                new object[]
                {
                    "textparam = 12",
                    new object[] { 13 },
                    false,
                },
                new object[]
                {
                    "7+14+79<3+(7*12)",
                    new object[0],
                    false,
                },
                new object[]
                {
                    "-1.00<-1",
                    new object[0],
                    false,
                },
                new object[]
                {
                    "1<<1",
                    new object[0],
                    2L,
                },
                new object[]
                {
                    "7/2",
                    new object[0],
                    3.5,
                },
                new object[]
                {
                    "1<<1 + 2 << 1",
                    new object[0],
                    6L,
                },
                new object[]
                {
                    "((1+1)-(1-1))+((1-1)-(1+1))",
                    new object[0],
                    0L,
                },
                new object[]
                {
                    "((6-3)*(3+3))-1",
                    new object[0],
                    17L,
                },
                new object[]
                {
                    "2+sqrt(4)+2",
                    new object[0],
                    6L,
                },
                new object[]
                {
                    "2.0*x-7*y",
                    new object[] { 12.5D, 2 },
                    11.0D,
                },
                new object[]
                {
                    "!x",
                    new object[] { 32768 },
                    -32769L,
                },
                new object[]
                {
                    "strlen(x)",
                    new object[] { "alabala" },
                    7L,
                },
                new object[]
                {
                    "21*3-17",
                    new object[0],
                    46L,
                },
                new object[]
                {
                    "(1+1)*2-3",
                    new object[0],
                    1L,
                },
                new object[]
                {
                    "sqrt(4)",
                    new object[0],
                    2L,
                },
                new object[]
                {
                    "sqrt(4.0)",
                    new object[0],
                    2L,
                },
                new object[]
                {
                    "sqrt(0.49)",
                    new object[0],
                    0.7,
                },
                new object[]
                {
                    "!4+4",
                    new object[0],
                    -1L,
                },
                new object[]
                {
                    "212",
                    new object[0],
                    212L,
                },
                new object[]
                {
                    "String is wonderful",
                    new object[0],
                    "String is wonderful",
                },
                new object[]
                {
                    "212=String again",
                    new object[0],
                    "212=String again",
                },
                new object[]
                {
                    "0x10+26",
                    new object[0],
                    42L,
                },
                new object[]
                {
                    "e",
                    new object[0],
                    System.Math.E,
                },
                new object[]
                {
                    "[pi]",
                    new object[0],
                    System.Math.PI,
                },
                new object[]
                {
                    "e*[pi]",
                    new object[0],
                    System.Math.E * System.Math.PI,
                },
                new object[]
                {
                    "min(2,17)",
                    new object[0],
                    2L,
                },
                new object[]
                {
                    "max(2,17)+1",
                    new object[0],
                    18L,
                },
                new object[]
                {
                    "(max(2,17)+1)/2",
                    new object[0],
                    9L,
                },
                new object[]
                {
                    "max(2,17)+max(3,1)",
                    new object[0],
                    20L,
                },
                new object[]
                {
                    "(sqrt(16)+1)*4-max(20,13)+(27*5-27*4 - sqrt(49))",
                    new object[0],
                    20L,
                },
                new object[]
                {
                    "strlen(\"This that those\")",
                    new object[0],
                    15L,
                },
                new object[]
                {
                    "5+strlen(\"This that those\")-10",
                    new object[0],
                    10L,
                },
                new object[]
                {
                    "min(max(10,5),max(25,10))",
                    new object[0],
                    10L,
                },
                new object[]
                {
                    "min(max(10,5)+40,3*max(25,10))",
                    new object[0],
                    50L,
                },
                new object[]
                {
                    "min(max(5+strlen(\"This that those\")-10,5)+40,3*max(25,10))",
                    new object[0],
                    50L,
                },
                new object[]
                {
                    "1--2",
                    new object[0],
                    3L,
                },
                new object[]
                {
                    "x+y",
                    new object[2] { 1, -2 },
                    -1D,
                },
                new object[]
                {
                    "1*-2",
                    new object[0],
                    -2L,
                },
                new object[]
                {
                    "(x=0) & (y=1)",
                    new object[2] { 0, 1 },
                    true,
                },
                new object[]
                {
                    "(x=0) | (y=1)",
                    new object[2] { 0, 0 },
                    true,
                },
                new object[]
                {
                    "(x=0) & (y=1)",
                    new object[2] { 0, 2 },
                    false,
                },
                new object[]
                {
                    "(x>0) & (y<1)",
                    new object[2] { 1, 0 },
                    true,
                },
                new object[]
                {
                    "abs(x)",
                    new object[] { -1 },
                    1D,
                },
                new object[]
                {
                    "abs(0x1)",
                    new object[0],
                    1L,
                },
                new object[]
                {
                    "sqrt(x)",
                    new object[] { 2 },
                    1.4142135623730951,
                },
                new object[]
                {
                    "sqrt(x)",
                    new object[] { 9 },
                    3D,
                },
                new object[]
                {
                    "ceil(x)",
                    new object[] { 2.2 },
                    3D,
                },
                new object[]
                {
                    "floor(x)",
                    new object[] { 4.9 },
                    4D,
                },
                new object[]
                {
                    "round(x)",
                    new object[] { 3.5 },
                    4D,
                },
                new object[]
                {
                    "round(x)",
                    new object[] { 2.49 },
                    2D,
                },
            };
        }

        [Theory(DisplayName = "Para")]
        [MemberData(nameof(ProvideDataForTheory))]
        public void ComputedExpressionWithParameters(string expression, object[] parameters, object expectedResult)
        {
            ExpressionParsingService service = new ExpressionParsingService();

            ComputedExpression del;
            try
            {
                del = service.Interpret(expression);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"The generation process should not have thrown an exception, but it threw {ex.GetType()} with message \"{ex.Message}\".");
            }

            if (del == null)
            {
                throw new InvalidOperationException("No computed expression was generated!");
            }

            object result;
            try
            {
                result = del.Compute(parameters);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"The method should not have thrown an exception, but it threw {ex.GetType()} with message \"{ex.Message}\".");
            }

            Assert.Equal(expectedResult, result);
        }

        [Theory(DisplayName = "Findr")]
        [MemberData(nameof(ProvideDataForTheory))]
        public void ComputedExpressionWithFinder(string expression, object[] parameters, object expectedResult)
        {
            ExpressionParsingService service = new ExpressionParsingService();
            Mock<IDataFinder> finder = new Mock<IDataFinder>(MockBehavior.Loose);

            ComputedExpression del;
            try
            {
                del = service.Interpret(expression);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"The generation process should not have thrown an exception, but it threw {ex.GetType()} with message \"{ex.Message}\".");
            }

            if (del == null)
            {
                throw new InvalidOperationException("No computed expression was generated!");
            }

            for (int i = 0; i < System.Math.Min(del.ParameterNames.Length, parameters.Length); i++)
            {
                string valueName = del.ParameterNames[i];
                object outValue = parameters[i];

                finder.Setup(p => p.TryGetData(valueName, out outValue)).Returns(true);
            }

            object result;
            try
            {
                result = del.Compute(finder.Object);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"The method should not have thrown an exception, but it threw {ex.GetType()} with message \"{ex.Message}\".");
            }

            Assert.Equal(expectedResult, result);
        }
    }
}