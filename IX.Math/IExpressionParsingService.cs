﻿// <copyright file="IExpressionParsingService.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Threading;

namespace IX.Math
{
    /// <summary>
    /// A contract for a service that is able to parse strings containing mathematical expressions and solve them.
    /// </summary>
    public interface IExpressionParsingService
    {
        /// <summary>
        /// Interprets the mathematical expression and returns a container that can be invoked for solving using specific mathematical types.
        /// </summary>
        /// <param name="expression">The expression to interpret.</param>
        /// <param name="cancellationToken">The cancellation token for this operation.</param>
        /// <returns>A <see cref="ComputedExpression"/> that represents the interpreted expression.</returns>
        ComputedExpression Interpret(string expression, CancellationToken cancellationToken = default(CancellationToken));
    }
}