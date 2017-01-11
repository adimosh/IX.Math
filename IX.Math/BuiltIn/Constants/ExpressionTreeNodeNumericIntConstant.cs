// <copyright file="ExpressionTreeNodeNumericIntConstant.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

namespace IX.Math.BuiltIn.Constants
{
    internal sealed class ExpressionTreeNodeNumericIntConstant : ExpressionTreeNodeNumericConstant
    {
        public ExpressionTreeNodeNumericIntConstant(int value)
            : base(typeof(int), value)
        {
        }
    }
}