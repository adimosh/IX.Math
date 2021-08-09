using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using IX.Math.Values;

namespace IX.Math.Nodes.Operators.Unary
{
    /// <summary>
    /// A subtraction unary operator.
    /// </summary>
    internal class SubtractOperator : UnaryOperatorNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SubtractOperator"/> class.
        /// </summary>
        /// <param name="operand">The operand.</param>
        internal SubtractOperator(NodeBase operand)
            : base(operand) { }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new SubtractOperator(this.Operand.DeepClone(context));

        /// <summary>
        ///     Generates the expression that will be compiled into code.
        /// </summary>
        /// <param name="forType">What type the expression is generated for.</param>
        /// <param name="tolerance">The tolerance. If this parameter is <c>null</c> (<c>Nothing</c> in Visual Basic), then the operation is exact.</param>
        /// <returns>
        ///     The generated <see cref="Expression" />.
        /// </returns>
        [SuppressMessage(
            "ReSharper",
            "HeapView.BoxingAllocation",
            Justification = "Expressions work that way.")]
        public override Expression GenerateExpression(
            SupportedValueType forType,
            Tolerance? tolerance = null)
        {
            Expression operandExpression;
            Expression zeroExpression;
            switch (forType)
            {
                case SupportedValueType.Integer:
                    operandExpression = this.Operand.GenerateExpression(
                        SupportedValueType.Integer,
                        tolerance);
                    zeroExpression = Expression.Constant(
                        0L,
                        typeof(long));

                    break;
                case SupportedValueType.Numeric:
                    operandExpression = this.Operand.GenerateExpression(
                        SupportedValueType.Numeric,
                        tolerance);
                    zeroExpression = Expression.Constant(
                        0D,
                        typeof(double));

                    break;
                default:
                    throw new ExpressionNotValidLogicallyException();
            }

            return Expression.Subtract(zeroExpression, operandExpression);
        }

        /// <summary>
        /// Calculates all supportable value types, as a result of the node and all nodes above it.
        /// </summary>
        /// <param name="constraints">The constraints to place on the node's supportable types.</param>
        /// <returns>The resulting supportable value type.</returns>
        /// <exception cref="ExpressionNotValidLogicallyException">The expression is not valid, either structurally or given the constraints.</exception>
        public override SupportableValueType CalculateSupportableValueType(
            SupportableValueType constraints = SupportableValueType.All)
        {
            if (this.Operand.CalculateSupportableValueType(
                    SupportableValueType.Integer | SupportableValueType.Numeric) ==
                SupportableValueType.None)
            {
                return SupportableValueType.None;
            }

            return constraints & (SupportableValueType.Integer | SupportableValueType.Numeric);
        }


        /// <summary>
        ///     Simplifies this node, if possible, based on a constant operand value.
        /// </summary>
        /// <returns>A simplified node, or this instance.</returns>
        private protected override NodeBase SimplifyOnConvertibleValue(ConvertibleValue value)
        {
            if (value.HasInteger)
            {
                return new ConstantNode(0 - value.GetInteger());
            }

            if (value.HasNumeric)
            {
                return new ConstantNode(0 - value.GetNumeric());
            }

            throw new ExpressionNotValidLogicallyException();
        }
    }
}