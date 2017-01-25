// <copyright file="PowerNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Binary
{
    [DebuggerDisplay("{Left} ^ {Right}")]
    internal sealed class PowerNode : BinaryOperationNodeBase
    {
        public PowerNode(NumericNode left, NumericNode right)
            : base(left, right)
        {
        }

        public PowerNode(NumericNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public PowerNode(NumericParameterNode left, NumericNode right)
            : base(left, right)
        {
        }

        public PowerNode(NumericParameterNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public PowerNode(NumericNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public PowerNode(OperationNodeBase left, NumericNode right)
            : base(left?.Simplify(), right)
        {
        }

        public PowerNode(NumericParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public PowerNode(OperationNodeBase left, NumericParameterNode right)
            : base(left?.Simplify(), right)
        {
        }

        public PowerNode(OperationNodeBase left, OperationNodeBase right)
            : base(left?.Simplify(), right?.Simplify())
        {
        }

        public PowerNode(NumericNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineNumeric())
        {
        }

        public PowerNode(UndefinedParameterNode left, NumericNode right)
            : base(left?.DetermineNumeric(), right)
        {
        }

        public PowerNode(NumericParameterNode left, UndefinedParameterNode right)
            : base(left, right?.DetermineNumeric())
        {
        }

        public PowerNode(UndefinedParameterNode left, NumericParameterNode right)
            : base(left?.DetermineNumeric(), right)
        {
        }

        public override NodeBase Simplify()
        {
            if (this.Left is NumericNode && this.Right is NumericNode)
            {
                return NumericNode.Power((NumericNode)this.Left, (NumericNode)this.Right);
            }

            return this;
        }

        protected override Expression GenerateExpressionInternal()
        {
            return Expression.Power(this.Left.GenerateExpression(), this.Right.GenerateExpression());
        }
    }
}