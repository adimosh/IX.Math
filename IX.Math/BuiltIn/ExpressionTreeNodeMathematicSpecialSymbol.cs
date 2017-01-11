// <copyright file="ExpressionTreeNodeMathematicSpecialSymbol.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;

namespace IX.Math.BuiltIn
{
    internal sealed class ExpressionTreeNodeMathematicSpecialSymbol : ExpressionTreeNodeBase
    {
        private readonly string name;
        private readonly double value;

        internal ExpressionTreeNodeMathematicSpecialSymbol(string name, double value)
            : base(typeof(double))
        {
            this.name = name;
            this.value = value;
        }

        public override SupportedValueType[] OperandTypes
        {
            get
            {
                return new SupportedValueType[0];
            }
        }

        public override SupportedValueType ReturnType
        {
            get
            {
                return SupportedValueType.Numeric;
            }
        }

        internal string Name
        {
            get
            {
                return this.name;
            }
        }

        protected override Expression GenerateExpressionWithOperands(ExpressionTreeNodeBase[] operandExpressions, int numericTypeValue)
        {
            return Expression.Constant(this.value, typeof(double));
        }
    }
}