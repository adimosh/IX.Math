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
            };
        }
    }
}