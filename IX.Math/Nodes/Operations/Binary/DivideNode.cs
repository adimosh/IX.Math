﻿// <copyright file="DivideNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Binary
{
    internal sealed class DivideNode : BinaryOperationNodeBase
    {
        public DivideNode(NumericNode left, NumericNode right)
            : base(left, right)
        {
        }

        public DivideNode(NumericNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public DivideNode(NumericParameterNode left, NumericNode right)
            : base(left, right)
        {
        }

        public DivideNode(NumericParameterNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public DivideNode(NumericNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public DivideNode(OperationNodeBase left, NumericNode right)
            : base(left?.Simplify(), right)
        {
        }

        public DivideNode(NumericParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public DivideNode(OperationNodeBase left, NumericParameterNode right)
            : base(left?.Simplify(), right)
        {
        }

        public DivideNode(OperationNodeBase left, OperationNodeBase right)
            : base(left?.Simplify(), right?.Simplify())
        {
        }

        public DivideNode(NumericNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineNumeric())
        {
        }

        public DivideNode(UndefinedParameterNode left, NumericNode right)
            : base(left?.DetermineNumeric(), right)
        {
        }

        public DivideNode(NumericParameterNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineNumeric())
        {
        }

        public DivideNode(UndefinedParameterNode left, NumericParameterNode right)
            : base(left?.DetermineNumeric(), right)
        {
        }

        public override NodeBase Simplify()
        {
            if (this.Left is NumericNode && this.Right is NumericNode)
            {
                return NumericNode.Divide((NumericNode)this.Left, (NumericNode)this.Right);
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal()
        {
            return Expression.Divide(this.Left.GenerateExpression(), this.Right.GenerateExpression());
        }
    }
}