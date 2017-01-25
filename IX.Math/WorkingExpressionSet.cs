// <copyright file="WorkingExpressionSet.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Threading;
using IX.Math.BuiltIn;
using IX.Math.BuiltIn.Operators;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Operations.Binary;
using IX.Math.Nodes;

namespace IX.Math
{
    internal class WorkingExpressionSet
    {
        // Definition
#pragma warning disable SA1401 // Fields must be private
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
        internal Dictionary<string, ConstantNodeBase> ConstantsTable;
        internal Dictionary<string, string> ReverseConstantsTable;
        internal Dictionary<string, RawExpressionContainer> SymbolTable;
        internal Dictionary<string, string> ReverseSymbolTable;
        internal string Expression;
        internal Dictionary<string, ExpressionTreeNodeParameter> ExternalParameters;
        internal ExpressionTreeNodeBase Body;
        internal object ValueIfConstant;

        // Results
        internal bool Success = false;
        internal bool InternallyValid = false;
        internal bool Constant = false;
        internal bool PossibleString = false;
#pragma warning restore SA1401 // Fields must be private

        internal WorkingExpressionSet(string expression, MathDefinition mathDefinition, CancellationToken cancellationToken)
        {
            this.ConstantsTable = new Dictionary<string, ConstantNodeBase>();
            this.ReverseConstantsTable = new Dictionary<string, string>();

            this.SymbolTable = new Dictionary<string, RawExpressionContainer>();
            this.ReverseSymbolTable = new Dictionary<string, string>();
            this.ExternalParameters = new Dictionary<string, ExpressionTreeNodeParameter>();
            this.InitialExpression = expression;
            this.CancellationToken = cancellationToken;
            this.Expression = expression;
            this.Definition = new MathDefinition(mathDefinition);

            this.AllOperatorsInOrder = new[]
            {
                this.Definition.GreaterThanOrEqualSymbol,
                this.Definition.LessThanOrEqualSymbol,
                this.Definition.GreaterThanSymbol,
                this.Definition.LessThanSymbol,
                this.Definition.DoesNotEqualSymbol,
                this.Definition.EqualsSymbol,
                this.Definition.XorSymbol,
                this.Definition.OrSymbol,
                this.Definition.AndSymbol,
                this.Definition.AddSymbol,
                this.Definition.SubtractSymbol,
                this.Definition.DivideSymbol,
                this.Definition.MultiplySymbol,
                this.Definition.PowerSymbol,
                this.Definition.ShiftLeftSymbol,
                this.Definition.ShiftRightSymbol,
                this.Definition.NotSymbol
            };

            this.FunctionRegex = new Regex($@"(?'functionName'.*?){Regex.Escape(this.Definition.Parantheses.Item1)}(?'expression'.*?){Regex.Escape(this.Definition.Parantheses.Item2)}");
        }

        internal void Initialize()
        {
            var operators = this.AllOperatorsInOrder
                .OrderByDescending(p => p.Length)
                .Where(p => this.AllOperatorsInOrder.Any(q => q.Length < p.Length && p.Contains(q)));

            int i = 1;
            foreach (var op in operators.OrderByDescending(p => p.Length))
            {
                var s = $"@op{i}@";

                this.Expression = this.Expression.Replace(op, s);

                var allIndex = Array.IndexOf(this.AllOperatorsInOrder, op);
                if (allIndex != -1)
                {
                    this.AllOperatorsInOrder[allIndex] = s;
                }

                if (this.Definition.AddSymbol == op)
                {
                    this.Definition.AddSymbol = s;
                }

                if (this.Definition.AndSymbol == op)
                {
                    this.Definition.AndSymbol = s;
                }

                if (this.Definition.DivideSymbol == op)
                {
                    this.Definition.DivideSymbol = s;
                }

                if (this.Definition.DoesNotEqualSymbol == op)
                {
                    this.Definition.DoesNotEqualSymbol = s;
                }

                if (this.Definition.EqualsSymbol == op)
                {
                    this.Definition.EqualsSymbol = s;
                }

                if (this.Definition.GreaterThanOrEqualSymbol == op)
                {
                    this.Definition.GreaterThanOrEqualSymbol = s;
                }

                if (this.Definition.GreaterThanSymbol == op)
                {
                    this.Definition.GreaterThanSymbol = s;
                }

                if (this.Definition.LessThanOrEqualSymbol == op)
                {
                    this.Definition.LessThanOrEqualSymbol = s;
                }

                if (this.Definition.LessThanSymbol == op)
                {
                    this.Definition.LessThanSymbol = s;
                }

                if (this.Definition.MultiplySymbol == op)
                {
                    this.Definition.MultiplySymbol = s;
                }

                if (this.Definition.NotSymbol == op)
                {
                    this.Definition.NotSymbol = s;
                }

                if (this.Definition.OrSymbol == op)
                {
                    this.Definition.OrSymbol = s;
                }

                if (this.Definition.PowerSymbol == op)
                {
                    this.Definition.PowerSymbol = s;
                }

                if (this.Definition.ShiftLeftSymbol == op)
                {
                    this.Definition.ShiftLeftSymbol = s;
                }

                if (this.Definition.ShiftRightSymbol == op)
                {
                    this.Definition.ShiftRightSymbol = s;
                }

                if (this.Definition.SubtractSymbol == op)
                {
                    this.Definition.SubtractSymbol = s;
                }

                if (this.Definition.XorSymbol == op)
                {
                    this.Definition.XorSymbol = s;
                }

                i++;
            }

            // Operator and function support
            //this.NumericBinaryOperators = new Dictionary<string, Func<NodeBase, NodeBase, BinaryOperationNodeBase>>
            //{
            //    [this.Definition.AddSymbol] = (left, right) => new AddNode(left, right),
            //    [this.Definition.DivideSymbol] = () => new ExpressionTreeNodeNumericBinaryOperator(ExpressionType.Divide),
            //    [this.Definition.MultiplySymbol] = () => new ExpressionTreeNodeNumericBinaryOperator(ExpressionType.Multiply),
            //    [this.Definition.SubtractSymbol] = () => new ExpressionTreeNodeNumericBinaryOperator(ExpressionType.Subtract),
            //    [this.Definition.AndSymbol] = () => new ExpressionTreeNodeNumericBinaryOperator(ExpressionType.And),
            //    [this.Definition.OrSymbol] = () => new ExpressionTreeNodeNumericBinaryOperator(ExpressionType.Or),
            //    [this.Definition.XorSymbol] = () => new ExpressionTreeNodeNumericBinaryOperator(ExpressionType.ExclusiveOr),
            //    [this.Definition.ShiftLeftSymbol] = () => new ExpressionTreeNodeNumericBinaryOperator(ExpressionType.LeftShift),
            //    [this.Definition.ShiftRightSymbol] = () => new ExpressionTreeNodeNumericBinaryOperator(ExpressionType.RightShift),
            //    [this.Definition.PowerSymbol] = () => new ExpressionTreeNodeNumericBinaryOperator(),
            //    [this.Definition.DoesNotEqualSymbol] = () => new ExpressionTreeNodeNumericLogicalBinaryOperator(ExpressionType.NotEqual),
            //    [this.Definition.EqualsSymbol] = () => new ExpressionTreeNodeNumericLogicalBinaryOperator(ExpressionType.Equal),
            //    [this.Definition.GreaterThanOrEqualSymbol] = () => new ExpressionTreeNodeNumericLogicalBinaryOperator(ExpressionType.GreaterThanOrEqual),
            //    [this.Definition.GreaterThanSymbol] = () => new ExpressionTreeNodeNumericLogicalBinaryOperator(ExpressionType.GreaterThan),
            //    [this.Definition.LessThanOrEqualSymbol] = () => new ExpressionTreeNodeNumericLogicalBinaryOperator(ExpressionType.LessThanOrEqual),
            //    [this.Definition.LessThanSymbol] = () => new ExpressionTreeNodeNumericLogicalBinaryOperator(ExpressionType.LessThan),
            //};
            this.BooleanBinaryOperators = new Dictionary<string, Func<ExpressionTreeNodeBase>>
            {
                [this.Definition.AndSymbol] = () => new ExpressionTreeNodeBooleanBinaryOperator(ExpressionType.And),
                [this.Definition.OrSymbol] = () => new ExpressionTreeNodeBooleanBinaryOperator(ExpressionType.Or),
                [this.Definition.XorSymbol] = () => new ExpressionTreeNodeBooleanBinaryOperator(ExpressionType.ExclusiveOr),
                [this.Definition.DoesNotEqualSymbol] = () => new ExpressionTreeNodeBooleanBinaryOperator(ExpressionType.NotEqual),
                [this.Definition.EqualsSymbol] = () => new ExpressionTreeNodeBooleanBinaryOperator(ExpressionType.Equal),
            };
            this.NumericUnaryOperators = new Dictionary<string, Func<ExpressionTreeNodeBase>>
            {
                [this.Definition.NotSymbol] = () => new ExpressionTreeNodeNumericUnaryOperator(ExpressionType.Not),
                [this.Definition.SubtractSymbol] = () => new ExpressionTreeNodeNumericUnaryOperator(ExpressionType.Negate),
            };
            this.BooleanUnaryOperators = new Dictionary<string, Func<ExpressionTreeNodeBase>>
            {
                [this.Definition.NotSymbol] = () => new ExpressionTreeNodeBooleanUnaryOperator(ExpressionType.Not),
            };

            // Operator string interpretation support
            this.BinaryOperatorsInOrder = new[]
            {
                this.Definition.GreaterThanOrEqualSymbol,
                this.Definition.LessThanOrEqualSymbol,
                this.Definition.GreaterThanSymbol,
                this.Definition.LessThanSymbol,
                this.Definition.DoesNotEqualSymbol,
                this.Definition.EqualsSymbol,
                this.Definition.XorSymbol,
                this.Definition.OrSymbol,
                this.Definition.AndSymbol,
                this.Definition.AddSymbol,
                this.Definition.SubtractSymbol,
                this.Definition.DivideSymbol,
                this.Definition.MultiplySymbol,
                this.Definition.PowerSymbol,
                this.Definition.ShiftLeftSymbol,
                this.Definition.ShiftRightSymbol,
            };

            this.UnaryOperatorsInOrder = new[]
            {
                this.Definition.SubtractSymbol,
                this.Definition.NotSymbol
            };

            this.AllSymbols = this.AllOperatorsInOrder
                .Union(new[]
                {
                    this.Definition.ParameterSeparator,
                    this.Definition.Parantheses.Item1,
                    this.Definition.Parantheses.Item2
                })
                .ToArray();
        }
    }
}