// <copyright file="ConstantNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using JetBrains.Annotations;

namespace IX.Math.Nodes.Constants
{
    /// <summary>
    ///     A base class for constants.
    /// </summary>
    /// <seealso cref="NodeBase" />
    [PublicAPI]
    public abstract class ConstantNodeBase : NodeBase
    {
#region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="ConstantNodeBase" /> class.
        /// </summary>
        protected private ConstantNodeBase()
        {
        }

#endregion

#region Properties and indexers

        /// <summary>
        ///     Gets a value indicating whether or not this node is actually a constant.
        /// </summary>
        /// <value><see langword="true" /> if the node is a constant, <see langword="false" /> otherwise.</value>
        public sealed override bool IsConstant =>
            true;

        /// <summary>
        ///     Gets a value indicating whether this node supports tolerance.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the node is tolerant, <see langword="false" /> otherwise.
        /// </value>
        public sealed override bool IsTolerant =>
            false;

        /// <summary>
        ///     Gets a value indicating whether this node requires preservation of the original expression.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the node requires original expression preservation, or <see langword="false" />
        ///     if it can be polynomially-reduced.
        /// </value>
        public override bool RequiresPreservedExpression =>
            false;

        /// <summary>
        ///     Gets the value as object.
        /// </summary>
        /// <value>
        ///     The value as object.
        /// </value>
        public abstract object ValueAsObject
        {
            get;
        }

        /// <summary>
        ///     Gets or sets the original string value.
        /// </summary>
        /// <value>
        ///     The original string value.
        /// </value>
        public string OriginalStringValue
        {
            get;
            set;
        }

        /// <summary>
        ///     Gets or sets the value as string.
        /// </summary>
        /// <value>
        ///     The value as string.
        /// </value>
        public string ValueAsString
        {
            get;
            protected set;
        }

#endregion

#region Methods

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>A reflexive return.</returns>
        public sealed override NodeBase Simplify() =>
            this;

        /// <summary>
        ///     Tries to get an integer value out of this constant node.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the constant can safely be converted to an integer, <c>false</c> otherwise.</returns>
        public virtual bool TryGetInteger(out long value)
        {
            value = default;

            return false;
        }

        /// <summary>
        ///     Tries to get a numeric value out of this constant node.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the constant can safely be converted to a numeric value, <c>false</c> otherwise.</returns>
        public virtual bool TryGetNumeric(out double value)
        {
            value = default;

            return false;
        }

        /// <summary>
        ///     Tries to get a byte array value out of this constant node.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the constant can safely be converted to a byte array, <c>false</c> otherwise.</returns>
        public virtual bool TryGetBinary(out byte[] value)
        {
            value = default;

            return false;
        }

        /// <summary>
        ///     Tries to get a boolean value out of this constant node.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the constant can safely be converted to a boolean, <c>false</c> otherwise.</returns>
        public virtual bool TryGetBoolean(out bool value)
        {
            value = default;

            return false;
        }

        /// <summary>
        ///     Tries to get a string value out of this constant node.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns><c>true</c> if the constant can safely be converted to a string, <c>false</c> otherwise.</returns>
        public abstract bool TryGetString(out string value);

#endregion
    }
}