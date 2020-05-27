// <copyright file="NumericUnaryFunctionNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

namespace IX.Math.Nodes.Function.Unary
{
    /// <summary>
    ///     A base class for numeric unary functions.
    /// </summary>
    /// <seealso cref="UnaryFunctionNodeBase" />
    internal abstract class NumericUnaryFunctionNodeBase : UnaryFunctionNodeBase
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NumericUnaryFunctionNodeBase" /> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected NumericUnaryFunctionNodeBase(NodeBase parameter)
            : base(parameter)
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
                throw new Exceptions.ExpressionNotValidLogicallyException();
            }
        }

        /// <summary>
        ///     Weakly determines the node's type, if possible, and, optionally, strongly determines if there is only one possible
        ///     type left.
        /// </summary>
        /// <param name="type">The type or types to determine to.</param>
        public override void DetermineWeakly(SupportableValueType type)
        {
            if ((type & SupportableValueType.Numeric) == 0)
            {
                throw new Exceptions.ExpressionNotValidLogicallyException();
            }
        }

        /// <summary>
        ///     Ensures that the parameter that is received is compatible with the function, optionally allowing the parameter
        ///     reference to change.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        /// <exception cref="ExpressionNotValidLogicallyException">The expression is not valid logically.</exception>
        protected sealed override void EnsureCompatibleParameter(NodeBase parameter)
        {
            parameter.DetermineStrongly(SupportedValueType.Numeric);

            if (parameter.ReturnType != SupportedValueType.Numeric)
            {
                throw new Exceptions.ExpressionNotValidLogicallyException();
            }
        }
    }
}