using System;
using Uranium.CodeAnalysis.Binding.NodeKinds;

namespace Uranium.CodeAnalysis.Syntax.EvaluatorSupport
{
    internal static class CompoundExpressionEvaluator
    {
        public static void EvaluateCompoundOperator(BoundAssignmentExpression a, int value, Evaluator eval)
        {
            switch (a.CompoundOperator!.Kind)
            {
                case SyntaxKind.PlusEquals:
                    eval.Variables[a.Variable] = (int)eval.Variables[a.Variable] + value;
                    break;
                case SyntaxKind.MinusEquals:
                    eval.Variables[a.Variable] = (int)eval.Variables[a.Variable] - value;
                    break;
                case SyntaxKind.MultiplyEquals:
                    eval.Variables[a.Variable] = (int)eval.Variables[a.Variable] * value;
                    break;
                case SyntaxKind.DivideEquals:
                    eval.Variables[a.Variable] = (int)eval.Variables[a.Variable] / value;
                    break;
                case SyntaxKind.PowEquals:
                    eval.Variables[a.Variable] = (int)Math.Pow((int)eval.Variables[a.Variable], value);
                    break;
                case SyntaxKind.PlusPlus:
                    eval.Variables[a.Variable] = (int)eval.Variables[a.Variable] + 1;
                    break;
                case SyntaxKind.MinusMinus:
                    eval.Variables[a.Variable] = (int)eval.Variables[a.Variable] - 1;
                    break;
            }
        }

    }
}
