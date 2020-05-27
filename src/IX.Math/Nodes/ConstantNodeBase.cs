// <copyright file="ConstantNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using JetBrains.Annotations;

namespace IX.Math.Nodes
{
    /// <summary>
    /// A base class for constants.
    /// </summary>
    /// <seealso cref="NodeBase" />
    [PublicAPI]
    public abstract class ConstantNodeBase : CachedExpressionNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ConstantNodeBase"/> class.
        /// </summary>
        protected private ConstantNodeBase()
        {
        }

        /// <summary>
        /// Gets a value indicating whether or not this node is actually a constant.
        /// </summary>
        /// <value><see langword="true"/> if the node is a constant, <see langword="false"/> otherwise.</value>
        public sealed override bool IsConstant => true;

        /// <summary>
        ///     Gets a value indicating whether this node supports tolerance.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is tolerant; otherwise, <c>false</c>.
        /// </value>
        public override bool IsTolerant => false;

        /// <summary>
        /// Distills the value into a usable constant.
        /// </summary>
        /// <returns>A usable constant.</returns>
        [NotNull]
        public abstract object DistillValue();

        /// <summary>
        /// Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>A reflexive return.</returns>
        public sealed override NodeBase Simplify() => this;

        /// <summary>
        /// Strongly determines the node's type, if possible.
        /// </summary>
        /// <param name="type">The type to determine to.</param>
        public sealed override void DetermineStrongly(SupportedValueType type)
        {
            if (type != this.ReturnType)
            {
                throw new Exceptions.ExpressionNotValidLogicallyException();
            }
        }

        /// <summary>
        /// Weakly determines the node's type, if possible, and, optionally, strongly determines if there is only one possible type left.
        /// </summary>
        /// <param name="type">The type or types to determine to.</param>
        public sealed override void DetermineWeakly(SupportableValueType type)
        {
            if ((type & (SupportableValueType)(int)this.ReturnType) == 0)
            {
                throw new Exceptions.ExpressionNotValidLogicallyException();
            }
        }
    }
}