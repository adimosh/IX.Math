using System;
using System.Linq.Expressions;

namespace IX.Math
{
    /// <summary>
    /// A model for a special symbol.
    /// </summary>
    public abstract class SpecialSymbol
    {
        /// <summary>
        /// The name of the symbol.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The type of the symbol.
        /// </summary>
        public abstract SupportedValueType Type { get; }

        /// <summary>
        /// Gets the actual type of numeric data that is required for this function to work.
        /// </summary>
        public abstract Type ActualMinimalNumericTypeRequired { get; }

        /// <summary>
        /// Generates an expression representing the special symbol.
        /// </summary>
        /// <returns>The expression representing the special symbol.</returns>
        public abstract Expression GenerateExpression();
    }
}
