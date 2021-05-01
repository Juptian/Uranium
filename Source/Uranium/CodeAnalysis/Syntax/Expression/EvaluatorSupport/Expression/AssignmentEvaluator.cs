using System;
using System.Collections.Generic;
using Uranium.CodeAnalysis.Binding;
using Uranium.CodeAnalysis.Binding.Statements;
using Uranium.CodeAnalysis.Binding.NodeKinds;
using Uranium.CodeAnalysis.Text;
using Uranium.CodeAnalysis.Syntax.Expression.EvaluatorSupport.Types;

namespace Uranium.CodeAnalysis.Syntax.Expression.EvaluatorSupport.Expression
{
    internal static class AssignmentEvaluator
    {
        public static object EvaluateAssignmentExpression(BoundAssignmentExpression a, Evaluator eval)
        {
            var value = ExpressionEvaluator.Evaluate(a.Expression, eval);
            if (a.IsCompound)
            {
                CompoundExpressionEvaluator.EvaluateCompoundOperator(a, (int)value, eval);
            }
            else
            {
                eval.Variables[a.Variable] = value;
            }
            return eval.Variables[a.Variable];
        }
    }
}
