using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IX.Math;
using Moq;
using Xunit;

namespace IX.Math.UnitTests
{
    public class CachedExpressionChecker
    {
        [Fact(DisplayName = "Test cached expressions with strings.")]
        public void TestStringString()
        {
            Mock<IDataFinder> dfMock = new Mock<IDataFinder>(MockBehavior.Strict);
            string input = "This is fine";

            using (CachedExpressionParsingService service = new CachedExpressionParsingService())
            {
                object result = service.ExecuteExpression(input, dfMock.Object);

                Assert.NotNull(result);
                Assert.IsType<string>(result);
                Assert.Equal(input, result);
            }
        }
    }
}