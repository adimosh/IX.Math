using IX.Math.SupportedFunctions;
using System.Collections.Generic;

namespace IX.Math
{
    internal static class SpecialSymbolsLocator
    {
        internal static Dictionary<string, SpecialSymbol> BuiltInSpecialSymbols;
        internal static Dictionary<string, string> BuiltInSpecialSymbolsAlternateWriting;

        static SpecialSymbolsLocator()
        {
            BuiltInSpecialSymbols = new Dictionary<string, SpecialSymbol>
            {
                ["e"] = new BuiltInMathematicSpecialSymbol("Euler-Napier constant (e)", System.Math.E),
                ["π"] = new BuiltInMathematicSpecialSymbol("Archimedes-Ludolph constant (pi)", System.Math.PI),
                ["φ"] = new BuiltInMathematicSpecialSymbol("Golden ratio", 1.6180339887498948),
                ["β"] = new BuiltInMathematicSpecialSymbol("Bernstein constant", 0.2801694990238691),
                ["γ"] = new BuiltInMathematicSpecialSymbol("Euler-Mascheroni constant", 0.5772156649015328),
                ["λ"] = new BuiltInMathematicSpecialSymbol("Gauss-Kuzmin-Wirsing constant", 0.3036630028987326),
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