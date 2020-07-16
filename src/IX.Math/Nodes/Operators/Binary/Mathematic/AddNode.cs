// <copyright file="AddNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Exceptions;
using IX.Math.Extensibility;
using IX.Math.Formatters;
using IX.Math.Nodes.Constants;
using IX.StandardExtensions.Extensions;

namespace IX.Math.Nodes.Operators.Binary.Mathematic
{
    /// <summary>
    ///     A node representing an addition operation.
    /// </summary>
    /// <seealso cref="BinaryOperatorNodeBase" />
    [DebuggerDisplay("{" + nameof(Left) + "} + {" + nameof(Right) + "}")]
    internal sealed class AddNode : BinaryOperatorNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AddNode" /> class.
        /// </summary>
        /// <param name="stringFormatters">The string formatters.</param>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public AddNode(
            List<IStringFormatter> stringFormatters,
            NodeBase left,
            NodeBase right)
            : base(
                stringFormatters,
                left,
                right)
        {
        }

        /// <summary>
        ///     Stitches the specified byte arrays.
        /// </summary>
        /// <param name="left">The left array operand.</param>
        /// <param name="right">The right array operand.</param>
        /// <returns>A stitched array of bytes.</returns>
        private static byte[] Stitch(
            byte[] left,
            byte[] right)
        {
            var r = new byte[left.Length + right.Length];
            Array.Copy(
                left,
                r,
                left.Length);
            Array.Copy(
                right,
                0,
                r,
                left.Length,
                right.Length);
            return r;
        }

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>
        ///     A simplified node, or this instance.
        /// </returns>
        public override NodeBase Simplify()
        {
            if (this.Left.IsConstant != this.Right.IsConstant)
            {
                return this;
            }

            if (!this.Left.IsConstant)
            {
                return this;
            }

            switch (this.Left)
            {
                // Numeric simplification
                case NumericNode cLeft when this.Right is NumericNode cRight:
                    return this.GenerateConstantNumeric(cLeft.Value + cRight.Value);
                case NumericNode cLeft when this.Right is IntegerNode cRight:
                    return this.GenerateConstantNumeric(cLeft.Value + cRight.Value);
                case IntegerNode cLeft when this.Right is NumericNode cRight:
                    return this.GenerateConstantNumeric(cLeft.Value + cRight.Value);
                case IntegerNode cLeft when this.Right is IntegerNode cRight:
                    return this.GenerateConstantInteger(cLeft.Value + cRight.Value);

                // Bite array simplification
                case ByteArrayNode cLeft when this.Right is ByteArrayNode cRight:
                    return this.GenerateConstantByteArray(
                        Stitch(
                            cLeft.Value,
                            cRight.Value));

                // String simplification
                case StringNode cLeft when this.Right is StringNode cRight:
                    return this.GenerateConstantString(cLeft.Value + cRight.Value);
                case NumericNode cLeft when this.Right is StringNode cRight:
                    return this.GenerateConstantString(
                        StringFormatter.FormatIntoString(
                            cLeft.Value,
                            this.StringFormatters) +
                        cRight.Value);
                case StringNode cLeft when this.Right is NumericNode cRight:
                    return this.GenerateConstantString(
                        cLeft.Value +
                        StringFormatter.FormatIntoString(
                            cRight.Value,
                            this.StringFormatters));
                case IntegerNode cLeft when this.Right is StringNode cRight:
                    return this.GenerateConstantString(
                        StringFormatter.FormatIntoString(
                            cLeft.Value,
                            this.StringFormatters) +
                        cRight.Value);
                case StringNode cLeft when this.Right is IntegerNode cRight:
                    return this.GenerateConstantString(
                        cLeft.Value +
                        StringFormatter.FormatIntoString(
                            cRight.Value,
                            this.StringFormatters));
                case BoolNode cLeft when this.Right is StringNode cRight:
                    return this.GenerateConstantString(
                        StringFormatter.FormatIntoString(
                            cLeft.Value,
                            this.StringFormatters) +
                        cRight.Value);
                case StringNode cLeft when this.Right is BoolNode cRight:
                    return this.GenerateConstantString(
                        cLeft.Value +
                        StringFormatter.FormatIntoString(
                            cRight.Value,
                            this.StringFormatters));
                case ByteArrayNode cLeft when this.Right is StringNode cRight:
                    return this.GenerateConstantString(
                        StringFormatter.FormatIntoString(
                            cLeft.Value,
                            this.StringFormatters) +
                        cRight.Value);
                case StringNode cLeft when this.Right is ByteArrayNode cRight:
                    return this.GenerateConstantString(
                        cLeft.Value +
                        StringFormatter.FormatIntoString(
                            cRight.Value,
                            this.StringFormatters));

                // Anything else
                default:
                    return this;
            }
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new AddNode(
                this.StringFormatters,
                this.Left.DeepClone(context),
                this.Right.DeepClone(context));

        /// <summary>
        ///     Ensures that the operands are compatible.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        protected override void EnsureCompatibleOperandsAndRefineReturnType(
            NodeBase left,
            NodeBase right)
        {
            const SupportableValueType completeSupport =
                SupportableValueType.Integer |
                SupportableValueType.Numeric |
                SupportableValueType.ByteArray |
                SupportableValueType.String;

            this.CalculatedCosts.Clear();
            this.PossibleReturnType = SupportableValueType.None;

            var commonType = right.VerifyPossibleType(right.VerifyPossibleType(left.VerifyPossibleType(completeSupport)));

            // Cost calculation
            int intCost = int.MaxValue, numericCost = int.MaxValue, binaryCost = int.MaxValue, stringCost = int.MaxValue;

            if ((commonType & SupportableValueType.Integer) != SupportableValueType.None)
            {
                checked
                {
                    intCost = left.CalculateStrategyCost(SupportedValueType.Integer) +
                            right.CalculateStrategyCost(SupportedValueType.Integer);
                }

                this.PossibleReturnType |= GetSupportableConversions(SupportedValueType.Integer);
            }

            if ((commonType & SupportableValueType.Numeric) != SupportableValueType.None)
            {
                checked
                {
                    numericCost = left.CalculateStrategyCost(SupportedValueType.Numeric) +
                            right.CalculateStrategyCost(SupportedValueType.Numeric);
                }

                this.PossibleReturnType |= GetSupportableConversions(SupportedValueType.Numeric);
            }

            if ((commonType & SupportableValueType.ByteArray) != SupportableValueType.None)
            {
                checked
                {
                    binaryCost = left.CalculateStrategyCost(SupportedValueType.ByteArray) +
                            right.CalculateStrategyCost(SupportedValueType.ByteArray);
                }

                this.PossibleReturnType |= GetSupportableConversions(SupportedValueType.ByteArray);
            }

            if ((commonType & SupportableValueType.String) != SupportableValueType.None)
            {
                checked
                {
                    stringCost = left.CalculateStrategyCost(SupportedValueType.String) +
                            right.CalculateStrategyCost(SupportedValueType.String);
                }

                this.PossibleReturnType |= GetSupportableConversions(SupportedValueType.String);
            }

            // Let's populate the cost tables
            foreach (var supportedType in GetSupportedTypeOptions(this.PossibleReturnType))
            {
                int totalIntCost = this.GetTotalConversionCosts(in intCost, SupportedValueType.Integer, supportedType);
                int totalNumericCost = this.GetTotalConversionCosts(in numericCost, SupportedValueType.Numeric, supportedType);
                int totalBinaryCost = this.GetTotalConversionCosts(in binaryCost, SupportedValueType.ByteArray, supportedType);
                int totalStringCost = this.GetTotalConversionCosts(in stringCost, SupportedValueType.String, supportedType);

                int minimalCost = global::System.Math.Min(
                    global::System.Math.Min(totalIntCost, totalNumericCost),
                    global::System.Math.Min(totalBinaryCost, totalStringCost));

                if (minimalCost == totalIntCost)
                {
                    this.CalculatedCosts[supportedType] = (minimalCost, SupportedValueType.Integer);
                }
                else if (minimalCost == totalNumericCost)
                {
                    this.CalculatedCosts[supportedType] = (minimalCost, SupportedValueType.Numeric);
                }
                else if (minimalCost == totalBinaryCost)
                {
                    this.CalculatedCosts[supportedType] = (minimalCost, SupportedValueType.ByteArray);
                }
                else if (minimalCost == totalStringCost)
                {
                    this.CalculatedCosts[supportedType] = (minimalCost, SupportedValueType.String);
                }
                else
                {
                    throw new ExpressionNotValidLogicallyException();
                }
            }
        }

        /// <summary>
        /// Generates the expression that this node represents.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="comparisonTolerance">The comparison tolerance.</param>
        /// <returns>A compiled expression, if one is possible.</returns>
        protected override Expression GenerateExpressionInternal(
            in SupportedValueType valueType,
            in ComparisonTolerance comparisonTolerance)
        {
            try
            {
                var (
                    left,
                    right,
                    internalType) = GenerateParameterExpressions(in valueType, in comparisonTolerance);

                switch (internalType)
                {
                    case SupportedValueType.Integer:
                        return Expression.Add(
                            left,
                            right);
                    case SupportedValueType.Numeric:
                        return Expression.Add(
                            left,
                            right);
                    case SupportedValueType.ByteArray:
                        MethodInfo mi2 = typeof(AddNode).GetMethodWithExactParameters(
                            nameof(Stitch),
                            typeof(byte[]),
                            typeof(byte[])) ?? throw new MathematicsEngineException();
                        return Expression.Call(
                            mi2,
                            left,
                            right);
                    case SupportedValueType.String:
                        MethodInfo mi1 = typeof(string).GetMethodWithExactParameters(
                            nameof(string.Concat),
                            typeof(string),
                            typeof(string)) ?? throw new MathematicsEngineException();
                        return Expression.Call(
                            mi1,
                            left,
                            right);
                    default:
                        throw new ExpressionNotValidLogicallyException();
                }
            }
            catch (ExpressionNotValidLogicallyException)
            {
                throw;
            }
            catch (MathematicsEngineException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ExpressionNotValidLogicallyException(ex);
            }

            (Expression Left, Expression Right, SupportedValueType InternalType) GenerateParameterExpressions(
                in SupportedValueType returnType,
                in ComparisonTolerance comparisonTolerance)
            {
                Expression left, right;
                if (this.CalculatedCosts.TryGetValue(
                    returnType,
                    out var tuple))
                {
                    left = this.Left.GenerateExpression(
                        in tuple.InternalType,
                        in comparisonTolerance);
                    right = this.Right.GenerateExpression(
                        in tuple.InternalType,
                        in comparisonTolerance);
                }
                else
                {
                    throw new ExpressionNotValidLogicallyException();
                }

                return (left, right, tuple.InternalType);
            }
        }
    }
}