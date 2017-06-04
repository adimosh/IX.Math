// <copyright file="SymbolOptimizationData.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

namespace IX.Math
{
    /// <summary>
    /// Optimization data that is attached to symbols.
    /// </summary>
    internal class SymbolOptimizationData
    {
        /// <summary>
        /// Gets or sets the number of symbols this symbol is contained in.
        /// </summary>
        public int ContainedIn { get; set; }

        /// <summary>
        /// Gets or sets the number of symbols that this symbol contains.
        /// </summary>
        public int Contains { get; set; }
    }
}