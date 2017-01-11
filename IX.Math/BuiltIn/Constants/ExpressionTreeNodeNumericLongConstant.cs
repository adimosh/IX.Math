namespace IX.Math.BuiltIn.Constants
{
    internal sealed class ExpressionTreeNodeNumericLongConstant : ExpressionTreeNodeNumericConstant
    {
        public ExpressionTreeNodeNumericLongConstant(long value)
            : base(typeof(long), value)
        {
        }
    }
}