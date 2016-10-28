using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace IX.Math
{
    /// <summary>
    /// A model for a supported function.
    /// </summary>
    public abstract class SupportedFunction
    {
        /// <summary>
        /// The name of the function.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// The return type.
        /// </summary>
        public abstract SupportedValueType ReturnType { get; }

        /// <summary>
        /// The type of the operands that this function accepts.
        /// </summary>
        public abstract SupportedValueType[] OperandTypes { get; }

        /// <summary>
        /// Gets the actual type of numeric data that is required for this function to work.
        /// </summary>
        public abstract Type ActualMinimalNumericTypeRequired { get; }

        /// <summary>
        /// Generates an expression with the specified operands.
        /// </summary>
        /// <param name="operandExpressions">The expressions for the operands.</param>
        /// <returns>The resulting expression</returns>
        /// <remarks>
        /// <para>The expression operands that are not of a predictable type (such as <see cref="ConstantExpression"/> or <see cref="ParameterExpression"/>) are not
        /// checked in any way. This might lead to an expression that is inconsistent.</para>
        /// </remarks>
        public Expression GenerateExpression(Expression[] operandExpressions)
        {
            SupportedValueType[] operandTypes = OperandTypes;

            if (operandTypes.Length != operandExpressions.Length)
            {
                throw new FunctionCallNotValidLogicallyException();
            }

            for (int i = 0; i < operandTypes.Length; i++)
            {
            }

            return GenerateExpressionWithOperands(operandExpressions);
        }

        /// <summary>
        /// When implemented in a child class, generates an expression with the specified operands.
        /// </summary>
        /// <param name="operandExpressions">The expressions for the operands.</param>
        /// <returns>The resulting expression</returns>
        protected abstract Expression GenerateExpressionWithOperands(Expression[] operandExpressions);
    }
}
