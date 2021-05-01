using Uranium.CodeAnalysis.Binding.Statements;
using Uranium.CodeAnalysis.Syntax.Expression.EvaluatorSupport.Expression;

namespace Uranium.CodeAnalysis.Syntax.Expression.EvaluatorSupport.Statements
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
