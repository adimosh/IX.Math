// <copyright file="ExternalParametersExtractor.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections;
using System.Text;
using IX.StandardExtensions;
using IX.StandardExtensions.Contracts;

namespace IX.Math.Extraction
{
    internal static class ExternalParametersExtractor
    {
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance",
            "HAA0301:Closure Allocation Source",
            Justification = "We are actively looking for a closure in this method.")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance",
            "HAA0302:Display class allocation to capture closure",
            Justification = "We are actively looking for a closure in this method.")]
        internal static object[] ComputeToStandard(this object[] existingParameters)
        {
            Requires.NotNull(
                existingParameters,
                nameof(existingParameters));

            object[]? newParameters = null;
            var length = existingParameters.Length;
            for (int i = 0; i < length; i++)
            {
                var par = existingParameters[i];
                switch (par)
                {
#region Values

                    case DynamicVariableValue dvv:
                    {
                        if (newParameters != null)
                        {
                            newParameters[i] = dvv;
                        }

                        break;
                    }

                    case string s:
                    {
                        if (newParameters != null)
                        {
                            newParameters[i] = s;
                        }

                        break;
                    }

                    case long l:
                    {
                        if (newParameters != null)
                        {
                            newParameters[i] = l;
                        }

                        break;
                    }

                    case double d:
                    {
                        if (newParameters != null)
                        {
                            newParameters[i] = d;
                        }

                        break;
                    }

                    case byte[] ba:
                    {
                        if (newParameters != null)
                        {
                            newParameters[i] = ba;
                        }

                        break;
                    }

                    case bool b:
                    {
                        if (newParameters != null)
                        {
                            newParameters[i] = b;
                        }

                        break;
                    }

#endregion

#region Functions

                    case Func<DynamicVariableValue> dvv:
                    {
                        if (newParameters != null)
                        {
                            newParameters[i] = dvv;
                        }

                        break;
                    }

                    case Func<string> s:
                    {
                        if (newParameters != null)
                        {
                            newParameters[i] = s;
                        }

                        break;
                    }

                    case Func<long> l:
                    {
                        if (newParameters != null)
                        {
                            newParameters[i] = l;
                        }

                        break;
                    }

                    case Func<double> d:
                    {
                        if (newParameters != null)
                        {
                            newParameters[i] = d;
                        }

                        break;
                    }

                    case Func<byte[]> ba:
                    {
                        if (newParameters != null)
                        {
                            newParameters[i] = ba;
                        }

                        break;
                    }

                    case Func<bool> b:
                    {
                        if (newParameters != null)
                        {
                            newParameters[i] = b;
                        }

                        break;
                    }

#endregion

                    default:
                    {
                        if (newParameters == null)
                        {
                            newParameters = new object[length];

                            for (int j = 0; j < i; j++)
                            {
                                newParameters[j] = existingParameters[j];
                            }
                        }

                        switch (par)
                        {
#region Convertible values

                            case byte b:
                            {
                                newParameters[i] = Convert.ToInt64(b);
                                break;
                            }

                            case sbyte b:
                            {
                                newParameters[i] = Convert.ToInt64(b);
                                break;
                            }

                            case short b:
                            {
                                newParameters[i] = Convert.ToInt64(b);
                                break;
                            }

                            case ushort b:
                            {
                                newParameters[i] = Convert.ToInt64(b);
                                break;
                            }

                            case int b:
                            {
                                newParameters[i] = Convert.ToInt64(b);
                                break;
                            }

                            case uint b:
                            {
                                newParameters[i] = Convert.ToInt64(b);
                                break;
                            }

                            case ulong b:
                            {
                                newParameters[i] = Convert.ToDouble(b);
                                break;
                            }

                            case float b:
                            {
                                newParameters[i] = Convert.ToDouble(b);
                                break;
                            }

                            case StringBuilder b:
                            {
                                newParameters[i] = b.ToString();
                                break;
                            }

                            case BitArray b:
                            {
                                int bLength = b.Length / 8;
                                if (b.Length % 8 != 0)
                                {
                                    bLength++;
                                }

                                byte[] resultingArray = new byte[bLength];

                                b.CopyTo(
                                    resultingArray,
                                    0);
                                newParameters[i] = resultingArray;
                                break;
                            }

#endregion

#region Convertible functions

                            case Func<byte> b:
                            {
                                newParameters[i] = new Func<long>(() => Convert.ToInt64(b()));
                                break;
                            }

                            case Func<sbyte> b:
                            {
                                newParameters[i] = new Func<long>(() => Convert.ToInt64(b()));
                                break;
                            }

                            case Func<short> b:
                            {
                                newParameters[i] = new Func<long>(() => Convert.ToInt64(b()));
                                break;
                            }

                            case Func<ushort> b:
                            {
                                newParameters[i] = new Func<long>(() => Convert.ToInt64(b()));
                                break;
                            }

                            case Func<int> b:
                            {
                                newParameters[i] = new Func<long>(() => Convert.ToInt64(b()));
                                break;
                            }

                            case Func<uint> b:
                            {
                                newParameters[i] = new Func<long>(() => Convert.ToInt64(b()));
                                break;
                            }

                            case Func<ulong> b:
                            {
                                newParameters[i] = new Func<double>(() => Convert.ToDouble(b()));
                                break;
                            }

                            case Func<float> b:
                            {
                                newParameters[i] = new Func<double>(() => Convert.ToDouble(b()));
                                break;
                            }

                            case Func<StringBuilder> b:
                            {
                                newParameters[i] = new Func<string>(
                                    () => b()
                                              ?.ToString() ??
                                          string.Empty);
                                break;
                            }

                            case Func<BitArray> b:
                            {
                                newParameters[i] = new Func<byte[]>(
                                    () =>
                                    {
                                        BitArray ba = b();
                                        if (ba == null)
                                        {
                                            return Array.Empty<byte>();
                                        }

                                        int bLength = ba.Length / 8;
                                        if (ba.Length % 8 != 0)
                                        {
                                            bLength++;
                                        }

                                        byte[] resultingArray = new byte[bLength];

                                        ba.CopyTo(
                                            resultingArray,
                                            0);
                                        return resultingArray;
                                    });
                                break;
                            }

#endregion

                            default:
                                throw new ArgumentInvalidTypeException($"{nameof(existingParameters)}[{i}]");
                        }

                        break;
                    }
                }
            }

            return newParameters ?? existingParameters;
        }
    }
}