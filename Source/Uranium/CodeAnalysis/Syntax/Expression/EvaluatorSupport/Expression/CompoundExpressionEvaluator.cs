using System;
using System.Collections.Generic;
using Uranium.CodeAnalysis.Binding;
using Uranium.CodeAnalysis.Binding.Statements;
using Uranium.CodeAnalysis.Binding.NodeKinds;
using Uranium.CodeAnalysis.Text;
using Uranium.CodeAnalysis.Syntax.Expression.EvaluatorSupport.Types;

namespace Uranium.CodeAnalysis.Syntax.Expression.EvaluatorSupport.Expression
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
