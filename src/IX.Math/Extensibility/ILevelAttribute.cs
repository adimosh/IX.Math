// <copyright file="ILevelAttribute.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

namespace IX.Math.Extensibility
{
    /// <summary>
    /// A service contract for an attribute that includes a level.
    /// </summary>
    public interface ILevelAttribute
    {
        /// <summary>
        /// Gets the level.
        /// </summary>
        public int Level { get; }
    }
}