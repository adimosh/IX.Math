// <copyright file="FunctionNodeLength.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;
using System.Linq.Expressions;
using IX.Math.Exceptions;
using IX.Math.Extensibility;
using IX.Math.Nodes.Constants;
using JetBrains.Annotations;

namespace IX.Math.Nodes.Functions.Unary
{
    /// <summary>
    ///     A node representing the string length function.
    /// </summary>
    /// <seealso cref="UnaryFunctionNodeBase" />
    [DebuggerDisplay("length({" + nameof(Parameter) + "})")]
    [CallableMathematicsFunction("length")]
    [UsedImplicitly]
    internal sealed class FunctionNodeLength : UnaryFunctionNodeBase
    {
#region Constructors

        /// <summary>
        ///     Initializes a new instance of the <see cref="FunctionNodeLength" /> class.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        public FunctionNodeLength(NodeBase parameter)
            : base(parameter)
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
        public override NodeBase Simplify() =>
            this.Parameter switch
            {
                ConstantNodeBase cn when cn.TryGetBinary(out var ba) => new IntegerNode(ba.LongLength),
                ConstantNodeBase cn when cn.TryGetString(out var s) => new IntegerNode(Convert.ToInt64(s.Length)),
                _ => this
            };

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>
        ///     A deep clone.
        /// </returns>
        public override NodeBase DeepClone(NodeCloningContext context) =>
            new FunctionNodeLength(this.Parameter.DeepClone(context));

        /// <summary>
        ///     Ensures that the parameter that is received is compatible with the function, optionally allowing the parameter
        ///     reference to change.
        /// </summary>
        /// <param name="parameter">The parameter.</param>
        protected override void EnsureCompatibleParameter(NodeBase parameter)
        {
            _ = parameter.VerifyPossibleType(SupportableValueType.Binary | SupportableValueType.String);

            this.PossibleReturnType = GetSupportableConversions(SupportedValueType.Integer);
            var cost = parameter.CalculateStrategyCost(SupportedValueType.Integer);

            foreach (SupportedValueType possibleType in GetSupportedTypeOptions(this.PossibleReturnType))
            {
                this.CalculatedCosts[possibleType] = (GetStandardConversionStrategyCost(
                                                          SupportedValueType.Integer,
                                                          in possibleType) +
                                                      cost, possibleType);
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
            if (this.Parameter.CheckSupportedType(SupportableValueType.Binary))
            {
                return Expression.Property(
                    this.Parameter.GenerateExpression(
                        SupportedValueType.Binary,
                        in comparisonTolerance),
                    typeof(byte[]),
                    nameof(Array.LongLength));
            }

            if (this.Parameter.CheckSupportedType(SupportableValueType.String))
            {
                return Expression.Convert(
                    Expression.Property(
                        this.Parameter.GenerateExpression(
                            SupportedValueType.String,
                            in comparisonTolerance),
                        typeof(string),
                        nameof(string.Length)),
                    typeof(long));
            }

            throw new ExpressionNotValidLogicallyException();
        }

#endregion
    }
}