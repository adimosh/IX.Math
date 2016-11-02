using System;
using System.Runtime.Serialization;

namespace IX.Math
{
    /// <summary>
    /// A definition for signs and symbols used in expression parsing of a mathematical expression.
    /// </summary>
    [DataContract]
    public class MathDefinition
    {
        /// <summary>
        /// What should be interpreted as parantheses.
        /// </summary>
        /// <remarks>
        /// <para>The first item in the tuple represents the opening paranthesis, whereas the second represents the closing paranthesis.</para>
        /// </remarks>
        [DataMember]
        public Tuple<string, string> Parantheses { get; set; }

        /// <summary>
        /// What should be interpreted as special symbols.
        /// </summary>
        /// <remarks>
        /// <para>The first item in the tuple represents the opening of the special symbol marker, whereas the second represents its closing.</para>
        /// </remarks>
        [DataMember]
        public Tuple<string, string> SpecialSymbolIndicators { get; set; }

        /// <summary>
        /// What should be interpreted as string markers.
        /// </summary>
        [DataMember]
        public string StringIndicator { get; set; }

        /// <summary>
        /// What should be interpreted as parameter separators in multi-parameter function calls.
        /// </summary>
        [DataMember]
        public string ParameterSeparator { get; set; }

        #region Mathematical symbols
        /// <summary>
        /// A symbol for the addition operation.
        /// </summary>
        [DataMember]
        public string AddSymbol { get; set; }

        /// <summary>
        /// A symbol for the subtraction operation.
        /// </summary>
        [DataMember]
        public string SubtractSymbol { get; set; }

        /// <summary>
        /// A symbol for the multiplication operation.
        /// </summary>
        [DataMember]
        public string MultiplySymbol { get; set; }

        /// <summary>
        /// A symbol for the division operation.
        /// </summary>
        [DataMember]
        public string DivideSymbol { get; set; }

        /// <summary>
        /// A symbol for the power operation.
        /// </summary>
        [DataMember]
        public string PowerSymbol { get; set; }
        #endregion

        #region Logical symbols
        /// <summary>
        /// A symbol for the &quot;and&quot; logical operation.
        /// </summary>
        [DataMember]
        public string AndSymbol { get; set; }

        /// <summary>
        /// A symbol for the &quot;or&quot; logical operation.
        /// </summary>
        [DataMember]
        public string OrSymbol { get; set; }

        /// <summary>
        /// A symbol for the &quot;xor&quot; logical operation.
        /// </summary>
        [DataMember]
        public string XorSymbol { get; set; }

        /// <summary>
        /// A symbol for the &quot;not&quot; logical operation.
        /// </summary>
        [DataMember]
        public string NotSymbol { get; set; }
        #endregion

        #region Comparison symbols
        /// <summary>
        /// A symbol for a comparison of equality.
        /// </summary>
        [DataMember]
        public string EqualsSymbol { get; set; }

        /// <summary>
        /// A symbol for a comparison of inequality.
        /// </summary>
        [DataMember]
        public string DoesNotEqualSymbol { get; set; }

        /// <summary>
        /// A symbol for a comparison of greater than.
        /// </summary>
        [DataMember]
        public string GreaterThanSymbol { get; set; }

        /// <summary>
        /// A symbol for a comparison of greater than or equal.
        /// </summary>
        [DataMember]
        public string GreaterThanOrEqualSymbol { get; set; }

        /// <summary>
        /// A symbol for a comparison of less than.
        /// </summary>
        [DataMember]
        public string LessThanSymbol { get; set; }

        /// <summary>
        /// A symbol for a comparison of less than or equal.
        /// </summary>
        [DataMember]
        public string LessThanOrEqualSymbol { get; set; }
        #endregion

        #region Bitwise symbols
        /// <summary>
        /// A symbol for a comparison of less than or equal.
        /// </summary>
        [DataMember]
        public string ShiftRightSymbol { get; set; }

        /// <summary>
        /// A symbol for a comparison of less than or equal.
        /// </summary>
        [DataMember]
        public string ShiftLeftSymbol { get; set; }
        #endregion
    }
}