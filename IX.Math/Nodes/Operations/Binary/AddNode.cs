// <copyright file="AddNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Binary
{
    internal sealed class AddNode : BinaryOperationNodeBase
    {
        public AddNode(NumericNode left, NumericNode right)
            : base(left, right)
        {
        }

        public AddNode(NumericNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public AddNode(NumericParameterNode left, NumericNode right)
            : base(left, right)
        {
        }

        public AddNode(NumericParameterNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public AddNode(StringNode left, StringNode right)
            : base(left, right)
        {
        }

        public AddNode(StringNode left, StringParameterNode right)
            : base(left, right)
        {
        }

        public AddNode(StringParameterNode left, StringNode right)
            : base(left, right)
        {
        }

        public AddNode(StringParameterNode left, StringParameterNode right)
            : base(left, right)
        {
        }

        public AddNode(NumericNode left, StringNode right)
            : base(left, right)
        {
        }

        public AddNode(StringNode left, NumericNode right)
            : base(left, right)
        {
        }

        public AddNode(NumericNode left, StringParameterNode right)
            : base(left, right)
        {
        }

        public AddNode(StringParameterNode left, NumericNode right)
            : base(left, right)
        {
        }

        public AddNode(NumericParameterNode left, StringNode right)
            : base(left, right)
        {
        }

        public AddNode(StringNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public AddNode(NumericParameterNode left, StringParameterNode right)
            : base(left, right)
        {
        }

        public AddNode(StringParameterNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public AddNode(BoolNode left, StringNode right)
            : base(left, right)
        {
        }

        public AddNode(StringNode left, BoolNode right)
            : base(left, right)
        {
        }

        public AddNode(BoolNode left, StringParameterNode right)
            : base(left, right)
        {
        }

        public AddNode(StringParameterNode left, BoolNode right)
            : base(left, right)
        {
        }

        public AddNode(BoolParameterNode left, StringNode right)
            : base(left, right)
        {
        }

        public AddNode(StringNode left, BoolParameterNode right)
            : base(left, right)
        {
        }

        public AddNode(BoolParameterNode left, StringParameterNode right)
            : base(left, right)
        {
        }

        public AddNode(StringParameterNode left, BoolParameterNode right)
            : base(left, right)
        {
        }

        public AddNode(NumericNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public AddNode(OperationNodeBase left, NumericNode right)
            : base(left?.Simplify(), right)
        {
        }

        public AddNode(OperationNodeBase left, OperationNodeBase right)
            : base(left?.Simplify(), right?.Simplify())
        {
        }

        public AddNode(NumericParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public AddNode(OperationNodeBase left, NumericParameterNode right)
            : base(left?.Simplify(), right)
        {
        }

        public AddNode(StringNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public AddNode(OperationNodeBase left, StringNode right)
            : base(left?.Simplify(), right)
        {
        }

        public AddNode(StringParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
        }

        public AddNode(OperationNodeBase left, StringParameterNode right)
            : base(left?.Simplify(), right)
        {
        }

        public override NodeBase Simplify()
        {
            if (this.Left is NumericNode && this.Right is NumericNode)
            {
                return NumericNode.Add((NumericNode)this.Left, (NumericNode)this.Right);
            }

            if (this.Left is StringNode && this.Right is StringNode)
            {
                return new StringNode(((StringNode)this.Left).Value + ((StringNode)this.Right).Value);
            }

            if (this.Left is NumericNode && this.Right is StringNode)
            {
                return new StringNode($"{((NumericNode)this.Left).Value}{((StringNode)this.Right).Value}");
            }

            if (this.Left is StringNode && this.Right is NumericNode)
            {
                return new StringNode($"{((StringNode)this.Left).Value}{((NumericNode)this.Right).Value}");
            }

            if (this.Left is BoolNode && this.Right is StringNode)
            {
                return new StringNode($"{((BoolNode)this.Left).Value}{((StringNode)this.Right).Value}");
            }

            if (this.Left is StringNode && this.Right is BoolNode)
            {
                return new StringNode($"{((StringNode)this.Left).Value}{((BoolNode)this.Right).Value}");
            }

            return this;
        }

        public override Expression GenerateExpression()
        {
            NodeBase simplifiedExpression = this.Simplify();

            if (simplifiedExpression != null)
            {
                return simplifiedExpression.GenerateExpression();
            }
            else
            {
                return Expression.Add(this.Left.GenerateExpression(), this.Right.GenerateExpression());
            }
        }
    }
}