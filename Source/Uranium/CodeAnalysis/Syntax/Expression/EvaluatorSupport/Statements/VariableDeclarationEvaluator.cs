using Uranium.CodeAnalysis.Binding.Statements;

namespace Uranium.CodeAnalysis.Syntax.EvaluatorSupport
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
