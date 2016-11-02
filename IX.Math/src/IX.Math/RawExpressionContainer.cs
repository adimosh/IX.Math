using System;

namespace IX.Math
{
    internal sealed class RawExpressionContainer : IEquatable<RawExpressionContainer>
    {
        internal RawExpressionContainer(string expression, bool isFunction = false, bool isString = false)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                Expression = null;
            }
            else
            {
                if (isString)
                {
                    Expression = expression;
                }
                else
                {
                    Expression = expression.Replace(" ", string.Empty);
                }
            }

            IsFunctionCall = isFunction;
            IsString = isString;
        }

        public string Expression { get; private set; }

        public bool IsFunctionCall { get; private set; }

        public bool IsString { get; private set; }

        public bool Equals(RawExpressionContainer other)
        {
            if (other == null)
                return false;

            return Expression == other.Expression &&
                IsFunctionCall == other.IsFunctionCall &&
                IsString == other.IsString;
        }
    }
}