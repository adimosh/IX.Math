// <copyright file="ExpressionTreeNodeMathematicBinarySupportedFunction.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.PlatformMitigation;
using IX.Math.SimplificationAide;

namespace IX.Math.BuiltIn.Functions
{
    internal sealed class ExpressionTreeNodeMathematicBinarySupportedFunction : ExpressionTreeNodeBase
    {
        private static readonly Type MathBinaryFunctionType = typeof(double);

        private readonly string name;

        internal ExpressionTreeNodeMathematicBinarySupportedFunction(string name)
            : base(MathBinaryFunctionType)
        {
            this.name = name;
        }

        public string Name => this.name;

        public override SupportedValueType[] OperandTypes
        {
            get
            {
                return new SupportedValueType[2] { SupportedValueType.Numeric, SupportedValueType.Numeric };
            }
        }

        public override SupportedValueType ReturnType
        {
            get
            {
                return SupportedValueType.Numeric;
            }
        }

        protected override Expression GenerateExpressionWithOperands(ExpressionTreeNodeBase[] operandExpressions, int numericTypeValue)
        {
            MethodInfo mi = typeof(System.Math).GetTypeMethod(this.name, MathBinaryFunctionType, new[] { MathBinaryFunctionType, MathBinaryFunctionType });
            if (mi == null)
            {
                throw new InvalidOperationException();
            }

            var operand1 = operandExpressions[0];
            var operandExpression1 = operand1.GenerateExpression(NumericTypeAide.NumericTypesConversionDictionary[MathBinaryFunctionType]);
            var operand2 = operandExpressions[1];
            var operandExpression2 = operand2.GenerateExpression(NumericTypeAide.NumericTypesConversionDictionary[MathBinaryFunctionType]);

            if (operandExpression1 is ConstantExpression && operandExpression2 is ConstantExpression)
            {
                var value = mi.Invoke(
                    null,
                    new[] { ((ConstantExpression)operandExpression1).Value, ((ConstantExpression)operandExpression2).Value, });
                return Expression.Constant(value, MathBinaryFunctionType);
            }

            return Expression.Call(mi, operandExpression1, operandExpression2);
        }
    }
}