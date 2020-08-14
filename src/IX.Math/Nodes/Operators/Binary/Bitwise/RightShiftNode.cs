// <copyright file="RightShiftNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Exceptions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using IX.StandardExtensions.Extensions;

namespace IX.Math.Nodes.Operators.Binary.Bitwise
{
    /// <summary>
    ///     A node representing a bitwise right shift operation.
    /// </summary>
    /// <seealso cref="ByteShiftOperatorNodeBase" />
    [DebuggerDisplay("{" + nameof(Left) + "} >> {" + nameof(Right) + "}")]
    internal sealed class RightShiftNode : ByteShiftOperatorNodeBase
    {
        private static readonly MethodInfo ShiftMethod = typeof(BitwiseExtensions).GetMethodWithExactParameters(
                                                             nameof(BitwiseExtensions.RightShift),
                                                             typeof(byte[]),
                                                             typeof(int)) ??
                                                         throw new MathematicsEngineException();

        public RightShiftNode(
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
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>
        ///     A simplified node, or this instance.
        /// </returns>
        public override NodeBase Simplify()
        {
            try
            {
                if (!(this.Right is ConstantNodeBase nRight) || !nRight.TryGetInteger(out var rv))
                {
                    return this;
                }

                if (!(this.Left is ConstantNodeBase nLeft))
                {
                    return this;
                }

                var intValue = Convert.ToInt32(rv);

                if (nLeft.TryGetInteger(out var lvi))
                {
                    return this.GenerateConstantInteger(lvi >> intValue);
                }

                if (nLeft.TryGetByteArray(out var lvb))
                {
                    return this.GenerateConstantByteArray(lvb.RightShift(intValue));
                }

                return this;
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
        }

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new RightShiftNode(
                this.StringFormatters,
                this.Left.DeepClone(context),
                this.Right.DeepClone(context));

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
                var leftExpression = this.Left.GenerateExpression(
                    in valueType,
                    in comparisonTolerance);

                var rightExpression = Expression.Call(ConvertToIntMethodInfo, this.Right.GenerateExpression(
                    SupportedValueType.Integer,
                    in comparisonTolerance));

                if (leftExpression.Type == typeof(long))
                {
                    return Expression.RightShift(
                        leftExpression,
                        rightExpression);
                }

                if (leftExpression.Type == typeof(byte[]))
                {
                    return Expression.Call(
                        ShiftMethod,
                        leftExpression,
                        rightExpression);
                }

                throw new ExpressionNotValidLogicallyException();
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
        }
    }
}