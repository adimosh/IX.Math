using System;
using System.Collections.Generic;
using System.Linq;

namespace IX.Math.SimplificationAide
{
    internal static class NumericTypeAide
    {
        internal static Dictionary<Type, int> NumericTypesConversionDictionary;

        internal static Dictionary<int, Type> InverseNumericTypesConversionDictionary;

        static NumericTypeAide()
        {
            NumericTypesConversionDictionary = new Dictionary<Type, int>
            {
                [typeof(int)] = 1,
                [typeof(long)] = 2,
                [typeof(float)] = 3,
                [typeof(double)] = 4,
            };

            InverseNumericTypesConversionDictionary = NumericTypesConversionDictionary.ToDictionary(p => p.Value, p => p.Key);
        }

        internal static object[] GetProperNumericTypeValues(object[] arguments, Type numericType)
        {
            if (arguments == null || arguments.Length == 0)
            {
                return new object[0];
            }

            int requestedTypeValue;
            if (!NumericTypesConversionDictionary.TryGetValue(numericType, out requestedTypeValue))
            {
                throw new InvalidOperationException(Resources.NumericTypeInvalid);
            }

            object[] convertedArguments = new object[arguments.Length];

            for (int i = 0; i < arguments.Length; i++)
            {
                object val = arguments[i];
                Type argType = val.GetType();

                int typeValue;
                if (!NumericTypesConversionDictionary.TryGetValue(argType, out typeValue))
                {
                    convertedArguments[i] = val;
                    continue;
                }

                if (typeValue > requestedTypeValue)
                {
                    throw new ExpressionNotValidLogicallyException(Resources.NumericTypeMismatched);
                }

                if (typeValue < requestedTypeValue)
                {
                    convertedArguments[i] = Convert.ChangeType(val, numericType);
                }
                else
                {
                    convertedArguments[i] = val;
                }
            }

            return convertedArguments;
        }

        internal static void GetProperRequestedNumericalType(object[] arguments, ref Type numericType)
        {
            foreach (var argument in arguments)
            {
                if (argument == null)
                {
                    throw new ArgumentNullException(nameof(arguments));
                }

                int numericTypeInt;
                if (!NumericTypesConversionDictionary.TryGetValue(numericType, out numericTypeInt))
                {
                    throw new InvalidOperationException(Resources.NumericTypeInvalid);
                }

                Type currentType = argument.GetType();

                int currentTypeInt;
                if (!NumericTypesConversionDictionary.TryGetValue(currentType, out currentTypeInt))
                {
                    return;
                }

                if (currentTypeInt > numericTypeInt)
                {
                    numericType = InverseNumericTypesConversionDictionary[currentTypeInt];
                }
            }
        }

        internal static object[] GetValuesFromFinder(IEnumerable<Tuple<string, Type>> externalParameters, IDataFinder dataFinder)
        {
            List<object> parameterValues = new List<object>();

            foreach (var v in externalParameters)
            {
                object data;
                if (dataFinder.TryGetData(v.Item1, out data))
                {
                    try
                    {
                        if (v.Item2 != data.GetType())
                        {
                            data = Convert.ChangeType(data, v.Item2);
                        }

                        parameterValues.Add(data);
                    }
                    catch
                    {
                        parameterValues.Add(v.Item1);
                    }
                }
                else
                {
                    parameterValues.Add(v.Item1);
                }
            }

            return parameterValues.ToArray();
        }
    }
}