// <copyright file="AddNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Exceptions;
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
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public AddNode(
            NodeBase left,
            NodeBase right)
            : base(
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
            // Constant check
            if (!(this.Left is ConstantNodeBase left) || !(this.Right is ConstantNodeBase right))
            {
                return this;
            }

            // We have two integer-convertible constants
            if (left.TryGetInteger(out long liv) && right.TryGetInteger(out long riv))
            {
                if (left is NumericNode && right is NumericNode)
                {
                    // We have an addition of two numeric nodes that happen to be integer in value
                    return GenerateConstantNumeric(Convert.ToDouble(liv + riv));
                }

                // Otherwise, we have integer values
                return GenerateConstantInteger(liv + riv);
            }

            // We have two numeric-convertible constants
            if (left.TryGetNumeric(out double lnv) && right.TryGetNumeric(out double rnv))
            {
                return GenerateConstantNumeric(lnv + rnv);
            }

            // We have two binary-convertible constants
            if (left.TryGetByteArray(out byte[] lbav) && right.TryGetByteArray(out byte[] rbav))
            {
                return GenerateConstantByteArray(
                    Stitch(
                        lbav,
                        rbav));
            }

            // We have string-able constants
            if (left.TryGetString(out string lsv) && right.TryGetString(out string rsv))
            {
                return GenerateConstantString(lsv + rsv);
            }

            // Anything else
            return this;
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new AddNode(
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
                int totalIntCost = GetTotalConversionCosts(in intCost, SupportedValueType.Integer, supportedType);
                int totalNumericCost = GetTotalConversionCosts(in numericCost, SupportedValueType.Numeric, supportedType);
                int totalBinaryCost = GetTotalConversionCosts(in binaryCost, SupportedValueType.ByteArray, supportedType);
                int totalStringCost = GetTotalConversionCosts(in stringCost, SupportedValueType.String, supportedType);

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