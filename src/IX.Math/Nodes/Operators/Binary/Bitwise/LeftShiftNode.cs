// <copyright file="LeftShiftNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Exceptions;
using IX.Math.Nodes.Constants;
using IX.StandardExtensions.Extensions;

namespace IX.Math.Nodes.Operators.Binary.Bitwise
{
    /// <summary>
    ///     A node representing a bitwise left shift operation.
    /// </summary>
    /// <seealso cref="ByteShiftOperatorNodeBase" />
    [DebuggerDisplay("{" + nameof(Left) + "} << {" + nameof(Right) + "}")]
    internal sealed class LeftShiftNode : ByteShiftOperatorNodeBase
    {
#region Internal state

        private static readonly MethodInfo ShiftMethod = typeof(BitwiseExtensions).GetMethodWithExactParameters(
                                                             nameof(BitwiseExtensions.LeftShift),
                                                             typeof(byte[]),
                                                             typeof(int)) ??
                                                         throw new MathematicsEngineException();

#endregion

#region Constructors

        public LeftShiftNode(
            NodeBase left,
            NodeBase right)
            : base(
                left,
                right)
        {
        }

#endregion

#region Methods

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
                    return new IntegerNode(lvi << intValue);
                }

                if (nLeft.TryGetBinary(out var lvb))
                {
                    return new BinaryNode(lvb.LeftShift(intValue));
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
            new LeftShiftNode(
                this.Left.DeepClone(context),
                this.Right.DeepClone(context));

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
                Expression? leftExpression = this.Left.GenerateExpression(
                    in valueType,
                    in comparisonTolerance);

                MethodCallExpression? rightExpression = Expression.Call(
                    ConvertToIntMethodInfo,
                    this.Right.GenerateExpression(
                        SupportedValueType.Integer,
                        in comparisonTolerance));

                if (leftExpression.Type == typeof(long))
                {
                    return Expression.LeftShift(
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

#endregion
    }
}