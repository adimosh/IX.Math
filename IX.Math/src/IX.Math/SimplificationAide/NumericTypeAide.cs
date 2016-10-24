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
                [typeof(sbyte)] = 1,
                [typeof(byte)] = 2,
                [typeof(char)] = 3,
                [typeof(short)] = 4,
                [typeof(ushort)] = 5,
                [typeof(int)] = 6,
                [typeof(uint)] = 7,
                [typeof(long)] = 8,
                [typeof(ulong)] = 9,
                [typeof(float)] = 10,
                [typeof(double)] = 11,
            };

            InverseNumericTypesConversionDictionary = NumericTypesConversionDictionary.ToDictionary(p => p.Value, p => p.Key);
        }
    }
}