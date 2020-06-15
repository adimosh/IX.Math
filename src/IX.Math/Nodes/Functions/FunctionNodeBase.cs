// <copyright file="FunctionNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Functions
{
    /// <summary>
    /// A base class for a function node.
    /// </summary>
    /// <seealso cref="NodeBase" />
    [PublicAPI]
    public abstract class FunctionNodeBase : NodeBase
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="FunctionNodeBase" /> class from being created.
        /// </summary>
        /// <param name="stringFormatters">The string formatters.</param>
        protected private FunctionNodeBase(List<IStringFormatter> stringFormatters)
        : base(stringFormatters)
        {
        }

        /// <summary>
        ///     Gets a value indicating whether or not this node is actually a constant. Since functions
        ///     can never be considered constant, this always returns false.
        /// </summary>
        /// <value><see langword="true" /> if the node is a constant, <see langword="false" /> otherwise.</value>
        public sealed override bool IsConstant => false;

        /// <summary>
        /// Gets the concrete type of a parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The parameter type.</returns>
        /// <exception cref="InvalidOperationException">The parameter could not be correctly recognized, or is undefined.</exception>
        //protected static Type ParameterTypeFromParameter(NodeBase parameter)
        //{
        //    Type parameterType;
        //    switch (parameter.ReturnType)
        //    {
        //        case SupportedValueType.Boolean:
        //            parameterType = typeof(bool);
        //            break;
        //        case SupportedValueType.ByteArray:
        //            parameterType = typeof(byte[]);
        //            break;
        //        case SupportedValueType.String:
        //            parameterType = typeof(string);
        //            break;
        //        case SupportedValueType.Numeric:
        //            parameterType = parameter switch
        //            {
        //                ParameterNode nn => nn.IsFloat == false ? typeof(long) : typeof(double),
        //                NumericNode cn => cn.IsFloat == false ? typeof(long) : typeof(double),
        //                _ => typeof(double)
        //            };
        //            break;

        //        default:
        //            throw new InvalidOperationException();
        //    }

        //    return parameterType;
        //}
    }
}