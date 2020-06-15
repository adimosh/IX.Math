// <copyright file="XorNode.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Exceptions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;

namespace IX.Math.Nodes.Operators.Binary.Logical
{
    /// <summary>
    ///     A node representing a logical exclusive or operation.
    /// </summary>
    /// <seealso cref="LogicalOperatorNodeBase" />
    [DebuggerDisplay("{" + nameof(Left) + "} & {" + nameof(Right) + "}")]
    internal sealed class XorNode : LogicalOperatorNodeBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="XorNode" /> class.
        /// </summary>
        /// <param name="stringFormatters">The string formatters.</param>
        /// <param name="left">The left.</param>
        /// <param name="right">The right.</param>
        public XorNode(
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
        /// Performs the binary operation.
        /// </summary>
        /// <param name="left">The left operand.</param>
        /// <param name="right">The right operand.</param>
        /// <returns>The result of the operation.</returns>
        public static byte[] PerformBinaryOperation(
            byte[] left,
            byte[] right)
        {
            byte[] result = new byte[global::System.Math.Max(
                left.Length,
                right.Length)];
            new BitArray(left).Xor(new BitArray(right)).CopyTo(result, 0);
            return result;
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
                return this.Left switch
                {
                    IntegerNode nnLeft when this.Right is IntegerNode nnRight => this.GenerateConstantInteger(
                        nnLeft.Value ^ nnRight.Value),
                    BoolNode bnLeft when this.Right is BoolNode bnRight => this.GenerateConstantBoolean(
                        bnLeft.Value ^ bnRight.Value),
                    _ => this
                };
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
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new XorNode(
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
                var (
                    left,
                    right,
                    isBinary) = this.GenerateParameterExpressions(in valueType, in comparisonTolerance);

                if (isBinary)
                {
                    Func<byte[], byte[], byte[]> del = PerformBinaryOperation;
                    return Expression.Call(
                        del.Method,
                        left,
                        right);
                }

                return Expression.ExclusiveOr(
                    left,
                    right);
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