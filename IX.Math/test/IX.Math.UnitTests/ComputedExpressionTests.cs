using Moq;
using System;
using Xunit;

namespace IX.Math.UnitTests
{
    public class ComputedExpressionTests
    {
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
                    "1<<2",
                    new object[0],
                    4
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
                    4.0
                },
                new object[]
                {
                    "x^3",
                    new object[] { 3 },
                    27.0
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
                },
                new object[]
                {
                    "-1.00<-1",
                    new object[0],
                    false
                },
                new object[]
                {
                    "1<<1",
                    new object[0],
                    2
                },
                new object[]
                {
                    "1<<1 + 2 << 1",
                    new object[0],
                    6
                },
                new object[]
                {
                    "((1+1)-(1-1))+((1-1)-(1+1))",
                    new object[0],
                    0
                },
                new object[]
                {
                    "((6-3)*(3+3))-1",
                    new object[0],
                    17
                },
                new object[]
                {
                    "2+sqrt(4)+2",
                    new object[0],
                    6.0
                },
                new object[]
                {
                    "2.0*x-7*y",
                    new object[] { (float)12.5, 2 },
                    (float)11.0
                },
                new object[]
                {
                    "!x",
                    new object[] { 32768 },
                    -32769
                },
                new object[]
                {
                    "strlen(x)",
                    new object[] { "alabala" },
                    7
                },
                new object[]
                {
                    "21*3-17",
                    new object[0],
                    46
                },
                new object[]
                {
                    "(1+1)*2-3",
                    new object[0],
                    1
                },
                new object[]
                {
                    "sqrt(4)",
                    new object[0],
                    2.0
                },
                new object[]
                {
                    "!4+4",
                    new object[0],
                    -1
                },
                new object[]
                {
                    "212",
                    new object[0],
                    212
                },
                new object[]
                {
                    "String is wonderful",
                    new object[0],
                    "String is wonderful"
                },
                new object[]
                {
                    "212=String again",
                    new object[0],
                    "212=String again"
                },
                new object[]
                {
                    "0x10+26",
                    new object[0],
                    42
                },
                new object[]
                {
                    "e",
                    new object[0],
                    System.Math.E
                },
                new object[]
                {
                    "[pi]",
                    new object[0],
                    System.Math.PI
                },
                new object[]
                {
                    "e*[pi]",
                    new object[0],
                    System.Math.E * System.Math.PI
                },
                new object[]
                {
                    "min(2,17)",
                    new object[0],
                    2D
                },
                new object[]
                {
                    "max(2,17)+1",
                    new object[0],
                    18D
                },
                new object[]
                {
                    "(max(2,17)+1)/2",
                    new object[0],
                    9D
                },
                new object[]
                {
                    "max(2,17)+max(3,1)",
                    new object[0],
                    20D
                },
                new object[]
                {
                    "(sqrt(16)+1)*4-max(20,13)+(27*5-27*4 - sqrt(49))",
                    new object[0],
                    20D
                },
                new object[]
                {
                    "strlen(\"This that those\")",
                    new object[0],
                    15
                },
                new object[]
                {
                    "5+strlen(\"This that those\")-10",
                    new object[0],
                    10
                },
                new object[]
                {
                    "min(max(10,5),max(25,10))",
                    new object[0],
                    10D
                },
                new object[]
                {
                    "min(max(10,5)+40,3*max(25,10))",
                    new object[0],
                    50D
                },
                new object[]
                {
                    "min(max(5+strlen(\"This that those\")-10,5)+40,3*max(25,10))",
                    new object[0],
                    50D
                },
                new object[]
                {
                    "1--2",
                    new object[0],
                    3
                },
            };
        }
    }
}
