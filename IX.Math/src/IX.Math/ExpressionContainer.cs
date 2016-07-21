using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IX.Math
{
    internal class RawExpressionContainer : IEquatable<RawExpressionContainer>
    {
        private string expression;
        public string Expression
        {
            get
            {
                return expression;
            }
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    expression = null;
                else
                    expression = value.Replace(" ", string.Empty);
            }
        }

        public bool Equals(RawExpressionContainer other)
        {
            if (other == null)
                return false;

            return expression == other.expression;
        }
    }
}