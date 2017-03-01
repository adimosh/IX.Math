// <copyright file="NumericFormatter.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Formatters
{
    internal static class NumericFormatter
    {
        internal static object[] FormatArgumentsAccordingToParameters(IEnumerable<object> parameterValues, IEnumerable<ParameterNodeBase> parameters)
        {
            object[] finalValues = new object[parameterValues.Count()];

            int i = 0;

            var pve = parameterValues.GetEnumerator();
            var pe = parameters.GetEnumerator();
            while (i < finalValues.Length)
            {
                if (!pe.MoveNext())
                {
                    break;
                }

                if (!pve.MoveNext())
                {
                    throw new InvalidOperationException();
                }

                switch (pe.Current)
                {
                    case NumericParameterNode n:
                        if (n.RequireFloat == false)
                        {
                            finalValues[i] = Convert.ToInt64(pve.Current);
                        }
                        else
                        {
                            finalValues[i] = Convert.ToDouble(pve.Current);
                        }

                        break;
                    case StringParameterNode s:
                        finalValues[i] = pve.Current.ToString();
                        break;
                    case BoolParameterNode b:
                        switch (pve.Current)
                        {
                            case bool be:
                                finalValues[i] = be;
                                break;
                            case byte bbe:
                                finalValues[i] = bbe != 0;
                                break;
                            case sbyte bbe:
                                finalValues[i] = bbe != 0;
                                break;
                            case short bbe:
                                finalValues[i] = bbe != 0;
                                break;
                            case char bbe:
                                finalValues[i] = bbe != 0;
                                break;
                            case ushort bbe:
                                finalValues[i] = bbe != 0;
                                break;
                            case int bbe:
                                finalValues[i] = bbe != 0;
                                break;
                            case uint bbe:
                                finalValues[i] = bbe != 0;
                                break;
                            case long bbe:
                                finalValues[i] = bbe != 0;
                                break;
                            case ulong bbe:
                                finalValues[i] = bbe != 0;
                                break;
                            case float bbe:
                                finalValues[i] = bbe != 0;
                                break;
                            case double bbe:
                                finalValues[i] = bbe != 0;
                                break;
                            case string se:
                                if (bool.TryParse(se, out bool bbb3))
                                {
                                    finalValues[i] = bbb3;
                                }
                                else
                                {
                                    throw new InvalidCastException();
                                }

                                break;
                            default:
                                throw new InvalidCastException();
                        }

                        break;
                    default:
                        throw new InvalidCastException();
                }

                i++;
            }

            return finalValues;
        }
    }
}