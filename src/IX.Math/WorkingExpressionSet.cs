// <copyright file="WorkingExpressionSet.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using IX.Math.ExpressionState;
using IX.Math.Extensibility;
using IX.Math.Generators;
using IX.Math.Nodes;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Operators.Binary;
using IX.Math.Nodes.Operators.Binary.Bitwise;
using IX.Math.Nodes.Operators.Binary.Comparison;
using IX.Math.Nodes.Operators.Binary.Logical;
using IX.Math.Nodes.Operators.Binary.Mathematic;
using IX.Math.Nodes.Operators.Unary;
using IX.Math.Nodes.Parameters;
using IX.StandardExtensions.ComponentModel;
using IX.StandardExtensions.Extensions;
using IX.System.Collections.Generic;
using DiagCA = System.Diagnostics.CodeAnalysis;
using SubtractNode = IX.Math.Nodes.Operators.Binary.Mathematic.SubtractNode;

namespace IX.Math
{
    [DiagCA.SuppressMessage(
        "IDisposableAnalyzers.Correctness",
        "IDISP008:Don't assign member with injected and created disposables.",
        Justification = "It is OK, but the analyzer can't tell.")]
    internal class WorkingExpressionSet : DisposableBase
    {
        // Operators
        [DiagCA.SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP002:Dispose member.",
            Justification = "This is correct, but the analyzer can't tell.")]
        [DiagCA.SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP006:Implement IDisposable.",
            Justification = "This is correct, but the analyzer can't tell.")]
        private LevelDictionary<string, Func<List<IStringFormatter>, NodeBase, NodeBase, BinaryOperatorNodeBase>> binaryOperators;

        [DiagCA.SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP002:Dispose member.",
            Justification = "This is correct, but the analyzer can't tell.")]
        [DiagCA.SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP006:Implement IDisposable.",
            Justification = "This is correct, but the analyzer can't tell.")]
        private LevelDictionary<string, Func<List<IStringFormatter>, NodeBase, UnaryOperatorNodeBase>> unaryOperators;

        // Constants
        private Dictionary<string, ConstantNodeBase> constantsTable;
        private Dictionary<string, string> reverseConstantsTable;

        // Symbols
        private Dictionary<string, ExpressionSymbol> symbolTable;
        private Dictionary<string, string> reverseSymbolTable;

        private bool initialized;

        internal WorkingExpressionSet(
            string expression,
            MathDefinition mathDefinition,
            Dictionary<string, Type> nonaryFunctions,
            Dictionary<string, Type> unaryFunctions,
            Dictionary<string, Type> binaryFunctions,
            Dictionary<string, Type> ternaryFunctions,
            LevelDictionary<Type, IConstantsExtractor> extractors,
            LevelDictionary<Type, IConstantInterpreter> interpreters,
            List<IStringFormatter> stringFormatters,
            CancellationToken cancellationToken)
        {
            this.ParameterRegistry = new Dictionary<string, ExternalParameterNode>();
            this.ConstantsTable = new Dictionary<string, ConstantNodeBase>();
            this.ReverseConstantsTable = new Dictionary<string, string>();
            this.SymbolTable = new Dictionary<string, ExpressionSymbol>();
            this.ReverseSymbolTable = new Dictionary<string, string>();
            this.StringFormatters = stringFormatters;

            this.CancellationToken = cancellationToken;
            this.Expression = expression;
            this.Definition = mathDefinition;

            this.AllOperatorsInOrder = new[]
            {
                mathDefinition.GreaterThanOrEqualSymbol,
                mathDefinition.LessThanOrEqualSymbol,
                mathDefinition.GreaterThanSymbol,
                mathDefinition.LessThanSymbol,
                mathDefinition.NotEqualsSymbol,
                mathDefinition.EqualsSymbol,
                mathDefinition.XorSymbol,
                mathDefinition.OrSymbol,
                mathDefinition.AndSymbol,
                mathDefinition.AddSymbol,
                mathDefinition.SubtractSymbol,
                mathDefinition.DivideSymbol,
                mathDefinition.ModuloSymbol,
                mathDefinition.MultiplySymbol,
                mathDefinition.PowerSymbol,
                mathDefinition.LeftShiftSymbol,
                mathDefinition.RightShiftSymbol,
                mathDefinition.NotSymbol
            };

            this.NonaryFunctions = nonaryFunctions;
            this.UnaryFunctions = unaryFunctions;
            this.BinaryFunctions = binaryFunctions;
            this.TernaryFunctions = ternaryFunctions;

            this.Extractors = extractors;
            this.Interpreters = interpreters;

            this.FunctionRegex = new Regex(
                $@"(?'functionName'.*?){Regex.Escape(mathDefinition.Parentheses.Item1)}(?'expression'.*?){Regex.Escape(mathDefinition.Parentheses.Item2)}");
        }

        /// <summary>
        ///     Gets all operators in order.
        /// </summary>
        /// <value>
        ///     All operators in order.
        /// </value>
        internal string[] AllOperatorsInOrder { get; }

        /// <summary>
        ///     Gets the cancellation token.
        /// </summary>
        /// <value>
        ///     The cancellation token.
        /// </value>
        internal CancellationToken CancellationToken { get; }

        /// <summary>
        ///     Gets the definition.
        /// </summary>
        /// <value>
        ///     The definition.
        /// </value>
        internal MathDefinition Definition { get; }

        /// <summary>
        ///     Gets the constant extractors.
        /// </summary>
        /// <value>
        ///     The constant extractors.
        /// </value>
        internal LevelDictionary<Type, IConstantsExtractor> Extractors { get; }

        /// <summary>
        ///     Gets the constant interpreters.
        /// </summary>
        /// <value>
        ///     The constant interpreters.
        /// </value>
        internal LevelDictionary<Type, IConstantInterpreter> Interpreters { get; }

        /// <summary>
        ///     Gets the function regex.
        /// </summary>
        /// <value>
        ///     The function regex.
        /// </value>
        internal Regex FunctionRegex { get; }

        /// <summary>
        ///     Gets the parameter registry.
        /// </summary>
        /// <value>
        ///     The parameter registry.
        /// </value>
        internal IDictionary<string, ExternalParameterNode> ParameterRegistry { get; }

        /// <summary>
        ///     Gets the nonary functions.
        /// </summary>
        /// <value>
        ///     The nonary functions.
        /// </value>
        internal Dictionary<string, Type> NonaryFunctions { get; }

        /// <summary>
        ///     Gets the ternary functions.
        /// </summary>
        /// <value>
        ///     The ternary functions.
        /// </value>
        internal Dictionary<string, Type> TernaryFunctions { get; }

        /// <summary>
        ///     Gets the unary functions.
        /// </summary>
        /// <value>
        ///     The unary functions.
        /// </value>
        internal Dictionary<string, Type> UnaryFunctions { get; }

        /// <summary>
        ///     Gets the binary functions.
        /// </summary>
        /// <value>
        ///     The binary functions.
        /// </value>
        internal Dictionary<string, Type> BinaryFunctions { get; }

        /// <summary>
        /// Gets the string formatters.
        /// </summary>
        /// <value>
        /// The string formatters.
        /// </value>
        internal List<IStringFormatter> StringFormatters { get; }

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
        [DiagCA.SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP002:Dispose member.",
            Justification = "This is correct, but the analyzer can't tell.")]
        [DiagCA.SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP006:Implement IDisposable.",
            Justification = "This is correct, but the analyzer can't tell.")]
        internal LevelDictionary<string, Func<List<IStringFormatter>, NodeBase, NodeBase, BinaryOperatorNodeBase>> BinaryOperators
        {
            get => this.binaryOperators;
            private set => this.binaryOperators = value;
        }

        /// <summary>
        ///     Gets the constants table.
        /// </summary>
        /// <value>
        ///     The constants table.
        /// </value>
        internal Dictionary<string, ConstantNodeBase> ConstantsTable
        {
            get => this.constantsTable;
            private set => this.constantsTable = value;
        }

        /// <summary>
        ///     Gets or sets the expression.
        /// </summary>
        /// <value>
        ///     The expression.
        /// </value>
        internal string Expression { get; set; }

        /// <summary>
        ///     Gets the reverse constants table.
        /// </summary>
        /// <value>
        ///     The reverse constants table.
        /// </value>
        internal Dictionary<string, string> ReverseConstantsTable
        {
            get => this.reverseConstantsTable;
            private set => this.reverseConstantsTable = value;
        }

        /// <summary>
        ///     Gets the reverse symbol table.
        /// </summary>
        /// <value>
        ///     The reverse symbol table.
        /// </value>
        internal Dictionary<string, string> ReverseSymbolTable
        {
            get => this.reverseSymbolTable;
            private set => this.reverseSymbolTable = value;
        }

        /// <summary>
        ///     Gets or sets a value indicating whether this <see cref="WorkingExpressionSet" /> is success.
        /// </summary>
        /// <value>
        ///     <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        internal bool Success { get; set; }

        /// <summary>
        ///     Gets the symbol table.
        /// </summary>
        /// <value>
        ///     The symbol table.
        /// </value>
        internal Dictionary<string, ExpressionSymbol> SymbolTable
        {
            get => this.symbolTable;
            private set => this.symbolTable = value;
        }

        /// <summary>
        ///     Gets the unary operators.
        /// </summary>
        /// <value>
        ///     The unary operators.
        /// </value>
        [DiagCA.SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP002:Dispose member.",
            Justification = "This is correct, but the analyzer can't tell.")]
        [DiagCA.SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP006:Implement IDisposable.",
            Justification = "This is correct, but the analyzer can't tell.")]
        internal LevelDictionary<string, Func<List<IStringFormatter>, NodeBase, UnaryOperatorNodeBase>> UnaryOperators
        {
            get => this.unaryOperators;
            private set => this.unaryOperators = value;
        }

        /// <summary>
        ///     Initializes this instance. This method shuld be called after initialization and extraction of major constants.
        /// </summary>
        [DiagCA.SuppressMessage(
            "Performance",
            "HAA0401:Possible allocation of reference type enumerator",
            Justification = "Too much LINQ.")]
        [DiagCA.SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP003:Dispose previous before re-assigning.",
            Justification = "It is, but the analyzer cannot tell.")]
        [DiagCA.SuppressMessage(
            "Performance",
            "HAA0603:Delegate allocation from a method group",
            Justification = "We actively want this to keep a reference to this working set.")]
        [DiagCA.SuppressMessage(
            "Performance",
            "HAA0301:Closure Allocation Source",
            Justification = "We actively want this to keep a reference to this working set.")]
        [DiagCA.SuppressMessage(
            "Performance",
            "HAA0302:Display class allocation to capture closure",
            Justification = "We actively want this to keep a reference to this working set.")]
        internal void Initialize()
        {
            if (this.initialized)
            {
                return;
            }

            this.initialized = true;

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

                this.Expression = this.Expression.Replace(
                    op,
                    s);

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

                if (definition.ModuloSymbol == op)
                {
                    definition.ModuloSymbol = s;
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
            this.BinaryOperators = new LevelDictionary<string, Func<List<IStringFormatter>, NodeBase, NodeBase, BinaryOperatorNodeBase>>
            {
                // First tier - Comparison and equation operators
                {
                    definition.GreaterThanOrEqualSymbol, (
                        definitionL1,
                        leftOperand,
                        rightOperand) => new GreaterThanOrEqualOperatorNode(
                        definitionL1,
                        leftOperand,
                        rightOperand),
                    10
                },
                {
                    definition.LessThanOrEqualSymbol, (
                        definitionL1,
                        leftOperand,
                        rightOperand) => new LessThanOrEqualOperatorNode(
                        definitionL1,
                        leftOperand,
                        rightOperand),
                    10
                },
                {
                    definition.GreaterThanSymbol, (
                        definitionL1,
                        leftOperand,
                        rightOperand) => new GreaterThanOperatorNode(
                        definitionL1,
                        leftOperand,
                        rightOperand),
                    10
                },
                {
                    definition.LessThanSymbol, (
                        definitionL1,
                        leftOperand,
                        rightOperand) => new LessThanOperatorNode(
                        definitionL1,
                        leftOperand,
                        rightOperand),
                    10
                },
                {
                    definition.NotEqualsSymbol, (
                        definitionL1,
                        leftOperand,
                        rightOperand) => new NotEqualsNode(
                        definitionL1,
                        leftOperand,
                        rightOperand),
                    10
                },
                {
                    definition.EqualsSymbol, (
                        definitionL1,
                        leftOperand,
                        rightOperand) => new EqualsNode(
                        definitionL1,
                        leftOperand,
                        rightOperand),
                    10
                },

                // Second tier - Logical operators
                {
                    definition.OrSymbol, (
                        definitionL1,
                        leftOperand,
                        rightOperand) => new OrNode(
                        definitionL1,
                        leftOperand,
                        rightOperand),
                    20
                },
                {
                    definition.XorSymbol, (
                        definitionL1,
                        leftOperand,
                        rightOperand) => new XorNode(
                        definitionL1,
                        leftOperand,
                        rightOperand),
                    definition.OperatorPrecedenceStyle == OperatorPrecedenceStyle.CStyle ? 21 : 20
                },
                {
                    definition.AndSymbol, (
                        definitionL1,
                        leftOperand,
                        rightOperand) => new AndNode(
                        definitionL1,
                        leftOperand,
                        rightOperand),
                    definition.OperatorPrecedenceStyle == OperatorPrecedenceStyle.CStyle ? 22 : 20
                },

                // Third tier - Arithmetic second-rank operators
                {
                    definition.AddSymbol, (
                        definitionL1,
                        leftOperand,
                        rightOperand) => new AddNode(
                        definitionL1,
                        leftOperand,
                        rightOperand),
                    30
                },
                {
                    definition.SubtractSymbol, (
                        definitionL1,
                        leftOperand,
                        rightOperand) => new SubtractNode(
                        definitionL1,
                        leftOperand,
                        rightOperand),
                    30
                },

                // Fourth tier - Arithmetic first-rank operators
                {
                    definition.DivideSymbol, (
                        definitionL1,
                        leftOperand,
                        rightOperand) => new DivideNode(
                        definitionL1,
                        leftOperand,
                        rightOperand),
                    40
                },
                {
                    definition.ModuloSymbol, (
                        definitionL1,
                        leftOperand,
                        rightOperand) => new ModuloNode(
                        definitionL1,
                        leftOperand,
                        rightOperand),
                    40
                },
                {
                    definition.MultiplySymbol, (
                        definitionL1,
                        leftOperand,
                        rightOperand) => new MultiplyNode(
                        definitionL1,
                        leftOperand,
                        rightOperand),
                    40
                },

                // Fifth tier - Power operator
                {
                    definition.PowerSymbol, (
                        definitionL1,
                        leftOperand,
                        rightOperand) => new PowerNode(
                        definitionL1,
                        leftOperand,
                        rightOperand),
                    50
                },

                // Sixth tier - Bitwise shift operators
                {
                    definition.LeftShiftSymbol, (
                        definitionL1,
                        leftOperand,
                        rightOperand) => new LeftShiftNode(
                        definitionL1,
                        leftOperand,
                        rightOperand),
                    60
                },
                {
                    definition.RightShiftSymbol, (
                        definitionL1,
                        leftOperand,
                        rightOperand) => new RightShiftNode(
                        definitionL1,
                        leftOperand,
                        rightOperand),
                    60
                }
            };

            // Unary operators
            this.UnaryOperators = new LevelDictionary<string, Func<List<IStringFormatter>, NodeBase, UnaryOperatorNodeBase>>
            {
                // First tier - Negation and inversion
                {
                    definition.SubtractSymbol, (
                        definitionL1,
                        operand) => new Nodes.Operators.Unary.SubtractNode(
                        definitionL1,
                        operand),
                    1
                },
                {
                    definition.NotSymbol, (
                        definitionL1,
                        operand) => new NotNode(
                        definitionL1,
                        operand),
                    1
                }
            };

            // All symbols
            this.AllSymbols = allOperatorsInOrder.Union(
                    new[]
                    {
                        definition.ParameterSeparator,
                        definition.Parentheses.Item1,
                        definition.Parentheses.Item2
                    })
                .ToArray();

            // Special symbols

            // Euler-Napier constant (e)
            ConstantsGenerator.GenerateNamedNumericSymbol(
                this.ConstantsTable,
                this.ReverseConstantsTable,
                "e",
                global::System.Math.E,
                this.StringFormatters);

            // Archimedes-Ludolph constant (pi)
            ConstantsGenerator.GenerateNamedNumericSymbol(
                this.ConstantsTable,
                this.ReverseConstantsTable,
                "π",
                global::System.Math.PI,
                this.StringFormatters,
                $"{definition.SpecialSymbolIndicators.Item1}pi{definition.SpecialSymbolIndicators.Item2}");

            // Golden ratio
            ConstantsGenerator.GenerateNamedNumericSymbol(
                this.ConstantsTable,
                this.ReverseConstantsTable,
                "φ",
                1.6180339887498948,
                this.StringFormatters,
                $"{definition.SpecialSymbolIndicators.Item1}phi{definition.SpecialSymbolIndicators.Item2}");

            // Bernstein constant
            ConstantsGenerator.GenerateNamedNumericSymbol(
                this.ConstantsTable,
                this.ReverseConstantsTable,
                "β",
                0.2801694990238691,
                this.StringFormatters,
                $"{definition.SpecialSymbolIndicators.Item1}beta{definition.SpecialSymbolIndicators.Item2}");

            // Euler-Mascheroni constant
            ConstantsGenerator.GenerateNamedNumericSymbol(
                this.ConstantsTable,
                this.ReverseConstantsTable,
                "γ",
                0.5772156649015328,
                this.StringFormatters,
                $"{definition.SpecialSymbolIndicators.Item1}gamma{definition.SpecialSymbolIndicators.Item2}");

            // Gauss-Kuzmin-Wirsing constant
            ConstantsGenerator.GenerateNamedNumericSymbol(
                this.ConstantsTable,
                this.ReverseConstantsTable,
                "λ",
                0.3036630028987326,
                this.StringFormatters,
                $"{definition.SpecialSymbolIndicators.Item1}lambda{definition.SpecialSymbolIndicators.Item2}");
        }

        /// <summary>
        /// Disposes in the managed context.
        /// </summary>
        [DiagCA.SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP003:Dispose previous before re-assigning.",
            Justification = "It is, but the analyzer can't tell.")]
        protected override void DisposeManagedContext()
        {
            base.DisposeManagedContext();

            Interlocked.Exchange(
                    ref this.constantsTable,
                    null)
                .Clear();
            Interlocked.Exchange(
                    ref this.reverseConstantsTable,
                    null)
                .Clear();
            Interlocked.Exchange(
                    ref this.symbolTable,
                    null)
                ?.Clear();
            Interlocked.Exchange(
                    ref this.reverseSymbolTable,
                    null)
                ?.Clear();
            Interlocked.Exchange(
                    ref this.unaryOperators,
                    null)
                ?.Dispose();
            Interlocked.Exchange(
                    ref this.binaryOperators,
                    null)
                ?.Dispose();
        }
    }
}