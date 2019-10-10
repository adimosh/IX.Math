// <copyright file="NumericNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Constants
{
    /// <summary>
    /// A numeric node. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="ConstantNodeBase" />
    [DebuggerDisplay("{" + nameof(Value) + "}")]
    [PublicAPI]
    public sealed class NumericNode : ConstantNodeBase
    {
        /// <summary>
        /// The integer value.
        /// </summary>
        private long integerValue;

        /// <summary>
        /// The float value.
        /// </summary>
        private double floatValue;

        /// <summary>
        /// Whether or not the number is floating-point.
        /// </summary>
        private bool isFloat;

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericNode"/> class.
        /// </summary>
        /// <param name="value">The integer value.</param>
        public NumericNode(long value)
        {
            this.Initialize(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericNode"/> class.
        /// </summary>
        /// <param name="value">The floating-point value.</param>
        public NumericNode(double value)
        {
            this.Initialize(value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NumericNode"/> class.
        /// </summary>
        /// <param name="value">The undefined value.</param>
        /// <exception cref="ArgumentException">The value is not in an expected format.</exception>
        public NumericNode([NotNull] object value)
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

        private NumericNode()
        {
        }

        /// <summary>
        /// Gets the return type of this node.
        /// </summary>
        /// <value>Always <see cref="SupportedValueType.Numeric"/>.</value>
        public override SupportedValueType ReturnType => SupportedValueType.Numeric;

#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation - This is actually desired
        /// <summary>
        /// Gets the value.
        /// </summary>
        /// <value>The value.</value>
        public object Value => this.isFloat ? this.floatValue : this.integerValue;
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation

        /// <summary>
        /// Does an addition between two numeric nodes.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The resulting node.</returns>
        [NotNull]
        public static NumericNode Add([NotNull] NumericNode left, [NotNull] NumericNode right)
        {
            if (left.isFloat && right.isFloat)
            {
                return new NumericNode(left.floatValue + right.floatValue);
            }

            if (left.isFloat && !right.isFloat)
            {
                return new NumericNode(left.floatValue + Convert.ToDouble(right.integerValue));
            }

            if (!left.isFloat && right.isFloat)
            {
                return new NumericNode(Convert.ToDouble(left.integerValue) + right.floatValue);
            }

            return new NumericNode(left.integerValue + right.integerValue);
        }

        /// <summary>
        /// Does a subtraction between two numeric nodes.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The resulting node.</returns>
        [NotNull]
        public static NumericNode Subtract([NotNull] NumericNode left, [NotNull] NumericNode right)
        {
            if (left.isFloat && right.isFloat)
            {
                return new NumericNode(left.floatValue - right.floatValue);
            }

            if (left.isFloat && !right.isFloat)
            {
                return new NumericNode(left.floatValue - Convert.ToDouble(right.integerValue));
            }

            if (!left.isFloat && right.isFloat)
            {
                return new NumericNode(Convert.ToDouble(left.integerValue) - right.floatValue);
            }

            return new NumericNode(left.integerValue - right.integerValue);
        }

        /// <summary>
        /// Does a multiplication between two numeric nodes.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The resulting node.</returns>
        [NotNull]
        public static NumericNode Multiply([NotNull] NumericNode left, [NotNull] NumericNode right)
        {
            if (left.isFloat && right.isFloat)
            {
                return new NumericNode(left.floatValue * right.floatValue);
            }

            if (left.isFloat && !right.isFloat)
            {
                return new NumericNode(left.floatValue * Convert.ToDouble(right.integerValue));
            }

            if (!left.isFloat && right.isFloat)
            {
                return new NumericNode(Convert.ToDouble(left.integerValue) * right.floatValue);
            }

            return new NumericNode(left.integerValue * right.integerValue);
        }

        /// <summary>
        /// Does a division between two numeric nodes.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The resulting node.</returns>
        [NotNull]
        public static NumericNode Divide([NotNull] NumericNode left, [NotNull] NumericNode right)
        {
            var (divided, divisor) = ExtractFloats(left, right);
            return new NumericNode(divided / divisor);
        }

        /// <summary>
        /// Raises the left node's value to the power specified by the right node's value.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The resulting node.</returns>
        [NotNull]
        public static NumericNode Power([NotNull] NumericNode left, [NotNull] NumericNode right)
        {
            var (@base, pow) = ExtractFloats(left, right);
            return new NumericNode(global::System.Math.Pow(@base, pow));
        }

        /// <summary>
        /// Does a left shift between two numeric nodes.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The resulting node.</returns>
        [NotNull]
        public static NumericNode LeftShift([NotNull] NumericNode left, [NotNull] NumericNode right)
        {
            var by = right.ExtractInt();
            var data = left.ExtractInteger();

            return new NumericNode(data << by);
        }

        /// <summary>
        /// Does a right shift between two numeric nodes.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The resulting node.</returns>
        [NotNull]
        public static NumericNode RightShift([NotNull] NumericNode left, [NotNull] NumericNode right)
        {
            var by = right.ExtractInt();
            var data = left.ExtractInteger();

            return new NumericNode(data >> by);
        }

        /// <summary>
        /// Extracts the floating-point values from two nodes.
        /// </summary>
        /// <param name="left">The left node.</param>
        /// <param name="right">The right node.</param>
        /// <returns>A tuple of floating-point values.</returns>
        [NotNull]
        internal static Tuple<double, double> ExtractFloats([NotNull] NumericNode left, [NotNull] NumericNode right)
        {
            if (left.isFloat && right.isFloat)
            {
                return new Tuple<double, double>(left.floatValue, right.floatValue);
            }

            if (left.isFloat && !right.isFloat)
            {
                return new Tuple<double, double>(left.floatValue, Convert.ToDouble(right.integerValue));
            }

            if (!left.isFloat && right.isFloat)
            {
                return new Tuple<double, double>(Convert.ToDouble(left.integerValue), right.floatValue);
            }

            return new Tuple<double, double>(Convert.ToDouble(left.integerValue), Convert.ToDouble(right.integerValue));
        }

#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation - This is required here, as we are generating an expression tree.
        /// <summary>
        /// Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>The expression.</returns>
        public override Expression GenerateCachedExpression() => this.isFloat ?
            Expression.Constant(this.floatValue, typeof(double)) :
            Expression.Constant(this.integerValue, typeof(long));

        /// <summary>
        /// Generates a floating-point expression.
        /// </summary>
        /// <returns>The expression.</returns>
        [NotNull]
        public Expression GenerateFloatExpression() => this.isFloat ? this.GenerateExpression() : Expression.Constant(Convert.ToDouble(this.floatValue), typeof(double));

        /// <summary>
        /// Generates an integer expression.
        /// </summary>
        /// <returns>The expression.</returns>
        /// <exception cref="InvalidCastException">The node is floating-point and cannot be transformed.</exception>
        [NotNull]
        public Expression GenerateLongExpression()
        {
            if (!this.isFloat)
            {
                return this.GenerateExpression();
            }

            if (global::System.Math.Floor(this.floatValue) != this.floatValue)
            {
                throw new InvalidCastException();
            }

            return Expression.Constant(Convert.ToInt64(this.floatValue), typeof(long));
        }
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation

        /// <summary>
        /// Extracts an integer.
        /// </summary>
        /// <returns>An integer value.</returns>
        /// <exception cref="InvalidCastException">The current value is floating-point and cannot be transformed.</exception>
        public long ExtractInteger()
        {
            if (!this.isFloat)
            {
                return this.integerValue;
            }

            if (global::System.Math.Floor(this.floatValue) != this.floatValue)
            {
                throw new InvalidCastException();
            }

            return Convert.ToInt64(this.floatValue);
        }

        /// <summary>
        /// Extracts a floating-point value.
        /// </summary>
        /// <returns>A floating-point value.</returns>
        public double ExtractFloat()
        {
            if (this.isFloat)
            {
                return this.floatValue;
            }

            return this.integerValue;
        }

        /// <summary>
        /// Extracts a 32-bit integer value.
        /// </summary>
        /// <returns>A 32-bit integer value.</returns>
        /// <exception cref="InvalidCastException">The value is either floating-point or larger than 32-bit.</exception>
        public int ExtractInt()
        {
            if (!this.isFloat)
            {
                return Convert.ToInt32(this.integerValue);
            }

            if (global::System.Math.Floor(this.floatValue) != this.floatValue)
            {
                throw new InvalidCastException();
            }

            return Convert.ToInt32(this.floatValue);
        }

        /// <summary>
        /// Distills the value into a usable constant.
        /// </summary>
        /// <returns>A usable constant.</returns>
        public override object DistillValue() => this.Value;

        /// <summary>
        /// Generates the expression that will be compiled into code as a string expression.
        /// </summary>
        /// <returns>The string expression.</returns>
        public override Expression GenerateCachedStringExpression() => Expression.Constant(this.Value.ToString(), typeof(string));

        /// <summary>
        /// Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) => new NumericNode
        {
            integerValue = this.integerValue,
            floatValue = this.floatValue,
            isFloat = this.isFloat,
        };

        /// <summary>
        /// Initializes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        private void Initialize(long value)
        {
            this.integerValue = value;
            this.isFloat = false;
        }

        /// <summary>
        /// Initializes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        private void Initialize(double value)
        {
            if (global::System.Math.Floor(value) == value)
            {
                try
                {
                    this.integerValue = Convert.ToInt64(value);
                    this.isFloat = false;
                }
                catch (OverflowException)
                {
                    this.floatValue = value;
                    this.isFloat = true;
                }
            }
            else
            {
                this.floatValue = value;
                this.isFloat = true;
            }
        }
    }
}