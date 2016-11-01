using System;
using System.Linq.Expressions;

namespace IX.Math.SupportedFunctions
{
    internal class BuiltInMathematicSpecialSymbol : SpecialSymbol
    {
        private readonly string name;
        private readonly double value;

        internal BuiltInMathematicSpecialSymbol(string name, double value)
        {
            this.name = name;
            this.value = value;
        }

        public override Type ActualMinimalNumericTypeRequired
        {
            get
            {
                return typeof(double);
            }
        }

        public override string Name
        {
            get
            {
                return name;
            }
        }

        public override SupportedValueType Type
        {
            get
            {
                return SupportedValueType.Numeric;
            }
        }

        public override Expression GenerateExpression()
        {
            return Expression.Constant(value, typeof(double));
        }
    }
}