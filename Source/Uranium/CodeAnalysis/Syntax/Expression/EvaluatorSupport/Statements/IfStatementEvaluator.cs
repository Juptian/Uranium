using Uranium.CodeAnalysis.Binding.Statements;

namespace Uranium.CodeAnalysis.Syntax.EvaluatorSupport
{
    internal static class IfStatementEvaluator
    {
        public static void Evaluate(BoundIfStatement statement, Evaluator eval)
        {
            if ( (bool)ExpressionEvaluator.Evaluate(statement.Condition, eval) )
            {
                eval.EvaluateStatement(statement.Statement);
            }
            else if (statement.ElseStatement is not null)
            {
                eval.EvaluateStatement(statement.ElseStatement);
            }
        }
    }
}
