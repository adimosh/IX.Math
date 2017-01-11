namespace IX.Math.BuiltIn.Constants
{
    internal sealed class ExpressionTreeNodeNumericDoubleConstant : ExpressionTreeNodeNumericConstant
    {
        public ExpressionTreeNodeNumericDoubleConstant(double value)
            : base(typeof(double), value)
        {
        }
    }
}