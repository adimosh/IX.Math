// <copyright file="PassThroughStateContainerBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Runtime.Serialization;
using IX.StandardExtensions;
using JetBrains.Annotations;

namespace IX.Math.Extensibility
{
    /// <summary>
    /// An abstract base class for a pass-through state container.
    /// </summary>
    /// <seealso cref="IDeepCloneable{T}" />
    [DataContract]
    [PublicAPI]
    public abstract class PassThroughStateContainerBase : IDeepCloneable<PassThroughStateContainerBase>
    {
        /// <summary>
        /// Creates a deep clone of the source object.
        /// </summary>
        /// <returns>
        /// A deep clone.
        /// </returns>
        public abstract PassThroughStateContainerBase DeepClone();
    }
}