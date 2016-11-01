using IX.Math.BuiltIn;
using System.Collections.Generic;
using System.Threading;

namespace IX.Math
{
    internal class WorkingExpressionSet
    {
        internal WorkingExpressionSet(string expression, CancellationToken cancellationToken)
        {
            SymbolTable = new Dictionary<string, RawExpressionContainer>();
            ReverseSymbolTable = new Dictionary<string, string>();
            ExternalParameters = new Dictionary<string, ExpressionTreeNodeParameter>();
            Constants = new ConstantsContainer();
            InitialExpression = expression;
            CancellationToken = cancellationToken;
        }

        internal Dictionary<string, RawExpressionContainer> SymbolTable;
        internal Dictionary<string, string> ReverseSymbolTable;
        internal string InitialExpression;
        internal CancellationToken CancellationToken;

        internal Dictionary<string, ExpressionTreeNodeParameter> ExternalParameters;
        internal ConstantsContainer Constants;
        internal ExpressionTreeNodeBase Body;
        internal object ValueIfConstant;

        internal bool Success = false;
        internal bool InternallyValid = false;
        internal bool Constant = false;
        internal bool PossibleString = false;
    }
}