// <copyright file="NumericTypeAide.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;

namespace IX.Math.SimplificationAide
{
    internal static class NumericTypeAide
    {
#pragma warning disable SA1401 // Fields must be private
        internal static Dictionary<Type, int> NumericTypesConversionDictionary;
        internal static Dictionary<int, Type> InverseNumericTypesConversionDictionary;
#pragma warning restore SA1401 // Fields must be private

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

            if (!NumericTypesConversionDictionary.TryGetValue(numericType, out int requestedTypeValue))
            {
                throw new InvalidOperationException(Resources.NumericTypeInvalid);
            }

            object[] convertedArguments = new object[arguments.Length];

            for (int i = 0; i < arguments.Length; i++)
            {
                object val = arguments[i];
                Type argType = val.GetType();
                if (!NumericTypesConversionDictionary.TryGetValue(argType, out int typeValue))
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

                if (!NumericTypesConversionDictionary.TryGetValue(numericType, out int numericTypeInt))
                {
                    throw new InvalidOperationException(Resources.NumericTypeInvalid);
                }

                Type currentType = argument.GetType();
                if (!NumericTypesConversionDictionary.TryGetValue(currentType, out int currentTypeInt))
                {
                    return;
                }

                if (currentTypeInt > numericTypeInt)
                {
                    numericType = InverseNumericTypesConversionDictionary[currentTypeInt];
                }
            }
        }
    }
}