using System;
using Uranium.CodeAnalysis.Binding.NodeKinds;

namespace Uranium.CodeAnalysis.Syntax.EvaluatorSupport
{
    internal static class CompoundExpressionEvaluator
    {
        public static void EvaluateCompoundOperator(BoundAssignmentExpression a, object value, Evaluator eval)
        {
            var current = eval.Variables[a.Variable];
            Console.WriteLine($"\r\n\r\nCurrent type: {current.GetType()} COMPOUNDEXPRESSIONEVALUTOR 11");
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
            Console.WriteLine($"Variable type after evaluation: {eval.Variables[a.Variable].GetType()} \r\nVariable value: {value} COMPOUNDEXPRESSIONEVALUATOR 22 - 23");
            Console.WriteLine($"\r\nValue type evaluated: {value.GetType()} COMPOUNDEXPRESSIONEVALUATOR 24");
        }

    }
}
