// <copyright file="ExpressionTreeNodeStringPropertySupportedFunction.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.SimplificationAide;

namespace IX.Math.BuiltIn.Functions
{
    internal sealed class ExpressionTreeNodeStringPropertySupportedFunction : ExpressionTreeNodeBase
    {
        public ExpressionTreeNodeStringPropertySupportedFunction(string name)
            : base(WorkingConstants.DefaultNumericType)
        {
            this.Name = name;
        }

        public string Name { get; private set; }

        public override SupportedValueType[] OperandTypes
        {
            get
            {
                return new SupportedValueType[1] { SupportedValueType.String };
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
            var pi = typeof(string).GetTypeInfo().DeclaredProperties.SingleOrDefault(p => p.Name == this.Name);

            ExpressionTreeNodeBase op = operandExpressions[0];
            var opExpression = op.GenerateExpression(numericTypeValue);

            if (opExpression is ConstantExpression)
            {
                Type numType = NumericTypeAide.InverseNumericTypesConversionDictionary[numericTypeValue];
                return Expression.Constant(Convert.ChangeType(pi.GetValue(((ConstantExpression)opExpression).Value), numType), numType);
            }

            return Expression.Property(opExpression, pi);
        }
    }
}