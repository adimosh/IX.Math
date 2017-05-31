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
        private string expression;

        internal RawExpressionContainer(string expression, bool isFunction = false)
        {
            this.Expression = expression;
            this.IsFunctionCall = isFunction;
        }

        public string Expression
        {
            get => this.expression;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                {
                    this.expression = null;
                }
                else
                {
                    this.expression = value;
                }
            }
        }

        public bool IsFunctionCall { get; private set; }

        public bool Equals(RawExpressionContainer other)
        {
            if (other == null)
            {
                return false;
            }

            return this.Expression == other.Expression &&
                this.IsFunctionCall == other.IsFunctionCall;
        }
    }
}