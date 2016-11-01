using IX.Math.BuiltIn;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace IX.Math
{
    internal class WorkingDefinition
    {
        internal WorkingDefinition(MathDefinition definition)
        {
            Definition = definition;

            ParanthesesMatcher =
                new Regex($"(?'before'.*)(?'toreplace'{Regex.Escape(definition.Parantheses.Item1)}(?'body'.*){Regex.Escape(definition.Parantheses.Item2)})(?'after'.*)");

            OperatorsForRegex =
                $"(?:{Regex.Escape(definition.AddSymbol)}|{Regex.Escape(definition.AndSymbol)}|{Regex.Escape(definition.DivideSymbol)}|{Regex.Escape(definition.DoesNotEqualSymbol)}|{Regex.Escape(definition.EqualsSymbol)}|{Regex.Escape(definition.MultiplySymbol)}|{Regex.Escape(definition.NotSymbol)}|{Regex.Escape(definition.OrSymbol)}|{Regex.Escape(definition.PowerSymbol)}|{Regex.Escape(definition.SubtractSymbol)}|{Regex.Escape(definition.XorSymbol)}|{Regex.Escape(definition.ShiftLeftSymbol)}|{Regex.Escape(definition.ShiftRightSymbol)})";

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

            BinaryExpressionGenerators = new Dictionary<string, Func<Expression, Expression, Expression>>
            {
                [definition.AddSymbol] = (left, right) => Expression.Add(left, right),
                [definition.AndSymbol] = (left, right) => Expression.And(left, right),
                [definition.DivideSymbol] = (left, right) => Expression.Divide(left, right),
                [definition.DoesNotEqualSymbol] = (left, right) => Expression.NotEqual(left, right),
                [definition.EqualsSymbol] = (left, right) => Expression.Equal(left, right),
                [definition.MultiplySymbol] = (left, right) => Expression.Multiply(left, right),
                [definition.OrSymbol] = (left, right) => Expression.Or(left, right),
                [definition.PowerSymbol] = (left, right) => Expression.Power(left, right),
                [definition.SubtractSymbol] = (left, right) => Expression.Subtract(left, right),
                [definition.XorSymbol] = (left, right) => Expression.ExclusiveOr(left, right),
                [definition.GreaterThanOrEqualSymbol] = (left, right) => Expression.GreaterThanOrEqual(left, right),
                [definition.GreaterThanSymbol] = (left, right) => Expression.GreaterThan(left, right),
                [definition.LessThanOrEqualSymbol] = (left, right) => Expression.LessThanOrEqual(left, right),
                [definition.LessThanSymbol] = (left, right) => Expression.LessThan(left, right),
                [definition.ShiftLeftSymbol] = (left, right) => Expression.LeftShift(left, right),
                [definition.ShiftRightSymbol] = (left, right) => Expression.RightShift(left, right),
            };

            UnaryExpressionGenerators = new Dictionary<string, Func<Type, Expression, Expression>>
            {
                [definition.SubtractSymbol] = (type, expr) => Expression.Subtract(Expression.Constant(Convert.ChangeType(0, type), type), expr),
                [definition.NotSymbol] = (type, expr) => Expression.Negate(expr)
            };

            // New expression tree implementation
            NumericBinaryOperators = new Dictionary<string, ExpressionTreeNodeBase>
            {
                [definition.AddSymbol] = new ExpressionTreeNodeNumericBinaryOperator(ExpressionType.Add),
                [definition.DivideSymbol] = new ExpressionTreeNodeNumericBinaryOperator(ExpressionType.Divide),
                [definition.MultiplySymbol] = new ExpressionTreeNodeNumericBinaryOperator(ExpressionType.Multiply),
                [definition.SubtractSymbol] = new ExpressionTreeNodeNumericBinaryOperator(ExpressionType.Subtract),
                [definition.AndSymbol] = new ExpressionTreeNodeNumericBinaryOperator(ExpressionType.And),
                [definition.OrSymbol] = new ExpressionTreeNodeNumericBinaryOperator(ExpressionType.Or),
                [definition.XorSymbol] = new ExpressionTreeNodeNumericBinaryOperator(ExpressionType.ExclusiveOr),
                [definition.ShiftLeftSymbol] = new ExpressionTreeNodeNumericBinaryOperator(ExpressionType.LeftShift),
                [definition.ShiftRightSymbol] = new ExpressionTreeNodeNumericBinaryOperator(ExpressionType.RightShift),
                [definition.PowerSymbol] = new ExpressionTreeNodeNumericBinaryOperator(),
                [definition.DoesNotEqualSymbol] = new ExpressionTreeNodeNumericLogicalBinaryOperator(ExpressionType.NotEqual),
                [definition.EqualsSymbol] = new ExpressionTreeNodeNumericLogicalBinaryOperator(ExpressionType.Equal),
                [definition.GreaterThanOrEqualSymbol] = new ExpressionTreeNodeNumericLogicalBinaryOperator(ExpressionType.GreaterThanOrEqual),
                [definition.GreaterThanSymbol] = new ExpressionTreeNodeNumericLogicalBinaryOperator(ExpressionType.GreaterThan),
                [definition.LessThanOrEqualSymbol] = new ExpressionTreeNodeNumericLogicalBinaryOperator(ExpressionType.LessThanOrEqual),
                [definition.LessThanSymbol] = new ExpressionTreeNodeNumericLogicalBinaryOperator(ExpressionType.LessThan),
            };
            BooleanBinaryOperators = new Dictionary<string, ExpressionTreeNodeBase>
            {
                [definition.AndSymbol] = new ExpressionTreeNodeBooleanBinaryOperator(ExpressionType.And),
                [definition.OrSymbol] = new ExpressionTreeNodeBooleanBinaryOperator(ExpressionType.Or),
                [definition.XorSymbol] = new ExpressionTreeNodeBooleanBinaryOperator(ExpressionType.ExclusiveOr),
                [definition.DoesNotEqualSymbol] = new ExpressionTreeNodeBooleanBinaryOperator(ExpressionType.NotEqual),
                [definition.EqualsSymbol] = new ExpressionTreeNodeBooleanBinaryOperator(ExpressionType.Equal),
            };
        }

        internal readonly MathDefinition Definition;
        internal readonly Regex ParanthesesMatcher;
        internal readonly string OperatorsForRegex;
        internal readonly string[] BinaryOperatorsInOrder;
        internal readonly string[] UnaryOperatorsInOrder;
        internal readonly string[] AllOperatorsInOrder;
        internal readonly Dictionary<string, Func<Expression, Expression, Expression>> BinaryExpressionGenerators;
        internal readonly Dictionary<string, Func<Type, Expression, Expression>> UnaryExpressionGenerators;

        // New expression tree implementation
        internal readonly Dictionary<string, ExpressionTreeNodeBase> NumericBinaryOperators;
        internal readonly Dictionary<string, ExpressionTreeNodeBase> BooleanBinaryOperators;
    }
}
