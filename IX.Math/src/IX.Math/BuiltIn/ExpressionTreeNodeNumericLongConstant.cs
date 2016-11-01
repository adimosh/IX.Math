namespace IX.Math.BuiltIn
{
    internal sealed class ExpressionTreeNodeNumericLongConstant : ExpressionTreeNodeNumericConstant
    {
        public ExpressionTreeNodeNumericLongConstant(long value)
            : base(typeof(long), value)
        {
        }
    }
}