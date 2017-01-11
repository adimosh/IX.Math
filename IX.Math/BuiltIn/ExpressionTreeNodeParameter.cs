// <copyright file="ExpressionTreeNodeParameter.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using IX.Math.SimplificationAide;

namespace IX.Math.BuiltIn
{
    internal sealed class ExpressionTreeNodeParameter : ExpressionTreeNodeBase
    {
        private readonly string name;

        private SupportedValueType setType;
        private ParameterExpression generatedExpression;

        internal ExpressionTreeNodeParameter(string name)
            : base(WorkingConstants.DefaultNumericType)
        {
            this.name = name;

            this.setType = SupportedValueType.Unknown;
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
                return this.setType;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public ParameterExpression GeneratedExpression
        {
            get
            {
                if (this.generatedExpression == null)
                {
                    throw new InvalidOperationException();
                }

                return this.generatedExpression;
            }
        }

        internal bool SetConcreteParameterType(SupportedValueType type)
        {
            if (this.setType == SupportedValueType.Unknown)
            {
                this.setType = type;
            }
            else
            {
                if (this.setType != type)
                {
                    return false;
                }
            }

            return true;
        }

        internal Type GetConcreteType(int numericTypeValue)
        {
            switch (this.setType)
            {
                case SupportedValueType.Numeric:
                case SupportedValueType.Unknown:
                    return NumericTypeAide.InverseNumericTypesConversionDictionary[numericTypeValue];
                case SupportedValueType.String:
                    return typeof(string);
                case SupportedValueType.Boolean:
                    return typeof(bool);
                default:
                    return null;
            }
        }

        internal void Reset()
        {
            this.generatedExpression = null;
        }

        protected override Expression GenerateExpressionWithOperands(ExpressionTreeNodeBase[] operandExpressions, int numericTypeValue)
        {
            if (this.generatedExpression == null)
            {
                Type concreteType = this.GetConcreteType(numericTypeValue);

                if (concreteType == null)
                {
                    return null;
                }

                this.generatedExpression = Expression.Parameter(concreteType, this.name);
            }

            return this.generatedExpression;
        }
    }
}