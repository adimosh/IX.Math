// <copyright file="SillyConstantsExtractor.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Collections.Generic;
using System.Text.RegularExpressions;
using IX.Math;
using IX.Math.Extensibility;
using IX.Math.Nodes;
using JetBrains.Annotations;

namespace IX.UnitTests.IX.Math.ExternalAssemblyCapabilities
{
    /// <summary>
    ///     A constants extractor used for testing purposes.
    /// </summary>
    /// <seealso cref="IConstantsExtractor" />
    [UsedImplicitly]
    [ConstantsExtractor]
    public class SillyConstantsExtractor : IConstantsExtractor
    {
#region Internal state

        private readonly Regex exponentialNotationRegex = new(@"silly");

#endregion

#region Methods

#region Interface implementations

        /// <summary>
        ///     Extracts all constants, replacing them from the original expression.
        /// </summary>
        /// <param name="originalExpression">The original expression.</param>
        /// <param name="constantsTable">The constants table.</param>
        /// <param name="reverseConstantsTable">The reverse constants table.</param>
        /// <param name="mathDefinition">The math definition.</param>
        /// <returns>The expression, after replacement.</returns>
        public string ExtractAllConstants(
            string originalExpression,
            IDictionary<string, ConstantNodeBase> constantsTable,
            IDictionary<string, string> reverseConstantsTable,
            MathDefinition mathDefinition) =>
            this.exponentialNotationRegex.Replace(
                originalExpression,
                "stupid",
                1);

#endregion

#endregion
    }
}