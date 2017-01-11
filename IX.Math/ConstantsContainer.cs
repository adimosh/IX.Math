// <copyright file="ConstantsContainer.cs" company="Adrian Mos">
// Copyright (c) Adrian Mos with all rights reserved. Part of the IX Framework.
// </copyright>

using System;
using System.Collections;
using System.Collections.Generic;
using IX.Math.BuiltIn.Constants;
using IX.Math.SimplificationAide;

namespace IX.Math
{
    internal class ConstantsContainer : IReadOnlyDictionary<string, ExpressionTreeNodeBase>
    {
        private Dictionary<string, ExpressionTreeNodeBase> constants = new Dictionary<string, ExpressionTreeNodeBase>();

        public int Count
        {
            get
            {
                return this.constants.Count;
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return this.constants.Keys;
            }
        }

        public IEnumerable<ExpressionTreeNodeBase> Values
        {
            get
            {
                return this.constants.Values;
            }
        }

        public ExpressionTreeNodeBase this[string key]
        {
            get
            {
                return this.constants[key];
            }
        }

        public bool ContainsKey(string key)
        {
            return this.constants.ContainsKey(key);
        }

        public ExpressionTreeNodeBase ParseNumeric(string value)
        {
            if (this.constants.TryGetValue(value, out var etnb))
            {
                return etnb;
            }

            Type nt = WorkingConstants.DefaultNumericType;
            if (!NumericTypeParsingAide.Parse(value, ref nt, out object val))
            {
                return null;
            }

            ExpressionTreeNodeBase result;
            if (nt == typeof(int))
            {
                result = new ExpressionTreeNodeNumericIntConstant((int)val);
            }
            else if (nt == typeof(long))
            {
                result = new ExpressionTreeNodeNumericLongConstant((long)val);
            }
            else if (nt == typeof(float))
            {
                result = new ExpressionTreeNodeNumericSingleConstant((float)val);
            }
            else if (nt == typeof(double))
            {
                result = new ExpressionTreeNodeNumericDoubleConstant((double)val);
            }
            else
            {
                return null;
            }

            this.constants.Add(value, result);
            return result;
        }

        public IEnumerator<KeyValuePair<string, ExpressionTreeNodeBase>> GetEnumerator()
        {
            return this.constants.GetEnumerator();
        }

        public bool TryGetValue(string key, out ExpressionTreeNodeBase value)
        {
            return this.constants.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.constants.GetEnumerator();
        }
    }
}