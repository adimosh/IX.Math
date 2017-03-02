// <copyright file="BinaryOperationNodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;

namespace IX.Math.Nodes.Operations.Binary
{
    internal abstract class BinaryOperationNodeBase : OperationNodeBase
    {
        protected BinaryOperationNodeBase(NodeBase left, NodeBase right)
        {
            this.Left = left ?? throw new ArgumentNullException(nameof(left));
            this.Right = right ?? throw new ArgumentNullException(nameof(right));
        }

        public NodeBase Left { get; private set; }

        public NodeBase Right { get; private set; }

        public override NodeBase RefreshParametersRecursive()
        {
            this.Left = this.Left.RefreshParametersRecursive();
            this.Right = this.Right.RefreshParametersRecursive();

            return this;
        }

        protected Tuple<Expression, Expression> GetExpressionsOfSameTypeFromOperands()
        {
            if (this.Left.ReturnType == SupportedValueType.String || this.Right.ReturnType == SupportedValueType.String)
            {
                return new Tuple<Expression, Expression>(this.Left.GenerateStringExpression(), this.Right.GenerateStringExpression());
            }

            var le = this.Left.GenerateExpression();
            var re = this.Right.GenerateExpression();

            if (le.Type == typeof(double) && re.Type == typeof(long))
            {
                return new Tuple<Expression, Expression>(le, Expression.Convert(re, typeof(double)));
            }
            else if (le.Type == typeof(long) && re.Type == typeof(double))
            {
                return new Tuple<Expression, Expression>(Expression.Convert(le, typeof(double)), re);
            }

            return new Tuple<Expression, Expression>(le, re);
        }
    }
}