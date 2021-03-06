// <copyright file="WorkingExpressionSet.ExpressionGeneration.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Linq;
using System.Text.RegularExpressions;
using IX.Math.Computation;
using IX.Math.Exceptions;
using IX.Math.ExpressionState;
using IX.Math.Generators;
using IX.Math.Nodes;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Functions.Binary;
using IX.Math.Nodes.Functions.Nonary;
using IX.Math.Nodes.Functions.Ternary;
using IX.Math.Nodes.Functions.Unary;
using IX.Math.Nodes.Operators.Binary;
using IX.Math.Nodes.Operators.Unary;
using IX.StandardExtensions.Contracts;
using IX.StandardExtensions.Globalization;
using JetBrains.Annotations;
using DiagCA = System.Diagnostics.CodeAnalysis;

namespace IX.Math.WorkingSet
{
    internal partial class WorkingExpressionSet
    {
        private readonly Regex whitespaceRegex = new Regex("\\s*");

        [DiagCA.SuppressMessage(
            "Performance",
            "HAA0401:Possible allocation of reference type enumerator",
            Justification = "Not avoidable for now.")]
        [DiagCA.SuppressMessage(
            "CodeSmell",
            "ERP022:Unobserved exception in generic exception handler",
            Justification = "We want this to happen.")]
        internal ComputationBody CreateBody()
        {
            // Stage 1. Extract literal constants from the expression
            // ======================================================
            this.expression = this.ExtractConstants();

            // Stage 2. Prepare expression for processing
            // ==========================================

            // Create a symbol for the main expression

            // Prepares expression and takes care of operators to ensure that they are all OK and usable
            this.Initialize();

            var symbolValueToCompare = this.whitespaceRegex.Replace(
                this.expression,
                string.Empty);

            this.symbolTable.Add(
                string.Empty,
                ExpressionSymbol.GenerateSymbol(
                    string.Empty,
                    symbolValueToCompare));

            // Stage 3. Replace functions to sub-expressions
            // =============================================
            this.ReplaceFunctions();

            // We save a split expression for determining parameter order
            var splitExpression = this.symbolTable[string.Empty]
                .Expression.Split(
                    this.allSymbols.ToArray(),
                    StringSplitOptions.RemoveEmptyEntries);

            if (splitExpression.Length == 1 &&
                this.symbolTable.Count == 1 &&
                this.symbolTable[string.Empty]
                    .Expression ==
                symbolValueToCompare)
            {
                // We are not dealing with an expression, but with a value, let's try to compute it somehow
                this.symbolTable[string.Empty]
                    .Expression = this.expression;
            }

            // Stage 4. Extract symbols
            // ========================
            this.FormatParentheses();

            // Populating symbol tables
            foreach (var p in this.symbolTable.Where(p => !p.Value.IsFunctionCall)
                .Select(p => p.Value.Expression))
            {
                this.PopulateTables(p);
            }

            // For each parameter from the table we've just populated, see where it's first used, and fill in that index as the order
            foreach (var paramForOrdering in this.parameterRegistry)
            {
                paramForOrdering.Value.Order = this.expression.CurrentCultureIndexOf(paramForOrdering.Key);
            }

            // Stage 5. Compute and return body
            // ================================
            NodeBase body;
            try
            {
                body = this.GenerateExpression(this.symbolTable[string.Empty].Expression);
            }
            catch
            {
                return ComputationBody.Empty;
            }

            this.Success = true;

            return new ComputationBody(
                body,
                this.parameterRegistry);
        }

        [CanBeNull]
        [DiagCA.SuppressMessage(
            "CodeSmell",
            "ERP022:Unobserved exception in generic exception handler",
            Justification = "We want that.")]
        private NodeBase GenerateExpression(string expression)
        {
            Requires.NotNullOrWhiteSpace(
                expression,
                nameof(expression));

            // Expression might be an already-defined constant
            if (this.constantsTable.TryGetValue(
                expression,
                out ConstantNodeBase c1))
            {
                return c1;
            }

            if (this.reverseConstantsTable.TryGetValue(
                expression,
                out var c2))
            {
                if (this.constantsTable.TryGetValue(
                    c2,
                    out ConstantNodeBase c3))
                {
                    return c3;
                }
            }

            // Check whether expression is an external parameter
            if (this.parameterRegistry.TryGetValue(expression, out var parameter))
            {
                return parameter;
            }

            // Check whether the expression already exists in the symbols table
            if (this.symbolTable.TryGetValue(
                expression,
                out ExpressionSymbol e1))
            {
                return this.GenerateExpression(e1.Expression);
            }

            if (this.reverseSymbolTable.TryGetValue(
                expression,
                out var e2))
            {
                if (this.symbolTable.TryGetValue(
                    e2,
                    out ExpressionSymbol e3))
                {
                    if (e3.Expression != expression)
                    {
                        return this.GenerateExpression(e3.Expression);
                    }
                }
            }

            // Check whether the expression is a function call
            if (expression.InvariantCultureContains(this.definition.Parentheses.Open) &&
                expression.InvariantCultureContains(this.definition.Parentheses.Close))
            {
                return GenerateFunctionCallExpression(expression);
            }

            // Check whether the expression is a binary operator
            foreach (var (_, operatorPosition, @operator) in OperatorSequenceGenerator
                .GetOperatorsInOrderInExpression(
                    expression,
                    this.binaryOperators.KeysByLevel.ToArray())
                .OrderBy(p => p.Item1)
                .ThenByDescending(p => p.Item2)
                .ToArray())
            {
                NodeBase exp = ExpressionByBinaryOperator(
                    this,
                    expression,
                    operatorPosition,
                    @operator);

                NodeBase ExpressionByBinaryOperator(
                    WorkingExpressionSet innerWorkingSet,
                    string s,
                    int position,
                    string op)
                {
                    if (position == 0)
                    {
                        // We certainly have an unary operator if the operator is at the beginning of the expression. We therefore cannot continue with binary
                        return null;
                    }

                    if (!innerWorkingSet.binaryOperators.TryGetValue(
                        op,
                        out Func<NodeBase, NodeBase, BinaryOperatorNodeBase> t))
                    {
                        // Binary operator not actually found.
                        return null;
                    }

                    // We have a normal, regular binary
                    var eee = s.Substring(
                        0,
                        position);
                    if (string.IsNullOrWhiteSpace(eee))
                    {
                        // Empty space before operator. Normally, this should never be hit.
                        return null;
                    }

                    NodeBase left = this.GenerateExpression(eee);
                    if (left == null)
                    {
                        // Left expression is invalid.
                        return null;
                    }

                    eee = s.Substring(position + op.Length);
                    if (string.IsNullOrWhiteSpace(eee))
                    {
                        // Empty space after operator. Normally, this should never be hit.
                        return null;
                    }

                    NodeBase right = this.GenerateExpression(eee);
                    if (right == null)
                    {
                        // Right expression is invalid.
                        return null;
                    }

                    return t(
                            left,
                            right)
                        .Simplify();
                }

                if (exp != null)
                {
                    // We have found a valid binary operator expression
                    return exp;
                }
            }

            // Check whether the expression is a unary operator
            foreach (Tuple<int, int, string> operatorPosition in OperatorSequenceGenerator
                .GetOperatorsInOrderInExpression(
                    expression,
                    this.unaryOperators.KeysByLevel.ToArray())
                .OrderBy(p => p.Item1)
                .ThenByDescending(p => p.Item2)
                .ToArray())
            {
                NodeBase exp = ExpressionByUnaryOperator(
                    expression,
                    operatorPosition.Item3);

                NodeBase ExpressionByUnaryOperator(
                    string s,
                    string op)
                {
                    if (!s.CurrentCultureStartsWith(op) ||
                    !this.unaryOperators.TryGetValue(
                        op,
                        out Func<NodeBase, UnaryOperatorNodeBase> t))
                    {
                        // The unary operator is not valid.
                        return null;
                    }

                    var eee = s.Substring(op.Length);
                    if (string.IsNullOrWhiteSpace(eee))
                    {
                        // We might have a valid unary operator but attached to nothing - the expression cannot be evaluated any further on this branch
                        return null;
                    }

                    // We have a valid unary operator and the expression starts with it.
                    NodeBase expr = this.GenerateExpression(eee);
                    if (expr == null)
                    {
                        // The operand expression was not valid, or cancellation was requested.
                        return null;
                    }

                    return t(
                            expr)
                        .Simplify();
                }

                if (exp != null)
                {
                    // We have found a valid unary operator expression.
                    return exp;
                }
            }

            return null;

            #region Local functions

            NodeBase GenerateFunctionCallExpression(
                string possibleFunctionCallExpression)
            {
                Match match = this.functionRegex.Match(possibleFunctionCallExpression);

                try
                {
                    if (match.Success)
                    {
                        var functionName = match.Groups["functionName"]
                            .Value;
                        var expressionValue = match.Groups["expression"]
                            .Value;

                        string[] parameterExpressions;

                        if (string.IsNullOrWhiteSpace(expressionValue))
                        {
                            parameterExpressions = Array.Empty<string>();
                        }
                        else
                        {
                            parameterExpressions = match.Groups["expression"]
                                .Value.Split(
                                    new[]
                                    {
                                        this.definition.ParameterSeparator
                                    },
                                    StringSplitOptions.None)
                                .Select(p => string.IsNullOrWhiteSpace(p) ? null : p)
                                .ToArray();
                        }

                        NodeBase returnValue;
                        switch (parameterExpressions.Length)
                        {
                            case 0:
                                returnValue = this.nonaryFunctions.TryGetValue(
                                    functionName,
                                    out Type t)
                                    ? ((NonaryFunctionNodeBase)Activator.CreateInstance(
                                        t)).Simplify()
                                    : null;
                                break;

                            case 1:
                                if (this.unaryFunctions.TryGetValue(
                                    functionName,
                                    out Type t1))
                                {
                                    returnValue = ((UnaryFunctionNodeBase)Activator.CreateInstance(
                                        t1,
                                        this.GenerateExpression(parameterExpressions[0]))).Simplify();
                                }
                                else
                                {
                                    returnValue = null;
                                }

                                break;

                            case 2:
                                if (this.binaryFunctions.TryGetValue(
                                    functionName,
                                    out Type t2))
                                {
                                    returnValue = ((BinaryFunctionNodeBase)Activator.CreateInstance(
                                        t2,
                                        this.GenerateExpression(parameterExpressions[0]),
                                        this.GenerateExpression(parameterExpressions[1]))).Simplify();
                                }
                                else
                                {
                                    returnValue = null;
                                }

                                break;

                            case 3:
                                if (this.ternaryFunctions.TryGetValue(
                                    functionName,
                                    out Type t3))
                                {
                                    returnValue = ((TernaryFunctionNodeBase)Activator.CreateInstance(
                                        t3,
                                        this.GenerateExpression(parameterExpressions[0]),
                                        this.GenerateExpression(parameterExpressions[1]),
                                        this.GenerateExpression(parameterExpressions[2]))).Simplify();
                                }
                                else
                                {
                                    returnValue = null;
                                }

                                break;

                            default:
                                returnValue = null;
                                break;
                        }

                        return returnValue;
                    }
                }
                catch (ExpressionNotValidLogicallyException)
                {
                    return null;
                }
                catch (MathematicsEngineException)
                {
                    throw;
                }
                catch (Exception)
                {
                    return null;
                }

                return null;
            }

            #endregion
        }
    }
}