// <copyright file="EqualsNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Binary
{
    internal sealed class EqualsNode : BinaryOperationNodeBase
    {
        public EqualsNode(NumericNode left, NumericNode right)
            : base(left, right)
        {
        }

        public EqualsNode(NumericNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public EqualsNode(NumericParameterNode left, NumericNode right)
            : base(left, right)
        {
        }

        public EqualsNode(NumericParameterNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public EqualsNode(StringNode left, StringNode right)
            : base(left, right)
        {
        }

        public EqualsNode(StringNode left, StringParameterNode right)
            : base(left, right)
        {
        }

        public EqualsNode(StringParameterNode left, StringNode right)
            : base(left, right)
        {
        }

        public EqualsNode(StringParameterNode left, StringParameterNode right)
            : base(left, right)
        {
        }

        public EqualsNode(BoolNode left, BoolNode right)
            : base(left, right)
        {
        }

        public EqualsNode(BoolNode left, BoolParameterNode right)
            : base(left, right)
        {
        }

        public EqualsNode(BoolParameterNode left, BoolNode right)
            : base(left, right)
        {
        }

        public EqualsNode(BoolParameterNode left, BoolParameterNode right)
            : base(left, right)
        {
        }

        public EqualsNode(NumericNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public EqualsNode(OperationNodeBase left, NumericNode right)
            : base(left?.Simplify(), right)
        {
        }

        public EqualsNode(OperationNodeBase left, OperationNodeBase right)
            : base(left?.Simplify(), right?.Simplify())
        {
        }

        public EqualsNode(NumericParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public EqualsNode(OperationNodeBase left, NumericParameterNode right)
            : base(left?.Simplify(), right)
        {
        }

        public EqualsNode(StringNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public EqualsNode(OperationNodeBase left, StringNode right)
            : base(left?.Simplify(), right)
        {
        }

        public EqualsNode(StringParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public EqualsNode(OperationNodeBase left, StringParameterNode right)
            : base(left?.Simplify(), right)
        {
        }

        public EqualsNode(BoolNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public EqualsNode(OperationNodeBase left, BoolNode right)
            : base(left?.Simplify(), right)
        {
        }

        public EqualsNode(BoolParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public EqualsNode(OperationNodeBase left, BoolParameterNode right)
            : base(left?.Simplify(), right)
        {
        }

        public EqualsNode(NumericNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineNumeric())
        {
        }

        public EqualsNode(UndefinedParameterNode left, NumericNode right)
            : base(left?.DetermineNumeric(), right)
        {
        }

        public EqualsNode(NumericParameterNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineNumeric())
        {
        }

        public EqualsNode(UndefinedParameterNode left, NumericParameterNode right)
            : base(left?.DetermineNumeric(), right)
        {
        }

        public EqualsNode(BoolNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineBool())
        {
        }

        public EqualsNode(UndefinedParameterNode left, BoolNode right)
            : base(left?.DetermineBool(), right)
        {
        }

        public EqualsNode(BoolParameterNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineBool())
        {
        }

        public EqualsNode(UndefinedParameterNode left, BoolParameterNode right)
            : base(left?.DetermineBool(), right)
        {
        }

        public EqualsNode(StringNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineString())
        {
        }

        public EqualsNode(UndefinedParameterNode left, StringNode right)
            : base(left?.DetermineString(), right)
        {
        }

        public EqualsNode(StringParameterNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineString())
        {
        }

        public EqualsNode(UndefinedParameterNode left, StringParameterNode right)
            : base(left?.DetermineString(), right)
        {
        }

        public override NodeBase Simplify()
        {
            if (this.Left is NumericNode && this.Right is NumericNode)
            {
                return new BoolNode(((NumericNode)this.Left).Value == ((NumericNode)this.Right).Value);
            }

            if (this.Left is StringNode && this.Right is StringNode)
            {
                return new BoolNode(((StringNode)this.Left).Value == ((StringNode)this.Right).Value);
            }

            if (this.Left is BoolNode && this.Right is BoolNode)
            {
                return new BoolNode(((BoolNode)this.Left).Value == ((BoolNode)this.Right).Value);
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal()
        {
            return Expression.Equal(this.Left.GenerateExpression(), this.Right.GenerateExpression());
        }
    }
}