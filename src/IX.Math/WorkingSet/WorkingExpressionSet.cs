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
using IX.StandardExtensions.Efficiency;
using IX.StandardExtensions.Extensions;
using IX.System.Collections.Generic;
using DiagCA = System.Diagnostics.CodeAnalysis;
using SubtractNode = IX.Math.Nodes.Operators.Binary.Mathematic.SubtractNode;

namespace IX.Math.WorkingSet
{
    [DiagCA.SuppressMessage(
        "IDisposableAnalyzers.Correctness",
        "IDISP008:Don't assign member with injected and created disposables.",
        Justification = "It is OK, but the analyzer can't tell.")]
    internal partial class WorkingExpressionSet : DisposableBase
    {
        /// <summary>
        /// A regular expression to determine functions.
        /// </summary>
        private readonly Regex functionRegex;

        /// <summary>
        ///     Gets all operators in order.
        /// </summary>
        private readonly string[] allOperatorsInOrder;

        /// <summary>
        ///     Gets the definition.
        /// </summary>
        private readonly MathDefinition definition;

        /// <summary>
        ///     Gets the constant extractors.
        /// </summary>
        private readonly LevelDictionary<Type, IConstantsExtractor> extractors;

        /// <summary>
        ///     Gets the constant interpreters.
        /// </summary>
        private readonly LevelDictionary<Type, IConstantInterpreter> interpreters;

        /// <summary>
        ///     Gets the parameter registry.
        /// </summary>
        private readonly ConcurrentDictionary<string, ExternalParameterNode> parameterRegistry;

        /// <summary>
        ///     Gets the nonary functions.
        /// </summary>
        private readonly Dictionary<string, Type> nonaryFunctions;

        /// <summary>
        ///     Gets the ternary functions.
        /// </summary>
        private readonly Dictionary<string, Type> ternaryFunctions;

        /// <summary>
        ///     Gets the unary functions.
        /// </summary>
        private readonly Dictionary<string, Type> unaryFunctions;

        /// <summary>
        ///     Gets the binary functions.
        /// </summary>
        private readonly Dictionary<string, Type> binaryFunctions;

        /// <summary>
        ///     The binary operators.
        /// </summary>
        [DiagCA.SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP002:Dispose member.",
            Justification = "This is correct, but the analyzer can't tell.")]
        [DiagCA.SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP006:Implement IDisposable.",
            Justification = "This is correct, but the analyzer can't tell.")]
        private LevelDictionary<string, Func<NodeBase, NodeBase, BinaryOperatorNodeBase>> binaryOperators;

        /// <summary>
        ///     The unary operators.
        /// </summary>
        [DiagCA.SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP002:Dispose member.",
            Justification = "This is correct, but the analyzer can't tell.")]
        [DiagCA.SuppressMessage(
            "IDisposableAnalyzers.Correctness",
            "IDISP006:Implement IDisposable.",
            Justification = "This is correct, but the analyzer can't tell.")]
        private LevelDictionary<string, Func<NodeBase, UnaryOperatorNodeBase>> unaryOperators;

        /// <summary>
        ///     The constants table.
        /// </summary>
        private Dictionary<string, ConstantNodeBase> constantsTable;

        /// <summary>
        ///     The reverse constants table.
        /// </summary>
        private Dictionary<string, string> reverseConstantsTable;

        /// <summary>
        ///     The symbol table.
        /// </summary>
        private Dictionary<string, ExpressionSymbol> symbolTable;

        /// <summary>
        ///     The reverse symbol table.
        /// </summary>
        private Dictionary<string, string> reverseSymbolTable;

        /// <summary>
        ///     All symbols.
        /// </summary>
        private string[] allSymbols;

        /// <summary>
        ///     Whether or not this expression set has been initialized already.
        /// </summary>
        private bool initialized;

        /// <summary>
        ///     The expression.
        /// </summary>
        private string expression;

        internal WorkingExpressionSet(
            string expression,
            MathDefinition mathDefinition,
            Dictionary<string, Type> nonaryFunctions,
            Dictionary<string, Type> unaryFunctions,
            Dictionary<string, Type> binaryFunctions,
            Dictionary<string, Type> ternaryFunctions,
            LevelDictionary<Type, IConstantsExtractor> extractors,
            LevelDictionary<Type, IConstantInterpreter> interpreters)
        {
            this.parameterRegistry = new ConcurrentDictionary<string, ExternalParameterNode>();
            this.constantsTable = new Dictionary<string, ConstantNodeBase>();
            this.reverseConstantsTable = new Dictionary<string, string>();
            this.symbolTable = new Dictionary<string, ExpressionSymbol>();
            this.reverseSymbolTable = new Dictionary<string, string>();

            this.expression = expression;
            this.definition = mathDefinition;

            this.allOperatorsInOrder = new[]
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

            this.nonaryFunctions = nonaryFunctions;
            this.unaryFunctions = unaryFunctions;
            this.binaryFunctions = binaryFunctions;
            this.ternaryFunctions = ternaryFunctions;

            this.extractors = extractors;
            this.interpreters = interpreters;

            this.functionRegex = new Regex(
                $@"(?'functionName'.*?){Regex.Escape(mathDefinition.Parentheses.Item1)}(?'expression'.*?){Regex.Escape(mathDefinition.Parentheses.Item2)}");
        }

        /// <summary>
        ///     Gets a value indicating whether this <see cref="WorkingExpressionSet" /> is success.
        /// </summary>
        /// <value>
        ///     <c>true</c> if success; otherwise, <c>false</c>.
        /// </value>
        internal bool Success { get; private set; }

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
        private void Initialize()
        {
            if (this.initialized)
            {
                return;
            }

            this.initialized = true;

            var i = 1;
            var operatorsInOrder = this.allOperatorsInOrder;
            var localDefinition = this.definition;

            #region Operators initialization

            foreach (var op in operatorsInOrder.OrderByDescending(p => p.Length)
                .Where(
                    (
                        p,
                        allOperatorsInOrderL1) => allOperatorsInOrderL1.Any(
                        (
                                q,
                                pL2) => q.Length < pL2.Length && pL2.Contains(q),
                        p),
                    operatorsInOrder)
                .OrderByDescending(p => p.Length))
            {
                var s = $"@op{i}@";

                this.expression = this.expression.Replace(
                    op,
                    s);

                var allIndex = Array.IndexOf(
                    operatorsInOrder,
                    op);
                if (allIndex != -1)
                {
                    operatorsInOrder[allIndex] = s;
                }

                if (localDefinition.AddSymbol == op)
                {
                    localDefinition.AddSymbol = s;
                }

                if (localDefinition.AndSymbol == op)
                {
                    localDefinition.AndSymbol = s;
                }

                if (localDefinition.DivideSymbol == op)
                {
                    localDefinition.DivideSymbol = s;
                }

                if (localDefinition.PowerSymbol == op)
                {
                    localDefinition.PowerSymbol = s;
                }

                if (localDefinition.ModuloSymbol == op)
                {
                    localDefinition.ModuloSymbol = s;
                }

                if (localDefinition.NotEqualsSymbol == op)
                {
                    localDefinition.NotEqualsSymbol = s;
                }

                if (localDefinition.EqualsSymbol == op)
                {
                    localDefinition.EqualsSymbol = s;
                }

                if (localDefinition.GreaterThanOrEqualSymbol == op)
                {
                    localDefinition.GreaterThanOrEqualSymbol = s;
                }

                if (localDefinition.GreaterThanSymbol == op)
                {
                    localDefinition.GreaterThanSymbol = s;
                }

                if (localDefinition.LessThanOrEqualSymbol == op)
                {
                    localDefinition.LessThanOrEqualSymbol = s;
                }

                if (localDefinition.LessThanSymbol == op)
                {
                    localDefinition.LessThanSymbol = s;
                }

                if (localDefinition.MultiplySymbol == op)
                {
                    localDefinition.MultiplySymbol = s;
                }

                if (localDefinition.NotSymbol == op)
                {
                    localDefinition.NotSymbol = s;
                }

                if (localDefinition.OrSymbol == op)
                {
                    localDefinition.OrSymbol = s;
                }

                if (localDefinition.LeftShiftSymbol == op)
                {
                    localDefinition.LeftShiftSymbol = s;
                }

                if (localDefinition.RightShiftSymbol == op)
                {
                    localDefinition.RightShiftSymbol = s;
                }

                if (localDefinition.SubtractSymbol == op)
                {
                    localDefinition.SubtractSymbol = s;
                }

                if (localDefinition.XorSymbol == op)
                {
                    localDefinition.XorSymbol = s;
                }

                i++;
            }

            #endregion

            // Operator string interpretation support
            // ======================================
            #region Binary operators

            this.binaryOperators = new LevelDictionary<string, Func<NodeBase, NodeBase, BinaryOperatorNodeBase>>
            {
                // First tier - Comparison and equation operators
                {
                    localDefinition.GreaterThanOrEqualSymbol, (
                        leftOperand,
                        rightOperand) => new GreaterThanOrEqualOperatorNode(
                        leftOperand,
                        rightOperand),
                    10
                },
                {
                    localDefinition.LessThanOrEqualSymbol, (
                        leftOperand,
                        rightOperand) => new LessThanOrEqualOperatorNode(
                        leftOperand,
                        rightOperand),
                    10
                },
                {
                    localDefinition.GreaterThanSymbol, (
                        leftOperand,
                        rightOperand) => new GreaterThanOperatorNode(
                        leftOperand,
                        rightOperand),
                    10
                },
                {
                    localDefinition.LessThanSymbol, (
                        leftOperand,
                        rightOperand) => new LessThanOperatorNode(
                        leftOperand,
                        rightOperand),
                    10
                },
                {
                    localDefinition.NotEqualsSymbol, (
                        leftOperand,
                        rightOperand) => new NotEqualsNode(
                        leftOperand,
                        rightOperand),
                    10
                },
                {
                    localDefinition.EqualsSymbol, (
                        leftOperand,
                        rightOperand) => new EqualsNode(
                        leftOperand,
                        rightOperand),
                    10
                },

                // Second tier - Logical operators
                {
                    localDefinition.OrSymbol, (
                        leftOperand,
                        rightOperand) => new OrNode(
                        leftOperand,
                        rightOperand),
                    20
                },
                {
                    localDefinition.XorSymbol, (
                        leftOperand,
                        rightOperand) => new XorNode(
                        leftOperand,
                        rightOperand),
                    localDefinition.OperatorPrecedenceStyle == OperatorPrecedenceStyle.CStyle ? 21 : 20
                },
                {
                    localDefinition.AndSymbol, (
                        leftOperand,
                        rightOperand) => new AndNode(
                        leftOperand,
                        rightOperand),
                    localDefinition.OperatorPrecedenceStyle == OperatorPrecedenceStyle.CStyle ? 22 : 20
                },

                // Third tier - Arithmetic second-rank operators
                {
                    localDefinition.AddSymbol, (
                        leftOperand,
                        rightOperand) => new AddNode(
                        leftOperand,
                        rightOperand),
                    30
                },
                {
                    localDefinition.SubtractSymbol, (
                        leftOperand,
                        rightOperand) => new SubtractNode(
                        leftOperand,
                        rightOperand),
                    30
                },

                // Fourth tier - Arithmetic first-rank operators
                {
                    localDefinition.DivideSymbol, (
                        leftOperand,
                        rightOperand) => new DivideNode(
                        leftOperand,
                        rightOperand),
                    40
                },
                {
                    localDefinition.ModuloSymbol, (
                        leftOperand,
                        rightOperand) => new RemainderNode(
                        leftOperand,
                        rightOperand),
                    40
                },
                {
                    localDefinition.MultiplySymbol, (
                        leftOperand,
                        rightOperand) => new MultiplyNode(
                        leftOperand,
                        rightOperand),
                    40
                },

                // Fifth tier - Power operator
                {
                    localDefinition.PowerSymbol, (
                        leftOperand,
                        rightOperand) => new PowerNode(
                        leftOperand,
                        rightOperand),
                    50
                },

                // Sixth tier - Bitwise shift operators
                {
                    localDefinition.LeftShiftSymbol, (
                        leftOperand,
                        rightOperand) => new LeftShiftNode(
                        leftOperand,
                        rightOperand),
                    60
                },
                {
                    localDefinition.RightShiftSymbol, (
                        leftOperand,
                        rightOperand) => new RightShiftNode(
                        leftOperand,
                        rightOperand),
                    60
                }
            };

            #endregion

            #region Unary operators

            this.unaryOperators = new LevelDictionary<string, Func<NodeBase, UnaryOperatorNodeBase>>
            {
                // First tier - Negation and inversion
                {
                    localDefinition.SubtractSymbol, (
                        operand) => new Nodes.Operators.Unary.SubtractNode(
                        operand),
                    1
                },
                {
                    localDefinition.NotSymbol, (
                        operand) => new NotNode(
                        operand),
                    1
                }
            };

            #endregion

            // All symbols
            this.allSymbols = operatorsInOrder.Union(
                    new[]
                    {
                        localDefinition.ParameterSeparator,
                        localDefinition.Parentheses.Item1,
                        localDefinition.Parentheses.Item2
                    })
                .ToArray();

            #region Special symbols

            // Euler-Napier constant (e)
            this.GenerateNamedNumericSymbol(
                "e",
                global::System.Math.E);

            // Archimedes-Ludolph constant (pi)
            this.GenerateNamedNumericSymbol(
                "π",
                global::System.Math.PI,
                $"{localDefinition.SpecialSymbolIndicators.Item1}pi{localDefinition.SpecialSymbolIndicators.Item2}");

            // Golden ratio
            this.GenerateNamedNumericSymbol(
                "φ",
                1.6180339887498948,
                $"{localDefinition.SpecialSymbolIndicators.Item1}phi{localDefinition.SpecialSymbolIndicators.Item2}");

            // Bernstein constant
            this.GenerateNamedNumericSymbol(
                "β",
                0.2801694990238691,
                $"{localDefinition.SpecialSymbolIndicators.Item1}beta{localDefinition.SpecialSymbolIndicators.Item2}");

            // Euler-Mascheroni constant
            this.GenerateNamedNumericSymbol(
                "γ",
                0.5772156649015328,
                $"{localDefinition.SpecialSymbolIndicators.Item1}gamma{localDefinition.SpecialSymbolIndicators.Item2}");

            // Gauss-Kuzmin-Wirsing constant
            this.GenerateNamedNumericSymbol(
                "λ",
                0.3036630028987326,
                $"{localDefinition.SpecialSymbolIndicators.Item1}lambda{localDefinition.SpecialSymbolIndicators.Item2}");

            #endregion
        }
    }
}