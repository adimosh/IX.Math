// <copyright file="RightShiftNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Binary
{
    internal sealed class RightShiftNode : BinaryOperationNodeBase
    {
        public RightShiftNode(NumericNode left, NumericNode right)
            : base(left, right)
        {
        }

        public RightShiftNode(NumericNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public RightShiftNode(NumericParameterNode left, NumericNode right)
            : base(left, right)
        {
        }

        public RightShiftNode(NumericParameterNode left, NumericParameterNode right)
            : base(left, right)
        {
        }

        public override NodeBase Simplify()
        {
            if (this.Right is NumericNode && this.Right is NumericNode)
            {
                return NumericNode.RightShift((NumericNode)this.Right, (NumericNode)this.Right);
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
                return Expression.RightShift(this.Right.GenerateExpression(), this.Right.GenerateExpression());
            }
        }
    }
}