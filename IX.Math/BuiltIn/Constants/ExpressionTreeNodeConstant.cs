// <copyright file="ExpressionTreeNodeConstant.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;

namespace IX.Math.BuiltIn.Constants
{
    internal abstract class ExpressionTreeNodeConstant : ExpressionTreeNodeBase
    {
        private readonly object value;

        internal ExpressionTreeNodeConstant(Type minimalRequiredNumericType, object value)
            : base(minimalRequiredNumericType)
        {
            this.value = value ?? throw new ArgumentNullException(nameof(value));
        }

        public override SupportedValueType[] OperandTypes
        {
            get
            {
                return new SupportedValueType[0];
            }
        }

        internal object Value
        {
            get
            {
                return this.value;
            }
        }
    }
}