// <copyright file="NumericNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq.Expressions;

namespace IX.Math.Nodes.Constants
{
    internal sealed class NumericNode : ConstantNodeBase
    {
        private readonly long integerValue;
        private readonly double floatValue;
        private readonly bool isFloat;

        public NumericNode(long value)
        {
            this.integerValue = value;
            this.isFloat = false;
        }

        public NumericNode(double value)
        {
            this.floatValue = value;
            this.isFloat = true;
        }

        public object Value => this.isFloat ? this.floatValue : this.integerValue;

        public static NumericNode Add(NumericNode left, NumericNode right)
        {
            if (left.isFloat && right.isFloat)
            {
                return new NumericNode(left.floatValue + right.floatValue);
            }
            else if (left.isFloat && !right.isFloat)
            {
                return new NumericNode(left.floatValue + Convert.ToDouble(right.integerValue));
            }
            else if (!left.isFloat && right.isFloat)
            {
                return new NumericNode(Convert.ToDouble(left.integerValue) + right.floatValue);
            }
            else
            {
                return new NumericNode(left.integerValue + right.integerValue);
            }
        }

        public static NumericNode Subtract(NumericNode left, NumericNode right)
        {
            if (left.isFloat && right.isFloat)
            {
                return new NumericNode(left.floatValue - right.floatValue);
            }
            else if (left.isFloat && !right.isFloat)
            {
                return new NumericNode(left.floatValue - Convert.ToDouble(right.integerValue));
            }
            else if (!left.isFloat && right.isFloat)
            {
                return new NumericNode(Convert.ToDouble(left.integerValue) - right.floatValue);
            }
            else
            {
                return new NumericNode(left.integerValue - right.integerValue);
            }
        }

        public static NumericNode Multiply(NumericNode left, NumericNode right)
        {
            if (left.isFloat && right.isFloat)
            {
                return new NumericNode(left.floatValue * right.floatValue);
            }
            else if (left.isFloat && !right.isFloat)
            {
                return new NumericNode(left.floatValue * Convert.ToDouble(right.integerValue));
            }
            else if (!left.isFloat && right.isFloat)
            {
                return new NumericNode(Convert.ToDouble(left.integerValue) * right.floatValue);
            }
            else
            {
                return new NumericNode(left.integerValue * right.integerValue);
            }
        }

        public static NumericNode Divide(NumericNode left, NumericNode right)
        {
            if (left.isFloat && right.isFloat)
            {
                return new NumericNode(left.floatValue / right.floatValue);
            }
            else if (left.isFloat && !right.isFloat)
            {
                return new NumericNode(left.floatValue / Convert.ToDouble(right.integerValue));
            }
            else if (!left.isFloat && right.isFloat)
            {
                return new NumericNode(Convert.ToDouble(left.integerValue) / right.floatValue);
            }
            else
            {
                return new NumericNode(Convert.ToDouble(left.integerValue) / Convert.ToDouble(right.integerValue));
            }
        }

        public static NumericNode LeftShift(NumericNode left, NumericNode right)
        {
            int by = right.ExtractInt();
            long data = left.ExtractInteger();

            return new NumericNode(data << by);
        }

        public static NumericNode RightShift(NumericNode left, NumericNode right)
        {
            int by = right.ExtractInt();
            long data = left.ExtractInteger();

            return new NumericNode(data >> by);
        }

        public override Expression GenerateExpression() => this.isFloat ?
            Expression.Constant(this.floatValue, typeof(double)) :
            Expression.Constant(this.integerValue, typeof(long));

        public Expression GenerateFloatExpression()
        {
            if (this.isFloat)
            {
                return this.GenerateExpression();
            }
            else
            {
                return Expression.Constant(Convert.ToDouble(this.floatValue), typeof(double));
            }
        }

        public Expression GenerateLongExpression()
        {
            if (!this.isFloat)
            {
                return this.GenerateExpression();
            }
            else
            {
                if (System.Math.Floor(this.floatValue) != this.floatValue)
                {
                    throw new InvalidCastException();
                }

                return Expression.Constant(Convert.ToInt64(this.floatValue), typeof(long));
            }
        }

        public long ExtractInteger()
        {
            if (this.isFloat)
            {
                if (System.Math.Floor(this.floatValue) != this.floatValue)
                {
                    throw new InvalidCastException();
                }

                return Convert.ToInt64(this.floatValue);
            }

            return this.integerValue;
        }

        public int ExtractInt()
        {
            if (this.isFloat)
            {
                if (System.Math.Floor(this.floatValue) != this.floatValue)
                {
                    throw new InvalidCastException();
                }

                return Convert.ToInt32(this.floatValue);
            }

            return Convert.ToInt32(this.integerValue);
        }
    }
}