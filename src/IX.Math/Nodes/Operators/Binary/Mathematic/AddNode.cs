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
#region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="AddNode" /> class.
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

#endregion

#region Methods

#region Static methods

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

#endregion

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
            if (left.TryGetInteger(out var liv) && right.TryGetInteger(out var riv))
            {
                if (left is NumericNode && right is NumericNode)
                {
                    // We have an addition of two numeric nodes that happen to be integer in value
                    return new NumericNode(Convert.ToDouble(liv + riv));
                }

                // Otherwise, we have integer values
                return new IntegerNode(liv + riv);
            }

            // We have two numeric-convertible constants
            if (left.TryGetNumeric(out var lnv) && right.TryGetNumeric(out var rnv))
            {
                return new NumericNode(lnv + rnv);
            }

            // We have two binary-convertible constants
            if (left.TryGetBinary(out byte[] lbav) && right.TryGetBinary(out byte[] rbav))
            {
                return new BinaryNode(
                    Stitch(
                        lbav,
                        rbav));
            }

            // We have string-able constants
            if (left.TryGetString(out string lsv) && right.TryGetString(out string rsv))
            {
                return new StringNode(lsv + rsv);
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
            ref NodeBase left,
            ref NodeBase right)
        {
            const SupportableValueType completeSupport = SupportableValueType.Integer |
                                                         SupportableValueType.Numeric |
                                                         SupportableValueType.Binary |
                                                         SupportableValueType.String;

            this.CalculatedCosts.Clear();
            this.PossibleReturnType = SupportableValueType.None;

            SupportableValueType commonType =
                right.VerifyPossibleType(right.VerifyPossibleType(left.VerifyPossibleType(completeSupport)));

            // Cost calculation
            int intCost = int.MaxValue,
                numericCost = int.MaxValue,
                binaryCost = int.MaxValue,
                stringCost = int.MaxValue;

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

            if ((commonType & SupportableValueType.Binary) != SupportableValueType.None)
            {
                checked
                {
                    binaryCost = left.CalculateStrategyCost(SupportedValueType.Binary) +
                                 right.CalculateStrategyCost(SupportedValueType.Binary);
                }

                this.PossibleReturnType |= GetSupportableConversions(SupportedValueType.Binary);
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
            foreach (SupportedValueType supportedType in GetSupportedTypeOptions(this.PossibleReturnType))
            {
                var totalIntCost = GetTotalConversionCosts(
                    in intCost,
                    SupportedValueType.Integer,
                    supportedType);
                var totalNumericCost = GetTotalConversionCosts(
                    in numericCost,
                    SupportedValueType.Numeric,
                    supportedType);
                var totalBinaryCost = GetTotalConversionCosts(
                    in binaryCost,
                    SupportedValueType.Binary,
                    supportedType);
                var totalStringCost = GetTotalConversionCosts(
                    in stringCost,
                    SupportedValueType.String,
                    supportedType);

                var minimalCost = global::System.Math.Min(
                    global::System.Math.Min(
                        totalIntCost,
                        totalNumericCost),
                    global::System.Math.Min(
                        totalBinaryCost,
                        totalStringCost));

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
                    this.CalculatedCosts[supportedType] = (minimalCost, SupportedValueType.Binary);
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
        ///     Generates the expression that this node represents.
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
                (var left, var right, SupportedValueType internalType) = GenerateParameterExpressions(
                    in valueType,
                    in comparisonTolerance);

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
                    case SupportedValueType.Binary:
                        MethodInfo mi2 = typeof(AddNode).GetMethodWithExactParameters(
                                             nameof(Stitch),
                                             typeof(byte[]),
                                             typeof(byte[])) ??
                                         throw new MathematicsEngineException();

                        return Expression.Call(
                            mi2,
                            left,
                            right);
                    case SupportedValueType.String:
                        MethodInfo mi1 = typeof(string).GetMethodWithExactParameters(
                                             nameof(string.Concat),
                                             typeof(string),
                                             typeof(string)) ??
                                         throw new MathematicsEngineException();

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
                in ComparisonTolerance innerComparisonTolerance)
            {
                Expression left, right;
                if (this.CalculatedCosts.TryGetValue(
                    returnType,
                    out (int Cost, SupportedValueType InternalType) tuple))
                {
                    left = this.Left.GenerateExpression(
                        in tuple.InternalType,
                        in innerComparisonTolerance);
                    right = this.Right.GenerateExpression(
                        in tuple.InternalType,
                        in innerComparisonTolerance);
                }
                else
                {
                    throw new ExpressionNotValidLogicallyException();
                }

                return (left, right, tuple.InternalType);
            }
        }

#endregion
    }
}