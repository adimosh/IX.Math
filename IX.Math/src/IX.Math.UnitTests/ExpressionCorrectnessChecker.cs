﻿using System;
using Xunit;

namespace IX.Math.UnitTests
{
    public class ExpressionCorrectnessChecker
    {
        [Theory(DisplayName="Expression")]
        [MemberData(nameof(ProvideDataForTheory))]
        public void ExpressionCorrectnessCheckerTest(string expression, object[] parameters, object expectedResult)
        {
            ExpressionParsingService service = new ExpressionParsingService();
            var del = service.GenerateDelegate(expression);
            var result = del.DynamicInvoke(parameters);

            var convertedResult = Convert.ChangeType(result, expectedResult.GetType());

            Assert.Equal(expectedResult, convertedResult);
        }

        public static object[][] ProvideDataForTheory()
        {
            return new object[][]
            {
                new object[]
                {
                    "3+6",
                    new object[0],
                    9
                },
                new object[]
                {
                    "3+6-2*4",
                    new object[0],
                    1
                },
                new object[]
                {
                    "3+(6-2)*2",
                    new object[0],
                    11
                },
                new object[]
                {
                    "((2+3)*2-1)*2",
                    new object[0],
                    18
                },
                new object[]
                {
                    "  3         +        6      ",
                    new object[0],
                    9
                },
                new object[]
                {
                    "3=6",
                    new object[0],
                    false
                },
                new object[]
                {
                    "((2+3)*2-1)*2 - x",
                    new object[] { 12 },
                    6
                },
                new object[]
                {
                    "x^2",
                    new object[] { 2 },
                    4
                },
                new object[]
                {
                    "x^3",
                    new object[] { 3 },
                    27
                },
                new object[]
                {
                    "x",
                    new object[] { 12 },
                    12
                },
                new object[]
                {
                    "2*x-7*y",
                    new object[] { 12, 2 },
                    10
                },
                new object[]
                {
                    "textparam = 12",
                    new object[] { 13 },
                    false
                },
                new object[]
                {
                    "7+14+79<3+(7*12)",
                    new object[0],
                    false
                }
            };
        }
    }
}