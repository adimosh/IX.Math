using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IX.Math;

namespace IX.UnitTests.Helpers
{
    /// <summary>
    /// Data expressions helper.
    /// </summary>
    public static class DataExpressions
    {
        /// <summary>
        /// Gets the objects that relate to the fixture pattern.
        /// </summary>
        /// <returns>The data objects.</returns>
        public static object[][] GetFixturePatternObjects() =>
            new[]
            {
                new object[]
                {
                    new Func<CachedExpressionProviderFixture, IExpressionParsingService>((fix) => fix.CachedService),
                    null,
                },
                new object[]
                {
                    new Func<CachedExpressionProviderFixture, IExpressionParsingService>((fix) => fix.Service),
                    null,
                },
                new object[]
                {
                    new Func<CachedExpressionProviderFixture, IExpressionParsingService>((_) => new ExpressionParsingService()),
                    new Action<IExpressionParsingService>(eps => eps.Dispose()),
                },
                new object[]
                {
                    new Func<CachedExpressionProviderFixture, IExpressionParsingService>((_) => new CachedExpressionParsingService()),
                    new Action<IExpressionParsingService>(eps => eps.Dispose()),
                },
            };
    }
}
