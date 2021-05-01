using Uranium.CodeAnalysis.Binding.Statements;
using Uranium.CodeAnalysis.Syntax.Expression.EvaluatorSupport.Expression;

namespace Uranium.CodeAnalysis.Syntax.Expression.EvaluatorSupport.Statements
{
    internal static class VariableDeclarationEvaluator
    {
        public static void Evaluate(BoundVariableDeclaration statement, Evaluator eval)
        {
            var value = ExpressionEvaluator.Evaluate(statement.Initializer, eval);
            eval.Variables[statement.Variable] = value;
            eval.LastValue = value;
        }
    }
}
