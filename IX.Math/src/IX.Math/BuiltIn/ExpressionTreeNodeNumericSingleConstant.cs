namespace IX.Math.BuiltIn
{
    internal sealed class ExpressionTreeNodeNumericSingleConstant : ExpressionTreeNodeNumericConstant
    {
        public ExpressionTreeNodeNumericSingleConstant(float value)
            : base(typeof(float), value)
        {
        }
    }
}