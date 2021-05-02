using Uranium.CodeAnalysis.Binding.Statements;

namespace Uranium.CodeAnalysis.Syntax.EvaluatorSupport
{
    internal static class WhileStatementEvaluator
    {
        public static void Evaluate(BoundWhileStatement statement, Evaluator eval)
        {
            while ( (bool)ExpressionEvaluator.Evaluate(statement.Condition, eval))
            {
                eval.EvaluateStatement(statement.Body);
            }
        }
    }
}
