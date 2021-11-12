// <copyright file="MathDefinition.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Runtime.Serialization;
using IX.StandardExtensions;

namespace IX.Math;

/// <summary>
/// A definition for signs and symbols used in expression parsing of a mathematical expression.
/// </summary>
[DataContract]
public record MathDefinition : IDeepCloneable<MathDefinition>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="MathDefinition"/> class.
    /// </summary>
    public MathDefinition()
    {
        this.Parentheses = ("(", ")");
        this.SpecialSymbolIndicators = ("[", "]");
        this.StringIndicator = "\"";
        this.ParameterSeparator = ",";
        this.AddSymbol = "+";
        this.AndSymbol = "&";
        this.DivideSymbol = "/";
        this.NotEqualsSymbol = "!=";
        this.EqualsSymbol = "=";
        this.MultiplySymbol = "*";
        this.NotSymbol = "!";
        this.OrSymbol = "|";
        this.PowerSymbol = "^";
        this.SubtractSymbol = "-";
        this.XorSymbol = "#";
        this.GreaterThanOrEqualSymbol = ">=";
        this.GreaterThanSymbol = ">";
        this.LessThanOrEqualSymbol = "<=";
        this.LessThanSymbol = "<";
        this.RightShiftSymbol = ">>";
        this.LeftShiftSymbol = "<<";
        this.OperatorPrecedenceStyle = OperatorPrecedenceStyle.Mathematical;
        this.EscapeCharacter = "\\";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="MathDefinition"/> class.
    /// </summary>
    /// <param name="definition">The definition to use.</param>
    public MathDefinition(MathDefinition definition)
    {
        this.Parentheses = (definition.Parentheses.Left, definition.Parentheses.Right);
        this.SpecialSymbolIndicators = (definition.SpecialSymbolIndicators.Begin, definition.SpecialSymbolIndicators.End);
        this.StringIndicator = definition.StringIndicator;
        this.ParameterSeparator = definition.ParameterSeparator;
        this.AddSymbol = definition.AddSymbol;
        this.AndSymbol = definition.AndSymbol;
        this.DivideSymbol = definition.DivideSymbol;
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
        this.EscapeCharacter = definition.EscapeCharacter;
        this.OperatorPrecedenceStyle = definition.OperatorPrecedenceStyle;
    }

    /// <summary>
    /// Gets a copy of the default math definition.
    /// </summary>
    /// <value>
    /// The default math definition.
    /// </value>
    public static MathDefinition Default => new();

    /// <summary>
    /// Gets or sets what should be interpreted as parentheses.
    /// </summary>
    /// <value>The parentheses indicators.</value>
    /// <remarks>The first item in the tuple represents the opening parenthesis, whereas the second represents the closing parenthesis.</remarks>
    [DataMember]
    public (string Left, string Right) Parentheses { get; set; }

    /// <summary>
    /// Gets or sets what should be interpreted as special symbols.
    /// </summary>
    /// <value>The special symbol indicators.</value>
    /// <remarks>The first item in the tuple represents the opening of the special symbol marker, whereas the second represents its closing.</remarks>
    [DataMember]
    public (string Begin, string End) SpecialSymbolIndicators { get; set; }

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
    /// Creates a deep clone of the source object.
    /// </summary>
    /// <returns>A deep clone.</returns>
    public MathDefinition DeepClone() => new(this);
}