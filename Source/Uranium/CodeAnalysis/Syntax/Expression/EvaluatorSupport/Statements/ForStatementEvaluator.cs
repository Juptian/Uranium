using Uranium.CodeAnalysis.Binding.Statements;

namespace Uranium.CodeAnalysis.Syntax.EvaluatorSupport
{
    internal static class ForStatementEvaluator
    {
        public static void Evaluate(BoundForStatement statement, Evaluator eval)
        {
            if (statement.VariableDeclaration is not null)
            {
                eval.EvaluateStatement(statement.VariableDeclaration);
            }
            while (statement.Condition is null || (bool)ExpressionEvaluator.Evaluate(statement.Condition, eval))
            {
                eval.EvaluateStatement(statement.Body);
                if (statement.Increment is not null)
                {
                    ExpressionEvaluator.Evaluate(statement.Increment, eval);
                }
            }
        }
    }
}
