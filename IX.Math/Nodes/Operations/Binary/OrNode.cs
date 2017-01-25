// <copyright file="OrNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Binary
{
    [DebuggerDisplay("{Left} | {Right}")]
    internal sealed class OrNode : BinaryOperationNodeBase
    {
        public OrNode(NumericNode left, NumericNode right)
            : base(left, right)
        {
        }

        public OrNode(NumericNode left, NumericParameterNode right)
            : base(left, right)
        {
            OperationsHelper.ParameterMustBeInteger(right);
        }

        public OrNode(NumericParameterNode left, NumericNode right)
            : base(left, right)
        {
            OperationsHelper.ParameterMustBeInteger(left);
        }

        public OrNode(NumericParameterNode left, NumericParameterNode right)
            : base(left, right)
        {
            OperationsHelper.ParameterMustBeInteger(left);
            OperationsHelper.ParameterMustBeInteger(right);
        }

        public OrNode(NumericNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public OrNode(OperationNodeBase left, NumericNode right)
            : base(left?.Simplify(), right)
        {
        }

        public OrNode(NumericParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            OperationsHelper.ParameterMustBeInteger(left);
        }

        public OrNode(OperationNodeBase left, NumericParameterNode right)
            : base(left?.Simplify(), right)
        {
            OperationsHelper.ParameterMustBeInteger(right);
        }

        public OrNode(OperationNodeBase left, OperationNodeBase right)
            : base(left?.Simplify(), right?.Simplify())
        {
        }

        public OrNode(BoolNode left, BoolNode right)
            : base(left, right)
        {
        }

        public OrNode(BoolNode left, BoolParameterNode right)
            : base(left, right)
        {
        }

        public OrNode(BoolParameterNode left, BoolNode right)
            : base(left, right)
        {
        }

        public OrNode(BoolParameterNode left, BoolParameterNode right)
            : base(left, right)
        {
        }

        public OrNode(BoolNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public OrNode(OperationNodeBase left, BoolNode right)
            : base(left?.Simplify(), right)
        {
        }

        public OrNode(BoolParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public OrNode(OperationNodeBase left, BoolParameterNode right)
            : base(left?.Simplify(), right)
        {
        }

        public OrNode(NumericNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineNumeric())
        {
        }

        public OrNode(UndefinedParameterNode left, NumericNode right)
            : base(left?.DetermineNumeric(), right)
        {
        }

        public OrNode(BoolNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineBool())
        {
        }

        public OrNode(UndefinedParameterNode left, BoolNode right)
            : base(left?.DetermineBool(), right)
        {
        }

        public OrNode(NumericParameterNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineNumeric())
        {
        }

        public OrNode(UndefinedParameterNode left, NumericParameterNode right)
            : base(left?.DetermineNumeric(), right)
        {
        }

        public OrNode(BoolParameterNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineBool())
        {
        }

        public OrNode(UndefinedParameterNode left, BoolParameterNode right)
            : base(left?.DetermineBool(), right)
        {
        }

        public override NodeBase Simplify()
        {
            if (this.Left is NumericNode && this.Right is NumericNode)
            {
                var left = (this.Left as NumericNode).ExtractInteger();
                var right = (this.Right as NumericNode).ExtractInteger();

                return new NumericNode(left | right);
            }

            if (this.Left is BoolNode && this.Right is BoolNode)
            {
                return new BoolNode(((BoolNode)this.Left).Value | ((BoolNode)this.Right).Value);
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal()
        {
            return Expression.Or(this.Left.GenerateExpression(), this.Right.GenerateExpression());
        }
    }
}