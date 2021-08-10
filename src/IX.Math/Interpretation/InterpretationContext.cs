// <copyright file="InterpretationContext.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using IX.Abstractions.Logging;
using IX.Math.ExpressionState;
using IX.Math.Formatters;
using IX.Math.Nodes;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Operations.Binary;
using IX.Math.Nodes.Operators.Unary;
using IX.Math.Registration;
using IX.StandardExtensions.Contracts;
using IX.StandardExtensions.Extensions;
using IX.StandardExtensions.Globalization;
using IX.System.Collections.Generic;
using SubtractNode = IX.Math.Nodes.Operations.Binary.SubtractNode;

namespace IX.Math.Interpretation
{
    internal record InterpretationContext
    {
        private static readonly AsyncLocal<InterpretationContext?> current = new();

        // Constants
        private readonly Dictionary<string, ConstantNodeBase> constantsTable = new();
        private readonly Dictionary<string, string> reverseConstantsTable = new();

        // Symbols
        private readonly Dictionary<string, ExpressionSymbol> symbolTable = new();
        private readonly Dictionary<string, string> reverseSymbolTable = new();

        // Parameters
        private readonly StandardParameterRegistry parameterRegistry = new();

        // Operators
        private readonly LevelDictionary<string, Func<NodeBase, NodeBase, BinaryOperatorNodeBase>> binaryOperators = new();
        private readonly LevelDictionary<string, Func<NodeBase, UnaryOperatorNodeBase>> unaryOperators = new();

        private InterpretationContext(
            MathDefinition definition,
            string originalExpression,
            CancellationToken cancellationToken)
        {
            this.Definition = Requires.NotNull(definition, nameof(definition));
            this.OriginalExpression = Requires.NotNull(originalExpression, nameof(originalExpression));

            this.CancellationToken = cancellationToken;
            this.FunctionRegex = new Regex(
                $@"(?'functionName'.*?){Regex.Escape(definition.Parentheses.Left)}(?'expression'.*?){Regex.Escape(definition.Parentheses.Right)}");

            #region Operators

            // Put all operators in order
            this.AllOperatorsInOrder = new[]
            {
                definition.GreaterThanOrEqualSymbol,
                definition.LessThanOrEqualSymbol,
                definition.GreaterThanSymbol,
                definition.LessThanSymbol,
                definition.NotEqualsSymbol,
                definition.EqualsSymbol,
                definition.XorSymbol,
                definition.OrSymbol,
                definition.AndSymbol,
                definition.AddSymbol,
                definition.SubtractSymbol,
                definition.DivideSymbol,
                definition.MultiplySymbol,
                definition.PowerSymbol,
                definition.LeftShiftSymbol,
                definition.RightShiftSymbol,
                definition.NotSymbol
            };

            #endregion

            #region Numerological constants

            // Euler-Napier constant (e)
            this.GenerateNamedNumericSymbol(
                "e",
                global::System.Math.E);

            // Archimedes-Ludolph constant (pi)
            this.GenerateNamedNumericSymbol(
                "π",
                global::System.Math.PI,
                $"{definition.SpecialSymbolIndicators.Begin}pi{definition.SpecialSymbolIndicators.End}");

            // Golden ratio
            this.GenerateNamedNumericSymbol(
                "φ",
                1.6180339887498948,
                $"{definition.SpecialSymbolIndicators.Begin}phi{definition.SpecialSymbolIndicators.End}");

            // Bernstein constant
            this.GenerateNamedNumericSymbol(
                "β",
                0.2801694990238691,
                $"{definition.SpecialSymbolIndicators.Begin}beta{definition.SpecialSymbolIndicators.End}");

            // Euler-Mascheroni constant
            this.GenerateNamedNumericSymbol(
                "γ",
                0.5772156649015328,
                $"{definition.SpecialSymbolIndicators.Begin}gamma{definition.SpecialSymbolIndicators.End}");

            // Gauss-Kuzmin-Wirsing constant
            this.GenerateNamedNumericSymbol(
                "λ",
                0.3036630028987326,
                $"{definition.SpecialSymbolIndicators.Begin}lambda{definition.SpecialSymbolIndicators.End}");

            #endregion
        }

        public static InterpretationContext Current =>
            current.Value ?? throw new InvalidOperationException("Interpretation has not started for this context.");

        public MathDefinition Definition { get; }

        /// <summary>
        ///     Gets all operators in order.
        /// </summary>
        /// <value>
        ///     All operators in order.
        /// </value>
        internal string[] AllOperatorsInOrder { get; }

        /// <summary>
        ///     Gets the parameter registry.
        /// </summary>
        /// <value>
        ///     The parameter registry.
        /// </value>
        internal IParameterRegistry ParameterRegistry => this.parameterRegistry;

        /// <summary>
        ///     Gets the constants table.
        /// </summary>
        /// <value>
        ///     The constants table.
        /// </value>
        internal Dictionary<string, ConstantNodeBase> ConstantsTable => this.constantsTable;

        /// <summary>
        ///     Gets the reverse constants table.
        /// </summary>
        /// <value>
        ///     The reverse constants table.
        /// </value>
        internal Dictionary<string, string> ReverseConstantsTable => this.reverseConstantsTable;

        /// <summary>
        ///     Gets the reverse symbol table.
        /// </summary>
        /// <value>
        ///     The reverse symbol table.
        /// </value>
        internal Dictionary<string, string> ReverseSymbolTable => this.reverseSymbolTable;

        /// <summary>
        ///     Gets the symbol table.
        /// </summary>
        /// <value>
        ///     The symbol table.
        /// </value>
        internal Dictionary<string, ExpressionSymbol> SymbolTable => this.symbolTable;

        /// <summary>
        ///     Gets the function regex.
        /// </summary>
        /// <value>
        ///     The function regex.
        /// </value>
        internal Regex FunctionRegex { get; }

        public string OriginalExpression { get; private set; }

        public CancellationToken CancellationToken { get; }

        /// <summary>
        ///     Gets all symbols.
        /// </summary>
        /// <value>
        ///     All symbols.
        /// </value>
        internal string[] AllSymbols { get; private set; }

        /// <summary>
        ///     Gets the binary operators.
        /// </summary>
        /// <value>
        ///     The binary operators.
        /// </value>
        internal LevelDictionary<string, Func<NodeBase, NodeBase, BinaryOperatorNodeBase>> BinaryOperators => this.binaryOperators;

        /// <summary>
        ///     Gets the unary operators.
        /// </summary>
        /// <value>
        ///     The unary operators.
        /// </value>
        internal LevelDictionary<string, Func<NodeBase, UnaryOperatorNodeBase>> UnaryOperators => this.unaryOperators;

        internal static void Start(
            MathDefinition definition,
            string originalExpression,
            CancellationToken cancellationToken)
        {
            var localContext = new InterpretationContext(
                definition.DeepClone(),
                originalExpression,
                cancellationToken);

            current.Value = localContext;

            localContext.OriginalExpression = PluginCollection.Current.ExtractConstants(localContext.OriginalExpression)
                .Trim()
                .Replace(
                    " ",
                    string.Empty);
        }

        internal static void Stop() => current.Value = default;

        /// <summary>
        /// Generates a string constant.
        /// </summary>
        /// <param name="originalExpression">The original expression.</param>
        /// <param name="stringIndicator">The string indicator.</param>
        /// <param name="content">The content.</param>
        /// <returns>
        /// The name of the new constant.
        /// </returns>
        public string GenerateStringConstant(
            string originalExpression,
            string stringIndicator,
            string content)
        {
            Requires.NotNullOrWhiteSpace(
                originalExpression,
                nameof(originalExpression));
            Requires.NotNullOrWhiteSpace(
                stringIndicator,
                nameof(stringIndicator));
            Requires.NotNullOrWhiteSpace(
                content,
                nameof(content));

            if (this.reverseConstantsTable.TryGetValue(
                content,
                out var key))
            {
                return key;
            }

            var stringIndicatorLength = stringIndicator.Length;

            var name = this.GenerateName(
                originalExpression);
            this.constantsTable.Add(
                name,
                new StringNode(
                    content.Substring(
                        stringIndicatorLength,
                        content.Length - stringIndicatorLength * 2)));
            this.reverseConstantsTable.Add(
                content,
                name);
            return name;
        }

        /// <summary>
        ///     Generates a numeric constant out of a string.
        /// </summary>
        /// <param name="originalExpression">The original expression.</param>
        /// <param name="content">The content.</param>
        /// <returns>The name of the new constant.</returns>
        public string? GenerateNumericConstant(
            string originalExpression,
            string content)
        {
            Requires.NotNullOrWhiteSpace(
                originalExpression,
                nameof(originalExpression));
            Requires.NotNullOrWhiteSpace(
                content,
                nameof(content));

            var reverseConstantsTable = this.reverseConstantsTable;
            if (reverseConstantsTable.TryGetValue(
                content,
                out var key))
            {
                return key;
            }

            if (!double.TryParse(
                content,
                out var result))
            {
                Log.Debug($"No numeric constant can be parsed from {content}.");
                return null;
            }

            var name = this.GenerateName(
                originalExpression);
            this.constantsTable.Add(
                name,
                new NumericNode(result));
            reverseConstantsTable.Add(
                content,
                name);
            return name;
        }

        /// <summary>
        ///     Generates a named numeric symbol.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="value">The value.</param>
        /// <param name="alternateNames">The alternate names.</param>
        public void GenerateNamedNumericSymbol(
            string name,
            double value,
            params string[] alternateNames)
        {
            Requires.NotNullOrWhiteSpace(
                name,
                nameof(name));

            if (this.reverseConstantsTable.TryGetValue(
                name,
                out _))
            {
                return;
            }

            this.constantsTable.Add(
                name,
                new NumericNode(value));
            this.reverseConstantsTable.Add(
                value.ToString(CultureInfo.CurrentCulture),
                name);

            foreach (var alternateName in alternateNames)
            {
                this.reverseConstantsTable.Add(
                    alternateName,
                    name);
            }
        }

        /// <summary>
        /// Checks the constant to see if there isn't one already, then tries to guess what type it is, finally adding it to
        /// the constants table if one suitable type is found.
        /// </summary>
        /// <param name="originalExpression">The original expression.</param>
        /// <param name="content">The content.</param>
        /// <returns>
        /// The name of the new constant, or <see langword="null" /> (<see langword="Nothing" /> in Visual Basic) if a
        /// suitable type is not found.
        /// </returns>
        public string? CheckAndAddConstant(
             string originalExpression,
             string content)
        {
            // No content
            if (string.IsNullOrWhiteSpace(content))
            {
                Log.Debug("No content for constant.");
                return null;
            }

            // Constant has already been evaluated, let's skip
            var reverseConstantsTable = this.reverseConstantsTable;
            if (reverseConstantsTable.TryGetValue(
                content,
                out var key))
            {
                return key;
            }

            ConstantNodeBase? node = PluginCollection.Current.InterpretExpression(content);

            // Standard formatters
            if (node == null)
            {
                if (ParsingFormatter.ParseNumeric(
                    content,
                    out object? n))
                {
                    node = new NumericNode(n);
                }
                else if (ParsingFormatter.ParseByteArray(
                    content,
                    out byte[]? ba))
                {
                    node = new ByteArrayNode(ba);
                }
                else if (bool.TryParse(
                    content,
                    out var b))
                {
                    node = new BoolNode(b);
                }
            }

            // Node not recognized
            if (node == null)
            {
                // Not a constant - this is an expected execution path.
                return null;
            }

            // Get the constant a new name
            string name = this.GenerateName(originalExpression);

            // Add constant data to tables
            this.constantsTable.Add(
                name,
                node);
            reverseConstantsTable.Add(
                content,
                name);

            // Return
            return name;
        }

        internal void ProcessOperators()
        {
            var i = 1;
            var allOperatorsInOrder = this.AllOperatorsInOrder;
            var definition = this.Definition;

            foreach (var op in allOperatorsInOrder.OrderByDescending(p => p.Length)
                .Where(
                    (
                        p,
                        allOperatorsInOrderL1) => allOperatorsInOrderL1.Any(
                        (
                                q,
                                pL2) => q.Length < pL2.Length && pL2.Contains(q),
                        p),
                    allOperatorsInOrder)
                .OrderByDescending(p => p.Length))
            {
                var s = $"@op{i}@";

                this.symbolTable[string.Empty] = ExpressionSymbol.GenerateSymbol(
                    string.Empty,
                    this.symbolTable[string.Empty]
                        .Expression?.Replace(
                            op,
                            s));

                var allIndex = Array.IndexOf(
                    allOperatorsInOrder,
                    op);
                if (allIndex != -1)
                {
                    allOperatorsInOrder[allIndex] = s;
                }

                if (definition.AddSymbol == op)
                {
                    definition.AddSymbol = s;
                }

                if (definition.AndSymbol == op)
                {
                    definition.AndSymbol = s;
                }

                if (definition.DivideSymbol == op)
                {
                    definition.DivideSymbol = s;
                }

                if (definition.NotEqualsSymbol == op)
                {
                    definition.NotEqualsSymbol = s;
                }

                if (definition.EqualsSymbol == op)
                {
                    definition.EqualsSymbol = s;
                }

                if (definition.GreaterThanOrEqualSymbol == op)
                {
                    definition.GreaterThanOrEqualSymbol = s;
                }

                if (definition.GreaterThanSymbol == op)
                {
                    definition.GreaterThanSymbol = s;
                }

                if (definition.LessThanOrEqualSymbol == op)
                {
                    definition.LessThanOrEqualSymbol = s;
                }

                if (definition.LessThanSymbol == op)
                {
                    definition.LessThanSymbol = s;
                }

                if (definition.MultiplySymbol == op)
                {
                    definition.MultiplySymbol = s;
                }

                if (definition.NotSymbol == op)
                {
                    definition.NotSymbol = s;
                }

                if (definition.OrSymbol == op)
                {
                    definition.OrSymbol = s;
                }

                if (definition.PowerSymbol == op)
                {
                    definition.PowerSymbol = s;
                }

                if (definition.LeftShiftSymbol == op)
                {
                    definition.LeftShiftSymbol = s;
                }

                if (definition.RightShiftSymbol == op)
                {
                    definition.RightShiftSymbol = s;
                }

                if (definition.SubtractSymbol == op)
                {
                    definition.SubtractSymbol = s;
                }

                if (definition.XorSymbol == op)
                {
                    definition.XorSymbol = s;
                }

                i++;
            }

            // Operator string interpretation support
            // ======================================

            // Binary operators

            // First tier - Comparison and equation operators
            this.binaryOperators.Add(
                definition.GreaterThanOrEqualSymbol,
                (
                    leftOperand,
                    rightOperand) => new GreaterThanOrEqualNode(
                    leftOperand,
                    rightOperand),
                10);
            this.binaryOperators.Add(
                definition.LessThanOrEqualSymbol,
                (
                    leftOperand,
                    rightOperand) => new LessThanOrEqualNode(
                    leftOperand,
                    rightOperand),
                10);
            this.binaryOperators.Add(
                definition.GreaterThanSymbol,
                (
                    leftOperand,
                    rightOperand) => new GreaterThanNode(
                    leftOperand,
                    rightOperand),
                10);
            this.binaryOperators.Add(
                definition.LessThanSymbol,
                (
                    leftOperand,
                    rightOperand) => new LessThanNode(
                    leftOperand,
                    rightOperand),
                10);
            this.binaryOperators.Add(
                definition.NotEqualsSymbol,
                (
                    leftOperand,
                    rightOperand) => new NotEqualsNode(
                    leftOperand,
                    rightOperand),
                10);
            this.binaryOperators.Add(
                definition.EqualsSymbol,
                (
                    leftOperand,
                    rightOperand) => new EqualsNode(
                    leftOperand,
                    rightOperand),
                10);

            // Second tier - Logical operators
            this.binaryOperators.Add(
                definition.OrSymbol,
                (
                    leftOperand,
                    rightOperand) => new OrNode(
                    leftOperand,
                    rightOperand),
                20);
            this.binaryOperators.Add(
                definition.XorSymbol,
                (
                    leftOperand,
                    rightOperand) => new XorNode(
                    leftOperand,
                    rightOperand),
                definition.OperatorPrecedenceStyle == OperatorPrecedenceStyle.CStyle ? 21 : 20);
            this.binaryOperators.Add(
                definition.AndSymbol,
                (
                    leftOperand,
                    rightOperand) => new AndNode(
                    leftOperand,
                    rightOperand),
                definition.OperatorPrecedenceStyle == OperatorPrecedenceStyle.CStyle ? 22 : 20);

            // Third tier - Arithmetic second-rank operators
            this.binaryOperators.Add(
                definition.AddSymbol,
                (
                    leftOperand,
                    rightOperand) => new AddNode(
                    leftOperand,
                    rightOperand),
                30);
            this.binaryOperators.Add(
                definition.SubtractSymbol,
                (
                    leftOperand,
                    rightOperand) => new SubtractNode(
                    leftOperand,
                    rightOperand),
                30);

            // Fourth tier - Arithmetic first-rank operators
            this.binaryOperators.Add(
                definition.DivideSymbol,
                (
                    leftOperand,
                    rightOperand) => new DivideNode(
                    leftOperand,
                    rightOperand),
                40);
            this.binaryOperators.Add(
                definition.MultiplySymbol,
                (
                    leftOperand,
                    rightOperand) => new MultiplyNode(
                    leftOperand,
                    rightOperand),
                40);

            // Fifth tier - Power operator
            this.binaryOperators.Add(
                definition.PowerSymbol,
                (
                    leftOperand,
                    rightOperand) => new PowerNode(
                    leftOperand,
                    rightOperand),
                50);

            // Sixth tier - Bitwise shift operators
            this.binaryOperators.Add(
                definition.LeftShiftSymbol,
                (
                    leftOperand,
                    rightOperand) => new LeftShiftNode(
                    leftOperand,
                    rightOperand),
                60);
            this.binaryOperators.Add(
                definition.RightShiftSymbol,
                (
                    leftOperand,
                    rightOperand) => new RightShiftNode(
                    leftOperand,
                    rightOperand),
                60);

            // Unary operators

            // First tier - Negation and inversion
            this.unaryOperators.Add(
                definition.SubtractSymbol,
                (operand) => new Nodes.Operators.Unary.SubtractOperator(operand),
                1);
            this.unaryOperators.Add(
                definition.NotSymbol,
                (operand) => new NotOperator(operand),
                1);

            // All symbols
            this.AllSymbols = allOperatorsInOrder.Union(
                    new[]
                    {
                        definition.ParameterSeparator,
                        definition.Parentheses.Left,
                        definition.Parentheses.Right
                    })
                .ToArray();
        }

        private string GenerateName(
            string originalExpression)
        {
            var index = int.Parse(
                this.constantsTable.Keys.Where(p => p.InvariantCultureStartsWith("Const") && p.Length > 5).LastOrDefault()?.Substring(5) ?? "0", CultureInfo.CurrentCulture);

            do
            {
                index++;
            }
            while (originalExpression.InvariantCultureContains($"Const{index.ToString(CultureInfo.InvariantCulture)}"));

            return $"Const{index.ToString(CultureInfo.InvariantCulture)}";
        }
    }
}