// <copyright file="FixtureCreateDisposePatternHelper.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using IX.Math;
using IX.StandardExtensions.Contracts;

namespace IX.UnitTests.Helpers
{
    /// <summary>
    /// A helper to implement the fixture service create/dispose pattern.
    /// </summary>
    /// <seealso cref="IDisposable" />
    internal class FixtureCreateDisposePatternHelper : IDisposable
    {
        private readonly Action<IExpressionParsingService> dispose;

        /// <summary>
        /// Initializes a new instance of the <see cref="FixtureCreateDisposePatternHelper"/> class.
        /// </summary>
        /// <param name="fixture">The fixture.</param>
        /// <param name="create">The create.</param>
        /// <param name="dispose">The dispose.</param>
        public FixtureCreateDisposePatternHelper(
            CachedExpressionProviderFixture fixture,
            Func<CachedExpressionProviderFixture, IExpressionParsingService> create,
            Action<IExpressionParsingService> dispose)
        {
            Requires.NotNull(fixture, nameof(fixture));
            Requires.NotNull(create, nameof(create));

            this.Service = create(fixture);

            this.dispose = dispose;
        }

        /// <summary>
        /// Gets the service.
        /// </summary>
        /// <value>
        /// The service.
        /// </value>
        public IExpressionParsingService Service { get; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.Service != null)
            {
                dispose?.Invoke(this.Service);
            }
        }
    }
}