// <copyright file="BaseMathematicalException.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

// ReSharper disable once CheckNamespace
namespace IX.Math
{
    /// <summary>
    /// A base exception for exceptions thrown by the mathematics engine.
    /// </summary>
    /// <seealso cref="Exception" />
    [Serializable]
    [PublicAPI]
    [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Design",
        "CA1032:Implement standard exception constructors",
        Justification = "We'll use them in the derived exceptions.")]
    public abstract class BaseMathematicalException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMathematicalException"/> class.
        /// </summary>
        /// <param name="message">A custom message for the thrown exception.</param>
        protected BaseMathematicalException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMathematicalException"/> class.
        /// </summary>
        /// <param name="message">A custom message for the thrown exception.</param>
        /// <param name="internalException">The internal exception, if any.</param>
        protected BaseMathematicalException(string message, Exception internalException)
            : base(message, internalException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BaseMathematicalException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected BaseMathematicalException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}