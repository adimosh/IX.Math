// <copyright file="MathematicsEngineException.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Runtime.Serialization;
using JetBrains.Annotations;

namespace IX.Math.Exceptions
{
    /// <summary>
    /// An exception from the inner engine, which should be reported back to the development team.
    /// </summary>
    /// <seealso cref="Exception" />
    [Serializable]
    [PublicAPI]
    public class MathematicsEngineException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MathematicsEngineException"/> class.
        /// </summary>
        public MathematicsEngineException()
            : base(Resources.MathematicsEngineException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MathematicsEngineException"/> class.
        /// </summary>
        /// <param name="message">A custom message for the thrown exception.</param>
        public MathematicsEngineException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MathematicsEngineException"/> class.
        /// </summary>
        /// <param name="internalException">The internal exception, if any.</param>
        public MathematicsEngineException(Exception internalException)
            : base(Resources.MathematicsEngineException, internalException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MathematicsEngineException"/> class.
        /// </summary>
        /// <param name="message">A custom message for the thrown exception.</param>
        /// <param name="internalException">The internal exception, if any.</param>
        public MathematicsEngineException(string message, Exception internalException)
            : base(message, internalException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MathematicsEngineException"/> class.
        /// </summary>
        /// <param name="info">The <see cref="SerializationInfo" /> that holds the serialized object data about the exception being thrown.</param>
        /// <param name="context">The <see cref="StreamingContext" /> that contains contextual information about the source or destination.</param>
        protected MathematicsEngineException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}