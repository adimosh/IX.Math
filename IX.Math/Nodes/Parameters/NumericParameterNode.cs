// <copyright file="NumericParameterNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.PlatformMitigation;

namespace IX.Math.Nodes.Parameters
{
    [DebuggerDisplay("{ParameterName} (numeric)")]
    internal sealed class NumericParameterNode : ParameterNodeBase
    {
        public NumericParameterNode(string parameterName)
            : base(parameterName)
        {
        }

        public bool? RequireFloat { get; set; }

        public override SupportedValueType ReturnType => SupportedValueType.Numeric;

        public override Expression GenerateStringExpression() => Expression.Call(this.GenerateExpression(), typeof(object).GetTypeMethod(nameof(object.ToString)));

        public NumericParameterNode ParameterMustBeFloat()
        {
            if (this.RequireFloat != null)
            {
                if (!this.RequireFloat.Value)
                {
                    throw new InvalidOperationException(string.Format(Resources.ParameterMustBeFloatButAlreadyRequiredToBeInteger, this.ParameterName));
                }
            }
            else
            {
                this.RequireFloat = true;
            }

            return this;
        }

        public NumericParameterNode ParameterMustBeInteger()
        {
            if (this.RequireFloat != null)
            {
                if (this.RequireFloat.Value)
                {
                    throw new InvalidOperationException(string.Format(Resources.ParameterMustBeIntegerButAlreadyRequiredToBeFloat, this.ParameterName));
                }
            }
            else
            {
                this.RequireFloat = false;
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal()
        {
            return (this.RequireFloat ?? true) ?
                Expression.Parameter(typeof(double), this.ParameterName) :
                Expression.Parameter(typeof(long), this.ParameterName);
        }
    }
}