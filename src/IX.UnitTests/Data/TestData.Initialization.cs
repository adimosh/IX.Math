// <copyright file="TestData.Initialization.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using IX.StandardExtensions.TestUtils;

namespace IX.UnitTests.Data
{
    public static partial class TestData
    {
        private const int Limit = 1000;

        private static readonly Dictionary<string, (bool, Delegate)> Operators = new Dictionary<string, (bool, Delegate)>
        {
            ["+"] = (false, (Func<long, long, long>)((
                p,
                q) => p + q)),
            ["-"] = (false, (Func<long, long, long>)((
                p,
                q) => p - q)),
            ["*"] = (false, (Func<long, long, long>)((
                p,
                q) => p * q)),
            ["/"] = (true, (Func<double, double, double>)((
                p,
                q) => p / q)),
            ["&"] = (false, (Func<long, long, long>)((
                p,
                q) => p & q)),
            ["|"] = (false, (Func<long, long, long>)((
                p,
                q) => p | q)),
            ["^"] = (false, (Func<long, long, long>)((
                p,
                q) => p ^ q)),
        };

        private static IEnumerable<(object, string, Dictionary<string, object>)> GenerateThreeFoldTestData(Dictionary<string, (bool, Delegate)> operators)
        {
            List<(object, string, Dictionary<string, object>, bool)> initialStage = new List<(object, string, Dictionary<string, object>, bool)>();
            foreach (var op in operators.Keys)
            {
                var operand1 = DataGenerator.RandomNonNegativeInteger(Limit);
                var operand2 = DataGenerator.RandomNonNegativeInteger(Limit);

                initialStage.Add(
                    (
                        operators[op].Item1 ?
                            (object)((Func<double, double, double>)operators[op].Item2).Invoke(operand1, operand2) :
                            (object)((Func<long, long, long>)operators[op].Item2).Invoke(operand1, operand2),
                        $"{operand1} {op} {operand2}",
                        null,
                        operators[op].Item1));
                initialStage.Add(
                    (
                        operators[op].Item1 ?
                            (object)((Func<double, double, double>)operators[op].Item2).Invoke(operand1, operand2) :
                            (object)((Func<long, long, long>)operators[op].Item2).Invoke(operand1, operand2),
                        $"x {op} {operand2}",
                        new Dictionary<string, object>
                        {
                            ["x"] = operand1,
                        },
                        operators[op].Item1));
                initialStage.Add(
                    (
                        operators[op].Item1 ?
                            (object)((Func<double, double, double>)operators[op].Item2).Invoke(operand1, operand2) :
                            (object)((Func<long, long, long>)operators[op].Item2).Invoke(operand1, operand2),
                        $"{operand1} {op} y",
                        new Dictionary<string, object>
                        {
                            ["y"] = operand2,
                        },
                        operators[op].Item1));
                initialStage.Add(
                    (
                        operators[op].Item1 ?
                            (object)((Func<double, double, double>)operators[op].Item2).Invoke(operand1, operand2) :
                            (object)((Func<long, long, long>)operators[op].Item2).Invoke(operand1, operand2),
                        $"x {op} y",
                        new Dictionary<string, object>
                        {
                            ["x"] = operand1,
                            ["y"] = operand2,
                        },
                        operators[op].Item1));
            }

            List<(object, string, Dictionary<string, object>)> secondStage =
                new List<(object, string, Dictionary<string, object>)>();
            foreach (var op in operators.Keys)
            {
                foreach (var xy in initialStage)
                {
                    secondStage.Add((xy.Item1, xy.Item2, xy.Item3));

                    if (xy.Item4 != operators[op].Item1)
                    {
                        continue;
                    }

                    var operand3 = DataGenerator.RandomNonNegativeInteger(Limit);

                    secondStage.Add(
                        (
                            operators[op].Item1 ?
                                (object)((Func<double, double, double>)operators[op].Item2).Invoke(operand3, (double)xy.Item1) :
                                (object)((Func<long, long, long>)operators[op].Item2).Invoke(operand3, (long)xy.Item1),
                            $"{operand3} {op} ({xy.Item2})",
                            xy.Item3));
                    secondStage.Add(
                        (
                            operators[op].Item1 ?
                                (object)((Func<double, double, double>)operators[op].Item2).Invoke((double)xy.Item1, operand3) :
                                (object)((Func<long, long, long>)operators[op].Item2).Invoke((long)xy.Item1, operand3),
                            $"({xy.Item2}) {op} {operand3}",
                            xy.Item3));
                    secondStage.Add(
                        (
                            operators[op].Item1 ?
                                (object)((Func<double, double, double>)operators[op].Item2).Invoke(operand3, (double)xy.Item1) :
                                (object)((Func<long, long, long>)operators[op].Item2).Invoke(operand3, (long)xy.Item1),
                            $"z {op} ({xy.Item2})",
                            xy.Item3 == null ?
                                new Dictionary<string, object> { ["z"] = operand3 } :
                                xy.Item3.Union(new[] { new KeyValuePair<string, object>("z", operand3) }).ToDictionary(p => p.Key, p => p.Value)));
                    secondStage.Add(
                        (
                            operators[op].Item1 ?
                                (object)((Func<double, double, double>)operators[op].Item2).Invoke((double)xy.Item1, operand3) :
                                (object)((Func<long, long, long>)operators[op].Item2).Invoke((long)xy.Item1, operand3),
                            $"({xy.Item2}) {op} z",
                            xy.Item3 == null ?
                                new Dictionary<string, object> { ["z"] = operand3 } :
                                xy.Item3.Union(new[] { new KeyValuePair<string, object>("z", operand3) }).ToDictionary(p => p.Key, p => p.Value)));
                }
            }

            return secondStage;
        }
    }
}