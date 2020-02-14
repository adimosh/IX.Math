// <copyright file="SillyPassThroughConstantsExtractor.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using IX.Math.Extensibility;
using IX.StandardExtensions.Globalization;
using JetBrains.Annotations;

namespace IX.UnitTests.IX.Math.ExternalAssemblyCapabilities
{
    /// <summary>
    /// Test pass-through extractor baby.
    /// </summary>
    [ConstantsPassThroughExtractor]
    [UsedImplicitly]
    public class SillyPassThroughConstantsExtractor : IConstantPassThroughExtractor
    {
        /// <summary>
        /// Evaluates an expression and decides whether or not it should be a pass-through constant.
        /// </summary>
        /// <param name="expression">The expression to evaluate.</param>
        /// <returns><c>true</c> if the expression is a pass-through constant, <c>false</c> otherwise.</returns>
        public bool Evaluate(string expression) => expression.InvariantCultureEqualsInsensitive("1+2");
    }
}