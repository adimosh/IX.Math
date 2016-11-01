using IX.Math.SimplificationAide;
using System;
using System.Linq.Expressions;

namespace IX.Math.BuiltIn
{
    internal sealed class ExpressionTreeNodeParameter : ExpressionTreeNodeBase
    {
        private readonly string name;

        private SupportedValueType setType;

        internal ExpressionTreeNodeParameter(string name)
            : base(WorkingConstants.defaultNumericType)
        {
            this.name = name;

            setType = SupportedValueType.Unknown;
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
                return setType;
            }
        }

        protected override Expression GenerateExpressionWithOperands(ExpressionTreeNodeBase[] operandExpressions, int numericTypeValue)
        {
            Type concreteType = GetConcreteType(numericTypeValue);

            if (concreteType == null)
            {
                return null;
            }

            return Expression.Parameter(concreteType, name);
        }

        internal bool SetConcreteParameterType(SupportedValueType type)
        {
            if (setType == SupportedValueType.Unknown)
            {
                setType = type;
            }
            else
            {
                if (setType != type)
                    return false;
            }

            return true;
        }

        internal Type GetConcreteType(int numericTypeValue)
        {
            switch (setType)
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
    }
}