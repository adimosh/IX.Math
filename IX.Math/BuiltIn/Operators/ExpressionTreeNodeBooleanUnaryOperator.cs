// <copyright file="ExpressionTreeNodeBooleanUnaryOperator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using IX.Math.PlatformMitigation;
using IX.Math.SimplificationAide;

namespace IX.Math.BuiltIn.Operators
{
    internal sealed class ExpressionTreeNodeBooleanUnaryOperator : ExpressionTreeNodeBase
    {
        private readonly ExpressionType type;

        public ExpressionTreeNodeBooleanUnaryOperator(ExpressionType type)
            : base(WorkingConstants.DefaultNumericType)
        {
            this.type = type;
        }

        public override SupportedValueType[] OperandTypes
        {
            get
            {
                return new SupportedValueType[1] { SupportedValueType.Boolean };
            }
        }

        public override SupportedValueType ReturnType
        {
            get
            {
                return SupportedValueType.Boolean;
            }
        }

        protected override Expression GenerateExpressionWithOperands(ExpressionTreeNodeBase[] operandExpressions, int numericTypeValue)
        {
            var operand = operandExpressions[0];
            var operandExpression = operand.GenerateExpression(numericTypeValue);

            if (operandExpression is ConstantExpression)
            {
                var convertedOperand = (ConstantExpression)operandExpression;

                var mi = typeof(MathematicalUnaryOperationsAide).GetTypeMethod(Enum.GetName(typeof(ExpressionType), this.type), new Type[1] { typeof(bool) });

                if (mi != null)
                {
                    var result = mi.Invoke(null, new[] { convertedOperand.Value });

                    return Expression.Constant(result, typeof(bool));
                }
            }

            return Expression.MakeUnary(this.type, operandExpression, null);
        }
    }
}