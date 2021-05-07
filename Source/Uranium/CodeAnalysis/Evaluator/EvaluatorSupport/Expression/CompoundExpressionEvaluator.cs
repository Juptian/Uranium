using System;
using Uranium.CodeAnalysis.Binding.NodeKinds;

namespace Uranium.CodeAnalysis.Syntax.EvaluatorSupport
{
    internal static class CompoundExpressionEvaluator
    {
        public static void EvaluateCompoundOperator(BoundAssignmentExpression a, object value, Evaluator eval)
        {
            var current = eval.Variables[a.Variable];
            eval.Variables[a.Variable] = a.CompoundOperator!.Kind switch
            {
                SyntaxKind.PlusEquals => Operations.Addition(current, value),
                SyntaxKind.MinusEquals => Operations.Subtraction(current, value),
                SyntaxKind.MultiplyEquals => Operations.Multiplication(current, value),
                SyntaxKind.DivideEquals => Operations.Division(current, value),
                SyntaxKind.PowEquals => Operations.Pow(current, value),
                SyntaxKind.PlusPlus => Operations.Addition(current, 1),
                SyntaxKind.MinusMinus => Operations.Subtraction(current, 1),
                _ => throw new($"Unexpected compound operator {a.CompoundOperator!.Kind}"),
            };
        }

    }
}
