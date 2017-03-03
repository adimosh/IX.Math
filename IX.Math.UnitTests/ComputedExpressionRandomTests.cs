﻿// <copyright file="ComputedExpressionRandomTests.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using Xunit;

namespace IX.Math.UnitTests
{
    public class ComputedExpressionRandomTests
    {
        [Fact(DisplayName = "Tests the function \"random\".")]
        public void ComputedRandomFunctionCallExpression()
        {
            var service = new ExpressionParsingService();

            ComputedExpression del;
            try
            {
                del = service.Interpret("random(x)");
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"The generation process should not have thrown an exception, but it threw {ex.GetType()} with message \"{ex.Message}\".");
            }

            if (del == null)
            {
                throw new InvalidOperationException("No computed expression was generated!");
            }

            object result;
            try
            {
                result = del.Compute(100);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"The method should not have thrown an exception, but it threw {ex.GetType()} with message \"{ex.Message}\".");
            }

            Assert.IsType<long>(result);

            Assert.True(((long)result) < 100);
        }
    }
}