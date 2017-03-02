// <copyright file="AddNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Parameters;

namespace IX.Math.Nodes.Operations.Binary
{
    [DebuggerDisplay("{Left} + {Right}")]
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
            if (right?.ReturnType != SupportedValueType.Numeric && right?.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public AddNode(OperationNodeBase left, NumericNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.Numeric && left?.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public AddNode(OperationNodeBase left, OperationNodeBase right)
            : base(left?.Simplify(), right?.Simplify())
        {
            if (right?.ReturnType != left?.ReturnType && right?.ReturnType != SupportedValueType.String && left?.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public AddNode(NumericParameterNode left, OperationNodeBase right)
            : base(left, right?.Simplify())
        {
            if (right?.ReturnType != SupportedValueType.Numeric && right?.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
        }

        public AddNode(OperationNodeBase left, NumericParameterNode right)
            : base(left?.Simplify(), right)
        {
            if (left?.ReturnType != SupportedValueType.Numeric && left?.ReturnType != SupportedValueType.String)
            {
                throw new ExpressionNotValidLogicallyException();
            }
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

        public AddNode(UndefinedParameterNode left, UndefinedParameterNode right)
            : base(left?.DetermineNumeric(), right?.DetermineNumeric())
        {
        }

        public AddNode(UndefinedParameterNode left, NodeBase right)
            : base(left, right?.Simplify())
        {
            if (this.Right.ReturnType == SupportedValueType.Numeric)
            {
                this.Left = left.DetermineNumeric();
            }
            else
            {
                this.Left = left.DetermineString();
            }
        }

        public AddNode(NodeBase left, UndefinedParameterNode right)
            : base(left?.Simplify(), right)
        {
            if (this.Left.ReturnType == SupportedValueType.Numeric)
            {
                this.Right = right.DetermineNumeric();
            }
            else
            {
                this.Right = right.DetermineString();
            }
        }

        public override SupportedValueType ReturnType
        {
            get
            {
                if (this.Left.ReturnType == SupportedValueType.String || this.Right.ReturnType == SupportedValueType.String)
                {
                    return SupportedValueType.String;
                }

                return SupportedValueType.Numeric;
            }
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

        protected override Expression GenerateExpressionInternal()
        {
            var pars = this.GetExpressionsOfSameTypeFromOperands();

            if (this.ReturnType == SupportedValueType.String)
            {
                return Expression.Call(typeof(string), nameof(string.Concat), null, pars.Item1, pars.Item2);
            }
            else
            {
                return Expression.Add(pars.Item1, pars.Item2);
            }
        }
    }
}