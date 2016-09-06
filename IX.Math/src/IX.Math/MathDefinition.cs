using System;
using System.Runtime.Serialization;

namespace IX.Math
{
    [DataContract]
    public class MathDefinition
    {
        [DataMember]
        public Tuple<string, string> Parantheses { get; set; }

        #region Mathematical symbols
        [DataMember]
        public string AddSymbol { get; set; }

        [DataMember]
        public string SubtractSymbol { get; set; }

        [DataMember]
        public string MultiplySymbol { get; set; }

        [DataMember]
        public string DivideSymbol { get; set; }

        [DataMember]
        public string PowerSymbol { get; set; }
        #endregion

        #region Logical symbols
        [DataMember]
        public string AndSymbol { get; set; }

        [DataMember]
        public string OrSymbol { get; set; }

        [DataMember]
        public string XorSymbol { get; set; }

        [DataMember]
        public string NotSymbol { get; set; }
        #endregion

        #region Comparison symbols
        [DataMember]
        public string EqualsSymbol { get; set; }

        [DataMember]
        public string DoesNotEqualSymbol { get; set; }

        [DataMember]
        public string GreaterThanSymbol { get; set; }

        [DataMember]
        public string GreaterThanOrEqualSymbol { get; set; }

        [DataMember]
        public string LessThanSymbol { get; set; }

        [DataMember]
        public string LessThanOrEqualSymbol { get; set; }
        #endregion
    }
}