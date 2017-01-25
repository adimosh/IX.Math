// <copyright file="XorNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Binary
{
    internal sealed class XorNode : BinaryOperationNodeBase
    {
        public XorNode(NumericNode left, NumericNode right)
            : base(left, right)
        {
        }

        public XorNode(NumericNode left, NumericParameterNode right)
            : base(left, right)
        {
            OperationsHelper.ParameterMustBeInteger(right);
        }

        public XorNode(NumericParameterNode left, NumericNode right)
            : base(left, right)
        {
            OperationsHelper.ParameterMustBeInteger(left);
        }

        public XorNode(NumericParameterNode left, NumericParameterNode right)
            : base(left, right)
        {
            OperationsHelper.ParameterMustBeInteger(left);
            OperationsHelper.ParameterMustBeInteger(right);
        }

        public XorNode(NumericNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public XorNode(OperationNodeBase left, NumericNode right)
            : base(left?.Simplify(), right)
        {
        }

        public XorNode(NumericParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            OperationsHelper.ParameterMustBeInteger(left);
        }

        public XorNode(OperationNodeBase left, NumericParameterNode right)
            : base(left?.Simplify(), right)
        {
            OperationsHelper.ParameterMustBeInteger(right);
        }

        public XorNode(OperationNodeBase left, OperationNodeBase right)
            : base(left?.Simplify(), right?.Simplify())
        {
        }

        public XorNode(BoolNode left, BoolNode right)
            : base(left, right)
        {
        }

        public XorNode(BoolNode left, BoolParameterNode right)
            : base(left, right)
        {
        }

        public XorNode(BoolParameterNode left, BoolNode right)
            : base(left, right)
        {
        }

        public XorNode(BoolParameterNode left, BoolParameterNode right)
            : base(left, right)
        {
        }

        public XorNode(BoolNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public XorNode(OperationNodeBase left, BoolNode right)
            : base(left?.Simplify(), right)
        {
        }

        public XorNode(BoolParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public XorNode(OperationNodeBase left, BoolParameterNode right)
            : base(left?.Simplify(), right)
        {
        }

        public XorNode(NumericNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineNumeric())
        {
        }

        public XorNode(UndefinedParameterNode left, NumericNode right)
            : base(left?.DetermineNumeric(), right)
        {
        }

        public XorNode(BoolNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineBool())
        {
        }

        public XorNode(UndefinedParameterNode left, BoolNode right)
            : base(left?.DetermineBool(), right)
        {
        }

        public XorNode(NumericParameterNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineNumeric())
        {
        }

        public XorNode(UndefinedParameterNode left, NumericParameterNode right)
            : base(left?.DetermineNumeric(), right)
        {
        }

        public XorNode(BoolParameterNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineBool())
        {
        }

        public XorNode(UndefinedParameterNode left, BoolParameterNode right)
            : base(left?.DetermineBool(), right)
        {
        }

        public override NodeBase Simplify()
        {
            if (this.Left is NumericNode && this.Right is NumericNode)
            {
                var left = (this.Left as NumericNode).ExtractInteger();
                var right = (this.Right as NumericNode).ExtractInteger();

                return new NumericNode(left ^ right);
            }

            if (this.Left is BoolNode && this.Right is BoolNode)
            {
                return new BoolNode(((BoolNode)this.Left).Value ^ ((BoolNode)this.Right).Value);
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal()
        {
            return Expression.ExclusiveOr(this.Left.GenerateExpression(), this.Right.GenerateExpression());
        }
    }
}