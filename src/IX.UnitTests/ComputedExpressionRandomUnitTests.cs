// <copyright file="ComputedExpressionRandomUnitTests.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using IX.Math;
using Xunit;

namespace IX.UnitTests
{
    /// <summary>
    ///     Class ComputedExpressionRandomTests.
    /// </summary>
    public class ComputedExpressionRandomUnitTests
    {
        /// <summary>
        ///     Computes the unary random function call expression, for testing.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     No computed expression was generated.
        /// </exception>
        [Fact(DisplayName = "Tests the binary function \"random\".")]
        public void ComputedBinaryRandomFunctionCallExpression()
        {
            var r = new Random();
            int dingLimit;
            do
            {
                dingLimit = r.Next();
            }
            while (dingLimit <= 5);

            var highLimit = r.Next(
                dingLimit,
                int.MaxValue);
            var lowLimit = r.Next(dingLimit);

            using var service = new MathematicPortfolio();

            object result;
            try
            {
#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation - Not consequential
                result = service.Solve(
                    "random(x, y)",
                    lowLimit,
                    highLimit);
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "The method should not have thrown an exception, but it did.",
                    ex);
            }

            Assert.IsType<double>(result);

            Assert.True((double)result < highLimit);
            Assert.True((double)result >= lowLimit);
        }

        /// <summary>
        ///     Computes the unary random function call expression, for testing.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     No computed expression was generated.
        /// </exception>
        [Fact(DisplayName = "Tests the unary function \"random\".")]
        public void ComputedUnaryRandomFunctionCallExpression()
        {
            var r = new Random();
            var limit = r.Next();

            using var service = new MathematicPortfolio();

            object result;
            try
            {
#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation - Not consequential
                result = service.Solve("random(x)", limit);
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "The method should not have thrown an exception, but it did.",
                    ex);
            }

            Assert.IsType<double>(result);

            Assert.True((double)result < limit);
        }

        /// <summary>
        ///     Computes the random nonary function call expression, for testing.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     No computed expression was generated.
        /// </exception>
        [Fact(DisplayName = "Tests the nonary function \"random\".")]
        public void ComputedRandomNonaryFunctionCallExpression()
        {
            using var service = new MathematicPortfolio();

            object result;
            try
            {
                result = service.Solve("random()");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "The method should not have thrown an exception, but it did.",
                    ex);
            }

            Assert.IsType<double>(result);
        }

        /// <summary>
        ///     Computes the unary random function call expression, for testing.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     No computed expression was generated.
        /// </exception>
        [Fact(DisplayName = "Tests the binary function \"randomint\".")]
        public void ComputedBinaryRandomIntFunctionCallExpression()
        {
            var r = new Random();
            int dingLimit;
            do
            {
                dingLimit = r.Next();
            }
            while (dingLimit <= 5);

            var highLimit = r.Next(
                dingLimit,
                int.MaxValue);
            var lowLimit = r.Next(dingLimit);

            using var service = new MathematicPortfolio();

            object result;
            try
            {
#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation - Not consequential
                result = service.Solve(
                    "randomint(x, y)",
                    lowLimit,
                    highLimit);
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "The method should not have thrown an exception, but it did.",
                    ex);
            }

            Assert.IsType<long>(result);

            Assert.True((long)result < highLimit);
            Assert.True((long)result >= lowLimit);
        }

        /// <summary>
        ///     Computes the unary random function call expression, for testing.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     No computed expression was generated.
        /// </exception>
        [Fact(DisplayName = "Tests the unary function \"randomint\".")]
        public void ComputedUnaryRandomIntFunctionCallExpression()
        {
            var r = new Random();
            var limit = r.Next();

            using var service = new MathematicPortfolio();

            object result;
            try
            {
#pragma warning disable HAA0601 // Value type to reference type conversion causing boxing allocation - Not consequential
                result = service.Solve("randomint(x)", limit);
#pragma warning restore HAA0601 // Value type to reference type conversion causing boxing allocation
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "The method should not have thrown an exception, but it did.",
                    ex);
            }

            Assert.IsType<long>(result);

            Assert.True((long)result < limit);
        }

        /// <summary>
        ///     Computes the random nonary function call expression, for testing.
        /// </summary>
        /// <exception cref="InvalidOperationException">
        ///     No computed expression was generated.
        /// </exception>
        [Fact(DisplayName = "Tests the nonary function \"randomint\".")]
        public void ComputedRandomIntNonaryFunctionCallExpression()
        {
            using var service = new MathematicPortfolio();

            object result;
            try
            {
                result = service.Solve("randomint()");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(
                    "The method should not have thrown an exception, but it did.",
                    ex);
            }

            Assert.IsType<long>(result);
        }
    }
}