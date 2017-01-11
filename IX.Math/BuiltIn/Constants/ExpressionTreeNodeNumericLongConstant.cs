// <copyright file="ExpressionTreeNodeNumericLongConstant.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

namespace IX.Math.BuiltIn.Constants
{
    internal sealed class ExpressionTreeNodeNumericLongConstant : ExpressionTreeNodeNumericConstant
    {
        public ExpressionTreeNodeNumericLongConstant(long value)
            : base(typeof(long), value)
        {
        }
    }
}