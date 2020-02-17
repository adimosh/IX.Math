// <copyright file="IConstantInterpreter.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using IX.Math.Nodes;
using JetBrains.Annotations;

namespace IX.Math.Extensibility
{
    /// <summary>
    ///     A service contract for a class that can evaluate and interpret possible constants.
    /// </summary>
    [PublicAPI]
    public interface IConstantInterpreter
    {
        /// <summary>
        ///     Evaluates part of an expression, determining whether or not it is a constant.
        /// </summary>
        /// <param name="expressionPart">The expression part.</param>
        /// <returns>
        ///     <c>true</c>, along with the evaluated node, if the expression part correctly evaluates to a constant, <c>false</c>
        ///     otherwise.
        /// </returns>
        /// <remarks>
        ///     <para>
        ///         The part evaluation phase happens after constants extraction and after the expression has been split into
        ///         component parts. If you require the whole expression to be evaluated (which might include symbols otherwise
        ///         recognizable as operators), you should use the <see cref="IConstantsExtractor" /> interface instead.
        ///     </para>
        ///     <para>
        ///         A correctly-recognized constant will only be asked for once. Any subsequent discoveries of the same constant
        ///         will result in the same value used.
        ///     </para>
        ///     <para>
        ///         An expression part that is not recognized will flow down to other interpreters, and, ultimately, to the
        ///         standard formatters.
        ///     </para>
        /// </remarks>
        (bool Success, ConstantNodeBase Value) EvaluateIsConstant(string expressionPart);
    }
}