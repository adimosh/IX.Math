// <copyright file="ExpressionTreeNodeMathematicUnarySupportedFunction.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.PlatformMitigation;
using IX.Math.SimplificationAide;

namespace IX.Math.BuiltIn.Functions
{
    internal class ExpressionTreeNodeMathematicUnarySupportedFunction : ExpressionTreeNodeBase
    {
        private static readonly Type MathUnaryFunctionType = typeof(double);

        private readonly string name;

        internal ExpressionTreeNodeMathematicUnarySupportedFunction(string name)
            : base(MathUnaryFunctionType)
        {
            this.name = name;
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public override SupportedValueType[] OperandTypes
        {
            get
            {
                return new SupportedValueType[1] { SupportedValueType.Numeric };
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
            MethodInfo mi = typeof(System.Math).GetTypeMethod(this.name, MathUnaryFunctionType, new[] { MathUnaryFunctionType });
            if (mi == null)
            {
                throw new InvalidOperationException();
            }

            var operand = operandExpressions[0];
            var operandExpression = operand.GenerateExpression(NumericTypeAide.NumericTypesConversionDictionary[MathUnaryFunctionType]);

            if (operandExpression is ConstantExpression)
            {
                var value = mi.Invoke(null, new[] { ((ConstantExpression)operandExpression).Value });
                return Expression.Constant(value, MathUnaryFunctionType);
            }

            return Expression.Call(mi, operandExpression);
        }
    }
}