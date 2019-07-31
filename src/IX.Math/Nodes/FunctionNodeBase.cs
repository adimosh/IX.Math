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
                    {
                        switch (parameter)
                        {
                            case ParameterNode nn:
                                parameterType = nn.IsFloat == false ? typeof(long) : typeof(double);

                                break;
                            case NumericNode cn:
                                parameterType = cn.Value.GetType();
                                break;
                            default:
                                parameterType = typeof(double);
                                break;
                        }
                    }

                    break;

                default:
                    throw new InvalidOperationException();
            }

            return parameterType;
        }
    }
}