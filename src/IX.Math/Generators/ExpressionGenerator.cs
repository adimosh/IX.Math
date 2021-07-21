// <copyright file="ExpressionGenerator.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text.RegularExpressions;
using IX.Math.Computation;
using IX.Math.Computation.InitialExpressionParsers;
using IX.Math.ExpressionState;
using IX.Math.Extraction;
using IX.Math.Interpretation;
using IX.Math.Nodes;
using IX.Math.Nodes.Operations.Binary;
using IX.Math.Nodes.Operations.Unary;
using IX.Math.Registration;
using IX.StandardExtensions.Globalization;

namespace IX.Math.Generators
{
    internal static class ExpressionGenerator
    {
        [SuppressMessage(
            "Performance",
            "HAA0401:Possible allocation of reference type enumerator",
            Justification = "Not avoidable for now.")]
        [SuppressMessage(
            "CodeSmell",
            "ERP022:Unobserved exception in generic exception handler",
            Justification = "We want this to happen.")]
        internal static ComputationBody CreateBody()
        {
            var context = InterpretationContext.Current;

            var cancellationToken = context.CancellationToken;
            if (cancellationToken.IsCancellationRequested)
            {
                return ComputationBody.Empty;
            }

            // Start preparing expression
            context.SymbolTable.Add(
                string.Empty,
                ExpressionSymbol.GenerateSymbol(
                    string.Empty,
                    context.OriginalExpression));

            // Prepares expression and takes care of operators to ensure that they are all OK and usable
            context.ProcessOperators();

            if (cancellationToken.IsCancellationRequested)
            {
                return ComputationBody.Empty;
            }

            // Break expression based on function calls
            FunctionsExtractor.ReplaceFunctions(context.SymbolTable[string.Empty].Expression);

            if (cancellationToken.IsCancellationRequested)
            {
                return ComputationBody.Empty;
            }

            // We save a split expression for determining parameter order
            var splitExpression = context.SymbolTable[string.Empty].Expression.Split(
                context.AllSymbols.ToArray(),
                StringSplitOptions.RemoveEmptyEntries);

            // Break by parentheses
            ParenthesesParser.FormatParentheses();

            if (cancellationToken.IsCancellationRequested)
            {
                return ComputationBody.Empty;
            }

            // Populating symbol tables
            foreach (var p in context.SymbolTable.Where(p => !p.Value.IsFunctionCall)
                .Select(p => p.Value.Expression))
            {
                TablePopulationGenerator.PopulateTables(
                    p,
                    context.SymbolTable[string.Empty].Expression);
            }

            // For each parameter from the table we've just populated, see where it's first used, and fill in that index as the order
            foreach (ParameterContext paramForOrdering in context.ParameterRegistry.Dump())
            {
                paramForOrdering.Order = Array.IndexOf(
                    splitExpression,
                    paramForOrdering.Name);
            }

            if (cancellationToken.IsCancellationRequested)
            {
                return ComputationBody.Empty;
            }

            // Generate expressions
            NodeBase? body;
            try
            {
                body = GenerateExpression(context.SymbolTable[string.Empty].Expression);
            }
            catch
            {
                body = null;
            }

            if (body == null || cancellationToken.IsCancellationRequested)
            {
                return ComputationBody.Empty;
            }

            if (body is ConstantNodeBase && context.ParameterRegistry.Populated)
            {
                return ComputationBody.Empty;
            }

            return new ComputationBody(
                body,
                context.ParameterRegistry);
        }

        [SuppressMessage(
            "CodeSmell",
            "ERP022:Unobserved exception in generic exception handler",
            Justification = "We want that.")]
        private static NodeBase? GenerateExpression(
             string expression)
        {
            var context = InterpretationContext.Current;

            var cancellationToken = context.CancellationToken;
            if (cancellationToken.IsCancellationRequested)
            {
                // Cancellation requested, let's exit gracefully
                return null;
            }

            // Expression might be an already-defined constant
            if (context.ConstantsTable.TryGetValue(
                expression,
                out ConstantNodeBase c1))
            {
                return c1;
            }

            if (context.ReverseConstantsTable.TryGetValue(
                expression,
                out var c2))
            {
                if (context.ConstantsTable.TryGetValue(
                    c2,
                    out ConstantNodeBase c3))
                {
                    return c3;
                }
            }

            // Check whether expression is an external parameter
            if (context.ParameterRegistry.Exists(expression))
            {
                return new ParameterNode(
                    expression,
                    context.ParameterRegistry);
            }

            // Check whether the expression already exists in the symbols table
            if (context.SymbolTable.TryGetValue(
                expression,
                out ExpressionSymbol e1))
            {
                return GenerateExpression(
                    e1.Expression);
            }

            if (context.ReverseSymbolTable.TryGetValue(
                expression,
                out var e2))
            {
                if (context.SymbolTable.TryGetValue(
                    e2,
                    out ExpressionSymbol e3))
                {
                    if (e3.Expression != expression)
                    {
                        return GenerateExpression(
                            e3.Expression);
                    }
                }
            }

            // Check whether the expression is a function call
            if (expression.InvariantCultureContains(context.Definition.Parentheses.Left) &&
                expression.InvariantCultureContains(context.Definition.Parentheses.Right))
            {
                return GenerateFunctionCallExpression(
                    expression);
            }

            // Check whether the expression is a binary operator
            foreach (var (_, operatorPosition, @operator) in OperatorSequenceGenerator
                .GetOperatorsInOrderInExpression(
                    expression,
                    context.BinaryOperators.KeysByLevel).OrderBy(p => p.Item1).ThenByDescending(p => p.Item2).ToArray())
            {
                NodeBase? exp = ExpressionByBinaryOperator(
                    expression,
                    operatorPosition,
                    @operator);

                NodeBase? ExpressionByBinaryOperator(
                    string s,
                    int position,
                    string op)
                {
                    if (position == 0)
                    {
                        // We certainly have an unary operator if the operator is at the beginning of the expression. We therefore cannot continue with binary
                        return null;
                    }

                    if (InterpretationContext.Current.CancellationToken.IsCancellationRequested)
                    {
                        // Cancellation requested, let's exit gracefully
                        return null;
                    }

                    if (!context.BinaryOperators.TryGetValue(
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

                    NodeBase? left = GenerateExpression(
                        eee);
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

                    NodeBase? right = GenerateExpression(
                        eee);
                    if (right == null)
                    {
                        // Right expression is invalid.
                        return null;
                    }

                    var resultingNode = t(
                        left,
                        right);
                    return resultingNode.Simplify();
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
                    context.UnaryOperators.KeysByLevel).OrderBy(p => p.Item1).ThenByDescending(p => p.Item2).ToArray())
            {
                NodeBase? exp = ExpressionByUnaryOperator(
                    expression,
                    operatorPosition.Item3);

                NodeBase? ExpressionByUnaryOperator(
                    string s,
                    string op)
                {
                    if (InterpretationContext.Current.CancellationToken.IsCancellationRequested)
                    {
                        // Cancellation requested, let's stop
                        return null;
                    }

                    if (!s.CurrentCultureStartsWith(op) || !context.UnaryOperators.TryGetValue(
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
                    NodeBase? expr = GenerateExpression(
                        eee);
                    if (expr == null)
                    {
                        // The operand expression was not valid, or cancellation was requested.
                        return null;
                    }

                    var resultingNode = t(expr);
                    return resultingNode.Simplify();
                }

                if (exp != null)
                {
                    // We have found a valid unary operator expression.
                    return exp;
                }
            }

            return null;

            static NodeBase? GenerateFunctionCallExpression(
                string possibleFunctionCallExpression)
            {
                var context = InterpretationContext.Current;

                if (context.CancellationToken.IsCancellationRequested)
                {
                    // Cancellation requested, let's exit gracefully
                    return null;
                }

                Match match = context.FunctionRegex.Match(possibleFunctionCallExpression);

                try
                {
                    if (match.Success)
                    {
                        var functionName = match.Groups["functionName"].Value;
                        var expressionValue = match.Groups["expression"].Value;

                        string?[] parameterExpressions;

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
                                        context.Definition.ParameterSeparator
                                    },
                                    StringSplitOptions.None)
                                .Select(p => string.IsNullOrWhiteSpace(p) ? null : p)
                                .ToArray();
                        }

                        var plugins = PluginCollection.Current;
                        NodeBase? returnValue;
                        switch (parameterExpressions.Length)
                        {
                            case 0:
                                returnValue = plugins.TryFindNonary(
                                    functionName,
                                    out Type t) ? ((NonaryFunctionNodeBase)Activator.CreateInstance(t)).Simplify() : null;
                                break;

                            case 1:
                                if (plugins.TryFindUnary(
                                    functionName,
                                    out Type t1))
                                {
                                    returnValue = ((UnaryFunctionNodeBase)Activator.CreateInstance(
                                        t1,
                                        GenerateExpression(
                                            parameterExpressions[0] ?? throw new ExpressionNotValidLogicallyException()))).Simplify();
                                }
                                else
                                {
                                    returnValue = null;
                                }

                                break;

                            case 2:
                                if (plugins.TryFindBinary(
                                    functionName,
                                    out Type t2))
                                {
                                    returnValue = ((BinaryFunctionNodeBase)Activator.CreateInstance(
                                        t2,
                                        GenerateExpression(
                                            parameterExpressions[0] ?? throw new ExpressionNotValidLogicallyException()),
                                        GenerateExpression(
                                            parameterExpressions[1] ?? throw new ExpressionNotValidLogicallyException()))).Simplify();
                                }
                                else
                                {
                                    returnValue = null;
                                }

                                break;

                            case 3:
                                if (plugins.TryFindTernary(
                                    functionName,
                                    out Type t3))
                                {
                                    returnValue = ((TernaryFunctionNodeBase)Activator.CreateInstance(
                                        t3,
                                        GenerateExpression(
                                            parameterExpressions[0] ?? throw new ExpressionNotValidLogicallyException()),
                                        GenerateExpression(
                                            parameterExpressions[1] ?? throw new ExpressionNotValidLogicallyException()),
                                        GenerateExpression(
                                            parameterExpressions[2] ?? throw new ExpressionNotValidLogicallyException()))).Simplify();
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
                catch (Exception)
                {
                    return null;
                }

                return null;
            }
        }
    }
}