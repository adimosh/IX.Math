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

            var typeLeft = left.VerifyPossibleType(completeSupport);
            var typeRight = right.VerifyPossibleType(completeSupport);

            // We might have addition with limited types
            if (typeLeft == SupportableValueType.String || typeRight == SupportableValueType.String)
            {
                // If any of the operands are string-limited, the result can only be a string
                this.PossibleReturnType = GetSupportableConversions(SupportedValueType.String);
                foreach (var supportedType in GetSupportedTypeOptions(this.PossibleReturnType))
                {
                    this.CalculatedCosts[supportedType] = (GetStandardConversionStrategyCost(
                                                               SupportedValueType.String,
                                                               in supportedType) +
                                                           left.CalculateStrategyCost(SupportedValueType.String) +
                                                           right.CalculateStrategyCost(SupportedValueType.String),
                        SupportedValueType.String);
                }

                return;
            }

            // If the return type of any can be string, the return type of the additionh can be string-compatible
            SupportableValueType possibleStringType;
            int stringCosts;
            if (left.CheckSupportedType(SupportableValueType.String) ||
                 right.CheckSupportedType(SupportableValueType.String))
            {
                possibleStringType = GetSupportableConversions(SupportedValueType.String);
                stringCosts = left.CalculateStrategyCost(SupportedValueType.String) +
                              right.CalculateStrategyCost(SupportedValueType.String);
            }
            else
            {
                possibleStringType = SupportableValueType.None;
                stringCosts = int.MaxValue;
            }

            if ((typeLeft == SupportableValueType.ByteArray && right.CheckSupportedType(SupportableValueType.ByteArray)) ||
                (left.CheckSupportedType(SupportableValueType.ByteArray) && typeRight == SupportableValueType.ByteArray))
            {
                // If the operands are byte-array limited and compatible, and none of them is string,
                // the return type can only be a byte array
                this.PossibleReturnType = GetSupportableConversions(SupportedValueType.ByteArray) | possibleStringType;

                foreach (var supportedType in GetSupportedTypeOptions(this.PossibleReturnType))
                {
                    int totalStringCost = this.GetTotalConversionCosts(
                        in stringCosts,
                        SupportedValueType.String,
                        in supportedType);
                    int totalOtherCost = this.GetTotalConversionCosts(
                        left.CalculateStrategyCost(SupportedValueType.ByteArray) +
                            right.CalculateStrategyCost(SupportedValueType.ByteArray),
                        SupportedValueType.ByteArray,
                        in supportedType);

                    if (totalStringCost < totalOtherCost)
                    {
                        this.CalculatedCosts[supportedType] = (totalStringCost, SupportedValueType.String);
                    }
                    else
                    {
                        this.CalculatedCosts[supportedType] = (totalOtherCost, SupportedValueType.ByteArray);
                    }
                }

                return;
            }

            if ((typeLeft == SupportableValueType.Numeric && right.CheckSupportedType(SupportableValueType.Numeric)) ||
                (left.CheckSupportedType(SupportableValueType.Numeric) && typeRight == SupportableValueType.Numeric))
            {
                // If the operands are numeric limited and compatible, the return type can only be a numeric
                this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Numeric) | possibleStringType;

                foreach (var supportedType in GetSupportedTypeOptions(this.PossibleReturnType))
                {
                    int totalStringCost = this.GetTotalConversionCosts(
                        in stringCosts,
                        SupportedValueType.String,
                        in supportedType);
                    int totalOtherCost = this.GetTotalConversionCosts(
                        left.CalculateStrategyCost(SupportedValueType.Numeric) +
                        right.CalculateStrategyCost(SupportedValueType.Numeric),
                        SupportedValueType.Numeric,
                        in supportedType);

                    if (totalStringCost < totalOtherCost)
                    {
                        this.CalculatedCosts[supportedType] = (totalStringCost, SupportedValueType.String);
                    }
                    else
                    {
                        this.CalculatedCosts[supportedType] = (totalOtherCost, SupportedValueType.Numeric);
                    }
                }

                return;
            }

            if ((typeLeft == SupportableValueType.Integer && right.CheckSupportedType(SupportableValueType.Integer)) ||
                (left.CheckSupportedType(SupportableValueType.Integer) && typeRight == SupportableValueType.Integer))
            {
                // If the operands are integer limited and compatible, the return type can only be an integer
                this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Integer) | possibleStringType;

                foreach (var supportedType in GetSupportedTypeOptions(this.PossibleReturnType))
                {
                    int totalStringCost = this.GetTotalConversionCosts(
                        in stringCosts,
                        SupportedValueType.String,
                        in supportedType);
                    int totalOtherCost = this.GetTotalConversionCosts(
                        left.CalculateStrategyCost(SupportedValueType.Integer) +
                        right.CalculateStrategyCost(SupportedValueType.Integer),
                        SupportedValueType.Integer,
                        in supportedType);

                    if (totalStringCost < totalOtherCost)
                    {
                        this.CalculatedCosts[supportedType] = (totalStringCost, SupportedValueType.String);
                    }
                    else
                    {
                        this.CalculatedCosts[supportedType] = (totalOtherCost, SupportedValueType.Integer);
                    }
                }

                return;
            }

            // We do not have any limited types
            var commonType = typeLeft & typeRight;

            if (commonType == SupportableValueType.None)
            {
                if (possibleStringType != SupportableValueType.None)
                {
                    this.PossibleReturnType = possibleStringType;

                    foreach (var supportedType in GetSupportedTypeOptions(this.PossibleReturnType))
                    {
                        int totalStringCost = this.GetTotalConversionCosts(
                            in stringCosts,
                            SupportedValueType.String,
                            in supportedType);
                        this.CalculatedCosts[supportedType] = (totalStringCost, SupportedValueType.String);
                    }

                    return;
                }

                throw new ExpressionNotValidLogicallyException();
            }

            // Once we have determined the common types, let's check on what we can do with them
            int baCost = int.MaxValue;
            if ((commonType & SupportableValueType.ByteArray) != SupportableValueType.None)
            {
                possibleStringType |= GetSupportableConversions(SupportedValueType.ByteArray);
                baCost = left.CalculateStrategyCost(SupportedValueType.ByteArray) +
                         right.CalculateStrategyCost(SupportedValueType.ByteArray);
            }

            int iCost = int.MaxValue;
            if ((commonType & SupportableValueType.Integer) != SupportableValueType.None)
            {
                possibleStringType |= GetSupportableConversions(SupportedValueType.Integer);
                iCost = left.CalculateStrategyCost(SupportedValueType.Integer) +
                        right.CalculateStrategyCost(SupportedValueType.Integer);
            }

            int nCost = int.MaxValue;
            if ((commonType & SupportableValueType.Numeric) != SupportableValueType.None)
            {
                possibleStringType |= GetSupportableConversions(SupportedValueType.Numeric);
                nCost = left.CalculateStrategyCost(SupportedValueType.Numeric) +
                        right.CalculateStrategyCost(SupportedValueType.Numeric);
            }

            this.PossibleReturnType = possibleStringType;

            foreach (var supportedType in GetSupportedTypeOptions(this.PossibleReturnType))
            {
                int totalByteArrayCost = this.GetTotalConversionCosts(
                    in baCost,
                    SupportedValueType.ByteArray,
                    in supportedType);
                int totalIntegerCost = this.GetTotalConversionCosts(
                    in iCost,
                    SupportedValueType.Integer,
                    in supportedType);
                int totalNumericCost = this.GetTotalConversionCosts(
                    in nCost,
                    SupportedValueType.Numeric,
                    in supportedType);
                int totalStringCost = this.GetTotalConversionCosts(
                    in stringCosts,
                    SupportedValueType.String,
                    in supportedType);

                int minCost = global::System.Math.Min(
                    global::System.Math.Min(totalByteArrayCost, totalNumericCost),
                    global::System.Math.Min(totalIntegerCost, totalStringCost));

                if (totalIntegerCost == minCost)
                {
                    this.CalculatedCosts[supportedType] = (totalIntegerCost, SupportedValueType.Integer);
                }
                else if (totalNumericCost == minCost)
                {
                    this.CalculatedCosts[supportedType] = (totalNumericCost, SupportedValueType.Numeric);
                }
                else if (totalByteArrayCost == minCost)
                {
                    this.CalculatedCosts[supportedType] = (totalStringCost, SupportedValueType.ByteArray);
                }
                else if (totalStringCost == minCost)
                {
                    this.CalculatedCosts[supportedType] = (totalByteArrayCost, SupportedValueType.String);
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