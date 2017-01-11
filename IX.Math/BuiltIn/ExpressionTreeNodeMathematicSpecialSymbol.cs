using System.Linq.Expressions;

namespace IX.Math.BuiltIn
{
    internal sealed class ExpressionTreeNodeMathematicSpecialSymbol : ExpressionTreeNodeBase
    {
        private readonly string name;
        private readonly double value;

        internal ExpressionTreeNodeMathematicSpecialSymbol(string name, double value)
            : base(typeof(double))
        {
            this.name = name;
            this.value = value;
        }

        internal string Name
        {
            get
            {
                return name;
            }
        }

        public override SupportedValueType[] OperandTypes
        {
            get
            {
                return new SupportedValueType[0];
            }
        }

        public override SupportedValueType ReturnType
        {
            get
            {
                return SupportedValueType.Numeric;
            }
        }

        protected override Expression GenerateExpressionWithOperands(ExpressionTreeNodeBase[] operandExpressions, int numericTypeValue)
        {
            return Expression.Constant(value, typeof(double));
        }
    }
}