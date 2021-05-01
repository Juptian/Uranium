using Uranium.CodeAnalysis.Binding.Statements;
using Uranium.CodeAnalysis.Syntax.Expression.EvaluatorSupport.Expression;


namespace Uranium.CodeAnalysis.Syntax.Expression.EvaluatorSupport.Statements
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
