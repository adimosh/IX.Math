// <copyright file="FunctionNodenstrlen.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Function.Unary
{
    internal class FunctionNodenstrlen : UnaryFunctionNodeBase
    {
        public FunctionNodenstrlen(StringNode parameter)
            : base(parameter)
        {
        }

        public FunctionNodenstrlen(StringParameterNode parameter)
            : base(parameter)
        {
        }

        public FunctionNodenstrlen(UndefinedParameterNode parameter)
            : base(parameter?.DetermineString())
        {
        }

        public FunctionNodenstrlen(OperationNodeBase parameter)
            : base(parameter)
        {
            if (parameter?.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException(Resources.NotValidInternally);
            }
        }

        public override SupportedValueType ReturnType => SupportedValueType.Numeric;

        public override NodeBase Simplify()
        {
            throw new NotImplementedException();
        }

        protected override Expression GenerateExpressionInternal()
        {
            throw new NotImplementedException();
        }
    }
}