// <copyright file="FunctionNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using IX.Math.Nodes.Constants;
using JetBrains.Annotations;

namespace IX.Math.Nodes
{
    /// <summary>
    /// A base class for a function node.
    /// </summary>
    /// <seealso cref="OperationNodeBase" />
    [PublicAPI]
    public abstract class FunctionNodeBase : OperationNodeBase
    {
        /// <summary>
        /// Prevents a default instance of the <see cref="FunctionNodeBase"/> class from being created.
        /// </summary>
        protected private FunctionNodeBase()
        {
        }

        /// <summary>
        /// Gets the concrete type of a parameter.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <returns>The parameter type.</returns>
        /// <exception cref="InvalidOperationException">The parameter could not be correctly recognized, or is undefined.</exception>
        protected static Type ParameterTypeFromParameter(NodeBase parameter)
        {
            Type parameterType;
            switch (parameter.ReturnType)
            {
                case SupportedValueType.Boolean:
                    parameterType = typeof(bool);
                    break;
                case SupportedValueType.ByteArray:
                    parameterType = typeof(byte[]);
                    break;
                case SupportedValueType.String:
                    parameterType = typeof(string);
                    break;
                case SupportedValueType.Numeric:
                    parameterType = parameter switch
                    {
                        ParameterNode nn => nn.IsFloat == false ? typeof(long) : typeof(double),
                        NumericNode cn => cn.IsFloat == false ? typeof(long) : typeof(double),
                        _ => typeof(double)
                    };
                    break;

                default:
                    throw new InvalidOperationException();
            }

            return parameterType;
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public new abstract NodeBase DeepClone(NodeCloningContext context);

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        protected override OperationNodeBase DeepCloneNode(NodeCloningContext context) => (OperationNodeBase)this.DeepClone(context);
    }
}