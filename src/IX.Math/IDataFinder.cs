// <copyright file="IDataFinder.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics.CodeAnalysis;
using JetBrains.Annotations;

namespace IX.Math
{
    /// <summary>
    /// A contract for an external service that fetches data based on data keys.
    /// </summary>
    [PublicAPI]
    public interface IDataFinder
    {
        /// <summary>
        /// Tries to get data based on a data key.
        /// </summary>
        /// <param name="dataKey">The data key.</param>
        /// <param name="data">The output data.</param>
        /// <returns><c>true</c> if data is available, <c>false</c> otherwise.</returns>
        public bool TryGetData(
            string dataKey,
            [MaybeNullWhen(false)]
            out object data);
    }
}