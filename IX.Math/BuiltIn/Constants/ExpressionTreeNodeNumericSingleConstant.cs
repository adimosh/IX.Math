// <copyright file="ExpressionTreeNodeNumericSingleConstant.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

namespace IX.Math.BuiltIn.Constants
{
    internal sealed class ExpressionTreeNodeNumericSingleConstant : ExpressionTreeNodeNumericConstant
    {
        public ExpressionTreeNodeNumericSingleConstant(float value)
            : base(typeof(float), value)
        {
        }
    }
}