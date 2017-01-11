using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using IX.Math.SimplificationAide;
using IX.Math.BuiltIn;

namespace IX.Math
{
    /// <summary>
    /// A service that is able to parse strings containing mathematical expressions and solve them.
    /// </summary>
    public sealed class ExpressionParsingService : IExpressionParsingService
    {
        private readonly MathDefinition workingDefinition;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionParsingService"/> class with a standard math definition object.
        /// </summary>
        public ExpressionParsingService()
            : this(new MathDefinition
            {
                Parantheses = new Tuple<string, string>("(", ")"),
                SpecialSymbolIndicators = new Tuple<string, string>("[", "]"),
                StringIndicator = "\"",
                ParameterSeparator= ",",
                AddSymbol = "+",
                AndSymbol = "&",
                DivideSymbol = "/",
                DoesNotEqualSymbol = "!=",
                EqualsSymbol = "=",
                MultiplySymbol = "*",
                NotSymbol = "!",
                OrSymbol = "|",
                PowerSymbol = "^",
                SubtractSymbol = "-",
                XorSymbol = "#",
                GreaterThanOrEqualSymbol = ">=",
                GreaterThanSymbol = ">",
                LessThanOrEqualSymbol = "<=",
                LessThanSymbol = "<",
                ShiftRightSymbol = ">>",
                ShiftLeftSymbol = "<<",
            })
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpressionParsingService"/> class with a specified math definition object.
        /// </summary>
        /// <param name="definition">The math definition to use.</param>
        public ExpressionParsingService(MathDefinition definition)
        {
            workingDefinition = definition;
        }

        ///<inheritDoc/>
        public ComputedExpression Interpret(string expression, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                throw new ArgumentNullException(nameof(expression));
            }

            WorkingExpressionSet workingSet = new WorkingExpressionSet(expression, workingDefinition, cancellationToken);

            ExpressionGenerator.CreateBody(workingSet);

            if (!workingSet.Success)
            {
                return new ComputedExpression(expression, null, null, false);
            }
            else
            {
                return new ComputedExpression(expression, workingSet.Body, workingSet.ExternalParameters.Values.ToArray(), true);
            }
        }
    }
}