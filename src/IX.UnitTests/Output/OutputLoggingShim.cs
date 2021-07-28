// <copyright file="OutputLoggingShim.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using IX.Abstractions.Logging;
using IX.StandardExtensions.Contracts;
using Xunit.Abstractions;

namespace IX.UnitTests.Output
{
    /// <summary>
    /// An output logging shim for xUnit.
    /// </summary>
    public class OutputLoggingShim : ILog
    {
        private readonly ITestOutputHelper outputHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="OutputLoggingShim"/> class.
        /// </summary>
        /// <param name="outputHelper">The output helper to shim on.</param>
        public OutputLoggingShim(ITestOutputHelper outputHelper)
        {
            Requires.NotNull(out this.outputHelper, outputHelper, nameof(outputHelper));
        }

        /// <summary>Logs a debug message.</summary>
        /// <param name="message">The message to log.</param>
        public void Debug(string message) => this.outputHelper.WriteLine($"DEBUG: {message}");

        /// <summary>Logs a debug message.</summary>
        /// <param name="message">The message to log.</param>
        /// <param name="formatParameters">The string format parameters.</param>
        public void Debug(
            string message,
            params string[] formatParameters) =>
            this.outputHelper.WriteLine($"DEBUG: {message}", formatParameters);

        /// <summary>Logs a debug message.</summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message to log.</param>
        public void Debug(
            Exception exception,
            string message)
        {
            this.outputHelper.WriteLine($"DEBUG EXCEPTION: {message}");
            this.outputHelper.WriteLine(@$"DEBUG EXCEPTION DETAILS:
TYPE: {exception.GetType().FullName}
SOURCE: {exception.Source}
MESSAGE: {exception.Message}
STACK TRACE: {exception.StackTrace}");
        }

        /// <summary>Logs a debug message.</summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="formatParameters">The string format parameters.</param>
        public void Debug(
            Exception exception,
            string message,
            params string[] formatParameters)
        {
            this.outputHelper.WriteLine($"DEBUG EXCEPTION: {message}", formatParameters);
            this.outputHelper.WriteLine(@$"DEBUG EXCEPTION DETAILS:
TYPE: {exception.GetType().FullName}
SOURCE: {exception.Source}
MESSAGE: {exception.Message}
STACK TRACE: {exception.StackTrace}");
        }

        /// <summary>Logs an informational message.</summary>
        /// <param name="message">The message to log.</param>
        public void Info(string message) => this.outputHelper.WriteLine($"INFO: {message}");

        /// <summary>Logs an informational message.</summary>
        /// <param name="message">The message to log.</param>
        /// <param name="formatParameters">The string format parameters.</param>
        public void Info(
            string message,
            params string[] formatParameters) =>
            this.outputHelper.WriteLine($"INFO: {message}", formatParameters);

        /// <summary>Logs an informational message.</summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message to log.</param>
        public void Info(
            Exception exception,
            string message)
        {
            this.outputHelper.WriteLine($"INFO EXCEPTION: {message}");
            this.outputHelper.WriteLine(@$"INFO EXCEPTION DETAILS:
TYPE: {exception.GetType().FullName}
SOURCE: {exception.Source}
MESSAGE: {exception.Message}
STACK TRACE: {exception.StackTrace}");
        }

        /// <summary>Logs an informational message.</summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="formatParameters">The string format parameters.</param>
        public void Info(
            Exception exception,
            string message,
            params string[] formatParameters)
        {
            this.outputHelper.WriteLine($"INFO EXCEPTION: {message}", formatParameters);
            this.outputHelper.WriteLine(@$"INFO EXCEPTION DETAILS:
TYPE: {exception.GetType().FullName}
SOURCE: {exception.Source}
MESSAGE: {exception.Message}
STACK TRACE: {exception.StackTrace}");
        }

        /// <summary>Logs a warning message.</summary>
        /// <param name="message">The message to log.</param>
        public void Warning(string message) => this.outputHelper.WriteLine($"WARNING: {message}");

        /// <summary>Logs a warning message.</summary>
        /// <param name="message">The message to log.</param>
        /// <param name="formatParameters">The string format parameters.</param>
        public void Warning(
            string message,
            params string[] formatParameters) =>
            this.outputHelper.WriteLine($"WARNING: {message}", formatParameters);

        /// <summary>Logs a warning message.</summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message to log.</param>
        public void Warning(
            Exception exception,
            string message)
        {
            this.outputHelper.WriteLine($"WARNING EXCEPTION: {message}");
            this.outputHelper.WriteLine(@$"WARNING EXCEPTION DETAILS:
TYPE: {exception.GetType().FullName}
SOURCE: {exception.Source}
MESSAGE: {exception.Message}
STACK TRACE: {exception.StackTrace}");
        }

        /// <summary>Logs a warning message.</summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="formatParameters">The string format parameters.</param>
        public void Warning(
            Exception exception,
            string message,
            params string[] formatParameters)
        {
            this.outputHelper.WriteLine($"WARNING EXCEPTION: {message}", formatParameters);
            this.outputHelper.WriteLine(@$"WARNING EXCEPTION DETAILS:
TYPE: {exception.GetType().FullName}
SOURCE: {exception.Source}
MESSAGE: {exception.Message}
STACK TRACE: {exception.StackTrace}");
        }

        /// <summary>Logs an error message.</summary>
        /// <param name="message">The message to log.</param>
        public void Error(string message) => this.outputHelper.WriteLine($"ERROR: {message}");

        /// <summary>Logs an error message.</summary>
        /// <param name="message">The message to log.</param>
        /// <param name="formatParameters">The string format parameters.</param>
        public void Error(
            string message,
            params string[] formatParameters) =>
            this.outputHelper.WriteLine($"ERROR: {message}", formatParameters);

        /// <summary>Logs an error message.</summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message to log.</param>
        public void Error(
            Exception exception,
            string message)
        {
            this.outputHelper.WriteLine($"ERROR EXCEPTION: {message}");
            this.outputHelper.WriteLine(@$"DEBUG EXCEPTION DETAILS:
TYPE: {exception.GetType().FullName}
SOURCE: {exception.Source}
MESSAGE: {exception.Message}
STACK TRACE: {exception.StackTrace}");
        }

        /// <summary>Logs an error message.</summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="formatParameters">The string format parameters.</param>
        public void Error(
            Exception exception,
            string message,
            params string[] formatParameters)
        {
            this.outputHelper.WriteLine($"ERROR EXCEPTION: {message}", formatParameters);
            this.outputHelper.WriteLine(@$"ERROR EXCEPTION DETAILS:
TYPE: {exception.GetType().FullName}
SOURCE: {exception.Source}
MESSAGE: {exception.Message}
STACK TRACE: {exception.StackTrace}");
        }

        /// <summary>Logs a critical error message.</summary>
        /// <param name="message">The message to log.</param>
        public void Critical(string message) => this.outputHelper.WriteLine($"CRITICAL: {message}");

        /// <summary>Logs a critical error message.</summary>
        /// <param name="message">The message to log.</param>
        /// <param name="formatParameters">The string format parameters.</param>
        public void Critical(
            string message,
            params string[] formatParameters) =>
            this.outputHelper.WriteLine($"CRITICAL: {message}", formatParameters);

        /// <summary>Logs a critical error message.</summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message to log.</param>
        public void Critical(
            Exception exception,
            string message)
        {
            this.outputHelper.WriteLine($"CRITICAL EXCEPTION: {message}");
            this.outputHelper.WriteLine(@$"CRITICAL EXCEPTION DETAILS:
TYPE: {exception.GetType().FullName}
SOURCE: {exception.Source}
MESSAGE: {exception.Message}
STACK TRACE: {exception.StackTrace}");
        }

        /// <summary>Logs a critical error message.</summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="formatParameters">The string format parameters.</param>
        public void Critical(
            Exception exception,
            string message,
            params string[] formatParameters)
        {
            this.outputHelper.WriteLine($"CRITICAL EXCEPTION: {message}", formatParameters);
            this.outputHelper.WriteLine(@$"CRITICAL EXCEPTION DETAILS:
TYPE: {exception.GetType().FullName}
SOURCE: {exception.Source}
MESSAGE: {exception.Message}
STACK TRACE: {exception.StackTrace}");
        }

        /// <summary>Logs a fatal error message.</summary>
        /// <param name="message">The message to log.</param>
        public void Fatal(string message) => this.outputHelper.WriteLine($"FATAL: {message}");

        /// <summary>Logs a fatal error message.</summary>
        /// <param name="message">The message to log.</param>
        /// <param name="formatParameters">The string format parameters.</param>
        public void Fatal(
            string message,
            params string[] formatParameters) =>
            this.outputHelper.WriteLine($"FATAL: {message}", formatParameters);

        /// <summary>Logs a fatal error message.</summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message to log.</param>
        public void Fatal(
            Exception exception,
            string message)
        {
            this.outputHelper.WriteLine($"FATAL EXCEPTION: {message}");
            this.outputHelper.WriteLine(@$"FATAL EXCEPTION DETAILS:
TYPE: {exception.GetType().FullName}
SOURCE: {exception.Source}
MESSAGE: {exception.Message}
STACK TRACE: {exception.StackTrace}");
        }

        /// <summary>Logs a fatal error message.</summary>
        /// <param name="exception">The exception.</param>
        /// <param name="message">The message to log.</param>
        /// <param name="formatParameters">The string format parameters.</param>
        public void Fatal(
            Exception exception,
            string message,
            params string[] formatParameters)
        {
            this.outputHelper.WriteLine($"FATAL EXCEPTION: {message}", formatParameters);
            this.outputHelper.WriteLine(@$"FATAL EXCEPTION DETAILS:
TYPE: {exception.GetType().FullName}
SOURCE: {exception.Source}
MESSAGE: {exception.Message}
STACK TRACE: {exception.StackTrace}");
        }
    }
}