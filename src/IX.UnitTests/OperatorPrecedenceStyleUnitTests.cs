// <copyright file="OperatorPrecedenceStyleUnitTests.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using IX.Math;
using Xunit;

namespace IX.UnitTests
{
    /// <summary>
    /// Unit tests for operator precedence styles.
    /// </summary>
    public class OperatorPrecedenceStyleUnitTests
    {
        /// <summary>
        /// Tests mathematical and C-style operator precedence style.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        /// No computed expression was generated.
        /// </exception>
        [Fact(DisplayName = "Operator precedence style test")]
        public void Test1()
        {
            var expression = "true&true|false&false";

            object result1, result2;

            using (var service = new MathematicPortfolio())
            {
                result1 = service.Solve(expression);
            }

            using (var service = new MathematicPortfolio(new MathDefinition
            {
                Parentheses = new Tuple<string, string>("(", ")"),
                SpecialSymbolIndicators = new Tuple<string, string>("[", "]"),
                StringIndicator = "\"",
                ParameterSeparator = ",",
                AddSymbol = "+",
                AndSymbol = "&",
                DivideSymbol = "/",
                ModuloSymbol = "%",
                NotEqualsSymbol = "!=",
                EqualsSymbol = "=",
                MultiplySymbol = "*",
                NotSymbol = "!",
                OrSymbol = "|",
                PowerSymbol = "^",
                SubtractSymbol = "-",
                XorSymbol = "#",
                GreaterThanOrEqualSymbol = ">=",
                GreaterThanSymbol = ">",
                LessThanOrEqualSymbol = "<=",
                LessThanSymbol = "<",
                RightShiftSymbol = ">>",
                LeftShiftSymbol = "<<",
                OperatorPrecedenceStyle = OperatorPrecedenceStyle.CStyle,
                EscapeCharacter = "\\"
            }))
            {
                result2 = service.Solve(expression);
            }

            Assert.False((bool)result1);
            Assert.True((bool)result2);
        }
    }
}