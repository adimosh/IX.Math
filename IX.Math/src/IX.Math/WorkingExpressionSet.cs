using IX.Math.BuiltIn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;

namespace IX.Math
{
    internal class WorkingExpressionSet
    {
        // Definition
        internal MathDefinition Definition;
        internal Dictionary<string, Func<ExpressionTreeNodeBase>> NumericBinaryOperators;
        internal Dictionary<string, Func<ExpressionTreeNodeBase>> BooleanBinaryOperators;
        internal Dictionary<string, Func<ExpressionTreeNodeBase>> NumericUnaryOperators;
        internal Dictionary<string, Func<ExpressionTreeNodeBase>> BooleanUnaryOperators;
        internal string[] BinaryOperatorsInOrder;
        internal string[] UnaryOperatorsInOrder;
        internal string[] AllOperatorsInOrder;
        internal string[] AllSymbols;
        internal Regex FunctionRegex;

        // Initial data
        internal string InitialExpression;
        internal CancellationToken CancellationToken;

        // Working domain
        internal Dictionary<string, RawExpressionContainer> SymbolTable;
        internal Dictionary<string, string> ReverseSymbolTable;
        internal string Expression;
        internal Dictionary<string, ExpressionTreeNodeParameter> ExternalParameters;
        internal ConstantsContainer Constants;
        internal ExpressionTreeNodeBase Body;
        internal object ValueIfConstant;

        // Results
        internal bool Success = false;
        internal bool InternallyValid = false;
        internal bool Constant = false;
        internal bool PossibleString = false;

        internal WorkingExpressionSet(string expression, MathDefinition mathDefinition, CancellationToken cancellationToken)
        {
            SymbolTable = new Dictionary<string, RawExpressionContainer>();
            ReverseSymbolTable = new Dictionary<string, string>();
            ExternalParameters = new Dictionary<string, ExpressionTreeNodeParameter>();
            Constants = new ConstantsContainer();
            InitialExpression = expression;
            CancellationToken = cancellationToken;
            Expression = expression;
            Definition = new MathDefinition(mathDefinition);

            AllOperatorsInOrder = new[]
            {
                Definition.GreaterThanOrEqualSymbol,
                Definition.LessThanOrEqualSymbol,
                Definition.GreaterThanSymbol,
                Definition.LessThanSymbol,
                Definition.DoesNotEqualSymbol,
                Definition.EqualsSymbol,
                Definition.XorSymbol,
                Definition.OrSymbol,
                Definition.AndSymbol,
                Definition.AddSymbol,
                Definition.SubtractSymbol,
                Definition.DivideSymbol,
                Definition.MultiplySymbol,
                Definition.PowerSymbol,
                Definition.ShiftLeftSymbol,
                Definition.ShiftRightSymbol,
                Definition.NotSymbol
            };

            FunctionRegex = new Regex($@"(?'functionName'.*?){Regex.Escape(Definition.Parantheses.Item1)}(?'expression'.*?){Regex.Escape(Definition.Parantheses.Item2)}");
        }

        internal void Initialize()
        {
            // Operator and function support
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

            // Operator string interpretation support
            BinaryOperatorsInOrder = new[]
            {
                Definition.GreaterThanOrEqualSymbol,
                Definition.LessThanOrEqualSymbol,
                Definition.GreaterThanSymbol,
                Definition.LessThanSymbol,
                Definition.DoesNotEqualSymbol,
                Definition.EqualsSymbol,
                Definition.XorSymbol,
                Definition.OrSymbol,
                Definition.AndSymbol,
                Definition.AddSymbol,
                Definition.SubtractSymbol,
                Definition.DivideSymbol,
                Definition.MultiplySymbol,
                Definition.PowerSymbol,
                Definition.ShiftLeftSymbol,
                Definition.ShiftRightSymbol,
            };

            UnaryOperatorsInOrder = new[]
            {
                Definition.SubtractSymbol,
                Definition.NotSymbol
            };

            AllSymbols = AllOperatorsInOrder
                .Union(new[]
                {
                    Definition.ParameterSeparator,
                    Definition.Parantheses.Item1,
                    Definition.Parantheses.Item2
                })
                .ToArray();
        }
    }
}