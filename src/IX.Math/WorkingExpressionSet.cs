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
using IX.Math.Registration;
using IX.StandardExtensions.ComponentModel;
using IX.StandardExtensions.Extensions;
using IX.System.Collections.Generic;

namespace IX.Math
{
    internal class WorkingExpressionSet : DisposableBase
    {
        // Definition
#pragma warning disable SA1401 // Fields must be private
        internal MathDefinition Definition;

        internal string[] AllOperatorsInOrder;
        internal string[] AllSymbols;
        internal Regex FunctionRegex;

        // Initial data
        internal string InitialExpression;
        internal CancellationToken CancellationToken;

        // Working domain
        internal Dictionary<string, ConstantNodeBase> ConstantsTable;
        internal Dictionary<string, string> ReverseConstantsTable;
        internal Dictionary<string, ExpressionSymbol> SymbolTable;
        internal Dictionary<string, string> ReverseSymbolTable;
        internal string Expression;
        internal IParameterRegistry ParameterRegistry;

        // Scrap
#pragma warning disable IDISP002 // Dispose member.
#pragma warning disable IDISP006 // Implement IDisposable.
#pragma warning disable IDISP008 // Don't assign member with injected and created disposables.
        internal LevelDictionary<string, Func<MathDefinition, NodeBase, NodeBase>> UnaryOperators;
        internal LevelDictionary<string, Func<MathDefinition, NodeBase, NodeBase, NodeBase>> BinaryOperators;
        internal LevelDictionary<Type, IConstantsExtractor> Extractors;
        internal LevelDictionary<Type, IConstantPassThroughExtractor> PassThroughExtractors;
#pragma warning restore IDISP008 // Don't assign member with injected and created disposables.
#pragma warning restore IDISP006 // Implement IDisposable.
#pragma warning restore IDISP002 // Dispose member.

        internal Dictionary<string, Type> NonaryFunctions;
        internal Dictionary<string, Type> UnaryFunctions;
        internal Dictionary<string, Type> BinaryFunctions;
        internal Dictionary<string, Type> TernaryFunctions;

        // Results
        internal object ValueIfConstant;
        internal bool Success = false;
        internal bool InternallyValid = false;
        internal bool Constant = false;
        internal bool PossibleString = false;
#pragma warning restore SA1401 // Fields must be private

        private bool initialized;

        internal WorkingExpressionSet(
            string expression,
            MathDefinition mathDefinition,
            Dictionary<string, Type> nonaryFunctions,
            Dictionary<string, Type> unaryFunctions,
            Dictionary<string, Type> binaryFunctions,
            Dictionary<string, Type> ternaryFunctions,
            LevelDictionary<Type, IConstantsExtractor> extractors,
            LevelDictionary<Type, IConstantPassThroughExtractor> passThroughExtractors,
            CancellationToken cancellationToken)
        {
            this.ParameterRegistry = new StandardParameterRegistry();
            this.ConstantsTable = new Dictionary<string, ConstantNodeBase>();
            this.ReverseConstantsTable = new Dictionary<string, string>();
            this.SymbolTable = new Dictionary<string, ExpressionSymbol>();
            this.ReverseSymbolTable = new Dictionary<string, string>();

            this.InitialExpression = expression;
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
                this.Definition.NotSymbol,
            };

            this.NonaryFunctions = nonaryFunctions;
            this.UnaryFunctions = unaryFunctions;
            this.BinaryFunctions = binaryFunctions;
            this.TernaryFunctions = ternaryFunctions;

            this.Extractors = extractors;
            this.PassThroughExtractors = passThroughExtractors;

            this.FunctionRegex = new Regex($@"(?'functionName'.*?){Regex.Escape(this.Definition.Parentheses.Item1)}(?'expression'.*?){Regex.Escape(this.Definition.Parentheses.Item2)}");
        }

        internal void Initialize()
        {
            if (this.initialized)
            {
                return;
            }

            this.initialized = true;

            var i = 1;
#pragma warning disable HAA0401 // Possible allocation of reference type enumerator - Acceptable in this case
            foreach (var op in this.AllOperatorsInOrder
                .OrderByDescending(p => p.Length)
                .Where((p, thisL1) => thisL1.AllOperatorsInOrder.Any((q, pL2) => q.Length < pL2.Length && pL2.Contains(q), p), this)
                .OrderByDescending(p => p.Length))
#pragma warning restore HAA0401 // Possible allocation of reference type enumerator
            {
                var s = $"@op{i.ToString()}@";

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
#pragma warning disable IDISP003 // Dispose previous before re-assigning. - Not an issue, as Initialize is repeat-checked

            // Binary operators
            this.BinaryOperators = new LevelDictionary<string, Func<MathDefinition, NodeBase, NodeBase, NodeBase>>
            {
                // First tier - Comparison and equation operators
                { this.Definition.GreaterThanOrEqualSymbol, (definition, leftOperand, rightOperand) => new Nodes.Operations.Binary.GreaterThanOrEqualNode(leftOperand, rightOperand), 10 },
                { this.Definition.LessThanOrEqualSymbol, (definition, leftOperand, rightOperand) => new Nodes.Operations.Binary.LessThanOrEqualNode(leftOperand, rightOperand), 10 },
                { this.Definition.GreaterThanSymbol, (definition, leftOperand, rightOperand) => new Nodes.Operations.Binary.GreaterThanNode(leftOperand, rightOperand), 10 },
                { this.Definition.LessThanSymbol, (definition, leftOperand, rightOperand) => new Nodes.Operations.Binary.LessThanNode(leftOperand, rightOperand), 10 },
                { this.Definition.NotEqualsSymbol, (definition, leftOperand, rightOperand) => new Nodes.Operations.Binary.NotEqualsNode(leftOperand, rightOperand), 10 },
                { this.Definition.EqualsSymbol, (definition, leftOperand, rightOperand) => new Nodes.Operations.Binary.EqualsNode(leftOperand, rightOperand), 10 },

                // Second tier - Logical operators
                { this.Definition.OrSymbol, (definition, leftOperand, rightOperand) => new Nodes.Operations.Binary.OrNode(leftOperand, rightOperand), 20 },
                { this.Definition.XorSymbol, (definition, leftOperand, rightOperand) => new Nodes.Operations.Binary.XorNode(leftOperand, rightOperand), this.Definition.OperatorPrecedenceStyle == OperatorPrecedenceStyle.CStyle ? 21 : 20 },
                { this.Definition.AndSymbol, (definition, leftOperand, rightOperand) => new Nodes.Operations.Binary.AndNode(leftOperand, rightOperand), this.Definition.OperatorPrecedenceStyle == OperatorPrecedenceStyle.CStyle ? 22 : 20 },

                // Third tier - Arithmetic second-rank operators
                { this.Definition.AddSymbol, (definition, leftOperand, rightOperand) => new Nodes.Operations.Binary.AddNode(leftOperand, rightOperand), 30 },
                { this.Definition.SubtractSymbol, (definition, leftOperand, rightOperand) => new Nodes.Operations.Binary.SubtractNode(leftOperand, rightOperand), 30 },

                // Fourth tier - Arithmetic first-rank operators
                { this.Definition.DivideSymbol, (definition, leftOperand, rightOperand) => new Nodes.Operations.Binary.DivideNode(leftOperand, rightOperand), 40 },
                { this.Definition.MultiplySymbol, (definition, leftOperand, rightOperand) => new Nodes.Operations.Binary.MultiplyNode(leftOperand, rightOperand), 40 },

                // Fifth tier - Power operator
                { this.Definition.PowerSymbol, (definition, leftOperand, rightOperand) => new Nodes.Operations.Binary.PowerNode(leftOperand, rightOperand), 50 },

                // Sixth tier - Bitwise shift operators
                { this.Definition.LeftShiftSymbol, (definition, leftOperand, rightOperand) => new Nodes.Operations.Binary.LeftShiftNode(leftOperand, rightOperand), 60 },
                { this.Definition.RightShiftSymbol, (definition, leftOperand, rightOperand) => new Nodes.Operations.Binary.RightShiftNode(leftOperand, rightOperand), 60 },
            };

            // Unary operators
            this.UnaryOperators = new LevelDictionary<string, Func<MathDefinition, NodeBase, NodeBase>>
            {
                // First tier - Negation and inversion
                { this.Definition.SubtractSymbol, (definition, operand) => new Nodes.Operations.Unary.SubtractNode(operand), 1 },
                { this.Definition.NotSymbol, (definition, operand) => new Nodes.Operations.Unary.NotNode(operand), 1 },
            };
#pragma warning restore IDISP003 // Dispose previous before re-assigning.

            // All symbols
            this.AllSymbols = this.AllOperatorsInOrder
                .Union(new[]
                {
                    this.Definition.ParameterSeparator,
                    this.Definition.Parentheses.Item1,
                    this.Definition.Parentheses.Item2,
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

        protected override void DisposeManagedContext()
        {
            base.DisposeManagedContext();

            Interlocked.Exchange(ref this.ConstantsTable, null).Clear();
            Interlocked.Exchange(ref this.ReverseConstantsTable, null).Clear();
            Interlocked.Exchange(ref this.SymbolTable, null)?.Clear();
            Interlocked.Exchange(ref this.ReverseSymbolTable, null)?.Clear();
            Interlocked.Exchange(ref this.UnaryOperators, null)?.Dispose();
            Interlocked.Exchange(ref this.BinaryOperators, null)?.Dispose();
        }
    }
}