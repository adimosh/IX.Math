using System.Linq.Expressions;

namespace IX.Math.Nodes.Operators.Binary.Logical
{
    /// <summary>
    /// A base class for a logical operator.
    /// </summary>
    internal abstract class LogicalOperatorNodeBase : BinaryOperatorNodeBase
    {
        private const SupportableValueType SupportableValueTypes =
            SupportableValueType.Integer | SupportableValueType.ByteArray | SupportableValueType.Boolean;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogicalOperatorNodeBase"/> class.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        protected private LogicalOperatorNodeBase(
            NodeBase leftOperand,
            NodeBase rightOperand)
            : base(
                leftOperand,
                rightOperand) { }

        /// <summary>
        /// Calculates all supportable value types, as a result of the node and all nodes above it.
        /// </summary>
        /// <param name="constraints">The constraints to place on the node's supportable types.</param>
        /// <returns>The resulting supportable value type.</returns>
        /// <exception cref="ExpressionNotValidLogicallyException">The expression is not valid, either structurally or given the constraints.</exception>
        public sealed override SupportableValueType CalculateSupportableValueType(
            SupportableValueType constraints = SupportableValueType.All)
        {
            if ((constraints & SupportableValueTypes) == SupportableValueType.None)
            {
                return SupportableValueType.None;
            }

            var leftType = this.LeftOperand.CalculateSupportableValueType(SupportableValueTypes);
            if (leftType == SupportableValueType.None)
            {
                return SupportableValueType.None;
            }

            var rightType = this.RightOperand.CalculateSupportableValueType(SupportableValueTypes);
            if (rightType == SupportableValueType.None)
            {
                return SupportableValueType.None;
            }

            return leftType & rightType;
        }

        /// <summary>
        ///     Generates the expression that will be compiled into code.
        /// </summary>
        /// <param name="forType">What type the expression is generated for.</param>
        /// <param name="tolerance">The tolerance. If this parameter is <c>null</c> (<c>Nothing</c> in Visual Basic), then the operation is exact.</param>
        /// <returns>
        ///     The generated <see cref="Expression" />.
        /// </returns>
        public sealed override Expression GenerateExpression(
            SupportedValueType forType,
            Tolerance? tolerance = null) =>
            forType switch
            {
                SupportedValueType.ByteArray => this.GenerateBinaryExpression(
                    this.LeftOperand.GenerateExpression(
                        SupportedValueType.ByteArray,
                        tolerance),
                    this.RightOperand.GenerateExpression(
                        SupportedValueType.ByteArray,
                        tolerance)),
                SupportedValueType.Integer => this.GenerateIntegerExpression(
                    this.LeftOperand.GenerateExpression(
                        SupportedValueType.Integer,
                        tolerance),
                    this.RightOperand.GenerateExpression(
                        SupportedValueType.Integer,
                        tolerance)),
                SupportedValueType.Boolean => this.GenerateBooleanExpression(
                    this.LeftOperand.GenerateExpression(
                        SupportedValueType.Boolean,
                        tolerance),
                    this.RightOperand.GenerateExpression(
                        SupportedValueType.Boolean,
                        tolerance)),
                _ => throw new ExpressionNotValidLogicallyException()
            };

        /// <summary>
        /// Generates an integer mathematical expression.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <returns>An expression containing the operation.</returns>
        private protected abstract Expression GenerateIntegerExpression(
            Expression left,
            Expression right);

        /// <summary>
        /// Generates a binary mathematical expression.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <returns>An expression containing the operation.</returns>
        private protected abstract Expression GenerateBinaryExpression(
            Expression left,
            Expression right);

        /// <summary>
        /// Generates a boolean mathematical expression.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <returns>An expression containing the operation.</returns>
        private protected abstract Expression GenerateBooleanExpression(
            Expression left,
            Expression right);
    }
}