using JetBrains.Annotations;

namespace IX.Math.Formatters
{
    /// <summary>
    /// A service contract for a class that is able to format data types.
    /// </summary>
    [PublicAPI]
    public interface ITypeFormatter
    {
        /// <summary>
        /// Formats the specified input.
        /// </summary>
        /// <typeparam name="TInput">The type of the input.</typeparam>
        /// <param name="input">The input.</param>
        /// <returns>The formatted value, as a string.</returns>
        string Format<TInput>(TInput input);
    }
}