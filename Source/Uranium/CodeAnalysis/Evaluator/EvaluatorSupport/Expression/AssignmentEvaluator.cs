using System;
using Uranium.CodeAnalysis.Binding.NodeKinds;

namespace Uranium.CodeAnalysis.Syntax.EvaluatorSupport
{
    internal static class AssignmentEvaluator
    {
        public static object? EvaluateAssignmentExpression(BoundAssignmentExpression a, Evaluator eval)
        {
            var value = ExpressionEvaluator.Evaluate(a.Expression, eval);
            if (a.IsCompound)
            {
                CompoundExpressionEvaluator.EvaluateCompoundOperator(a, value!, eval);
            }
            else
            {
                eval.Variables[a.Variable] = value!;
            }
            return eval.Variables[a.Variable];
        }
    }
}
