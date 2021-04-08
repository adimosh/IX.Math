// <copyright file="NumericNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Extensibility;
using IX.Math.Formatters;
using IX.Math.TypeHelpers;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Constants
{
    /// <summary>
    /// A numeric node. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="ConstantNodeBase" />
    [DebuggerDisplay("{" + nameof(Value) + "}")]
    [PublicAPI]
    public sealed class NumericNode : ConstantNodeBase, ISpecialRequestNode
    {
        private Func<Type, object> specialObjectRequestFunction;

        /// <summary>
        /// The integer value.
        /// </summary>
        private long integerValue;

        /// <summary>
        /// The float value.
        /// </summary>
        private double floatValue;

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

        private NumericNode()
        {
        }

        /// <summary>
        /// Gets the return type of this node.
        /// </summary>
        /// <value>Always <see cref="SupportedValueType.Numeric"/>.</value>
        public override SupportedValueType ReturnType => SupportedValueType.Numeric;

        /// <summary>
        /// Gets a value indicating whether this instance is a floating point number.
        /// </summary>
        /// <value>
        ///   <c>true</c> if this instance is float; otherwise, <c>false</c>.
        /// </value>
        public bool IsFloat { get; private set; }

        /// <summary>
        /// Gets the value of this constant numeric node.
        /// </summary>
        /// <value>The value.</value>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "This is desired.")]
        public object Value
        {

            get
            {
                if (this.IsFloat)
                {
                    return this.floatValue;
                }

                return this.integerValue;
            }
        }

        /// <summary>
        /// Does an addition between two numeric nodes.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The resulting node.</returns>
        public static NumericNode Add(NumericNode left, NumericNode right)
        {
            if (left.IsFloat && right.IsFloat)
            {
                return new NumericNode(left.floatValue + right.floatValue);
            }

            if (left.IsFloat && !right.IsFloat)
            {
                return new NumericNode(left.floatValue + Convert.ToDouble(right.integerValue));
            }

            if (!left.IsFloat && right.IsFloat)
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
        public static NumericNode Subtract(NumericNode left, NumericNode right)
        {
            if (left.IsFloat && right.IsFloat)
            {
                return new NumericNode(left.floatValue - right.floatValue);
            }

            if (left.IsFloat && !right.IsFloat)
            {
                return new NumericNode(left.floatValue - Convert.ToDouble(right.integerValue));
            }

            if (!left.IsFloat && right.IsFloat)
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
        public static NumericNode Multiply(NumericNode left, NumericNode right)
        {
            if (left.IsFloat && right.IsFloat)
            {
                return new NumericNode(left.floatValue * right.floatValue);
            }

            if (left.IsFloat && !right.IsFloat)
            {
                return new NumericNode(left.floatValue * Convert.ToDouble(right.integerValue));
            }

            if (!left.IsFloat && right.IsFloat)
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
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "This is of little consequence at this point.")]
        public static NumericNode Divide(
             NumericNode left,
             NumericNode right)
        {
            var (divided, divisor, tryInteger) = NumericTypeHelper.ExtractFloats(
                left.Value,
                right.Value);

            double result = divided / divisor;

            return new NumericNode(tryInteger ? NumericTypeHelper.DistillIntegerIfPossible(result) : result);
        }

        /// <summary>
        /// Raises the left node's value to the power specified by the right node's value.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The resulting node.</returns>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "This is of little consequence at this point.")]
        public static NumericNode Power(
             NumericNode left,
             NumericNode right)
        {
            var (@base, pow, tryInteger) = NumericTypeHelper.ExtractFloats(
                left.Value,
                right.Value);

            double result = global::System.Math.Pow(
                @base,
                pow);

            return new NumericNode(tryInteger ? NumericTypeHelper.DistillIntegerIfPossible(result) : result);
        }

        /// <summary>
        /// Does a left shift between two numeric nodes.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The resulting node.</returns>
        public static NumericNode LeftShift(NumericNode left, NumericNode right)
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
        public static NumericNode RightShift(NumericNode left, NumericNode right)
        {
            var by = right.ExtractInt();
            var data = left.ExtractInteger();

            return new NumericNode(data >> by);
        }

        /// <summary>
        /// Generates the expression that will be compiled into code.
        /// </summary>
        /// <returns>The expression.</returns>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "This is desired.")]
        public override Expression GenerateCachedExpression() => this.IsFloat ?
            Expression.Constant(this.floatValue, typeof(double)) :
            Expression.Constant(this.integerValue, typeof(long));

        /// <summary>
        /// Generates a floating-point expression.
        /// </summary>
        /// <returns>The expression.</returns>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "This is desired.")]

        public Expression GenerateFloatExpression() => this.IsFloat ? this.GenerateExpression() : Expression.Constant(Convert.ToDouble(this.floatValue), typeof(double));

        /// <summary>
        /// Generates an integer expression.
        /// </summary>
        /// <returns>The expression.</returns>
        /// <exception cref="InvalidCastException">The node is floating-point and cannot be transformed.</exception>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "This is desired.")]
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "ReSharper",
            "CompareOfFloatsByEqualityOperator",
            Justification = "If we're reaching the precision loss boundary, it means we're rounding to an integer anyway, so it's acceptable.")]

        public Expression GenerateLongExpression()
        {
            if (!this.IsFloat)
            {
                return this.GenerateExpression();
            }

            if (global::System.Math.Floor(this.floatValue) != this.floatValue)
            {
                throw new InvalidCastException();
            }

            return Expression.Constant(Convert.ToInt64(this.floatValue), typeof(long));
        }

        /// <summary>
        /// Extracts an integer.
        /// </summary>
        /// <returns>An integer value.</returns>
        /// <exception cref="InvalidCastException">The current value is floating-point and cannot be transformed.</exception>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "ReSharper",
            "CompareOfFloatsByEqualityOperator",
            Justification = "If we're reaching the precision loss boundary, it means we're rounding to an integer anyway, so it's acceptable.")]
        public long ExtractInteger()
        {
            if (!this.IsFloat)
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
            if (this.IsFloat)
            {
                return this.floatValue;
            }

            return Convert.ToDouble(this.integerValue);
        }

        /// <summary>
        /// Extracts a 32-bit integer value.
        /// </summary>
        /// <returns>A 32-bit integer value.</returns>
        /// <exception cref="InvalidCastException">The value is either floating-point or larger than 32-bit.</exception>
        [global::System.Diagnostics.CodeAnalysis.SuppressMessage(
            "ReSharper",
            "CompareOfFloatsByEqualityOperator",
            Justification = "If we're reaching the precision loss boundary, it means we're rounding to an integer anyway, so it's acceptable.")]
        public int ExtractInt()
        {
            if (!this.IsFloat)
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
        public override Expression GenerateCachedStringExpression()
        {
            var stringFormatters = this.specialObjectRequestFunction?.Invoke(typeof(IStringFormatter)) as List<IStringFormatter>;
            return Expression.Constant(
                StringFormatter.FormatIntoString(this.Value, stringFormatters),
                typeof(string));
        }

        /// <summary>
        /// Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) => new NumericNode
        {
            integerValue = this.integerValue,
            floatValue = this.floatValue,
            IsFloat = this.IsFloat,
        };

        /// <summary>
        /// Sets the request special object function.
        /// </summary>
        /// <param name="func">The function to set.</param>
        void ISpecialRequestNode.SetRequestSpecialObjectFunction(Func<Type, object> func) => this.specialObjectRequestFunction = func;

        /// <summary>
        /// Initializes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        private void Initialize(long value)
        {
            this.integerValue = value;
            this.IsFloat = false;
        }

        /// <summary>
        /// Initializes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        private void Initialize(double value)
        {
            this.floatValue = value;
            this.IsFloat = true;
        }
    }
}