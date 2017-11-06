using IX.StandardExtensions.TestUtils;
using System;
using System.Collections.Generic;
using Xunit;

namespace IX.Math.UnitTests
{
    public class BasicExpressionsTests
    {
        public static object[][] ProvideDataForTheory()
        {
            var tests = new List<object[]>();

            // Positive integers

            // +
            {
                var limit = int.MaxValue / 2;
                double leftOperand = DataGenerator.RandomNonNegativeInteger(limit);
                double rightOperand = DataGenerator.RandomNonNegativeInteger(limit);
                tests.Add(
                    new object[]
                    {
                        $"{leftOperand}+{rightOperand}",
                        new object[0],
                        (long)(leftOperand + rightOperand)
                    });
                tests.Add(
                    new object[]
                    {
                        "leftOperand+rightOperand",
                        new object[2] { leftOperand, rightOperand },
                        leftOperand + rightOperand
                    });
            }

            // -
            {
                double leftOperand = DataGenerator.RandomNonNegativeInteger(int.MaxValue);
                double rightOperand = DataGenerator.RandomNonNegativeInteger(int.MaxValue);
                tests.Add(
                    new object[]
                    {
                        $"{leftOperand}-{rightOperand}",
                        new object[0],
                        (long)(leftOperand - rightOperand)
                    });
                tests.Add(
                    new object[]
                    {
                        "leftOperand-rightOperand",
                        new object[2] { leftOperand, rightOperand },
                        leftOperand - rightOperand
                    });
            }

            // *
            {
                var limit = (int)System.Math.Sqrt(int.MaxValue);
                double leftOperand = DataGenerator.RandomNonNegativeInteger(limit);
                double rightOperand = DataGenerator.RandomNonNegativeInteger(limit);
                tests.Add(
                    new object[]
                    {
                        $"{leftOperand}*{rightOperand}",
                        new object[0],
                        (long)(leftOperand * rightOperand)
                    });
                tests.Add(
                    new object[]
                    {
                        "leftOperand*rightOperand",
                        new object[2] { leftOperand, rightOperand },
                        leftOperand * rightOperand
                    });
            }

            // /
            {
                double leftOperand = DataGenerator.RandomNonNegativeInteger(int.MaxValue);
                double rightOperand = DataGenerator.RandomNonNegativeInteger(int.MaxValue);
                var result = leftOperand / rightOperand;
                tests.Add(
                    new object[]
                    {
                        $"{leftOperand}/{rightOperand}",
                        new object[0],
                        result
                    });
                tests.Add(
                    new object[]
                    {
                        "leftOperand/rightOperand",
                        new object[2] { leftOperand, rightOperand },
                        result
                    });
            }

            return tests.ToArray();
        }

        [Theory(DisplayName = "Basic expressions with random data")]
        [MemberData(nameof(ProvideDataForTheory), DisableDiscoveryEnumeration = true)]
        public void ComputedExpressionWithParameters(string expression, object[] parameters, object expectedResult)
        {
            using (var service = new ExpressionParsingService())
            {
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
        }
    }
}
