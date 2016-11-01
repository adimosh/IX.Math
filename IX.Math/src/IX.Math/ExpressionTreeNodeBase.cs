using IX.Math.SimplificationAide;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace IX.Math
{
    /// <summary>
    /// A container base class for an expression tree item.
    /// </summary>
    public abstract class ExpressionTreeNodeBase
    {
        private readonly Type minimalRequiredNumericType;
        private readonly int minimalRequiredNumericTypeValue;

        private bool operandsSet;

        private ExpressionTreeNodeBase[] operands;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionTreeNodeBase"/> class.
        /// </summary>
        protected ExpressionTreeNodeBase(Type minimalRequiredNumericType)
        {
            if (minimalRequiredNumericType == null)
            {
                throw new ArgumentNullException(nameof(minimalRequiredNumericType));
            }

            if (!NumericTypeAide.NumericTypesConversionDictionary.TryGetValue(minimalRequiredNumericType, out minimalRequiredNumericTypeValue))
            {
                throw new ArgumentException(Resources.NumericTypeInvalid, nameof(minimalRequiredNumericType));
            }

            this.minimalRequiredNumericType = minimalRequiredNumericType;
        }

        /// <summary>
        /// Gets the return type.
        /// </summary>
        public abstract SupportedValueType ReturnType { get; }

        /// <summary>
        /// Gets the type of the operands that this function accepts.
        /// </summary>
        public abstract SupportedValueType[] OperandTypes { get; }

        /// <summary>
        /// Gets the actual type of numeric data that is required for this function to work.
        /// </summary>
        public Type ActualMinimalNumericTypeRequired
        {
            get
            {
                return minimalRequiredNumericType;
            }
        }

        /// <summary>
        /// Sets the operands to use in the expression.
        /// </summary>
        /// <param name="operandExpressions"></param>
        public void SetOperands(params ExpressionTreeNodeBase[] operandExpressions)
        {
            if (operandExpressions == null)
            {
                throw new ArgumentNullException(nameof(operandExpressions));
            }

            SupportedValueType[] operandTypes = OperandTypes;

            if (operandTypes.Length != operandExpressions.Length)
            {
                throw new FunctionCallNotValidLogicallyException();
            }

            for (int i = 0; i < operandTypes.Length; i++)
            {
                var requiredType = operandTypes[i];

                // An unknown type or type that cannot be evaluated is required
                if (requiredType == SupportedValueType.Unknown)
                {
                    continue;
                }

                var operandExpression = operandExpressions[i];

                // Although a specific type is required, it cannot be readily evaluated
                if (operandExpression.ReturnType == SupportedValueType.Unknown)
                {
                    continue;
                }

                // Operand is of a totally different type
                if (operandExpression.ReturnType != requiredType)
                {
                    throw new FunctionCallNotValidLogicallyException();
                }
            }

            operands = operandExpressions;
            operandsSet = true;
        }

        /// <summary>
        /// Calculates a resulting minimal numeric type value out of this expression container and its sub-contained operands.
        /// </summary>
        /// <returns></returns>
        public int ComputeResultingNumericTypeValue()
        {
            if (!operandsSet)
            {
                return minimalRequiredNumericTypeValue;
            }

            var val = operands.Max(p => p.ComputeResultingNumericTypeValue());

            return minimalRequiredNumericTypeValue > val ? minimalRequiredNumericTypeValue : val;
        }

        /// <summary>
        /// Generates a LINQ expression tree out of the expression container and its sub-expressions.
        /// </summary>
        /// <param name="minimalNumericType">The expressions for the operands.</param>
        /// <returns>The resulting <see cref="Expression"/>.</returns>
        /// <remarks>
        /// <para>The expression operands that are not of a predictable type (such as <see cref="ConstantExpression"/> or <see cref="ParameterExpression"/>) cannot be
        /// reliably checked for type errors and mistakes in type inference. This might lead in rare cases to an expression that is inconsistent.</para>
        /// </remarks>
        public Expression GenerateExpression(int minimalNumericType)
        {
            if (!NumericTypeAide.InverseNumericTypesConversionDictionary.ContainsKey(minimalNumericType))
            {
                throw new ArgumentException(Resources.NumericTypeMismatched, nameof(minimalNumericType));
            }

            int computedNumericType = ComputeResultingNumericTypeValue();

            if (minimalNumericType < computedNumericType)
            {
                minimalNumericType = computedNumericType;
            }

            return GenerateExpressionWithOperands(operands, minimalNumericType);
        }

        /// <summary>
        /// Generates a LINQ expression tree out of the expression container and its sub-expressions.
        /// </summary>
        /// <returns>The resulting <see cref="Expression"/>.</returns>
        public Expression GenerateExpression()
        {
            int computedNumericType = ComputeResultingNumericTypeValue();

            return GenerateExpressionWithOperands(operands, computedNumericType);
        }

        /// <summary>
        /// When implemented in a child class, generates an expression with the specified operands and numeric type.
        /// </summary>
        /// <param name="operandExpressions">The expressions for the operands.</param>
        /// <param name="numericTypeValue">The numeric type value to use.</param>
        /// <returns>The resulting <see cref="Expression"/>.</returns>
        protected abstract Expression GenerateExpressionWithOperands(ExpressionTreeNodeBase[] operandExpressions, int numericTypeValue);
    }
}