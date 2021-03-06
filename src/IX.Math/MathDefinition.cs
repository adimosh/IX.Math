// <copyright file="MathDefinition.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Runtime.Serialization;
using IX.Abstractions.Logging;
using IX.Math.Extensibility;
using IX.StandardExtensions;
using IX.StandardExtensions.Contracts;

namespace IX.Math
{
    /// <summary>
    /// A definition for signs and symbols used in expression parsing of a mathematical expression.
    /// </summary>
    [DataContract]
    public class MathDefinition : IDeepCloneable<MathDefinition>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MathDefinition"/> class.
        /// </summary>
        public MathDefinition()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MathDefinition"/> class.
        /// </summary>
        /// <param name="definition">The definition to use.</param>
        public MathDefinition(MathDefinition definition)
        {
            definition = Requires.NotNull(definition, nameof(definition));

            this.Parentheses = definition.Parentheses;
            this.SpecialSymbolIndicators = definition.SpecialSymbolIndicators;
            this.IndexerIndicators = definition.IndexerIndicators;
            this.StringIndicator = definition.StringIndicator;
            this.ParameterSeparator = definition.ParameterSeparator;
            this.AddSymbol = definition.AddSymbol;
            this.AndSymbol = definition.AndSymbol;
            this.DivideSymbol = definition.DivideSymbol;
            this.ModuloSymbol = definition.ModuloSymbol;
            this.NotEqualsSymbol = definition.NotEqualsSymbol;
            this.EqualsSymbol = definition.EqualsSymbol;
            this.GreaterThanOrEqualSymbol = definition.GreaterThanOrEqualSymbol;
            this.GreaterThanSymbol = definition.GreaterThanSymbol;
            this.LessThanOrEqualSymbol = definition.LessThanOrEqualSymbol;
            this.LessThanSymbol = definition.LessThanSymbol;
            this.MultiplySymbol = definition.MultiplySymbol;
            this.NotSymbol = definition.NotSymbol;
            this.OrSymbol = definition.OrSymbol;
            this.PowerSymbol = definition.PowerSymbol;
            this.LeftShiftSymbol = definition.LeftShiftSymbol;
            this.RightShiftSymbol = definition.RightShiftSymbol;
            this.SubtractSymbol = definition.SubtractSymbol;
            this.XorSymbol = definition.XorSymbol;
            this.AutoConvertStringFormatSpecifier = definition.AutoConvertStringFormatSpecifier;
            this.EscapeCharacter = definition.EscapeCharacter;
            this.OperatorPrecedenceStyle = definition.OperatorPrecedenceStyle;
            this.PassThroughStateContainer = definition.PassThroughStateContainer;
        }

        /// <summary>
        /// Gets a copy of the default math definition.
        /// </summary>
        /// <value>
        /// The default math definition.
        /// </value>
        public static MathDefinition Default => new MathDefinition
        {
            Parentheses = ("(", ")"),
            SpecialSymbolIndicators = ("[", "]"),
            IndexerIndicators = ("[", "]"),
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
            PowerSymbol = "#",
            SubtractSymbol = "-",
            XorSymbol = "^",
            GreaterThanOrEqualSymbol = ">=",
            GreaterThanSymbol = ">",
            LessThanOrEqualSymbol = "<=",
            LessThanSymbol = "<",
            RightShiftSymbol = ">>",
            LeftShiftSymbol = "<<",
            OperatorPrecedenceStyle = OperatorPrecedenceStyle.Mathematical,
            EscapeCharacter = "\\",
        };

        /// <summary>
        /// Gets or sets what should be interpreted as parentheses.
        /// </summary>
        /// <value>The parentheses indicators.</value>
        /// <remarks>The first item in the tuple represents the opening parenthesis, whereas the second represents the closing parenthesis.</remarks>
        [DataMember]
        public (string Open, string Close) Parentheses { get; set; }

        /// <summary>
        /// Gets or sets what should be interpreted as special symbols.
        /// </summary>
        /// <value>The special symbol indicators.</value>
        /// <remarks>The first item in the tuple represents the opening of the special symbol marker, whereas the second represents its closing.</remarks>
        [DataMember]
        public (string Open, string Close) SpecialSymbolIndicators { get; set; }

        /// <summary>
        /// Gets or sets what should be interpreted as special symbols.
        /// </summary>
        /// <value>The special symbol indicators.</value>
        /// <remarks>The first item in the tuple represents the opening of the special symbol marker, whereas the second represents its closing.</remarks>
        [DataMember]
        public (string Open, string Close) IndexerIndicators { get; set; }

        /// <summary>
        /// Gets or sets what should be interpreted as string markers.
        /// </summary>
        /// <value>The string indicator.</value>
        [DataMember]
        public string StringIndicator { get; set; }

        /// <summary>
        /// Gets or sets what should be interpreted as parameter separators in multi-parameter function calls.
        /// </summary>
        /// <value>The parameter separator.</value>
        [DataMember]
        public string ParameterSeparator { get; set; }

        /// <summary>
        /// Gets or sets a symbol for the addition operation.
        /// </summary>
        /// <value>The add symbol.</value>
        [DataMember]
        public string AddSymbol { get; set; }

        /// <summary>
        /// Gets or sets a symbol for the subtraction operation.
        /// </summary>
        /// <value>The subtract symbol.</value>
        [DataMember]
        public string SubtractSymbol { get; set; }

        /// <summary>
        /// Gets or sets a symbol for the multiplication operation.
        /// </summary>
        /// <value>The multiply symbol.</value>
        [DataMember]
        public string MultiplySymbol { get; set; }

        /// <summary>
        /// Gets or sets a symbol for the division operation.
        /// </summary>
        /// <value>The divide symbol.</value>
        [DataMember]
        public string DivideSymbol { get; set; }

        /// <summary>
        /// Gets or sets a symbol for the modulo operation.
        /// </summary>
        /// <value>The divide symbol.</value>
        [DataMember]
        public string ModuloSymbol { get; set; }

        /// <summary>
        /// Gets or sets a symbol for the power operation.
        /// </summary>
        /// <value>The power symbol.</value>
        [DataMember]
        public string PowerSymbol { get; set; }

        /// <summary>
        /// Gets or sets a symbol for the &quot;and&quot; logical operation.
        /// </summary>
        /// <value>The and symbol.</value>
        [DataMember]
        public string AndSymbol { get; set; }

        /// <summary>
        /// Gets or sets a symbol for the &quot;or&quot; logical operation.
        /// </summary>
        /// <value>The or symbol.</value>
        [DataMember]
        public string OrSymbol { get; set; }

        /// <summary>
        /// Gets or sets a symbol for the &quot;xor&quot; logical operation.
        /// </summary>
        /// <value>The xor symbol.</value>
        [DataMember]
        public string XorSymbol { get; set; }

        /// <summary>
        /// Gets or sets a symbol for the &quot;not&quot; logical operation.
        /// </summary>
        /// <value>The not symbol.</value>
        [DataMember]
        public string NotSymbol { get; set; }

        /// <summary>
        /// Gets or sets a symbol for a comparison of equality.
        /// </summary>
        /// <value>The equals symbol.</value>
        [DataMember]
        public string EqualsSymbol { get; set; }

        /// <summary>
        /// Gets or sets a symbol for a comparison of inequality.
        /// </summary>
        /// <value>The not equals symbol.</value>
        [DataMember]
        public string NotEqualsSymbol { get; set; }

        /// <summary>
        /// Gets or sets a symbol for a comparison of greater than.
        /// </summary>
        /// <value>The greater than symbol.</value>
        [DataMember]
        public string GreaterThanSymbol { get; set; }

        /// <summary>
        /// Gets or sets a symbol for a comparison of greater than or equal.
        /// </summary>
        /// <value>The greater than or equal symbol.</value>
        [DataMember]
        public string GreaterThanOrEqualSymbol { get; set; }

        /// <summary>
        /// Gets or sets a symbol for a comparison of less than.
        /// </summary>
        /// <value>The less than symbol.</value>
        [DataMember]
        public string LessThanSymbol { get; set; }

        /// <summary>
        /// Gets or sets a symbol for a comparison of less than or equal.
        /// </summary>
        /// <value>The less than or equal symbol.</value>
        [DataMember]
        public string LessThanOrEqualSymbol { get; set; }

        /// <summary>
        /// Gets or sets a symbol for a comparison of less than or equal.
        /// </summary>
        /// <value>The right shift symbol.</value>
        [DataMember]
        public string RightShiftSymbol { get; set; }

        /// <summary>
        /// Gets or sets a symbol for a comparison of less than or equal.
        /// </summary>
        /// <value>The left shift symbol.</value>
        [DataMember]
        public string LeftShiftSymbol { get; set; }

        /// <summary>
        /// Gets or sets the automatic convert string format specifier.
        /// </summary>
        /// <value>The automatic convert string format specifier.</value>
        [DataMember]
        public string AutoConvertStringFormatSpecifier { get; set; }

        /// <summary>
        /// Gets or sets the escape character.
        /// </summary>
        /// <value>
        /// The escape character.
        /// </value>
        [DataMember]
        public string EscapeCharacter { get; set; }

        /// <summary>
        /// Gets or sets the operator precedence style. Default is mathematical.
        /// </summary>
        /// <value>The operator precedence style.</value>
        [DataMember]
        public OperatorPrecedenceStyle OperatorPrecedenceStyle { get; set; }

        /// <summary>
        /// Gets or sets the pass-through state container.
        /// </summary>
        /// <value>
        /// The pass-through state container.
        /// </value>
        [DataMember]
        public PassThroughStateContainerBase? PassThroughStateContainer { get; set; }

        /// <summary>
        /// Gets or sets the diagnostics logger.
        /// </summary>
        /// <value>
        /// The logger.
        /// </value>
        /// <remarks>The logger does not serialize, and cannot be cloned. It should be used for diagnostics only.</remarks>
        [IgnoreDataMember]
        public ILog? Logger { get; set; }

        /// <summary>
        /// Creates a deep clone of the source object.
        /// </summary>
        /// <returns>A deep clone.</returns>
        public MathDefinition DeepClone() =>
            new MathDefinition
            {
                AddSymbol = this.AddSymbol,
                AndSymbol = this.AndSymbol,
                DivideSymbol = this.DivideSymbol,
                ModuloSymbol = this.ModuloSymbol,
                PowerSymbol = this.PowerSymbol,
                EqualsSymbol = this.EqualsSymbol,
                GreaterThanOrEqualSymbol = this.GreaterThanOrEqualSymbol,
                GreaterThanSymbol = this.GreaterThanSymbol,
                LeftShiftSymbol = this.LeftShiftSymbol,
                LessThanOrEqualSymbol = this.LessThanOrEqualSymbol,
                LessThanSymbol = this.LessThanSymbol,
                MultiplySymbol = this.MultiplySymbol,
                NotEqualsSymbol = this.NotEqualsSymbol,
                NotSymbol = this.NotSymbol,
                OrSymbol = this.OrSymbol,
                ParameterSeparator = this.ParameterSeparator,
                Parentheses = this.Parentheses,
                RightShiftSymbol = this.RightShiftSymbol,
                SpecialSymbolIndicators = this.SpecialSymbolIndicators,
                IndexerIndicators = this.IndexerIndicators,
                StringIndicator = this.StringIndicator,
                SubtractSymbol = this.SubtractSymbol,
                XorSymbol = this.XorSymbol,
                AutoConvertStringFormatSpecifier = this.AutoConvertStringFormatSpecifier,
                EscapeCharacter = this.EscapeCharacter,
                OperatorPrecedenceStyle = this.OperatorPrecedenceStyle,
                PassThroughStateContainer = this.PassThroughStateContainer?.DeepClone()
            };
    }
}