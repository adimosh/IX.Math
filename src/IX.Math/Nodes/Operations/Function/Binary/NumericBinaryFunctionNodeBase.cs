// <copyright file="NumericBinaryFunctionNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

namespace IX.Math.Nodes.Operations.Function.Binary
{
    /// <summary>
    ///     A base class for numeric binary functions.
    /// </summary>
    /// <seealso cref="BinaryFunctionNodeBase" />
    internal abstract class NumericBinaryFunctionNodeBase : BinaryFunctionNodeBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NumericBinaryFunctionNodeBase" /> class.
        /// </summary>
        /// <param name="firstParameter">The first parameter.</param>
        /// <param name="secondParameter">The second parameter.</param>
        protected NumericBinaryFunctionNodeBase(
            NodeBase firstParameter,
            NodeBase secondParameter)
            : base(
                firstParameter,
                secondParameter)
        {
        }

        /// <summary>
        ///     Gets the return type of this node.
        /// </summary>
        /// <value>
        ///     The node return type.
        /// </value>
        public sealed override SupportedValueType ReturnType => SupportedValueType.Numeric;

        /// <summary>
        ///     Strongly determines the node's type, if possible.
        /// </summary>
        /// <param name="type">The type to determine to.</param>
        public sealed override void DetermineStrongly(SupportedValueType type)
        {
            if (type != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        /// <summary>
        ///     Weakly determines the node's type, if possible, and, optionally, strongly determines if there is only one possible
        ///     type left.
        /// </summary>
        /// <param name="type">The type or types to determine to.</param>
        public sealed override void DetermineWeakly(SupportableValueType type)
        {
            if ((type & SupportableValueType.Numeric) == 0)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        /// <summary>
        ///     Ensures that the parameters that are received are compatible with the function, optionally allowing the parameter
        ///     references to change.
        /// </summary>
        /// <param name="firstParameter">The first parameter.</param>
        /// <param name="secondParameter">The second parameter.</param>
        /// <exception cref="ExpressionNotValidLogicallyException">
        ///     The expression is not valid logically.
        /// </exception>
        protected sealed override void EnsureCompatibleParameters(
            NodeBase firstParameter,
            NodeBase secondParameter)
        {
            firstParameter.DetermineStrongly(SupportedValueType.Numeric);
            secondParameter.DetermineStrongly(SupportedValueType.Numeric);

            if (firstParameter.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }

            if (secondParameter.ReturnType != SupportedValueType.Numeric)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }
    }
}