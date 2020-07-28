using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IX.UnitTests.Data
{
    public partial class TestData
    {
        /// <summary>
        ///     Provides templated random text data for basic operators and parantheses.
        /// </summary>
        /// <returns>Test data.</returns>
        private static List<object[]> Gathered() =>
            new List<object[]>
            {
                new object[]
                {
                    "ln(2*(8-3))",
                    null,
                    0L,
                },
            };
    }
}