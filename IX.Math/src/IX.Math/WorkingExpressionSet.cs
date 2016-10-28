using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;

namespace IX.Math
{
    internal class WorkingExpressionSet
    {
        internal WorkingExpressionSet(string expression, CancellationToken cancellationToken)
        {
            SymbolTable = new Dictionary<string, RawExpressionContainer>();
            ReverseSymbolTable = new Dictionary<string, string>();
            ExternalParameters = new Dictionary<string, ParameterExpression>();
            Constants = new Dictionary<string, ConstantExpression>();
            NumericType = WorkingConstants.defaultNumericType;
            InitialExpression = expression;
            this.CancellationToken = cancellationToken;
        }

        internal Dictionary<string, RawExpressionContainer> SymbolTable;
        internal Dictionary<string, string> ReverseSymbolTable;
        internal Type NumericType;
        internal string InitialExpression;
        internal CancellationToken CancellationToken;

        internal Dictionary<string, ParameterExpression> ExternalParameters;
        internal Dictionary<string, ConstantExpression> Constants;
        internal Expression Body;
        internal object ValueIfConstant;

        internal bool Success = false;
        internal bool InternallyValid = false;
        internal bool Constant = false;
    }
}