﻿// <copyright file="WorkingExpressionSet.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using IX.Math.Nodes;
using IX.Math.Nodes.Constants;
using IX.Math.Nodes.Operations.Function.Binary;
using IX.Math.Nodes.Operations.Function.Unary;
using IX.Math.Nodes.Parameters;

namespace IX.Math
{
    internal class WorkingExpressionSet
    {
        // Definition
#pragma warning disable SA1401 // Fields must be private
        internal MathDefinition Definition;

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
        internal Dictionary<string, ParameterNodeBase> ParametersTable;
        internal string Expression;
        internal NodeBase Body;

        // Scrap
        internal Dictionary<string, Type> UnaryOperators;
        internal Dictionary<string, Type> BinaryOperators;
        internal Dictionary<string, Type> UnaryFunctions;
        internal Dictionary<string, Type> BinaryFunctions;

        // Results
        internal object ValueIfConstant;
        internal bool Success = false;
        internal bool InternallyValid = false;
        internal bool Constant = false;
        internal bool PossibleString = false;
#pragma warning restore SA1401 // Fields must be private

        internal WorkingExpressionSet(string expression, MathDefinition mathDefinition, CancellationToken cancellationToken)
        {
            this.ConstantsTable = new Dictionary<string, ConstantNodeBase>();
            this.ReverseConstantsTable = new Dictionary<string, string>();
            this.ParametersTable = new Dictionary<string, ParameterNodeBase>();
            this.SymbolTable = new Dictionary<string, RawExpressionContainer>();
            this.ReverseSymbolTable = new Dictionary<string, string>();

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
            this.BinaryOperatorsInOrder = new[]
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
            };

            this.UnaryOperatorsInOrder = new[]
            {
                this.Definition.SubtractSymbol,
                this.Definition.NotSymbol,
            };

            this.AllSymbols = this.AllOperatorsInOrder
                .Union(new[]
                {
                    this.Definition.ParameterSeparator,
                    this.Definition.Parantheses.Item1,
                    this.Definition.Parantheses.Item2,
                })
                .ToArray();

            // Special symbols

            // Euler-Napier constant (e)
            Generators.ConstantsGenerator.GenerateNamedNumericSymbol(
                this.ConstantsTable,
                this.ReverseConstantsTable,
                "e",
                System.Math.E);

            // Archimedes-Ludolph constant (pi)
            Generators.ConstantsGenerator.GenerateNamedNumericSymbol(
                this.ConstantsTable,
                this.ReverseConstantsTable,
                "π",
                System.Math.PI,
                $"{this.Definition.SpecialSymbolIndicators.Item1}pi{this.Definition.SpecialSymbolIndicators.Item2}");

            // Golden ratio
            Generators.ConstantsGenerator.GenerateNamedNumericSymbol(
                this.ConstantsTable,
                this.ReverseConstantsTable,
                "φ",
                1.6180339887498948,
                $"{this.Definition.SpecialSymbolIndicators.Item1}phi{this.Definition.SpecialSymbolIndicators.Item2}");

            // Bernstein constant
            Generators.ConstantsGenerator.GenerateNamedNumericSymbol(
                this.ConstantsTable,
                this.ReverseConstantsTable,
                "β",
                0.2801694990238691,
                $"{this.Definition.SpecialSymbolIndicators.Item1}beta{this.Definition.SpecialSymbolIndicators.Item2}");

            // Euler-Mascheroni constant
            Generators.ConstantsGenerator.GenerateNamedNumericSymbol(
                this.ConstantsTable,
                this.ReverseConstantsTable,
                "γ",
                0.5772156649015328,
                $"{this.Definition.SpecialSymbolIndicators.Item1}gamma{this.Definition.SpecialSymbolIndicators.Item2}");

            // Gauss-Kuzmin-Wirsing constant
            Generators.ConstantsGenerator.GenerateNamedNumericSymbol(
                this.ConstantsTable,
                this.ReverseConstantsTable,
                "λ",
                0.3036630028987326,
                $"{this.Definition.SpecialSymbolIndicators.Item1}lambda{this.Definition.SpecialSymbolIndicators.Item2}");

            this.InitializeUnaryOperators();
            this.InitializeBinaryOperators();
            this.InitializeUnaryFunctions();
            this.InitializeBinaryFunctions();
        }

        private void InitializeUnaryOperators()
        {
            this.UnaryOperators = typeof(MathDefinition)
                .GetRuntimeProperties()
                .Where(p => p.Name.EndsWith("Symbol"))
                .Select(p => new { Name = p.Name.Substring(0, p.Name.Length - 6), Value = (string)p.GetValue(this.Definition) })
                .Select(p => new { Name = p.Name, Type = Type.GetType($"IX.Math.Nodes.Operations.Unary.{p.Name}Node", false), Value = p.Value })
                .Where(p => p.Type != null)
                .ToDictionary(p => p.Value, p => p.Type);
        }

        private void InitializeBinaryOperators()
        {
            this.BinaryOperators = typeof(MathDefinition)
                .GetRuntimeProperties()
                .Where(p => p.Name.EndsWith("Symbol"))
                .Select(p => new { Name = p.Name.Substring(0, p.Name.Length - 6), Value = (string)p.GetValue(this.Definition) })
                .Select(p => new { Name = p.Name, Type = Type.GetType($"IX.Math.Nodes.Operations.Binary.{p.Name}Node", false), Value=p.Value })
                .Where(p => p.Type != null)
                .ToDictionary(p => p.Value, p => p.Type);
        }

        private void InitializeUnaryFunctions()
        {
            // TODO: Replace this with an injection mechanism
            this.UnaryFunctions = new Dictionary<string, Type>
            {
                ["strlen"] = typeof(FunctionNodestrlen),
                ["abs"] = typeof(FunctionNodeabs),
                ["acos"] = typeof(FunctionNodeacos),
                ["asin"] = typeof(FunctionNodeasin),
                ["atan"] = typeof(FunctionNodeatan),
                ["ceiling"] = typeof(FunctionNodeceiling),
                ["cos"] = typeof(FunctionNodecos),
                ["cosh"] = typeof(FunctionNodecosh),
                ["exp"] = typeof(FunctionNodeexp),
                ["floor"] = typeof(FunctionNodefloor),
                ["ln"] = typeof(FunctionNodeln),
                ["lg"] = typeof(FunctionNodelg),
                ["round"] = typeof(FunctionNoderound),
                ["sin"] = typeof(FunctionNodesin),
                ["sinh"] = typeof(FunctionNodesinh),
                ["sqrt"] = typeof(FunctionNodesqrt),
                ["tan"] = typeof(FunctionNodetan),
                ["tanh"] = typeof(FunctionNodetanh),
                ["trun"] = typeof(FunctionNodetrun),
            };
        }

        private void InitializeBinaryFunctions()
        {
            // TODO: Replace this with an injection mechanism
            this.BinaryFunctions = new Dictionary<string, Type>
            {
                ["log"] = typeof(FunctionNodelog),
                ["min"] = typeof(FunctionNodemin),
                ["max"] = typeof(FunctionNodemax),
                ["pow"] = typeof(FunctionNodepow),
            };
        }
    }
}