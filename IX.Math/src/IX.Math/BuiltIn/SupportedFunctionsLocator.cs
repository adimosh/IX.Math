﻿using System;
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
                ["abs"] = () => new ExpressionTreeNodeMathematicUnarySupportedFunction(nameof(System.Math.Abs)),
                ["acos"] = () => new ExpressionTreeNodeMathematicUnarySupportedFunction(nameof(System.Math.Acos)),
                ["asin"] = () => new ExpressionTreeNodeMathematicUnarySupportedFunction(nameof(System.Math.Asin)),
                ["atan"] = () => new ExpressionTreeNodeMathematicUnarySupportedFunction(nameof(System.Math.Atan)),
                ["ceiling"] = () => new ExpressionTreeNodeMathematicUnarySupportedFunction(nameof(System.Math.Ceiling)),
                ["cos"] = () => new ExpressionTreeNodeMathematicUnarySupportedFunction(nameof(System.Math.Cos)),
                ["cosh"] = () => new ExpressionTreeNodeMathematicUnarySupportedFunction(nameof(System.Math.Cosh)),
                ["exp"] = () => new ExpressionTreeNodeMathematicUnarySupportedFunction(nameof(System.Math.Exp)),
                ["floor"] = () => new ExpressionTreeNodeMathematicUnarySupportedFunction(nameof(System.Math.Floor)),
                ["ln"] = () => new ExpressionTreeNodeMathematicUnarySupportedFunction(nameof(System.Math.Log)),
                ["lg"] = () => new ExpressionTreeNodeMathematicUnarySupportedFunction(nameof(System.Math.Log10)),
                ["round"] = () => new ExpressionTreeNodeMathematicUnarySupportedFunction(nameof(System.Math.Round)),
                ["sin"] = () => new ExpressionTreeNodeMathematicUnarySupportedFunction(nameof(System.Math.Sin)),
                ["sinh"] = () => new ExpressionTreeNodeMathematicUnarySupportedFunction(nameof(System.Math.Sinh)),
                ["sqrt"] = () => new ExpressionTreeNodeMathematicUnarySupportedFunction(nameof(System.Math.Sqrt)),
                ["tan"] = () => new ExpressionTreeNodeMathematicUnarySupportedFunction(nameof(System.Math.Tan)),
                ["tanh"] = () => new ExpressionTreeNodeMathematicUnarySupportedFunction(nameof(System.Math.Tanh)),
                ["trun"] = () => new ExpressionTreeNodeMathematicUnarySupportedFunction(nameof(System.Math.Truncate)),
                ["log"] = () => new ExpressionTreeNodeMathematicBinarySupportedFunction(nameof(System.Math.Log)),
                ["pow"] = () => new ExpressionTreeNodeMathematicBinarySupportedFunction(nameof(System.Math.Pow)),
                ["min"] = () => new ExpressionTreeNodeMathematicBinarySupportedFunction(nameof(System.Math.Min)),
                ["max"] = () => new ExpressionTreeNodeMathematicBinarySupportedFunction(nameof(System.Math.Max)),
                ["strlen"] = () => new ExpressionTreeNodeStringPropertySupportedFunction(nameof(string.Length)),
            };
        }
    }
}