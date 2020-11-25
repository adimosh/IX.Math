// <copyright file="NodeBase.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using IX.Math.Exceptions;
using IX.Math.Formatters;
using IX.StandardExtensions;
using IX.StandardExtensions.Contracts;
using JetBrains.Annotations;

namespace IX.Math.Nodes
{
    /// <summary>
    ///     A base class for mathematics nodes.
    /// </summary>
    /// <seealso cref="IDeepCloneable{T}" />
    [PublicAPI]
    [SuppressMessage(
        "StyleCop.CSharp.OrderingRules",
        "SA1202:Elements should be ordered by access",
        Justification = "It works better to just have methods properly grouped.")]
    [SuppressMessage(
        "StyleCop.CSharp.OrderingRules",
        "SA1204:Static elements should appear before instance elements",
        Justification = "It works better to just have methods properly grouped.")]
    public abstract partial class NodeBase : IContextAwareDeepCloneable<NodeCloningContext, NodeBase>
    {
#region Internal state

#region Cached MethodInfo objects

        /// <summary>
        ///     The convert to int method information.
        /// </summary>
        [SuppressMessage(
            "StyleCop.CSharp.ReadabilityRules",
            "SA1118:Parameter should not span multiple lines",
            Justification = "This is hardly enforceable.")]
        [NotNull]
        protected static readonly MethodInfo ConvertToIntMethodInfo = typeof(Convert).GetMethod(
                                                                          nameof(Convert.ToInt32),
                                                                          new[]
                                                                          {
                                                                              typeof(long)
                                                                          }) ??
                                                                      throw new MathematicsEngineException();

#endregion

        private Expression binaryExpression;
        private Expression boolExpression;

        private Expression integerExpression;
        private Expression numericExpression;
        private Expression stringExpression;

#endregion

#region Constructors

        protected private NodeBase()
        {
            this.PossibleReturnType = SupportableValueType.All;

            this.CalculatedCosts = new Dictionary<SupportedValueType, (int, SupportedValueType)>();
        }

#endregion

#region Properties

        /// <summary>
        ///     Gets a value indicating whether or not this node is actually a constant.
        /// </summary>
        /// <value><see langword="true" /> if the node is a constant, <see langword="false" /> otherwise.</value>
        public abstract bool IsConstant
        {
            get;
        }

        /// <summary>
        ///     Gets a value indicating whether this node supports tolerance.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the node is tolerant, <see langword="false" /> otherwise.
        /// </value>
        public abstract bool IsTolerant
        {
            get;
        }

        /// <summary>
        ///     Gets a value indicating whether this node requires preservation of the original expression.
        /// </summary>
        /// <value>
        ///     <see langword="true" /> if the node requires original expression preservation, or <see langword="false" />
        ///     if it can be polynomially-reduced.
        /// </value>
        public abstract bool RequiresPreservedExpression
        {
            get;
        }

        /// <summary>
        ///     Gets or sets the possible return type of this node, if one is not certain at this point.
        /// </summary>
        /// <value>The node return type.</value>
        public SupportableValueType PossibleReturnType
        {
            get;
            protected set;
        }

        /// <summary>
        ///     Gets the calculated costs.
        /// </summary>
        /// <value>
        ///     The calculated costs.
        /// </value>
        [NotNull]
        protected Dictionary<SupportedValueType, (int Cost, SupportedValueType InternalType)> CalculatedCosts
        {
            get;
        }

#endregion

#region Phase 1 - Construction

#region Supportable type conversions

        /// <summary>
        ///     Gets the supportable conversions from an internal value type.
        /// </summary>
        /// <param name="internalValueType">Type of the internal value.</param>
        /// <param name="noneMeansAll">if set to <c>true</c>, a value of None means all are supported.</param>
        /// <returns>
        ///     The value types supported for conversion.
        /// </returns>
        [SuppressMessage(
            "Performance",
            "EPS02:A non-readonly struct used as in-parameter",
            Justification = "This is a primitive type, the compiler can handle it.")]
        public static SupportableValueType GetSupportableConversions(
            in SupportedValueType internalValueType,
            in bool noneMeansAll = false) =>
            internalValueType switch
            {
                SupportedValueType.Integer => SupportableValueType.Integer |
                                              SupportableValueType.Numeric |
                                              SupportableValueType.Binary |
                                              SupportableValueType.String,
                SupportedValueType.Numeric => SupportableValueType.Numeric |
                                              SupportableValueType.Binary |
                                              SupportableValueType.String,
                SupportedValueType.Binary => SupportableValueType.Binary | SupportableValueType.String,
                SupportedValueType.Boolean => SupportableValueType.Boolean | SupportableValueType.String,
                SupportedValueType.String => SupportableValueType.String,
                _ => noneMeansAll ? SupportableValueType.All : SupportableValueType.None
            };

        /// <summary>
        ///     Gets the supportable conversions from an internal value type.
        /// </summary>
        /// <param name="internalValueType">Type of the internal value.</param>
        /// <returns>The value types supported for conversion.</returns>
        public static SupportableValueType GetSupportableConversions(Type internalValueType)
        {
            if (internalValueType == typeof(long))
            {
                return SupportableValueType.Integer |
                       SupportableValueType.Numeric |
                       SupportableValueType.Binary |
                       SupportableValueType.String;
            }

            if (internalValueType == typeof(double))
            {
                return SupportableValueType.Numeric | SupportableValueType.Binary | SupportableValueType.String;
            }

            if (internalValueType == typeof(byte[]))
            {
                return SupportableValueType.Binary | SupportableValueType.String;
            }

            if (internalValueType == typeof(bool))
            {
                return SupportableValueType.Boolean | SupportableValueType.String;
            }

            if (internalValueType == typeof(string))
            {
                return SupportableValueType.String;
            }

            return SupportableValueType.None;
        }

#endregion

#region Supported type conversions

        /// <summary>
        ///     Gets the supported type options from a flagged list of supportable types.
        /// </summary>
        /// <param name="supportableTypes">The supportable types.</param>
        /// <returns>The supported types, as a list.</returns>
        [NotNull]
        public static List<SupportedValueType> GetSupportedTypeOptions(in SupportableValueType supportableTypes)
        {
            List<SupportedValueType> types = new List<SupportedValueType>(5);

            if ((supportableTypes & SupportableValueType.Integer) != SupportableValueType.None)
            {
                types.Add(SupportedValueType.Integer);
            }

            if ((supportableTypes & SupportableValueType.Numeric) != SupportableValueType.None)
            {
                types.Add(SupportedValueType.Numeric);
            }

            if ((supportableTypes & SupportableValueType.Binary) != SupportableValueType.None)
            {
                types.Add(SupportedValueType.Binary);
            }

            if ((supportableTypes & SupportableValueType.Boolean) != SupportableValueType.None)
            {
                types.Add(SupportedValueType.Boolean);
            }

            if ((supportableTypes & SupportableValueType.String) != SupportableValueType.None)
            {
                types.Add(SupportedValueType.String);
            }

            return types;
        }

#endregion

#region Strategy costs

        /// <summary>
        ///     Gets the standard strategy cost for converting between two supported types.
        /// </summary>
        /// <param name="internalValueType">Type of the internal value.</param>
        /// <param name="toType">The type to convert to.</param>
        /// <returns>
        ///     The standard strategy cost.
        /// </returns>
        public static int GetStandardConversionStrategyCost(
            in SupportedValueType internalValueType,
            in SupportedValueType toType) =>
            internalValueType switch
            {
                SupportedValueType.Integer => toType switch
                {
                    SupportedValueType.Integer => 0,
                    SupportedValueType.Numeric => 1,
                    SupportedValueType.Binary => 2,
                    SupportedValueType.String => 10,
                    _ => int.MaxValue
                },
                SupportedValueType.Numeric => toType switch
                {
                    SupportedValueType.Numeric => 0,
                    SupportedValueType.Binary => 1,
                    SupportedValueType.String => 10,
                    _ => int.MaxValue
                },
                SupportedValueType.Binary => toType switch
                {
                    SupportedValueType.Integer => 2,
                    SupportedValueType.Numeric => 2,
                    SupportedValueType.Binary => 0,
                    SupportedValueType.String => 10,
                    _ => int.MaxValue
                },
                SupportedValueType.String => toType switch
                {
                    SupportedValueType.String => 0,
                    _ => int.MaxValue
                },
                SupportedValueType.Boolean => toType switch
                {
                    SupportedValueType.Boolean => 0,
                    SupportedValueType.Binary => 2,
                    SupportedValueType.String => 10,
                    _ => int.MaxValue
                },
                _ => int.MaxValue
            };

        /// <summary>
        ///     Gets the standard strategy cost for converting between two supported types.
        /// </summary>
        /// <param name="internalValueType">Type of the internal value.</param>
        /// <param name="toType">The type to convert to.</param>
        /// <returns>
        ///     The standard strategy cost.
        /// </returns>
        public static int GetStandardStrategyCost(
            Type internalValueType,
            in SupportedValueType toType)
        {
            if (internalValueType == typeof(long))
            {
                return toType switch
                {
                    SupportedValueType.Integer => 0,
                    SupportedValueType.Numeric => 1,
                    SupportedValueType.Binary => 1,
                    SupportedValueType.String => 2,
                    _ => int.MaxValue
                };
            }

            if (internalValueType == typeof(double))
            {
                return toType switch
                {
                    SupportedValueType.Numeric => 0,
                    SupportedValueType.Binary => 1,
                    SupportedValueType.String => 2,
                    _ => int.MaxValue
                };
            }

            if (internalValueType == typeof(byte[]))
            {
                return toType switch
                {
                    SupportedValueType.Integer => 1,
                    SupportedValueType.Numeric => 1,
                    SupportedValueType.Binary => 0,
                    SupportedValueType.String => 2,
                    _ => int.MaxValue
                };
            }

            if (internalValueType == typeof(string))
            {
                return toType switch
                {
                    SupportedValueType.String => 0,
                    _ => int.MaxValue
                };
            }

            if (internalValueType == typeof(bool))
            {
                return toType switch
                {
                    SupportedValueType.Boolean => 0,
                    SupportedValueType.Binary => 1,
                    SupportedValueType.String => 2,
                    _ => int.MaxValue
                };
            }

            return int.MaxValue;
        }

#endregion

#region Utility methods

        /// <summary>
        ///     Gets the total conversion costs.
        /// </summary>
        /// <param name="initialCost">The initial cost.</param>
        /// <param name="fromType">From type.</param>
        /// <param name="toType">To type.</param>
        /// <returns>The total conversion cost.</returns>
        [SuppressMessage(
            "Performance",
            "EPS02:Non-readonly struct used as in-parameter",
            Justification = "It's a primitive type, the compiler can handle it.")]
        protected static int GetTotalConversionCosts(
            in int initialCost,
            in SupportedValueType fromType,
            in SupportedValueType toType)
        {
            int intTotalCost;
            if (initialCost == int.MaxValue)
            {
                intTotalCost = int.MaxValue;
            }
            else
            {
                var conversionCost = GetStandardConversionStrategyCost(
                    in fromType,
                    in toType);

                if (conversionCost == int.MaxValue)
                {
                    intTotalCost = int.MaxValue;
                }
                else
                {
                    intTotalCost = conversionCost + initialCost;
                }
            }

            return intTotalCost;
        }

#endregion

#endregion

#region Phase 2 - Verification

        /// <summary>
        ///     Verifies the type of this node against a mask of desired types, and returns what is possible.
        /// </summary>
        /// <param name="typeMask">The type mask to check agains.</param>
        /// <returns>A filtered type mask of supported types.</returns>
        /// <exception cref="ExpressionNotValidLogicallyException">None of the types in the mask are supported.</exception>
        public SupportableValueType VerifyPossibleType(in SupportableValueType typeMask)
        {
            SupportableValueType supportedType = this.PossibleReturnType & typeMask;
            if (supportedType == SupportableValueType.None)
            {
                throw new ExpressionNotValidLogicallyException();
            }

            return supportedType;
        }

        /// <summary>
        ///     Verifies this node and all nodes above it for logical validity.
        /// </summary>
        /// <remarks>
        ///     <para>This method is expected to be overridden, and is a good place to do type restriction verification.</para>
        /// </remarks>
        public virtual void Verify()
        {
        }

        /// <summary>
        ///     Checks whether or not a supportable type is actually supported.
        /// </summary>
        /// <param name="supportableType">Type of the supportable.</param>
        /// <returns>Whether or not the type is supported.</returns>
        public bool CheckSupportedType(in SupportableValueType supportableType) =>
            (this.PossibleReturnType & supportableType) != SupportableValueType.None;

#endregion

#region Phase 3 - Expression generation

        /// <summary>
        ///     Calculates a value indicating the cost of an execution strategy for a specific value type. Higher values are more
        ///     costly.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <returns>An execution strategy cost.</returns>
        public int CalculateStrategyCost(in SupportedValueType valueType)
        {
            if (this.CalculatedCosts.TryGetValue(
                valueType,
                out (int Cost, SupportedValueType InternalType) tuple))
            {
                return tuple.Cost;
            }

            throw new ExpressionNotValidLogicallyException();
        }

        /// <summary>
        ///     Calculates the least costly execution strategy.
        /// </summary>
        /// <returns>The preferred type for which the exdcution strategy is least costly.</returns>
        public SupportedValueType CalculateLeastCostlyStrategy() =>
            this.CalculatedCosts.GroupBy(p => p.Value.Cost)
                .OrderBy(p => p.Key)
                .FirstOrDefault()
                ?.First()
                .Key ??
            SupportedValueType.Unknown;

        /// <summary>
        ///     Generates an expression for a supported value type.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="comparisonTolerance">The comparison tolerance.</param>
        /// <returns>A compiled expression, if one is possible.</returns>
        [NotNull]
        public Expression GenerateExpression(
            in SupportedValueType valueType,
            in ComparisonTolerance comparisonTolerance)
        {
            if (((int)valueType | (int)this.PossibleReturnType) == 0)
            {
                throw new ExpressionNotValidLogicallyException();
            }

            try
            {
                return valueType switch
                {
                    SupportedValueType.Integer => this.integerExpression ??=
                        ConvertToIntegerExpression(
                            this.GenerateExpressionInternal(
                                in valueType,
                                in comparisonTolerance)),
                    SupportedValueType.Numeric => this.numericExpression ??=
                        ConvertToNumericExpression(
                            this.GenerateExpressionInternal(
                                in valueType,
                                in comparisonTolerance)),
                    SupportedValueType.Binary => this.binaryExpression ??= ConvertToByteArrayExpression(
                        this.GenerateExpressionInternal(
                            in valueType,
                            in comparisonTolerance)),
                    SupportedValueType.String => this.stringExpression ??=
                        StringFormatter.CreateStringConversionExpression(
                            this.GenerateExpressionInternal(
                                in valueType,
                                in comparisonTolerance)),
                    SupportedValueType.Boolean => this.boolExpression ??= ConvertToBooleanExpression(
                        this.GenerateExpressionInternal(
                            in valueType,
                            in comparisonTolerance)),
                    _ => throw new MathematicsEngineException()
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

#endregion

#region Abstract methods

        /// <summary>
        ///     Creates a deep clone of the source object.
        /// </summary>
        /// <param name="context">The deep cloning context.</param>
        /// <returns>A deep clone.</returns>
        public abstract NodeBase DeepClone(NodeCloningContext context);

        /// <summary>
        ///     Simplifies this node, if possible, reflexively returns otherwise.
        /// </summary>
        /// <returns>A simplified node, or this instance.</returns>
        [NotNull]
        public abstract NodeBase Simplify();

        /// <summary>
        ///     Generates the expression that this node represents.
        /// </summary>
        /// <param name="valueType">Type of the value.</param>
        /// <param name="comparisonTolerance">The comparison tolerance.</param>
        /// <returns>A compiled expression, if one is possible.</returns>
        [NotNull]
        protected abstract Expression GenerateExpressionInternal(
            in SupportedValueType valueType,
            in ComparisonTolerance comparisonTolerance);

#endregion

#region Conversion methods

        private static long ConvertToIntegerFromNumeric(double numeric)
        {
            if (numeric > long.MaxValue || numeric < long.MinValue)
            {
                throw new ArgumentInvalidTypeException(nameof(numeric));
            }

            var numericAbs = global::System.Math.Abs(numeric);
            if (numericAbs - global::System.Math.Floor(numericAbs) < double.Epsilon)
            {
                return Convert.ToInt64(numeric);
            }

            throw new ArgumentInvalidTypeException(nameof(numeric));
        }

        private static double ConvertToNumeric([NotNull] byte[] binary)
        {
            if (binary.Length > 8)
            {
                throw new ArgumentInvalidTypeException(nameof(binary));
            }

            if (binary.Length < 8)
            {
                byte[] bytes = new byte[8];
                Array.Copy(
                    binary,
                    bytes,
                    binary.Length);
                binary = bytes;
            }

            return BitConverter.ToDouble(
                binary,
                0);
        }

        private static long ConvertToInteger([NotNull] byte[] binary)
        {
            if (binary.Length > 8)
            {
                throw new ArgumentInvalidTypeException(nameof(binary));
            }

            if (binary.Length < 8)
            {
                byte[] bytes = new byte[8];
                Array.Copy(
                    binary,
                    bytes,
                    binary.Length);
                binary = bytes;
            }

            return BitConverter.ToInt64(
                binary,
                0);
        }

        /// <summary>
        ///     Converts an expression to byte array expression.
        /// </summary>
        /// <param name="originalExpression">The original expression.</param>
        /// <returns>A converted expression.</returns>
        /// <exception cref="MathematicsEngineException">An internal exception that cannot be avoided.</exception>
        [NotNull]
        protected static Expression ConvertToByteArrayExpression([NotNull] Expression originalExpression)
        {
            Requires.NotNull(
                originalExpression,
                nameof(originalExpression));

            if (originalExpression.Type == typeof(byte[]))
            {
                return originalExpression;
            }

            if (originalExpression.Type == typeof(string))
            {
                throw new ExpressionNotValidLogicallyException();
            }

            MethodInfo? mi = typeof(BitConverter).GetMethod(
                                 nameof(BitConverter.GetBytes),
                                 new[]
                                 {
                                     originalExpression.Type
                                 }) ??
                             throw new MathematicsEngineException();

            return Expression.Call(
                mi,
                originalExpression);
        }

        /// <summary>
        ///     Converts an expression to byte array expression.
        /// </summary>
        /// <param name="originalExpression">The original expression.</param>
        /// <returns>A converted expression.</returns>
        /// <exception cref="MathematicsEngineException">An internal exception that cannot be avoided.</exception>
        [SuppressMessage(
            "Performance",
            "HAA0603:Delegate allocation from a method group",
            Justification = "This is intended.")]
        [NotNull]
        protected static Expression ConvertToNumericExpression([NotNull] Expression originalExpression)
        {
            Requires.NotNull(
                originalExpression,
                nameof(originalExpression));

            if (originalExpression.Type == typeof(string) || originalExpression.Type == typeof(bool))
            {
                throw new MathematicsEngineException();
            }

            if (originalExpression.Type == typeof(double))
            {
                return originalExpression;
            }

            if (originalExpression.Type == typeof(long))
            {
                MethodInfo mi = typeof(Convert).GetMethod(
                                    nameof(Convert.ToDouble),
                                    new[]
                                    {
                                        typeof(long)
                                    }) ??
                                throw new MathematicsEngineException();

                return Expression.Call(
                    mi,
                    originalExpression);
            }

            if (originalExpression.Type == typeof(int))
            {
                MethodInfo mi = typeof(Convert).GetMethod(
                                    nameof(Convert.ToDouble),
                                    new[]
                                    {
                                        typeof(int)
                                    }) ??
                                throw new MathematicsEngineException();

                return Expression.Call(
                    mi,
                    originalExpression);
            }

            if (originalExpression.Type == typeof(byte[]))
            {
                Func<byte[], double> bf = ConvertToNumeric;

                return Expression.Call(
                    bf.Method,
                    originalExpression);
            }

            throw new MathematicsEngineException();
        }

        /// <summary>
        ///     Converts an expression to byte array expression.
        /// </summary>
        /// <param name="originalExpression">The original expression.</param>
        /// <returns>A converted expression.</returns>
        /// <exception cref="MathematicsEngineException">An internal exception that cannot be avoided.</exception>
        [SuppressMessage(
            "StyleCop.CSharp.ReadabilityRules",
            "SA1118:Parameter should not span multiple lines",
            Justification = "It should when it's an array.")]
        [SuppressMessage(
            "Performance",
            "HAA0601:Value type to reference type conversion causing boxing allocation",
            Justification = "This is intended.")]
        [NotNull]
        protected static Expression ConvertToBooleanExpression([NotNull] Expression originalExpression)
        {
            Requires.NotNull(
                originalExpression,
                nameof(originalExpression));

            if (originalExpression.Type == typeof(bool))
            {
                return originalExpression;
            }

            if (originalExpression.Type == typeof(byte[]))
            {
                MethodInfo mi = typeof(BitConverter).GetMethod(
                                    nameof(BitConverter.ToBoolean),
                                    new[]
                                    {
                                        typeof(byte[]),
                                        typeof(int)
                                    }) ??
                                throw new MathematicsEngineException();

                return Expression.Call(
                    mi,
                    originalExpression,
                    Expression.Constant(
                        0,
                        typeof(int)));
            }

            throw new MathematicsEngineException();
        }

        /// <summary>
        ///     Converts an expression to byte array expression.
        /// </summary>
        /// <param name="originalExpression">The original expression.</param>
        /// <returns>A converted expression.</returns>
        /// <exception cref="MathematicsEngineException">An internal exception that cannot be avoided.</exception>
        [SuppressMessage(
            "Performance",
            "HAA0603:Delegate allocation from a method group",
            Justification = "This is intended.")]
        [NotNull]
        protected static Expression ConvertToIntegerExpression([NotNull] Expression originalExpression)
        {
            Requires.NotNull(
                originalExpression,
                nameof(originalExpression));

            if (originalExpression.Type == typeof(string) || originalExpression.Type == typeof(bool))
            {
                throw new MathematicsEngineException();
            }

            if (originalExpression.Type == typeof(long))
            {
                return originalExpression;
            }

            if (originalExpression.Type == typeof(double))
            {
                Func<double, long> bf = ConvertToIntegerFromNumeric;

                return Expression.Call(
                    bf.Method,
                    originalExpression);
            }

            if (originalExpression.Type == typeof(int))
            {
                MethodInfo mi = typeof(Convert).GetMethod(
                                    nameof(Convert.ToInt64),
                                    new[]
                                    {
                                        typeof(int)
                                    }) ??
                                throw new MathematicsEngineException();

                return Expression.Call(
                    mi,
                    originalExpression);
            }

            if (originalExpression.Type == typeof(int))
            {
                MethodInfo mi = typeof(Convert).GetMethod(
                                    nameof(Convert.ToDouble),
                                    new[]
                                    {
                                        typeof(int)
                                    }) ??
                                throw new MathematicsEngineException();

                return Expression.Call(
                    mi,
                    originalExpression);
            }

            if (originalExpression.Type == typeof(byte[]))
            {
                Func<byte[], long> bf = ConvertToInteger;

                return Expression.Call(
                    bf.Method,
                    originalExpression);
            }

            throw new MathematicsEngineException();
        }

#endregion
    }
}