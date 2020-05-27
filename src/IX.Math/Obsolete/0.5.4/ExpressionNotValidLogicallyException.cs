// <copyright file="ExpressionNotValidLogicallyException.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Runtime.Serialization;

// ReSharper disable once CheckNamespace
namespace IX.Math
{
    /// <summary>
    /// Thrown when an expression is not internally logical or consistent.
    /// </summary>
    [Serializable]
    [Obsolete("Use the base exception.")]
    public class ExpressionNotValidLogicallyException : Exceptions.ExpressionNotValidLogicallyException
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionNotValidLogicallyException"/> class.
        /// </summary>
        public ExpressionNotValidLogicallyException()
            : base(Resources.NotValidInternally)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionNotValidLogicallyException"/> class.
        /// </summary>
        /// <param name="message">A custom message for the thrown exception.</param>
        public ExpressionNotValidLogicallyException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionNotValidLogicallyException"/> class.
        /// </summary>
        /// <param name="internalException">The internal exception, if any.</param>
        public ExpressionNotValidLogicallyException(Exception internalException)
            : base(Resources.NotValidInternally, internalException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionNotValidLogicallyException"/> class.
        /// </summary>
        /// <param name="message">A custom message for the thrown exception.</param>
        /// <param name="internalException">The internal exception, if any.</param>
        public ExpressionNotValidLogicallyException(string message, Exception internalException)
            : base(message, internalException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionNotValidLogicallyException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected ExpressionNotValidLogicallyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}