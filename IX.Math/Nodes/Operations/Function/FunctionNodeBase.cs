// <copyright file="FunctionNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Function
{
    internal abstract class FunctionNodeBase : OperationNodeBase
    {
        protected static Type ParameterTypeFromParameter(NodeBase parameter)
        {
            Type parameterType;

            switch (parameter.ReturnType)
            {
                case SupportedValueType.Numeric:
                    {
                        switch (parameter)
                        {
                            case NumericParameterNode nn:
                                if (nn.RequireFloat == false)
                                {
                                    parameterType = typeof(long);
                                }
                                else
                                {
                                    parameterType = typeof(double);
                                }

                                break;
                            case NumericNode cn:
                                parameterType = cn.Value.GetType();
                                break;
                            default:
                                parameterType = typeof(long);
                                break;
                        }
                    }

                    break;
                case SupportedValueType.Boolean:
                    parameterType = typeof(bool);
                    break;
                case SupportedValueType.String:
                    parameterType = typeof(string);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return parameterType;
        }
    }
}