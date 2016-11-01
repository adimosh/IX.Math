using IX.Math.BuiltIn;
using IX.Math.SimplificationAide;
using System;
using System.Collections;
using System.Collections.Generic;

namespace IX.Math
{
    internal class ConstantsContainer : IReadOnlyDictionary<string, ExpressionTreeNodeBase>
    {
        private Dictionary<string, ExpressionTreeNodeBase> constants = new Dictionary<string, ExpressionTreeNodeBase>();

        public ExpressionTreeNodeBase ParseNumeric(string value)
        {
            ExpressionTreeNodeBase etnb;
            if (constants.TryGetValue(value, out etnb))
            {
                return etnb;
            }

            Type nt = WorkingConstants.defaultNumericType;
            object val;
            if (!NumericTypeParsingAide.Parse(value, ref nt, out val))
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

            constants.Add(value, result);
            return result;
        }

        public ExpressionTreeNodeBase this[string key]
        {
            get
            {
                return constants[key];
            }
        }

        public int Count
        {
            get
            {
                return constants.Count;
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return constants.Keys;
            }
        }

        public IEnumerable<ExpressionTreeNodeBase> Values
        {
            get
            {
                return constants.Values;
            }
        }

        public bool ContainsKey(string key)
        {
            return constants.ContainsKey(key);
        }

        public IEnumerator<KeyValuePair<string, ExpressionTreeNodeBase>> GetEnumerator()
        {
            return constants.GetEnumerator();
        }

        public bool TryGetValue(string key, out ExpressionTreeNodeBase value)
        {
            return constants.TryGetValue(key, out value);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return constants.GetEnumerator();
        }
    }
}
