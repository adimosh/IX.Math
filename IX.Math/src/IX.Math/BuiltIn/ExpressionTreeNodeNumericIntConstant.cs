namespace IX.Math.BuiltIn
{
    internal sealed class ExpressionTreeNodeNumericIntConstant : ExpressionTreeNodeNumericConstant
    {
        public ExpressionTreeNodeNumericIntConstant(int value)
            : base(typeof(int), value)
        {
        }
    }
}