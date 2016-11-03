using IX.Math.BuiltIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace IX.Math
{
    internal class WorkingDefinition
    {
        internal readonly MathDefinition OriginalDefinition;
        internal readonly string[] BinaryOperatorsInOrder;
        internal readonly string[] UnaryOperatorsInOrder;
        internal readonly string[] AllOperatorsInOrder;
        internal readonly string[] AllSymbols;
        internal readonly Regex FunctionRegex;

        // New expression tree implementation
        internal MathDefinition Definition;
        internal Dictionary<string, Func<ExpressionTreeNodeBase>> NumericBinaryOperators;
        internal Dictionary<string, Func<ExpressionTreeNodeBase>> BooleanBinaryOperators;
        internal Dictionary<string, Func<ExpressionTreeNodeBase>> NumericUnaryOperators;
        internal Dictionary<string, Func<ExpressionTreeNodeBase>> BooleanUnaryOperators;

        internal WorkingDefinition(MathDefinition definition)
        {
            OriginalDefinition = definition;
            Definition = new MathDefinition(definition);

            BinaryOperatorsInOrder = new[]
            {
                definition.GreaterThanOrEqualSymbol,
                definition.LessThanOrEqualSymbol,
                definition.GreaterThanSymbol,
                definition.LessThanSymbol,
                definition.DoesNotEqualSymbol,
                definition.EqualsSymbol,
                definition.XorSymbol,
                definition.OrSymbol,
                definition.AndSymbol,
                definition.AddSymbol,
                definition.SubtractSymbol,
                definition.DivideSymbol,
                definition.MultiplySymbol,
                definition.PowerSymbol,
                definition.ShiftLeftSymbol,
                definition.ShiftRightSymbol,
            };

            UnaryOperatorsInOrder = new[]
            {
                definition.SubtractSymbol,
                definition.NotSymbol
            };

            AllOperatorsInOrder = new[]
            {
                definition.GreaterThanOrEqualSymbol,
                definition.LessThanOrEqualSymbol,
                definition.GreaterThanSymbol,
                definition.LessThanSymbol,
                definition.DoesNotEqualSymbol,
                definition.EqualsSymbol,
                definition.XorSymbol,
                definition.OrSymbol,
                definition.AndSymbol,
                definition.AddSymbol,
                definition.SubtractSymbol,
                definition.DivideSymbol,
                definition.MultiplySymbol,
                definition.PowerSymbol,
                definition.ShiftLeftSymbol,
                definition.ShiftRightSymbol,
                definition.NotSymbol
            };
            AllSymbols = AllOperatorsInOrder
                .Union(new[]
                {
                    definition.ParameterSeparator,
                    definition.Parantheses.Item1,
                    definition.Parantheses.Item2
                })
                .ToArray();

            FunctionRegex = new Regex($@"(?'functionName'.*?){Regex.Escape(definition.Parantheses.Item1)}(?'expression'.*?){Regex.Escape(definition.Parantheses.Item2)}");

            // New expression tree implementation
        }

        internal void Initialize()
        {
            NumericBinaryOperators = new Dictionary<string, Func<ExpressionTreeNodeBase>>
            {
                [Definition.AddSymbol] = () => new ExpressionTreeNodeNumericBinaryOperator(ExpressionType.Add),
                [Definition.DivideSymbol] = () => new ExpressionTreeNodeNumericBinaryOperator(ExpressionType.Divide),
                [Definition.MultiplySymbol] = () => new ExpressionTreeNodeNumericBinaryOperator(ExpressionType.Multiply),
                [Definition.SubtractSymbol] = () => new ExpressionTreeNodeNumericBinaryOperator(ExpressionType.Subtract),
                [Definition.AndSymbol] = () => new ExpressionTreeNodeNumericBinaryOperator(ExpressionType.And),
                [Definition.OrSymbol] = () => new ExpressionTreeNodeNumericBinaryOperator(ExpressionType.Or),
                [Definition.XorSymbol] = () => new ExpressionTreeNodeNumericBinaryOperator(ExpressionType.ExclusiveOr),
                [Definition.ShiftLeftSymbol] = () => new ExpressionTreeNodeNumericBinaryOperator(ExpressionType.LeftShift),
                [Definition.ShiftRightSymbol] = () => new ExpressionTreeNodeNumericBinaryOperator(ExpressionType.RightShift),
                [Definition.PowerSymbol] = () => new ExpressionTreeNodeNumericBinaryOperator(),
                [Definition.DoesNotEqualSymbol] = () => new ExpressionTreeNodeNumericLogicalBinaryOperator(ExpressionType.NotEqual),
                [Definition.EqualsSymbol] = () => new ExpressionTreeNodeNumericLogicalBinaryOperator(ExpressionType.Equal),
                [Definition.GreaterThanOrEqualSymbol] = () => new ExpressionTreeNodeNumericLogicalBinaryOperator(ExpressionType.GreaterThanOrEqual),
                [Definition.GreaterThanSymbol] = () => new ExpressionTreeNodeNumericLogicalBinaryOperator(ExpressionType.GreaterThan),
                [Definition.LessThanOrEqualSymbol] = () => new ExpressionTreeNodeNumericLogicalBinaryOperator(ExpressionType.LessThanOrEqual),
                [Definition.LessThanSymbol] = () => new ExpressionTreeNodeNumericLogicalBinaryOperator(ExpressionType.LessThan),
            };
            BooleanBinaryOperators = new Dictionary<string, Func<ExpressionTreeNodeBase>>
            {
                [Definition.AndSymbol] = () => new ExpressionTreeNodeBooleanBinaryOperator(ExpressionType.And),
                [Definition.OrSymbol] = () => new ExpressionTreeNodeBooleanBinaryOperator(ExpressionType.Or),
                [Definition.XorSymbol] = () => new ExpressionTreeNodeBooleanBinaryOperator(ExpressionType.ExclusiveOr),
                [Definition.DoesNotEqualSymbol] = () => new ExpressionTreeNodeBooleanBinaryOperator(ExpressionType.NotEqual),
                [Definition.EqualsSymbol] = () => new ExpressionTreeNodeBooleanBinaryOperator(ExpressionType.Equal),
            };
            NumericUnaryOperators = new Dictionary<string, Func<ExpressionTreeNodeBase>>
            {
                [Definition.NotSymbol] = () => new ExpressionTreeNodeNumericUnaryOperator(ExpressionType.Not),
                [Definition.SubtractSymbol] = () => new ExpressionTreeNodeNumericUnaryOperator(ExpressionType.Negate),
            };
            BooleanUnaryOperators = new Dictionary<string, Func<ExpressionTreeNodeBase>>
            {
                [Definition.NotSymbol] = () => new ExpressionTreeNodeBooleanUnaryOperator(ExpressionType.Not),
            };
        }
    }
}
