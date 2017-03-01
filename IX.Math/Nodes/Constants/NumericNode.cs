// <copyright file="NumericNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace IX.Math.Nodes.Constants
{
    [DebuggerDisplay("{Value}")]
    internal sealed class NumericNode : ConstantNodeBase
    {
        private long integerValue;
        private double floatValue;
        private bool isFloat;

        public NumericNode(long value)
        {
            this.Initialize(value);
        }

        public NumericNode(double value)
        {
            this.Initialize(value);
        }

        public NumericNode(object value)
        {
            switch (value)
            {
                case double d:
                    this.Initialize(d);
                    break;
                case long l:
                    this.Initialize(l);
                    break;
                default:
                    throw new ArgumentException(Resources.NumericTypeInvalid, nameof(value));
            }
        }

        public override SupportedValueType ReturnType => SupportedValueType.Numeric;

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
            var floats = ExtractFloats(left, right);
            return new NumericNode(floats.Item1 / floats.Item2);
        }

        public static NumericNode Power(NumericNode left, NumericNode right)
        {
            var floats = ExtractFloats(left, right);
            return new NumericNode(System.Math.Pow(floats.Item1, floats.Item2));
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

        internal static Tuple<double, double> ExtractFloats(NumericNode left, NumericNode right)
        {
            if (left.isFloat && right.isFloat)
            {
                return new Tuple<double, double>(left.floatValue, right.floatValue);
            }
            else if (left.isFloat && !right.isFloat)
            {
                return new Tuple<double, double>(left.floatValue, Convert.ToDouble(right.integerValue));
            }
            else if (!left.isFloat && right.isFloat)
            {
                return new Tuple<double, double>(Convert.ToDouble(left.integerValue), right.floatValue);
            }
            else
            {
                return new Tuple<double, double>(Convert.ToDouble(left.integerValue), Convert.ToDouble(right.integerValue));
            }
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

        public double ExtractFloat()
        {
            if (this.isFloat)
            {
                return this.floatValue;
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

        public override object DistilValue() => this.Value;

        private void Initialize(long value)
        {
            this.integerValue = value;
            this.isFloat = false;
        }

        private void Initialize(double value)
        {
            if (System.Math.Floor(value) == value)
            {
                this.integerValue = Convert.ToInt64(value);
                this.isFloat = false;
            }
            else
            {
                this.floatValue = value;
                this.isFloat = true;
            }
        }
    }
}