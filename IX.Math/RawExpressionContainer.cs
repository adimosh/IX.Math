// <copyright file="RawExpressionContainer.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Diagnostics;

namespace IX.Math
{
    [DebuggerDisplay("{Expression}")]
    internal sealed class RawExpressionContainer : IEquatable<RawExpressionContainer>
    {
        internal RawExpressionContainer(string expression, bool isFunction = false, bool isString = false)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                this.Expression = null;
            }
            else
            {
                if (isString)
                {
                    this.Expression = expression;
                }
                else
                {
                    this.Expression = expression.Replace(" ", string.Empty);
                }
            }

            this.IsFunctionCall = isFunction;
            this.IsString = isString;
        }

        public string Expression { get; private set; }

        public bool IsFunctionCall { get; private set; }

        public bool IsString { get; private set; }

        public bool Equals(RawExpressionContainer other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Expression == other.Expression &&
                this.IsFunctionCall == other.IsFunctionCall &&
                this.IsString == other.IsString;
        }
    }
}