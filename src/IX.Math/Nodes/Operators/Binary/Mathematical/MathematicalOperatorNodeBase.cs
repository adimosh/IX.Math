using System.Linq.Expressions;

namespace IX.Math.Nodes.Operators.Binary.Mathematical
{
    /// <summary>
    /// A base class for a binary operator that does numerical calculations.
    /// </summary>
    internal abstract class MathematicalOperatorNodeBase : BinaryOperatorNodeBase
    {
        private const SupportableValueType SupportableValueTypes =
            SupportableValueType.Integer | SupportableValueType.Numeric;

        /// <summary>
        /// Initializes a new instance of the <see cref="MathematicalOperatorNodeBase"/> class.
        /// </summary>
        /// <param name="leftOperand">The left operand.</param>
        /// <param name="rightOperand">The right operand.</param>
        protected private MathematicalOperatorNodeBase(
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

            return (leftType & rightType) switch
            {
                SupportableValueType.Integer => SupportableValueType.Integer,
                SupportableValueType.Integer | SupportableValueType.Numeric => SupportableValueType.Integer |
                                                                               SupportableValueType.Numeric,
                _ => SupportableValueType.Numeric
            };
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
            Tolerance? tolerance = null)
        {
            var leftType = this.LeftOperand.CalculateSupportableValueType(SupportableValueTypes);
            var rightType = this.RightOperand.CalculateSupportableValueType(SupportableValueTypes);

            Expression leftExpression, rightExpression;

            switch (forType)
            {
                case SupportedValueType.Integer:
                    if ((leftType & SupportableValueType.Integer) == SupportableValueType.None ||
                        (rightType & SupportableValueType.Integer) == SupportableValueType.None)
                    {
                        throw new ExpressionNotValidLogicallyException();
                    }

                    leftExpression = this.LeftOperand.GenerateExpression(
                        SupportedValueType.Integer,
                        tolerance);
                    rightExpression = this.RightOperand.GenerateExpression(
                        SupportedValueType.Integer,
                        tolerance);

                    return this.GenerateIntegerExpression(
                        leftExpression,
                        rightExpression);

                case SupportedValueType.Numeric:
                    if (leftType == SupportableValueType.Integer)
                    {
                        leftExpression = Expression.Convert(
                            this.LeftOperand.GenerateExpression(
                                SupportedValueType.Integer,
                                tolerance),
                            typeof(double));
                    }
                    else
                    {
                        leftExpression = this.LeftOperand.GenerateExpression(
                            SupportedValueType.Numeric,
                            tolerance);
                    }

                    if (rightType == SupportableValueType.Integer)
                    {
                        rightExpression = Expression.Convert(
                            this.RightOperand.GenerateExpression(
                                SupportedValueType.Integer,
                                tolerance),
                            typeof(double));
                    }
                    else
                    {
                        rightExpression = this.RightOperand.GenerateExpression(
                            SupportedValueType.Numeric,
                            tolerance);
                    }

                    return this.GenerateNumericExpression(
                        leftExpression,
                        rightExpression);

                default:
                    throw new ExpressionNotValidLogicallyException();
            }
        }

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
        /// Generates a numeric mathematical expression.
        /// </summary>
        /// <param name="left">The left operand expression.</param>
        /// <param name="right">The right operand expression.</param>
        /// <returns>An expression containing the operation.</returns>
        private protected abstract Expression GenerateNumericExpression(
            Expression left,
            Expression right);
    }
}