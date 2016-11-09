using System;
using Xunit;

namespace IX.Math.UnitTests
{
    public class AutoCalculationChecker
    {
        [Theory(DisplayName = "Expression")]
        [MemberData(nameof(ProvideDataForTheory))]
        public void AutoCalculationCheckerTest(string expression, object expectedResult)
        {
            ExpressionParsingService service = new ExpressionParsingService();

            object result;

            try
            {
                result = service.ExecuteExpression(expression);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"The generation process should not have thrown an exception, but it threw {ex.GetType()} with message \"{ex.Message}\".");
            }

            Assert.Equal(expectedResult, result);
        }

        public static object[][] ProvideDataForTheory()
        {
            return new object[][]
            {
                new object[]
                {
                    "3+6",
                    9
                },
                new object[]
                {
                    "21*3-17",
                    46
                },
                new object[]
                {
                    "(1+1)*2-3",
                    1
                },
                new object[]
                {
                    "sqrt(4)",
                    2.0
                },
                new object[]
                {
                    "!4+4",
                    -1
                },
                new object[]
                {
                    "212",
                    212
                },
                new object[]
                {
                    "String is wonderful",
                    "String is wonderful"
                },
                new object[]
                {
                    "212=String again",
                    "212=String again"
                },
                new object[]
                {
                    "0x10+26",
                    42
                },
                new object[]
                {
                    "e",
                    System.Math.E
                },
                new object[]
                {
                    "[pi]",
                    System.Math.PI
                },
                new object[]
                {
                    "e*[pi]",
                    System.Math.E * System.Math.PI
                },
                new object[]
                {
                    "min(2,17)",
                    2D
                },
                new object[]
                {
                    "max(2,17)+1",
                    18D
                },
                new object[]
                {
                    "(max(2,17)+1)/2",
                    9D
                },
                new object[]
                {
                    "max(2,17)+max(3,1)",
                    20D
                },
                new object[]
                {
                    "(sqrt(16)+1)*4-max(20,13)+(27*5-27*4 - sqrt(49))",
                    20D
                },
                new object[]
                {
                    "strlen(\"This that those\")",
                    15
                },
                new object[]
                {
                    "5+strlen(\"This that those\")-10",
                    10
                },
                new object[]
                {
                    "min(max(10,5),max(25,10))",
                    10D
                },
            };
        }
    }
}