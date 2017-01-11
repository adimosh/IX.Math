// <copyright file="ExpressionTreeNodeNumericDoubleConstant.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

namespace IX.Math.BuiltIn.Constants
{
    internal sealed class ExpressionTreeNodeNumericDoubleConstant : ExpressionTreeNodeNumericConstant
    {
        public ExpressionTreeNodeNumericDoubleConstant(double value)
            : base(typeof(double), value)
        {
        }
    }
}