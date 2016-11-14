﻿using System.Collections.Generic;

namespace IX.Math.BuiltIn
{
    internal static class SpecialSymbolsLocator
    {
        internal static Dictionary<string, ExpressionTreeNodeMathematicSpecialSymbol> BuiltInSpecialSymbols;
        internal static Dictionary<string, string> BuiltInSpecialSymbolsAlternateWriting;

        static SpecialSymbolsLocator()
        {
            BuiltInSpecialSymbols = new Dictionary<string, ExpressionTreeNodeMathematicSpecialSymbol>
            {
                ["e"] = new ExpressionTreeNodeMathematicSpecialSymbol("Euler-Napier constant (e)", System.Math.E),
                ["π"] = new ExpressionTreeNodeMathematicSpecialSymbol("Archimedes-Ludolph constant (pi)", System.Math.PI),
                ["φ"] = new ExpressionTreeNodeMathematicSpecialSymbol("Golden ratio", 1.6180339887498948),
                ["β"] = new ExpressionTreeNodeMathematicSpecialSymbol("Bernstein constant", 0.2801694990238691),
                ["γ"] = new ExpressionTreeNodeMathematicSpecialSymbol("Euler-Mascheroni constant", 0.5772156649015328),
                ["λ"] = new ExpressionTreeNodeMathematicSpecialSymbol("Gauss-Kuzmin-Wirsing constant", 0.3036630028987326),
            };

            BuiltInSpecialSymbolsAlternateWriting = new Dictionary<string, string>
            {
                ["pi"] = "π",
                ["phi"] = "φ",
                ["beta"] = "β",
                ["gamma"] = "γ",
                ["lambda"] = "λ"
            };
        }
    }
}