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
            symbolTable = new Dictionary<string, RawExpressionContainer>();
            reverseSymbolTable = new Dictionary<string, string>();
            externalParams = new Dictionary<string, ParameterExpression>();
            constants = new Dictionary<string, ConstantExpression>();
            numericType = WorkingConstants.defaultNumericType;
            initialExpression = expression;
            this.cancellationToken = cancellationToken;
        }

        internal Dictionary<string, RawExpressionContainer> symbolTable;
        internal Dictionary<string, string> reverseSymbolTable;
        internal Type numericType;
        internal string initialExpression;
        internal CancellationToken cancellationToken;

        internal Dictionary<string, ParameterExpression> externalParams;
        internal Dictionary<string, ConstantExpression> constants;
        internal Expression body;
    }
}