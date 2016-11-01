namespace IX.Math.BuiltIn
{
    internal sealed class ExpressionTreeNodeNumericDoubleConstant : ExpressionTreeNodeNumericConstant
    {
        public ExpressionTreeNodeNumericDoubleConstant(double value)
            : base(typeof(double), value)
        {
        }
    }
}