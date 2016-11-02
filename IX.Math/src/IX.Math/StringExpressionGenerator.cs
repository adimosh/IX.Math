namespace IX.Math
{
    internal static class StringExpressionGenerator
    {
        internal static void ReplaceStrings(WorkingExpressionSet workingSet, WorkingDefinition definition)
        {
            string process = workingSet.InitialExpression;

            while (true)
            {
                int op = process.IndexOf(definition.Definition.StringIndicator);

                if (op == -1)
                {
                    break;
                }

                int cp = process.IndexOf(definition.Definition.StringIndicator, op + definition.Definition.StringIndicator.Length);

                escapeRoute:
                if (cp == -1 || (cp + definition.Definition.StringIndicator.Length) >= process.Length)
                {
                    break;
                }

                if (process.Substring(cp + definition.Definition.StringIndicator.Length).StartsWith(definition.Definition.StringIndicator))
                {
                    cp = process.IndexOf(definition.Definition.StringIndicator, cp + definition.Definition.StringIndicator.Length * 2);
                    goto escapeRoute;
                }

                string itemName = SymbolExpressionGenerator.GenerateSymbolExpression(
                    workingSet,
                    process.Substring(op + definition.Definition.StringIndicator.Length, cp - op - definition.Definition.StringIndicator.Length),
                    isString: true
                    );

                process = $"{process.Substring(0, op)}{itemName}{process.Substring(cp + definition.Definition.StringIndicator.Length)}";
            }

            workingSet.InitialExpression = process;
        }
    }
}
