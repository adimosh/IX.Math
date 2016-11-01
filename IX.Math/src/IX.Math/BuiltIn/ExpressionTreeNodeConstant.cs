using System;

namespace IX.Math.BuiltIn
{
    internal abstract class ExpressionTreeNodeConstant : ExpressionTreeNodeBase
    {
        private readonly object value;

        internal ExpressionTreeNodeConstant(Type minimalRequiredNumericType, object value)
            : base(minimalRequiredNumericType)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            this.value = value;
        }

        internal object Value
        {
            get
            {
                return value;
            }
        }

        public override SupportedValueType[] OperandTypes
        {
            get
            {
                return new SupportedValueType[0];
            }
        }
    }
}