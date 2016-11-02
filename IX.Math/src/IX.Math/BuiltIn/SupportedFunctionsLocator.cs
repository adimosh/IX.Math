using System;
using System.Collections.Generic;

namespace IX.Math.BuiltIn
{
    internal static class SupportedFunctionsLocator
    {
        internal static Dictionary<string, Func<ExpressionTreeNodeBase>> BuiltInFunctions;

        static SupportedFunctionsLocator()
        {
            BuiltInFunctions = new Dictionary<string, Func<ExpressionTreeNodeBase>>
            {
                ["abs"] = () => new BuiltInMathematicUnarySupportedFunction(nameof(System.Math.Abs)),
                ["acos"] = () => new BuiltInMathematicUnarySupportedFunction(nameof(System.Math.Acos)),
                ["asin"] = () => new BuiltInMathematicUnarySupportedFunction(nameof(System.Math.Asin)),
                ["atan"] = () => new BuiltInMathematicUnarySupportedFunction(nameof(System.Math.Atan)),
                ["ceiling"] = () => new BuiltInMathematicUnarySupportedFunction(nameof(System.Math.Ceiling)),
                ["cos"] = () => new BuiltInMathematicUnarySupportedFunction(nameof(System.Math.Cos)),
                ["cosh"] = () => new BuiltInMathematicUnarySupportedFunction(nameof(System.Math.Cosh)),
                ["exp"] = () => new BuiltInMathematicUnarySupportedFunction(nameof(System.Math.Exp)),
                ["floor"] = () => new BuiltInMathematicUnarySupportedFunction(nameof(System.Math.Floor)),
                ["ln"] = () => new BuiltInMathematicUnarySupportedFunction(nameof(System.Math.Log)),
                ["lg"] = () => new BuiltInMathematicUnarySupportedFunction(nameof(System.Math.Log10)),
                ["round"] = () => new BuiltInMathematicUnarySupportedFunction(nameof(System.Math.Round)),
                ["sin"] = () => new BuiltInMathematicUnarySupportedFunction(nameof(System.Math.Sin)),
                ["sinh"] = () => new BuiltInMathematicUnarySupportedFunction(nameof(System.Math.Sinh)),
                ["sqrt"] = () => new BuiltInMathematicUnarySupportedFunction(nameof(System.Math.Sqrt)),
                ["tan"] = () => new BuiltInMathematicUnarySupportedFunction(nameof(System.Math.Tan)),
                ["tanh"] = () => new BuiltInMathematicUnarySupportedFunction(nameof(System.Math.Tanh)),
                ["trun"] = () => new BuiltInMathematicUnarySupportedFunction(nameof(System.Math.Truncate)),
                ["log"] = () => new BuiltInMathematicBinarySupportedFunction(nameof(System.Math.Log)),
                ["pow"] = () => new BuiltInMathematicBinarySupportedFunction(nameof(System.Math.Pow)),
                ["min"] = () => new BuiltInMathematicBinarySupportedFunction(nameof(System.Math.Min)),
                ["max"] = () => new BuiltInMathematicBinarySupportedFunction(nameof(System.Math.Max)),
            };
        }
    }
}