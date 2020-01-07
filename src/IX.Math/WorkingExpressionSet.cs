// <copyright file="WorkingExpressionSet.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using IX.Math.ExpressionState;
using IX.Math.Extraction;
using IX.Math.Generators;
using IX.Math.Nodes;
using IX.Math.Nodes.Operations.Binary;
using IX.Math.Nodes.Operations.Unary;
using IX.Math.Registration;
using IX.StandardExtensions.ComponentModel;
using IX.StandardExtensions.Extensions;
using IX.System.Collections.Generic;
using DiagCA = System.Diagnostics.CodeAnalysis;
using SubtractNode = IX.Math.Nodes.Operations.Binary.SubtractNode;

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
        private LevelDictionary<string, Func<MathDefinition, NodeBase, NodeBase, NodeBase>> binaryOperators;

        [DiagCA.SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP002:Dispose member.",
            Justification = "This is correct, but the analyzer can't tell.")]
        [DiagCA.SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP006:Implement IDisposable.",
            Justification = "This is correct, but the analyzer can't tell.")]
        private LevelDictionary<string, Func<MathDefinition, NodeBase, NodeBase>> unaryOperators;

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
            CancellationToken cancellationToken)
        {
            this.ParameterRegistry = new StandardParameterRegistry();
            this.ConstantsTable = new Dictionary<string, ConstantNodeBase>();
            this.ReverseConstantsTable = new Dictionary<string, string>();
            this.SymbolTable = new Dictionary<string, ExpressionSymbol>();
            this.ReverseSymbolTable = new Dictionary<string, string>();

            this.CancellationToken = cancellationToken;
            this.Expression = expression;
            this.Definition = mathDefinition;

            this.AllOperatorsInOrder = new[]
            {
                this.Definition.GreaterThanOrEqualSymbol,
                this.Definition.LessThanOrEqualSymbol,
                this.Definition.GreaterThanSymbol,
                this.Definition.LessThanSymbol,
                this.Definition.NotEqualsSymbol,
                this.Definition.EqualsSymbol,
                this.Definition.XorSymbol,
                this.Definition.OrSymbol,
                this.Definition.AndSymbol,
                this.Definition.AddSymbol,
                this.Definition.SubtractSymbol,
                this.Definition.DivideSymbol,
                this.Definition.MultiplySymbol,
                this.Definition.PowerSymbol,
                this.Definition.LeftShiftSymbol,
                this.Definition.RightShiftSymbol,
                this.Definition.NotSymbol
            };

            this.NonaryFunctions = nonaryFunctions;
            this.UnaryFunctions = unaryFunctions;
            this.BinaryFunctions = binaryFunctions;
            this.TernaryFunctions = ternaryFunctions;

            this.Extractors = extractors;

            this.FunctionRegex = new Regex(
                $@"(?'functionName'.*?){Regex.Escape(this.Definition.Parentheses.Item1)}(?'expression'.*?){Regex.Escape(this.Definition.Parentheses.Item2)}");
        }

        /// <summary>
        ///     Gets all operators in order.
        /// </summary>
        /// <value>
        ///     All operators in order.
        /// </value>
        internal string[] AllOperatorsInOrder { get; }

        /// <summary>
        ///     Gets the binary functions.
        /// </summary>
        /// <value>
        ///     The binary functions.
        /// </value>
        internal Dictionary<string, Type> BinaryFunctions { get; }

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
        ///     Gets the extractors.
        /// </summary>
        /// <value>
        ///     The extractors.
        /// </value>
        internal LevelDictionary<Type, IConstantsExtractor> Extractors { get; }

        /// <summary>
        ///     Gets the function regex.
        /// </summary>
        /// <value>
        ///     The function regex.
        /// </value>
        internal Regex FunctionRegex { get; }

        /// <summary>
        ///     Gets the nonary functions.
        /// </summary>
        /// <value>
        ///     The nonary functions.
        /// </value>
        internal Dictionary<string, Type> NonaryFunctions { get; }

        /// <summary>
        ///     Gets the parameter registry.
        /// </summary>
        /// <value>
        ///     The parameter registry.
        /// </value>
        internal IParameterRegistry ParameterRegistry { get; }

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
        internal LevelDictionary<string, Func<MathDefinition, NodeBase, NodeBase, NodeBase>> BinaryOperators
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
        internal LevelDictionary<string, Func<MathDefinition, NodeBase, NodeBase>> UnaryOperators
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
        internal void Initialize()
        {
            if (this.initialized)
            {
                return;
            }

            this.initialized = true;

            var i = 1;
            foreach (var op in this.AllOperatorsInOrder.OrderByDescending(p => p.Length)
                .Where(
                    (
                        p,
                        thisL1) => thisL1.AllOperatorsInOrder.Any(
                        (
                                q,
                                pL2) => q.Length < pL2.Length && pL2.Contains(q),
                        p),
                    this)
                .OrderByDescending(p => p.Length))
            {
                var s = $"@op{i.ToString()}@";

                this.Expression = this.Expression.Replace(
                    op,
                    s);

                var allIndex = Array.IndexOf(
                    this.AllOperatorsInOrder,
                    op);
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

                if (this.Definition.NotEqualsSymbol == op)
                {
                    this.Definition.NotEqualsSymbol = s;
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

                if (this.Definition.LeftShiftSymbol == op)
                {
                    this.Definition.LeftShiftSymbol = s;
                }

                if (this.Definition.RightShiftSymbol == op)
                {
                    this.Definition.RightShiftSymbol = s;
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

            // Operator string interpretation support
            // ======================================

            // Binary operators
            this.BinaryOperators = new LevelDictionary<string, Func<MathDefinition, NodeBase, NodeBase, NodeBase>>
            {
                // First tier - Comparison and equation operators
                {
                    this.Definition.GreaterThanOrEqualSymbol, (
                        definition,
                        leftOperand,
                        rightOperand) => new GreaterThanOrEqualNode(
                        leftOperand,
                        rightOperand),
                    10
                },
                {
                    this.Definition.LessThanOrEqualSymbol, (
                        definition,
                        leftOperand,
                        rightOperand) => new LessThanOrEqualNode(
                        leftOperand,
                        rightOperand),
                    10
                },
                {
                    this.Definition.GreaterThanSymbol, (
                        definition,
                        leftOperand,
                        rightOperand) => new GreaterThanNode(
                        leftOperand,
                        rightOperand),
                    10
                },
                {
                    this.Definition.LessThanSymbol, (
                        definition,
                        leftOperand,
                        rightOperand) => new LessThanNode(
                        leftOperand,
                        rightOperand),
                    10
                },
                {
                    this.Definition.NotEqualsSymbol, (
                        definition,
                        leftOperand,
                        rightOperand) => new NotEqualsNode(
                        leftOperand,
                        rightOperand),
                    10
                },
                {
                    this.Definition.EqualsSymbol, (
                        definition,
                        leftOperand,
                        rightOperand) => new EqualsNode(
                        leftOperand,
                        rightOperand),
                    10
                },

                // Second tier - Logical operators
                {
                    this.Definition.OrSymbol, (
                        definition,
                        leftOperand,
                        rightOperand) => new OrNode(
                        leftOperand,
                        rightOperand),
                    20
                },
                {
                    this.Definition.XorSymbol, (
                        definition,
                        leftOperand,
                        rightOperand) => new XorNode(
                        leftOperand,
                        rightOperand),
                    this.Definition.OperatorPrecedenceStyle == OperatorPrecedenceStyle.CStyle ? 21 : 20
                },
                {
                    this.Definition.AndSymbol, (
                        definition,
                        leftOperand,
                        rightOperand) => new AndNode(
                        leftOperand,
                        rightOperand),
                    this.Definition.OperatorPrecedenceStyle == OperatorPrecedenceStyle.CStyle ? 22 : 20
                },

                // Third tier - Arithmetic second-rank operators
                {
                    this.Definition.AddSymbol, (
                        definition,
                        leftOperand,
                        rightOperand) => new AddNode(
                        leftOperand,
                        rightOperand),
                    30
                },
                {
                    this.Definition.SubtractSymbol, (
                        definition,
                        leftOperand,
                        rightOperand) => new SubtractNode(
                        leftOperand,
                        rightOperand),
                    30
                },

                // Fourth tier - Arithmetic first-rank operators
                {
                    this.Definition.DivideSymbol, (
                        definition,
                        leftOperand,
                        rightOperand) => new DivideNode(
                        leftOperand,
                        rightOperand),
                    40
                },
                {
                    this.Definition.MultiplySymbol, (
                        definition,
                        leftOperand,
                        rightOperand) => new MultiplyNode(
                        leftOperand,
                        rightOperand),
                    40
                },

                // Fifth tier - Power operator
                {
                    this.Definition.PowerSymbol, (
                        definition,
                        leftOperand,
                        rightOperand) => new PowerNode(
                        leftOperand,
                        rightOperand),
                    50
                },

                // Sixth tier - Bitwise shift operators
                {
                    this.Definition.LeftShiftSymbol, (
                        definition,
                        leftOperand,
                        rightOperand) => new LeftShiftNode(
                        leftOperand,
                        rightOperand),
                    60
                },
                {
                    this.Definition.RightShiftSymbol, (
                        definition,
                        leftOperand,
                        rightOperand) => new RightShiftNode(
                        leftOperand,
                        rightOperand),
                    60
                }
            };

            // Unary operators
            this.UnaryOperators = new LevelDictionary<string, Func<MathDefinition, NodeBase, NodeBase>>
            {
                // First tier - Negation and inversion
                {
                    this.Definition.SubtractSymbol, (
                        definition,
                        operand) => new Nodes.Operations.Unary.SubtractNode(operand),
                    1
                },
                {
                    this.Definition.NotSymbol, (
                        definition,
                        operand) => new NotNode(operand),
                    1
                }
            };

            // All symbols
            this.AllSymbols = this.AllOperatorsInOrder.Union(
                    new[]
                    {
                        this.Definition.ParameterSeparator,
                        this.Definition.Parentheses.Item1,
                        this.Definition.Parentheses.Item2
                    })
                .ToArray();

            // Special symbols

            // Euler-Napier constant (e)
            ConstantsGenerator.GenerateNamedNumericSymbol(
                this.ConstantsTable,
                this.ReverseConstantsTable,
                "e",
                global::System.Math.E);

            // Archimedes-Ludolph constant (pi)
            ConstantsGenerator.GenerateNamedNumericSymbol(
                this.ConstantsTable,
                this.ReverseConstantsTable,
                "π",
                global::System.Math.PI,
                $"{this.Definition.SpecialSymbolIndicators.Item1}pi{this.Definition.SpecialSymbolIndicators.Item2}");

            // Golden ratio
            ConstantsGenerator.GenerateNamedNumericSymbol(
                this.ConstantsTable,
                this.ReverseConstantsTable,
                "φ",
                1.6180339887498948,
                $"{this.Definition.SpecialSymbolIndicators.Item1}phi{this.Definition.SpecialSymbolIndicators.Item2}");

            // Bernstein constant
            ConstantsGenerator.GenerateNamedNumericSymbol(
                this.ConstantsTable,
                this.ReverseConstantsTable,
                "β",
                0.2801694990238691,
                $"{this.Definition.SpecialSymbolIndicators.Item1}beta{this.Definition.SpecialSymbolIndicators.Item2}");

            // Euler-Mascheroni constant
            ConstantsGenerator.GenerateNamedNumericSymbol(
                this.ConstantsTable,
                this.ReverseConstantsTable,
                "γ",
                0.5772156649015328,
                $"{this.Definition.SpecialSymbolIndicators.Item1}gamma{this.Definition.SpecialSymbolIndicators.Item2}");

            // Gauss-Kuzmin-Wirsing constant
            ConstantsGenerator.GenerateNamedNumericSymbol(
                this.ConstantsTable,
                this.ReverseConstantsTable,
                "λ",
                0.3036630028987326,
                $"{this.Definition.SpecialSymbolIndicators.Item1}lambda{this.Definition.SpecialSymbolIndicators.Item2}");
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